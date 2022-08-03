using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blamer.Utility
{
    public class Strings
    {
        /// <summary>
        /// Standardizes a string by removing leading and trailing whitespace and converting to invariant uppercase.
        /// </summary>
        /// <param name="value">The string to be standardized</param>
        /// <returns>The standardized representation of <paramref name="value"/></returns>
        public static string Standardize(string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            return value.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Return a substring that is between two strings within a string using Regex.
        /// </summary>
        public string GetBetween(string sourceString, string startPattern = "", string endPattern = "", bool includeStartEndStrings = false, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(sourceString)) throw new Exception("Method requires the 'sourceString' parameter to contain one or more characters.");

            if (string.IsNullOrEmpty(startPattern) && string.IsNullOrEmpty(endPattern)) throw new Exception("Method requires the 'startPattern' or 'endPattern' parameters to not be NULL.");

            if (new Regex(startPattern).IsMatch(sourceString) || new Regex(endPattern).IsMatch(sourceString))
            {
                int start;

                if (string.IsNullOrEmpty(startPattern))
                {
                    start = 0;
                }
                else
                {
                    var m = Regex.Match(sourceString, startPattern, options);

                    if (!includeStartEndStrings)
                    {
                        start = m.Index + m.Value.Length;
                    }
                    else
                    {
                        start = m.Index;
                    }
                }

                int end;

                if (string.IsNullOrEmpty(endPattern))
                {
                    end = sourceString.Length;
                }
                else
                {
                    var source = sourceString.Substring(start + startPattern.Length);

                    var match = Regex.Match(source, endPattern, options);

                    if (!includeStartEndStrings)
                    {
                        end = start + startPattern.Length + match.Index;
                    }
                    else
                    {
                        end = start + startPattern.Length + match.Index + match.Value.Length;
                    }
                }

                return sourceString.Substring(start, end - start);
            }

            throw new Exception("Unable to find substring given the entered parameters");
        }

        public string InsertBetween(string sourceString, string startString, string endString, string insertedString)
        {
            if (string.IsNullOrWhiteSpace(sourceString)) throw new Exception("Method requires the 'sourceString' parameter to contain one or more characters.");

            if (sourceString.Contains(startString) && sourceString.Contains(endString))
            {
                var begin = GetBetween(sourceString, endPattern: endString);
                var end = GetBetween(sourceString, startString);
                return begin.Trim() + " " + insertedString.Trim() + " " + end.Trim();
            }

            throw new Exception("Unable to insert string.");
        }

        public List<string> ConvertStringToList(string stringToConvert)
        {
            // Scrub old setting text formats from the string in case they are present
            stringToConvert = stringToConvert.Replace("True", "").Replace(" ", "");

            // Split the string to get a list of log_id values
            var list = new List<string>();
            if (stringToConvert.Contains(","))
            {
                list = stringToConvert.Split(',').ToList();
            }
            else
            {
                list.Add(stringToConvert.Trim());
            }

            return list;
        }

        public string ConvertListToString(List<string> list)
        {
            // Scrub old setting text formats from the string in case they are present
            list.RemoveAll(x => x.Contains("False"));

            // Create the string object to return
            var master = string.Empty;
            var seq = 1;

            foreach (var item in list)
            {
                if (seq < list.Count)
                {
                    master += item + ", ";
                }
                else
                {
                    master += item;
                }

                seq++;
            }

            return master;
        }

        public string[] ConvertStringToStringArray(string s, string delimiter)
        {
            var array = Regex.Split(s, delimiter);

            return array;
        }

        public List<string> ConvertStringArrayToList(string[] stringArrayToConvert)
        {
            var list = new List<string>(stringArrayToConvert);

            return list;
        }
    }
}