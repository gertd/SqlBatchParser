using System;
using System.Diagnostics;
using System.Linq;

namespace SqlBatchParserCmd.Common
{
    public static class Helper
    {
        public static void DisplayVersion()
        {
            var arguments = Environment.GetCommandLineArgs();

            if (!string.IsNullOrEmpty(arguments[0]) && System.IO.File.Exists(arguments[0]))
            {
                var vi = FileVersionInfo.GetVersionInfo(arguments[0]);

                Console.WriteLine("{0} ({1})", vi.Comments, vi.OriginalFilename);

                if (vi.IsDebug)
                    Console.Out.WriteLine("!!!DEBUG VERSION!!!");

                Console.WriteLine(vi.LegalCopyright);
                Console.WriteLine(
                    "Version {0}.{1}.{2}.{3}",
                    vi.FileMajorPart,
                    vi.FileMinorPart,
                    vi.FileBuildPart,
                    vi.FilePrivatePart);
            }
        }

        public static string Repeat(this string s, int n)
        {
            return new string(Enumerable.Range(0, n).SelectMany(x => s).ToArray());
        }

        public static string Repeat(this char c, int n)
        {
            return new string(c, n);
        }
    }
}
