using SqlBatchParser;
using SqlBatchParserTest.Common;
using System;
using System.Linq;
using Xunit;

namespace SqlBatchParserTest
{
    public class SqlBatchReaderTests1
    {
        private readonly SqlBatchReaderConfig _config = new SqlBatchReaderConfig()
        {
            TrimWhiteSpace = false,
            RemoveComments = true,
            OmitEmptyLines = false
        };

#region LineComments
        [Fact(DisplayName = "LineCommentTest1")]
        public void LineCommentTest1()
        {
            const string input = @"-- inline comment";
            string output = @"" + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(1, batches.Length);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "LineCommentTest2")]
        public void LineCommentTest2()
        {
            const string input = @"select @@version -- inline comment";
            string output = @"select @@version " + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }
        #endregion

#region BlockComments
        [Fact(DisplayName = "BlockCommentTest1")]
        public void BlockCommentTest1()
        {
            const string input = @"/* block comment */";
            string output = @"" + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest2")]
        public void BlockCommentTest2()
        {
            const string input = @"/* block comment */ after";
            string output = @" after" + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest3")]
        public void BlockCommentTest3()
        {
            const string input = @"before /* block comment */";
            string output = @"before " + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest4")]
        public void BlockCommentTest4()
        {
            const string input = @"before /* block comment */ after";
            string output = @"before  after" + Environment.NewLine;

            _config.TrimWhiteSpace = true;
            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest5")]
        public void BlockCommentTest5()
        {
            const string input = @"
select getdate() /* comments start here
continue
and end here */ select getutcdate()
go
";

            string output = @"
select getdate() 

 select getutcdate()
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest6")]
        public void BlockCommentTest6()
        {
            const string input = @"
/* comment
/* nested comment
*/
still a comment
*/
select 1
";

            string output = @"





select 1
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest7")]
        public void BlockCommentTest7()
        {
            const string input = @"
select 0;
/*  
  comment
*/
select 1;
";

            string output = @"
select 0;



select 1;
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest8")]
        public void BlockCommentTest8()
        {
            const string input = @"
/* --------------------------------------------------------------------------------
/* This is the whole complete line of comments with 80 dashes to delimted        */
-------------------------------------------------------------------------------- */
select 2;
";

            string output = @"



select 2;
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BlockCommentTest9")]
        public void BlockCommentTest9()
        {
            const string input = @"
/*  
  comment
*/
select 1;
/* --------------------------------------------------------------------------------
/* This is the whole complete line of comments with 80 dashes to delimted        */
-------------------------------------------------------------------------------- */
select 2;
/* ------------------------------------------------------------------------------ */
/* This is the whole complete line of comments with 80 dashes to delimted         */
/* -----------------------------------------------------------------------------= */
select 3;
";

            string output = @"



select 1;



select 2;



select 3;
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        #endregion

        #region BatchDelimiter

        [Fact(DisplayName = "BatchDelimiterTest1")]
        public void BatchDelimiterTest1()
        {
            const string input = @"
select 1
go
select 2
GO
select 3
Go
select 4
gO
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(4, batches.Length);
        }

        [Fact(DisplayName = "BatchDelimiterTest2")]
        public void BatchDelimiterTest2()
        {
            const string input = @"
print N'
go
'

select 1 as [
go
]


/*
go 1
*/

";
            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
        }



        [Fact(DisplayName = "BatchDelimiterTest3")]
        public void BatchDelimiterTest3()
        {
            const string input = @"
-- line comment
select getdate() as now

";

            const string output = @"

select getdate() as now

";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BatchDelimiterTest4")]
        public void BatchDelimiterTest4()
        {
            const string input = @"/* GO */";
            string output = @"" + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BatchDelimiterTest5")]
        public void BatchDelimiterTest5()
        {
            const string input = @"
/*
GO
*/
";
            string output = @"


" + Environment.NewLine;

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        [Fact(DisplayName = "BatchDelimiterTest6")]
        public void BatchDelimiterTest6()
        {
            const string input = @"
create proc dbo.test @p1 int, /*@p2 int,*/ @p3 int, @p4 /*int*/ float/*, @p5 int
output */, @p6 datetime = getdate()
as
select @@version, @p1, @p3
go
";

            string output = @"
create proc dbo.test @p1 int,  @p3 int, @p4  float
, @p6 datetime = getdate()
as
select @@version, @p1, @p3
";

            var reader = new SqlBatchParser.SqlBatchReader(input.ToStream(), _config);
            var batches = reader.Batches().ToArray();

            Assert.Equal(batches.Length, 1);
            Assert.Equal(output, batches.First());
        }

        #endregion
    }
}    