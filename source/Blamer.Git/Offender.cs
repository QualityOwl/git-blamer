using Blamer.Command;
using Blamer.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Blamer.Git
{
    public class Offender
    {
        public string ID { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public string Notes { get; set; }
        public string OriginalLine { get; set; }
        public string ModifiedLine { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public int LineCount { get; set; }
        public string Command { get; set; }

        private FileReference _fileRef = new FileReference();
        private Strings _stringUtil = new Strings();
        private Regex _addLineMatch = new Regex(@"^\+");
        private Regex _removeLineMatch = new Regex(@"(^-)");

        public List<Offender> GetOffenderList(string stacktrace)
        {
            // Get a list of files with line numbers from the stack trace.
            var fileLineRefList = _fileRef.GetStackTraceFileLineReferences(stacktrace);

            // Initialize an empty 'Offender' list.
            var offenderList = new List<Offender>();

            // Add offenders to the 'Offender' list.
            foreach (var fileLineRef in fileLineRefList)
            {
                // Build the list of offenders based on the file and line number.
                var referenceOffenderList = BuildReferenceOffenderList(fileLineRef);

                // Add the newly built 'Offender' objects to the 'Offender' list.
                offenderList.AddRange(referenceOffenderList);
            }

            return offenderList;
        }

        public List<Offender> BuildReferenceOffenderList(string fileLineRef)
        {
            //var fileMethod = _fileRef.GetMethod(fileLineRef);
            var filePath = _fileRef.GetFilePath(fileLineRef);
            var lineNumber = _fileRef.GetLineNumber(fileLineRef);
            LineNumber = lineNumber;

            var offenderObjectList = new List<Offender>();

            var command = $"git log -p -m {filePath}";

            var psObject = Shell.GetPSObject(command);

            if (psObject.Count != 0)
            {
                var commitObjects = GetCommitObjects(psObject);

                foreach (var commit in commitObjects)
                {
                    var offender = new Offender()
                    {
                        FilePath = filePath,
                        LineNumber = lineNumber,
                        ID = GetCommitId(commit),
                        Author = GetAuthor(commit),
                        Date = GetDate(commit),
                        DescriptionShort = GetDescriptionShort(commit),
                        DescriptionLong = GetDescriptionLong(commit),
                        OriginalLine = GetOriginalLine(commit),
                        ModifiedLine = GetModifiedLine(commit),
                        Command = command
                    };

                    if ((string.IsNullOrWhiteSpace(offender.OriginalLine) && string.IsNullOrWhiteSpace(offender.ModifiedLine))  // Do not add when both lines are empty
                    || (!string.IsNullOrWhiteSpace(offender.OriginalLine) && string.IsNullOrWhiteSpace(offender.ModifiedLine))) // Do not add when the original line was deleted
                    {
                        continue;
                    }
                    else
                    {
                        offenderObjectList.Add(offender);
                    }
                }
            }

            return offenderObjectList;
        }

        private List<Collection<PSObject>> GetCommitObjects(Collection<PSObject> psObject)
        {
            // Convert the collection into one string.
            var psString = ConvertPSObjectCollectionToString(psObject);

            // Parse the psString into an array of commits.
            var commitArray = _stringUtil.ConvertStringToStringArray(psString, @"\r\ncommit ");

            // Create the list of commit collections.
            var commitObjectList = new List<Collection<PSObject>>();

            foreach (var arrayItem in commitArray)
            {
                // Create the collection.
                var collection = new Collection<PSObject>();

                // Reassign arratItem (string) to commit.
                var commit = arrayItem;

                if (!new Regex(@"^commit ").IsMatch(commit))
                {
                    commit = $"commit {commit}";
                }

                StringReader reader = new StringReader(commit);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        collection.Add(new PSObject(line));
                    }
                    else
                    {
                        break;
                    }
                }

                commitObjectList.Add(collection);
            }

            return commitObjectList;
        }

        private string ConvertPSObjectCollectionToString(Collection<PSObject> collection)
        {
            var stringCollection = string.Empty;

            foreach (var line in collection)
            {
                stringCollection += $"{line.ToString()}\r\n";
            }

            return stringCollection.Trim();
        }

        private string GetCommitId(Collection<PSObject> collection)
        {
            return GetBlameItemValue(collection, "commit ");
        }

        private string GetAuthor(Collection<PSObject> collection)
        {
            return _stringUtil.GetBetween(GetBlameItemValue(collection, "Author: "), endPattern: "<");
        }

        private string GetDate(Collection<PSObject> collection)
        {
            var dateString = GetBlameItemValue(collection, "Date: ").Trim();

            var dateArray = dateString.Split(' ');

            dateString = DateTime.Parse($"{dateArray[1]} {dateArray[2]}, {dateArray[4]} {dateArray[3]} {dateArray[5]}").ToString();

            return dateString;
        }

        private string GetDescriptionShort(Collection<PSObject> collection)
        {
            var itemvalue = GetDescriptionLong(collection);

            itemvalue = itemvalue.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0].Trim();

            return itemvalue;
        }

        private string GetDescriptionLong(Collection<PSObject> collection)
        {
            var list = collection.ToList<PSObject>();

            var startIndex = list.FindIndex(x => x.ToString().Contains("Date: ")) + 1;
            var endIndex = list.FindIndex(x => x.ToString().Contains("diff ")) - 1;

            var originalLine = string.Empty;
            var seq = 0;

            foreach (var line in collection)
            {
                if (startIndex < seq && seq < endIndex)
                {
                    originalLine += list[seq].ToString().Trim() + "\r\n";
                }
                else if (seq == endIndex)
                {
                    break;
                }
                seq++;
            }

            return originalLine.Trim();
        }

        private string GetOriginalLine(Collection<PSObject> commit)
        {
            var originalLine = string.Empty;

            // Parse out the patches
            var patchObjects = GetPatchObjects(commit);

            // Search the patch for the respective method/line change.
            foreach (var patch in patchObjects)
            {
                // Get the original and modified code block coordinate data.
                var original = GetOriginal(patch);
                var modified = GetModified(patch);

                // Calculate how many lines were added or removed between the commit code blocks.
                var lineCountDiff = modified.NumberOfLinesTotal - original.NumberOfLinesTotal;

                // Proceed if the file reference line number is within the patch.
                if (modified.StartLineNumber < LineNumber && (modified.StartLineNumber + modified.NumberOfLinesTotal) > LineNumber)
                {
                    var index = 0;

                    if (original.CodeBlock.Count != 0) // Find the original line.
                    {
                        if (lineCountDiff == 0) // No lines were added or removed; only a change to one or more lines.
                        {
                            index = LineNumber - modified.StartLineNumber;
                        }
                        else // One or more lines were added or removed.
                        {
                            if (original.NumberOfLinesRemoved == 0 && modified.NumberOfLinesAdded > 0 || original.NumberOfLinesRemoved > 0 && modified.NumberOfLinesAdded == 0) // No line changes occurred.
                            {
                                index = LineNumber - modified.StartLineNumber;
                            }
                            else // One or more line changes also occurred.
                            {
                                // Get modified line value
                                var modifiedLine = GetModifiedLine(commit);

                                // Get the modified line index
                                var removeMod = 0;
                                var addMod = 0;
                                var existMod = 0;
                                var modifiedLineIndex = 0;
                                patch.RemoveAt(0);
                                foreach (var line in patch)
                                {
                                    var item = line.ToString();

                                    if (!IsRemovedLine(item) && item.Contains(modifiedLine))
                                    {
                                        break;
                                    }

                                    if (IsRemovedLine(item))
                                    {
                                        removeMod++;
                                    }
                                    else if (IsAddedLine(item))
                                    {
                                        addMod++;
                                    }
                                    else
                                    {
                                        existMod++;
                                    }

                                    modifiedLineIndex++;
                                }

                                index += modifiedLineIndex;
                                index += (addMod - removeMod) > 0 ? removeMod - addMod : addMod - removeMod;
                            }
                        }

                        try
                        {
                            originalLine = original.CodeBlock[index].ToString();

                            if (string.IsNullOrWhiteSpace(originalLine))
                            {
                                originalLine = $"Index: {index}";
                            }
                        }
                        catch
                        {
                            originalLine = "*** Manual review required. ***";
                        }
                    }
                }
            }

            return originalLine.Replace("-", string.Empty).Trim();
        }

        private string GetModifiedLine(Collection<PSObject> commit)
        {
            var originalLine = string.Empty;

            // Parse out the patchs
            var patchObjects = GetPatchObjects(commit);

            // Search the patch for the respective method/line change.
            foreach (var patch in patchObjects)
            {
                var modified = GetModified(patch);

                if (modified.StartLineNumber < LineNumber && modified.StartLineNumber + modified.NumberOfLinesTotal > LineNumber)
                {
                    var ModifiedCodeBlock = GetModifiedCodeBlock(patch);

                    if (ModifiedCodeBlock.Count == 0)
                    {
                        originalLine = string.Empty;
                    }
                    else
                    {
                        var index = (LineNumber - modified.StartLineNumber);

                        originalLine = ModifiedCodeBlock[index].ToString();
                    }
                }
            }

            originalLine = Regex.Replace(originalLine, _addLineMatch.ToString(), "");

            return originalLine.Trim();
        }

        private List<Collection<PSObject>> GetPatchObjects(Collection<PSObject> commit)
        {
            var patchObjectList = new List<Collection<PSObject>>();

            var psString = ConvertPSObjectCollectionToString(commit);

            var patchArray = _stringUtil.ConvertStringToStringArray(psString.Trim(), @"\r\n@@");

            foreach (var arrayItem in patchArray)
            {
                if (!arrayItem.StartsWith("commit "))
                {
                    var patch = $"@@{arrayItem}";

                    var collection = new Collection<PSObject>();

                    StringReader reader = new StringReader(patch);
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            collection.Add(new PSObject(line));
                        }
                        else
                        {
                            break;
                        }
                    }

                    patchObjectList.Add(collection);
                }
            }

            return patchObjectList;
        }

        private Block GetOriginal(Collection<PSObject> patch)
        {
            var originalString = _stringUtil.GetBetween(patch[0].ToString(), "-", @" \+", false).Trim();
            var originalArray = originalString.Split(',');

            var blockObject = new Block()
            {
                StartLineNumber = int.Parse(originalArray[0]),
                NumberOfLinesTotal = int.Parse(originalArray[1]),
                NumberOfLinesAdded = GetNumberOfLinesAdded(GetOriginalCodeBlock(patch)),
                NumberOfLinesRemoved = GetNumberOfLinesRemoved(GetOriginalCodeBlock(patch)),
                CodeBlock = GetOriginalCodeBlock(patch)
            };

            return blockObject;
        }

        private Block GetModified(Collection<PSObject> patch)
        {
            var modifiedArray = _stringUtil.GetBetween(patch[0].ToString(), @"\+", "@@", false).Trim().Split(',');

            var blockObject = new Block()
            {
                StartLineNumber = int.Parse(modifiedArray[0]),
                NumberOfLinesTotal = int.Parse(modifiedArray[1]),
                NumberOfLinesAdded = GetNumberOfLinesAdded(GetModifiedCodeBlock(patch)),
                NumberOfLinesRemoved = GetNumberOfLinesRemoved(GetModifiedCodeBlock(patch)),
                CodeBlock = GetModifiedCodeBlock(patch)
            };

            return blockObject;
        }

        private Collection<PSObject> GetOriginalCodeBlock(Collection<PSObject> patch)
        {
            var originalLineList = new Collection<PSObject>();

            foreach (var item in patch)
            {
                var line = item.ToString();

                if (_addLineMatch.IsMatch(line))
                {
                    originalLineList.Add(string.Empty);
                }
                else if (!new Regex(@"^@@").IsMatch(line) && !line.Contains("No newline at end of file"))
                {
                    originalLineList.Add(new PSObject(line));
                }
            }

            return originalLineList;
        }

        private Collection<PSObject> GetModifiedCodeBlock(Collection<PSObject> patch)
        {
            var modifiedLineList = new Collection<PSObject>();

            foreach (var item in patch)
            {
                var line = item.ToString();

                if (!_removeLineMatch.IsMatch(line) && !new Regex(@"^@@").IsMatch(line) && !line.Contains("No newline at end of file"))
                {
                    modifiedLineList.Add(new PSObject(line));
                }
            }

            return modifiedLineList;
        }

        private int GetNumberOfLinesAdded(Collection<PSObject> patch)
        {
            var number = 0;

            foreach (var item in patch)
            {
                var line = item.ToString();

                if (_addLineMatch.IsMatch(line))
                {
                    number++;
                }
            }

            return number;
        }

        private int GetNumberOfLinesRemoved(Collection<PSObject> patch)
        {
            var number = 0;

            foreach (var item in patch)
            {
                var line = item.ToString();

                if (_removeLineMatch.IsMatch(line))
                {
                    number++;
                }
            }

            return number;
        }

        private string GetBlameItemValue(Collection<PSObject> collection, string itemLabel)
        {
            var originalLine = string.Empty;

            foreach (var obj in collection)
            {
                var line = obj.ToString();

                if (line.ToString().Contains(itemLabel))
                {
                    originalLine = _stringUtil.GetBetween(line, itemLabel).Trim();
                    break;
                }
            }

            return originalLine;
        }

        private bool IsRemovedLine(string line)
        {
            return _removeLineMatch.IsMatch(line);
        }

        private bool IsAddedLine(string line)
        {
            return _addLineMatch.IsMatch(line);
        }

        private bool IsAddedOrRemovedLine(string line)
        {
            return IsAddedLine(line) || IsRemovedLine(line);
        }
    }

    public class Block
    {
        public int StartLineNumber { get; set; }
        public int NumberOfLinesTotal { get; set; }
        public int NumberOfLinesAdded { get; set; }
        public int NumberOfLinesRemoved { get; set; }
        public Collection<PSObject> CodeBlock { get; set; }
    }
}