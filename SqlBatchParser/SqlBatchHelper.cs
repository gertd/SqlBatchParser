namespace SqlBatchParser
{
    public static class SqlBatchHelper
    {
        public static string Trim(this string str, bool condition)
        {
            return condition ? str.Trim() : str;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}
