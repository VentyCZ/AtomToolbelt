using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AtomToolbelt.Utils
{
    class AtomRegistry
    {
        private DirectoryInfo baseLocation;

        public readonly Dictionary<string, DirectoryInfo> Versions = new Dictionary<string, DirectoryInfo>();
        public Version LatestVersion { get; private set; }
        public DirectoryInfo LatestVersionFolder { get; private set; }

        public enum DiscoveryResult
        {
            BaseDirectoryNotFound,
            NoVersionsFound,
            NoValidVersionsFound,
            Success,
        }

        public AtomRegistry()
        {
            baseLocation = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "atom"));
        }

        public void SetBaseLocation(string baseLocation)
        {
            this.baseLocation = new DirectoryInfo(baseLocation);
        }

        Regex r_version = new Regex("([0-9\\.]+)", RegexOptions.Compiled);
        public DiscoveryResult DiscoverAtomVersions()
        {
            Versions.Clear();

            if (baseLocation.Exists)
            {
                var dirs = baseLocation.EnumerateDirectories("app-*");

                using (var sequenceEnum = dirs.GetEnumerator())
                {
                    //Sequence contains elements
                    if (sequenceEnum.MoveNext())
                    {
                        do
                        {
                            var dir = sequenceEnum.Current;

                            if (dir.ContainsFile("atom.exe"))
                            {
                                Match m;
                                if ((m = r_version.Match(dir.Name)).Success)
                                {
                                    Versions.Add(m.Groups[1].Value, dir);

                                    if (LatestVersion != null)
                                    {
                                        var ver = m.Groups[1].Value.Versionate();
                                        LatestVersion = (ver > LatestVersion ? ver : LatestVersion);
                                    }
                                    else
                                    {
                                        LatestVersion = m.Groups[1].Value.Versionate();
                                    }

                                    LatestVersionFolder = dir;
                                }
                            }
                        } while (sequenceEnum.MoveNext());

                        if (Versions.Count > 0)
                            return DiscoveryResult.Success;

                        return DiscoveryResult.NoValidVersionsFound;
                    }
                    else
                    {
                        return DiscoveryResult.NoVersionsFound;
                    }
                }
            }
            else
            {
                return DiscoveryResult.BaseDirectoryNotFound;
            }
        }

        private string getIconFormat(DirectoryInfo versionPath)
        {
            return string.Format("\"{0}\"", string.Join(Path.DirectorySeparatorChar.ToString(), new string[] { versionPath.FullName, "atom.exe" }));
        }

        private string getCommandFormat(DirectoryInfo versionPath)
        {
            return string.Format("{0} \"%1\"", getIconFormat(versionPath));
        }

        private bool setCommandAndIcon(string keyPath, DirectoryInfo versionPath)
        {
            if (keyPath == null || versionPath == null)
                return false;

            try
            {
                using (var x = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (x == null)
                        return false;

                    using (var y = x.CreateSubKey("command"))
                    {
                        if (y == null)
                            return false;

                        x.SetValue("Icon", getIconFormat(versionPath));
                        y.SetValue(null, getCommandFormat(versionPath));
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool setCommandOnly(string commandKeyPath, DirectoryInfo versionPath)
        {
            if (commandKeyPath == null || versionPath == null)
                return false;

            try
            {
                using (var x = Registry.CurrentUser.CreateSubKey(commandKeyPath))
                {
                    if (x == null)
                        return false;

                    x.SetValue(null, getCommandFormat(versionPath));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SetApplicationCommand(DirectoryInfo versionPath)
        {
            return setCommandOnly(@"Software\Classes\Applications\atom.exe\shell\open\command", versionPath);
        }

        public bool SetShellCommand(DirectoryInfo versionPath)
        {
            return setCommandAndIcon(@"Software\Classes\*\shell\Atom", versionPath);
        }

        public bool SetDirectoryBackgroundCommand(DirectoryInfo versionPath)
        {
            return setCommandAndIcon(@"Software\Classes\Directory\Background\shell\Atom", versionPath);
        }

        public bool SetDirectoryCommand(DirectoryInfo versionPath)
        {
            return setCommandAndIcon(@"Software\Classes\Directory\shell\Atom", versionPath);
        }

        //private Regex r_location = new Regex("\"(\\S+)\"", RegexOptions.Compiled);
        //public string GetAtomLocation()
        //{
        //    var val = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications\atom.exe\shell\open\command")?.GetValue(null);

        //    if (val != null)
        //    {
        //        //Yay, the key exists!
        //        var match = r_location.Match(val.ToString());
        //        if (match.Success)
        //        {
        //            var loc = match.Groups[1].Value;

        //            return loc;
        //        }
        //    }

        //    return null;
        //}
    }
}
