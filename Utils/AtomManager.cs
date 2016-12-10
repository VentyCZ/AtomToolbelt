using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace AtomToolbelt.Utils
{
    class AtomManager
    {
        private DirectoryInfo baseLocation;

        private Regex r_escape = new Regex(@"[\u001b\u009b][[()#;?]*(?:[0-9]{1,4}(?:;[0-9]{0,4})*)?[0-9A-ORZcf-nqry=><]", RegexOptions.Compiled);
        private Regex r_outDated = new Regex(@"(\S+)\s([0-9\.]+)\s->\s([0-9\.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public AtomManager()
        {
            baseLocation = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".atom"));
        }

        /// <summary>
        /// Runs a command-line command
        /// </summary>
        /// <param name="commandWithArgs">The command to be run</param>
        /// <returns>Output of the command</returns>
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

        /// <summary>
        /// Removes all ANSI escape codes from the target string
        /// </summary>
        /// <param name="unescaped">The target string</param>
        /// <returns>String with removed ANSI escape codes</returns>
        private string escape(string unescaped)
        {
            return r_escape.Replace(unescaped, "");
        }

        /// <summary>
        /// Gets the list of all installed packages
        /// </summary>
        /// <returns>List of installed packages</returns>
        public PackageList GetInstalledPackages()
        {
            var output = runCommand("apm list --installed --bare");
            return PackageList.Parse(output);
        }

        /// <summary>
        /// Gets the list of all outdated packages
        /// </summary>
        /// <returns>List of outdated packages</returns>
        public PackageList GetOutdatedPackages()
        {
            var list = new PackageList();

            var output = runCommand("apm upgrade --list");
            output = escape(output); // Remove the ANSI escape codes
            // output = Regex.Replace(output, @"â”[ś”]â”€â”€\s", "");

            var outdated = r_outDated.Matches(output);
            foreach (Match m in outdated)
            {
                list.Add(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }

            return list;
        }

        public class PackageList : IEnumerable<Package>
        {
            private Dictionary<string, Package> packages = new Dictionary<string, Package>();

            /// <summary>
            /// Parses the input list of packages
            /// The list must have this format (--bare):
            ///  - one package per line
            ///  - package name and its version is separated by @ symbol
            /// </summary>
            /// <param name="list">The list of packages (as string)</param>
            /// <returns>The newly formed package list containing packages from the input</returns>
            public static PackageList Parse(string list)
            {
                var ret = new PackageList();

                var lines = list.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("@"))
                    {
                        var split = line.Split('@');

                        ret.Add(split[0], split[1]);
                    }
                }

                return ret;
            }

            /// <summary>
            /// Adds new package to this package list
            /// </summary>
            /// <param name="p">The package info</param>
            /// <returns>The input package info</returns>
            public Package Add(Package p)
            {
                packages.Add(p.Name, p);
                return p;
            }

            /// <summary>
            /// Adds new package to this package list
            /// </summary>
            /// <param name="name">The name of the package</param>
            /// <param name="version">The installed version of the package</param>
            /// <param name="newVersion">The latest version of the package</param>
            /// <returns>The new package info</returns>
            public Package Add(string name, string version, string newVersion = null)
            {
                return Add(new Package(name, version, newVersion));
            }

            /// <summary>
            /// Gets package info by its name
            /// </summary>
            /// <param name="packageName">The target name</param>
            /// <returns>Package info</returns>
            public Package Get(string packageName)
            {
                return (packages.ContainsKey(packageName) ? packages[packageName] : null);
            }

            public IEnumerator<Package> GetEnumerator()
            {
                return packages.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return packages.Values.GetEnumerator();
            }
        }

        public class Package
        {
            /// <summary>
            /// Name of the package
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Installed version of the package
            /// </summary>
            public string Version { get; private set; }

            /// <summary>
            /// Latest version of the package (usually unknown)
            /// </summary>
            public string NewVersion { get; private set; }

            public Package(string name, string version, string newVersion = null)
            {
                Name = name;
                Version = version;
                NewVersion = newVersion;
            }
        }
    }
}