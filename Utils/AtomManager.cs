using System;
using System.Diagnostics;
using System.IO;

namespace AtomToolbelt.Utils
{
    class AtomManager
    {
        private DirectoryInfo baseLocation;

        public AtomManager()
        {
            baseLocation = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".atom"));
        }

        private string runCommand(string commandWithArgs)
        {
            ProcessStartInfo pi = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = string.Format("/C {0}", commandWithArgs),
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = baseLocation.FullName
            };

            Process p = new Process()
            {
                StartInfo = pi
            };

            p.Start();
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd();
        }

        public Package[] GetInstalledPackages()
        {
            var output = runCommand("apm list --installed --bare");
            var lines = output.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var packages = new Package[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                packages[i] = new Package(lines[i]);
            }

            return packages;
        }

        public class Package
        {
            public string Name { get; private set; }
            public string Version { get; private set; }

            public Package(string bare)
            {
                if (bare.Contains("@"))
                {
                    var split = bare.Split('@');

                    Name = split[0];
                    Version = split[1];
                }
            }
        }
    }
}