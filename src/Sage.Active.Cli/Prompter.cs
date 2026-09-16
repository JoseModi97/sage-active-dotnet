using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage.Active.Cli
{
    public static class Prompter
    {
        public static string Prompt(string message, string defaultValue = "")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"? ");
            Console.ResetColor();

            if (!string.IsNullOrEmpty(defaultValue))
            {
                Console.Write($"{message} ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({defaultValue}): ");
                Console.ResetColor();
            }
            else
            {
                Console.Write($"{message}: ");
            }

            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
        }

        public static string PromptSecret(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"? ");
            Console.ResetColor();
            Console.Write($"{message}: ");

            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    pass += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return pass;
        }

        public static void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✔ {message}");
            Console.ResetColor();
        }

        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✖ {message}");
            Console.ResetColor();
        }
    }

    public static class TableFormatter
    {
        public static void PrintTable(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
        {
            var headerList = headers.ToList();
            var rowList = rows.Select(r => r.ToList()).ToList();

            var colWidths = new int[headerList.Count];
            for (var i = 0; i < headerList.Count; i++)
            {
                var max = headerList[i].Length;
                foreach (var row in rowList)
                {
                    if (i < row.Count && row[i] != null)
                    {
                        max = Math.Max(max, row[i].Length);
                    }
                }
                colWidths[i] = max + 2;
            }

            // Print Header
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (var i = 0; i < headerList.Count; i++)
            {
                Console.Write(headerList[i].PadRight(colWidths[i]));
            }
            Console.WriteLine();

            // Print Divider
            for (var i = 0; i < headerList.Count; i++)
            {
                Console.Write(new string('-', colWidths[i] - 1).PadRight(colWidths[i]));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print Rows
            foreach (var row in rowList)
            {
                for (var i = 0; i < headerList.Count; i++)
                {
                    var text = i < row.Count ? row[i] ?? "" : "";
                    Console.Write(text.PadRight(colWidths[i]));
                }
                Console.WriteLine();
            }
        }
    }
}
