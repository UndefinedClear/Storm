using System;
namespace Storm;

class Program
{
    static readonly string TRASH_DIR = ".trash_can";
    static void Main(string[] args)
    {
        Utils.Init();

        Alias alias_history = new Alias(["--his", "--h", "-h", "-history", "-his", "--history"]);
        Alias alias_restore = new Alias(["--res", "--r", "-r", "-restore", "-res", "--restore"]);
        Alias alias_permanent = new Alias(["-p", "--permament", "--permanent"]);


        if (args.Length == 0)
        {
            Utils.WrteLineColor("Usage:", ConsoleColor.Green);
            Utils.WrteLineColor($"  --history (ALIASES: {alias_history.ToString()})", ConsoleColor.DarkGreen);
            Utils.WrteLineColor($"  --permament <files...> (ALIASES: {alias_permanent.ToString()})", ConsoleColor.DarkGreen);
            Utils.WrteLineColor($"  --restore <ID|VERSION> <FILENAME> (ALIASES: {alias_restore.ToString()})", ConsoleColor.DarkGreen);
            Utils.WrteLineColor("  <files...> to trash", ConsoleColor.DarkGreen);
            return;
        }

        //Console.WriteLine(alias_restore.ToString());

        if (alias_history.Have(args[0]))
        {
            History();
        }
        else if (alias_restore.Have(args[0]))
        {
            if (args.Length < 3)
            {
                Utils.WrteLineColor(
                    "Error: --restore requires <ID|VERSION> and <FILENAME>",
                    ConsoleColor.Red);
                return;
            }

            Restore(args[1], args[2]);
        }
        else if (alias_permanent.Have(args[0]))
        {
            if (args.Length < 2)
            {
                Utils.WrteLineColor("Error: --permament requires at least one file or folder", ConsoleColor.Red);
                return;
            }

            PermanentDelete(args.Skip(1).ToArray());
        }
        else if (args[0].StartsWith("-"))  // Unknown flag
        {
            Utils.WrteLineColor($"Unknown option: {args[0]}", ConsoleColor.Red);
            return;
        }
        else
        {
            Trash(args);
        }

    }

    static void PermanentDelete(string[] paths)
    {
        Utils.WrteLineColor("WARNING: Permanent deletion cannot be undone!", ConsoleColor.Yellow);
        Utils.WrteLineColor("Are you Sure? (y/n)", ConsoleColor.Red);

        string ?answer = Console.ReadLine();

        if (answer == "") return;
        else if (answer.ToLower() == "y") { }

        foreach (var path in paths)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Utils.WrteLineColor($"Permanently deleted file '{path}'", ConsoleColor.Red);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Utils.WrteLineColor($"Permanently deleted directory '{path}'", ConsoleColor.Red);
                }
                else
                {
                    Console.WriteLine($"Path not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Utils.WrteLineColor($"Failed to delete '{path}': {ex.Message}", ConsoleColor.Red);
            }
        }
    }


    class Alias
    {
        private string[] _names = [];

        public Alias(string[] names)
        {
            _names = names;
        }


        /// <summary>
        /// Checks is the String in the aliases
        /// </summary>
        /// <returns>bool</returns>
        public bool Have(string name)
        {
            if (name.Length == 0 || name == null) return false;

            if (_names.Contains(name)) return true;

            return false;
        }

        /// <summary>
        /// Returns String list of all aliases
        /// </summary>
        /// <returns>String[]</returns>
        public string[] Names()
        {
            return _names;
        }

        /// <summary>
        /// Returns String ine of all aliases
        /// </summary>
        /// <returns>String</returns>
        override
        public string ToString()
        {
            string _out = "";

            foreach (string alias in _names)
            {
                _out += alias + " ";
            }

            return _out;
        }
    }

    static List<string> GetVersions()
    {
        if (!Directory.Exists(TRASH_DIR))
            return new List<string>();

        return Directory.GetDirectories(TRASH_DIR)
                        .Select(Path.GetFileName)
                        .OrderBy(v => v)
                        .ToList();
    }

    static void Trash(string[] paths)
    {
        if (!Directory.Exists(TRASH_DIR))
            Directory.CreateDirectory(TRASH_DIR);

        if (paths.Length == 0) return;

        string version = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string versionDir = Path.Combine(TRASH_DIR, version);
        Directory.CreateDirectory(versionDir);

        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                string dest = Path.Combine(versionDir, Path.GetFileName(path));
                File.Move(path, dest);
                PrintRed($"Moved file '{path}' to trash (Version: {version})");
            }
            else if (Directory.Exists(path))
            {
                string dest = Path.Combine(versionDir, Path.GetFileName(path));
                Directory.Move(path, dest);
                PrintRed($"Moved directory '{path}' to trash (Version: {version})");
            }
            else
            {
                Console.WriteLine($"Path not found: {path}");
            }
        }
    }

    static void History()
    {
        var versions = GetVersions();
        if (!versions.Any())
        {
            Console.WriteLine("No history available.");
            return;
        }

        Console.WriteLine($"{"ID",-4} | {"Version (Timestamp)",-20} | Files");
        Console.WriteLine(new string('-', 70));

        var versionList = versions.Select((v, i) => new { Version = v, Id = i + 1 })
                                  .Reverse()
                                  .ToList();

        for (int i = 0; i < versionList.Count; i++)
        {
            var entry = versionList[i];
            string vPath = Path.Combine(TRASH_DIR, entry.Version);
            string contents = string.Join(", ", Directory.GetFileSystemEntries(vPath)
                                                         .Select(Path.GetFileName));

            string line = $"{entry.Id,-4} | {entry.Version,-20} | {contents}";
            if (i == 0)
                PrintRed(line + " (LAST TEMPED)");
            else
                PrintRedIdOnly(entry.Id, line);
        }
    }

    static void Restore(string idOrVersion, string filename)
    {
        var versions = GetVersions();
        string targetVersion = null;

        if (int.TryParse(idOrVersion, out int id))
        {
            int idx = id - 1;
            if (idx >= 0 && idx < versions.Count)
                targetVersion = versions[idx];
        }
        else if (versions.Contains(idOrVersion))
        {
            targetVersion = idOrVersion;
        }

        if (targetVersion == null)
        {
            Utils.WrteLineColor($"Error: Version or ID '{idOrVersion}' not found.", ConsoleColor.Red);
            return;
        }

        string src = Path.Combine(TRASH_DIR, targetVersion, filename);
        string dest = Path.Combine(Directory.GetCurrentDirectory(), filename);

        if (File.Exists(src))
        {
            File.Move(src, dest);
            Console.WriteLine($"Restored file '{filename}' from {targetVersion}");
        }
        else if (Directory.Exists(src))
        {
            Directory.Move(src, dest);
            Console.WriteLine($"Restored directory '{filename}' from {targetVersion}");
        }
        else
        {
            Utils.WrteLineColor($"Error: {filename} in version {targetVersion} not found.", ConsoleColor.Red);
            return;
        }

        string versionPath = Path.Combine(TRASH_DIR, targetVersion);
        if (!Directory.EnumerateFileSystemEntries(versionPath).Any())
            Directory.Delete(versionPath);
    }

    static void PrintRed(string text)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = prev;
    }

    static void PrintRedIdOnly(int id, string fullLine)
    {
        var prev = Console.ForegroundColor;

        string idStr = id.ToString().PadRight(4);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(idStr);
        Console.ForegroundColor = prev;

        Console.WriteLine(fullLine.Substring(4));
    }
}
