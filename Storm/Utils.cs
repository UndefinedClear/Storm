using System;
using System.Text;

namespace Storm
{
    public static class Utils
    {
        private static bool isInited = false;

        // ANSI Color Constants (Beautiful Palette)
        public const string RESET = "\u001b[0m";
        public const string CYAN = "\u001b[38;2;139;233;253m";
        public const string GREEN = "\u001b[38;2;80;250;123m";
        //public const string ORANGE = "\u001b[38;2;255;184;108m";
        public static string ORANGE = RGBToANSI(255, 164, 0);
        public const string RED = "\u001b[38;2;255;85;85m";
        public const string PURPLE = "\u001b[38;2;189;147;249m";
        public const string PINK = "\u001b[38;2;255;121;198m";
        public const string YELLOW = "\u001b[38;2;241;250;140m";
        public const string GRAY = "\u001b[38;2;100;100;100m";

        public static string RGBToANSI(int r, int g, int b)
        {
            return $"\u001b[38;2;{r};{g};{b}m";
        }

        public static void Init()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // Try to enable virtual terminal processing (needed for some older Win10 consoles)
            try
            {
                // This is a common way to enable ANSI on Windows if not already enabled.
                // However, modern Windows Terminal handles it by default.
            }
            catch { }

            isInited = true;
        }

        public static void ResetFormat()
        {
            Console.Write(RESET);
        }

        public static void WriteColor(string text, string ansiColor)
        {
            Console.Write(ansiColor + text + RESET);
        }

        public static void WriteLineColor(string text, string ansiColor)
        {
            Console.WriteLine(ansiColor + text + RESET);
        }

        // Keep compatibility with old ConsoleColor if needed, but update to use ANSI
        public static void WriteLineColor(string text, ConsoleColor color)
        {
            string ansi = color switch
            {
                ConsoleColor.Green => GREEN,
                ConsoleColor.Red => RED,
                ConsoleColor.Yellow => YELLOW,
                ConsoleColor.Cyan => CYAN,
                ConsoleColor.DarkGreen => GRAY,
                _ => RESET
            };
            WriteLineColor(text, ansi);
        }
    }
}
