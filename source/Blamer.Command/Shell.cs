using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Blamer.Command
{
    public static class Shell
    {
        public static PowerShell powershell = PowerShell.Create();
        private static Properties.Settings _settings = new Properties.Settings();

        public static List<string> GetPSObjectAsListString(string command)
        {
            var objectList = new List<string>();

            var psObjects = GetPSObject(command);

            foreach (PSObject item in psObjects)
            {
                objectList.Add(item.ToString());
            }

            return objectList;
        }

        public static Collection<PSObject> GetPSObject(string command)
        {
            return InvokeCommand(command);
        }

        private static Collection<PSObject> InvokeCommand(string command)
        {
            CleanShell();

            powershell.AddScript(command);

            Collection<PSObject> results = new Collection<PSObject>();

            try
            {
                results = powershell.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return results;
        }

        private static PSDataStreams GetStreams()
        {
            return powershell.Streams;
        }

        private static PSDataCollection<ErrorRecord> StreamErrors()
        {
            return GetStreams().Error;
        }

        public static void SetCommandDirectory()
        {
            SetCommandDirectory(_settings.LocalRepoDirectory);
        }

        public static void SetCommandDirectory(string directory)
        {
            InvokeCommand($"cd {directory}");

            var errors = StreamErrors();

            foreach (var line in errors)
            {
                var item = line.ToString();

                if (item.Contains("Cannot find path"))
                {
                    throw new Exception(item);
                }
            }

            if (string.IsNullOrWhiteSpace(GetCurrentBranch()))
            {
                throw new Exception($"'{directory}' does not contain a repository.");
            }
        }

        private static string GetCurrentBranch()
        {
            var results = InvokeCommand("git branch");

            var current = string.Empty;

            foreach (PSObject item in results)
            {
                var line = item.ToString();

                if (line.Contains("*"))
                {
                    current = line.Replace("*", string.Empty).Trim();
                    continue;
                }
            }

            return current;
        }

        public static void SetBranch(string branch)
        {
            var current = GetCurrentBranch();

            if (current != branch)
            {
                InvokeCommand($"git switch {branch} -f");

                current = GetCurrentBranch();

                if (current != branch)
                {
                    throw new Exception($"Git was not able to switch to branch '{branch}'.\r\n\r\nCurrent branch is '{current}'");
                }
            }
        }

        public static void InvokeStash()
        {
            InvokeCommand("git stash");
        }

        public static void InvokePull()
        {
            InvokeCommand("git pull");
        }

        private static void CleanShell()
        {
            powershell.Commands.Clear();
            powershell.Streams.ClearStreams();
        }

        public static void Dispose()
        {
            powershell.Dispose();
        }
    }
}