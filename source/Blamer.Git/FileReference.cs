using Blamer.Utility;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blamer.Git
{
    public class FileReference
    {
        private Strings _stringUtil = new Strings();

        public List<string> GetStackTraceFileLineReferences(string stacktrace)
        {
            var stackArray = stacktrace.Split(new string[] { " at " }, StringSplitOptions.None);

            var refList = new List<string>();

            foreach (var line in stackArray)
            {
                if (new Regex(@":line \d+").IsMatch(line))
                {
                    var refLine = string.Empty;

                    if (line.Contains("\r\n"))
                    {
                        refLine = _stringUtil.GetBetween(line, endPattern: @"\r\n").Trim();
                    }
                    else
                    {
                        refLine = line.Trim();
                    }

                    refList.Add(refLine);
                }
            }

            return refList;
        }

        public string GetMethod(string fileLineRef)
        {
            var methodPath = _stringUtil.GetBetween(fileLineRef, endPattern: @"\(");

            var pathArray = methodPath.Split('.');

            var method = pathArray[pathArray.Length - 1].Trim() + "(";

            return method;
        }

        public string GetFilePath(string fileLineRef)
        {
            var filePath = _stringUtil.GetBetween(fileLineRef, @"root\\", ":line", true);

            filePath = filePath.Replace(":line", "");

            filePath = filePath.Replace(@"\", "/");

            return filePath;
        }

        public int GetLineNumber(string fileLineRef)
        {
            var numberString = _stringUtil.GetBetween(fileLineRef, ":line ", @"\d+", true);

            var lineNumber = int.Parse(numberString.Replace(":line ", ""));

            return lineNumber;
        }
    }
}