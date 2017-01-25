using Xunit;

namespace SqlBatchReaderTest
{
    public class SqlBatchRegExTests
    {
        [Theory(DisplayName = "SqlBatchRegExTests1")]
        [InlineData(1, "GO", "GO", @"batch", true)]
        [InlineData(2, "--", "--", @"inline", true)]
        [InlineData(3, "/*", "/*", @"blockstart", true)]
        [InlineData(4, "*/", "*/", @"blockend", true)]
        [InlineData(5, "\'", "\'", @"singlequote", true)]
        [InlineData(6, "\"", "\"", @"doublequote", true)]
        [InlineData(7, "[", "[", @"lbracket", true)]
        [InlineData(8, "]", "]", @"rbracket", true)]
        public void SqlBatchRegExTests1(int id, string input, string expectedOutput, string groupname, bool expectedMatch)
        {
            var regex = SqlBatchReader.SqlBatchReader.InitRegEx("GO");
            var matches = regex.Matches(input);

            Assert.Equal(matches.Count, 1);
            Assert.Equal(matches[0].Groups[groupname].Success, expectedMatch);
            Assert.Equal(matches[0].Groups[groupname].Value, expectedOutput);
        }
    }
}
