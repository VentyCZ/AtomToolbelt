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
            baseLocation = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "atom");
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

        //public bool SetShellCommand(string version)
        //{
        //    var x = Registry.CurrentUser.OpenSubKey(@"Software\Classes\*\shell\Atom", true);
        //    if (x != null)
        //    {
                
        //    }

        //    return false;

        //    //x.SetValue(null, version);
        //}

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
