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
        Alias alias_clear = new Alias(["-c", "--clear", "--clear"]);


        if (!Directory.Exists(TRASH_DIR))
        {
            Directory.CreateDirectory(TRASH_DIR);
            Utils.WriteLineColor("Trash Can created!", ConsoleColor.Green);
        }


        if (args.Length == 0)
        {
            PrintBanner();
            Utils.WriteLineColor("USAGE:", Utils.PURPLE);
            Utils.WriteLineColor($"  storm --history", Utils.CYAN);
            Utils.WriteColor("    View deleted files history | ", Utils.GRAY);
            Utils.WriteLineColor(alias_history.ToString(), Utils.PINK);

            Utils.WriteLineColor($"  storm --permanent <files...>", Utils.CYAN);
            Utils.WriteColor("    Delete files forever | ", Utils.GRAY);
            Utils.WriteLineColor(alias_permanent.ToString(), Utils.PINK);

            Utils.WriteLineColor($"  storm --clear", Utils.CYAN);
            Utils.WriteColor("    Empty the trash can | ", Utils.GRAY);
            Utils.WriteLineColor(alias_clear.ToString(), Utils.PINK);

            Utils.WriteLineColor($"  storm --restore <ID|VERSION> <FILENAME>", Utils.CYAN);
            Utils.WriteColor("    Bring files back | ", Utils.GRAY);
            Utils.WriteLineColor(alias_restore.ToString(), Utils.PINK);

            Utils.WriteLineColor("  storm <files...>", Utils.CYAN);
            Utils.WriteLineColor("    Move files to trash", Utils.GRAY);
            return;
        }

        //Console.WriteLine(alias_restore.ToString());

        if (alias_history.Have(args[0]))
        {
            History();
        }
        else if (alias_clear.Have(args[0]))
        {
            ClearTrashCan();
        }
        else if (alias_restore.Have(args[0]))
        {
            if (args.Length < 3)
            {
                Utils.WriteLineColor(
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
                Utils.WriteLineColor("Error: --permament requires at least one file or folder", ConsoleColor.Red);
                return;
            }

            PermanentDelete(args.Skip(1).ToArray());
        }
        else if (args[0].StartsWith("-"))  // Unknown flag
        {
            Utils.WriteLineColor($"Unknown option: {args[0]}", ConsoleColor.Red);
            return;
        }
        else
        {
            Trash(args);
        }

    }

    public static DirectoryInfo? Get_Folder_Object(string name)
    {
        DirectoryInfo[]? array = Directory.GetParent(name)?.GetDirectories();
        for (int i = 0; i < array?.Length; i++)
        {
            DirectoryInfo directoryInfo = array[i];
            if (directoryInfo.Name == name)
            {
                return directoryInfo;
            }
        }

        return null;
    }

    static void ClearTrashCan()
    {
        if (Directory.Exists(TRASH_DIR) &&
            Directory.EnumerateDirectories(TRASH_DIR).Any())
        {
            Utils.WriteLineColor("! WARNING: Permanent deletion cannot be undone !", Utils.RED);
            Utils.WriteColor("Are you sure? (y/n) > ", Utils.ORANGE);

            string? answer = Console.ReadLine();

            if (answer?.ToLower() != "y") return;

            Directory.Delete(TRASH_DIR, true);
            Directory.CreateDirectory(TRASH_DIR);

            Utils.WriteLineColor("[+] Trash cleared!", Utils.GREEN);
        }
        else
        {
            Utils.WriteLineColor("[!] Trash can is already empty.", Utils.ORANGE);
        }

    }

    static void PermanentDelete(string[] paths)
    {
        Utils.WriteLineColor("! WARNING: Permanent deletion cannot be undone !", Utils.RED);
        Utils.WriteColor("Are you sure? (y/n) > ", Utils.ORANGE);

        string? answer = Console.ReadLine();

        if (answer?.ToLower() != "y") return;

        foreach (var path in paths)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Utils.WriteLineColor($"[-] Permanently deleted: {path}", Utils.RED);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Utils.WriteLineColor($"[-] Permanently deleted directory: {path}", Utils.RED);
                }
                else
                {
                    Utils.WriteLineColor($"[?] Path not found: {path}", Utils.GRAY);
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLineColor($"[!] Error: {ex.Message}", Utils.RED);
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
        if (paths.Length == 0) return;

        string version = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string versionDir = Path.Combine(TRASH_DIR, version);
        Directory.CreateDirectory(versionDir);

        List<string> not_safe = [];
        not_safe.Add("/");
        not_safe.Add("\\");
        not_safe.Add("C:");
        not_safe.Add("D:");
        not_safe.Add("./");
        not_safe.Add("/");
        not_safe.Add("..");
        not_safe.Add("../");
        not_safe.Add(TRASH_DIR);

        foreach (var path in paths)
        {
            if (not_safe.Contains(path))
            {
                Utils.WriteLineColor($"[!] Error: Access denied to system directory '{path}'", Utils.RED);
                return;
            }


            if (File.Exists(path))
            {
                string dest = Path.Combine(versionDir, Path.GetFileName(path));
                File.Move(path, dest);
                Utils.WriteColor("[+] Moved file: ", Utils.GREEN);
                Utils.WriteColor(path, Utils.CYAN);
                Utils.WriteLineColor($" (ID: {version})", Utils.GRAY);
            }
            else if (Directory.Exists(path))
            {
                string dest = Path.Combine(versionDir, Path.GetFileName(path));
                Directory.Move(path, dest);
                Utils.WriteColor("[+] Moved directory: ", Utils.GREEN);
                Utils.WriteColor(path, Utils.CYAN);
                Utils.WriteLineColor($" (ID: {version})", Utils.GRAY);
            }
            else
            {
                Utils.WriteLineColor($"[?] Path not found: {path}", Utils.GRAY);
            }
        }
    }

    static void History()
    {
        var versions = GetVersions();
        if (!versions.Any())
        {
            Utils.WriteLineColor("No history available.", ConsoleColor.Red);
            return;
        }

        Utils.WriteColor("ID  ", Utils.PURPLE);
        Utils.WriteColor("| ", Utils.GRAY);
        Utils.WriteColor("Timestamp           ", Utils.PURPLE);
        Utils.WriteColor("| ", Utils.GRAY);
        Utils.WriteLineColor("Contents", Utils.PURPLE);
        Utils.WriteLineColor(new string('━', 70), Utils.GRAY);

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
            {
                Utils.WriteColor($"{entry.Id,-3} ", Utils.RED);
                Utils.WriteColor("| ", Utils.GRAY);
                Utils.WriteColor($"{entry.Version,-20} ", Utils.CYAN);
                Utils.WriteColor("| ", Utils.GRAY);
                Utils.WriteLineColor(contents + " (NEWEST)", Utils.PINK);
            }
            else
            {
                Utils.WriteColor($"{entry.Id,-3} ", Utils.ORANGE);
                Utils.WriteColor("| ", Utils.GRAY);
                Utils.WriteColor($"{entry.Version,-20} ", Utils.YELLOW);
                Utils.WriteColor("| ", Utils.GRAY);
                Utils.WriteLineColor(contents, Utils.GRAY);
            }
        }
    }

    static void PrintBox(string title, Dictionary<string, string> rows, string borderColor)
    {
        int padding = 2;

        // Строки без ANSI для расчёта длины
        var rawLines = new List<string> { title };
        rawLines.AddRange(rows.Select(r => $"{r.Key} : {r.Value}"));

        int width = rawLines.Max(l => l.Length) + padding * 2;

        string top = $"╔{new string('═', width)}╗";
        string mid = $"╟{new string('─', width)}╢";
        string bot = $"╚{new string('═', width)}╝";

        Console.WriteLine();
        Console.WriteLine($"{borderColor}{top}{Utils.RESET}");

        // Title centered
        string titlePad = title.PadRight(width - padding);
        Console.WriteLine($"{borderColor}║{Utils.RESET} {Utils.GREEN}{titlePad}{Utils.RESET} {borderColor}║{Utils.RESET}");

        Console.WriteLine($"{borderColor}{mid}{Utils.RESET}");

        foreach (var row in rows)
        {
            string line = $"{row.Key} : {row.Value}";
            Console.WriteLine($"{borderColor}║{Utils.RESET} {Utils.GRAY}{row.Key}{Utils.RESET} : {Utils.YELLOW}{row.Value.PadRight(width - row.Key.Length - 3 - padding)}{Utils.RESET} {borderColor}║{Utils.RESET}");
        }

        Console.WriteLine($"{borderColor}{bot}{Utils.RESET}");
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
            Utils.WriteLineColor($"Error: Version or ID '{idOrVersion}' not found.", ConsoleColor.Red);
            return;
        }

        string src = Path.Combine(TRASH_DIR, targetVersion, filename);
        string dest = Path.Combine(Directory.GetCurrentDirectory(), filename);

        if (File.Exists(src))
        {
            File.Move(src, dest);

            PrintBox(
                "✔ RESTORE SUCCESS",
                new Dictionary<string, string>
                {
            { "Type", "File" },
            { "Name", filename },
            { "Version", targetVersion }
                },
                Utils.CYAN
            );
        }
        else if (Directory.Exists(src))
        {
            Directory.Move(src, dest);

            PrintBox(
                "✔ RESTORE SUCCESS",
                new Dictionary<string, string>
                {
            { "Type", "Directory" },
            { "Name", filename },
            { "Version", targetVersion }
                },
                Utils.CYAN
            );
        }
        else
        {
            PrintBox(
                "✖ RESTORE FAILED",
                new Dictionary<string, string>
                {
            { "Missing", filename },
            { "Version", targetVersion }
                },
                Utils.RED
            );
            return;
        }

        string versionPath = Path.Combine(TRASH_DIR, targetVersion);
        if (!Directory.EnumerateFileSystemEntries(versionPath).Any())
            Directory.Delete(versionPath);
    }

    static void PrintBanner()
    {
        Console.WriteLine();
        Utils.WriteLineColor("   ⚡ STORM CLI ⚡   ", Utils.CYAN);
        Utils.WriteLineColor("   ━━━━━━━━━━━━━━━   ", Utils.PURPLE);
        Console.WriteLine();
    }

    static void PrintRed(string text)
    {
        Utils.WriteLineColor(text, Utils.RED);
    }

    static void PrintRedIdOnly(int id, string fullLine)
    {
        string idStr = id.ToString().PadRight(4);
        Utils.WriteColor(idStr, Utils.RED);
        Console.WriteLine(fullLine.Substring(4));
    }
}
