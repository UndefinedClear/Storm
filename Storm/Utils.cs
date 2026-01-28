using System;
using System.Drawing;

namespace Storm
{
    public static class Utils
    {
        private static bool isInited = false;

        public static void Init()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            isInited = true;
        }

        public static void ResetFormat()
        {
            if (!isInited) return;

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WrteColor(string text, ConsoleColor color)
        {
            if (!isInited) return;

            Console.ForegroundColor = color;
            Console.Write(text);
            ResetFormat();
        }

        public static void WrteLineColor(string text, ConsoleColor color)
        {
            if (!isInited) return;

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            ResetFormat();
        }
    }
}
