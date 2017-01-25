using System.IO;

namespace SqlBatchReader.Common
{
    public static class StringExtensions
    {
        public static string Trim(this string str, bool condition)
        {
            return condition ? str.Trim() : str;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static Stream ToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
