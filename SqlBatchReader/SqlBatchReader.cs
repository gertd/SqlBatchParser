using SqlBatchReader.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlBatchReader
{
    public class SqlBatchReader : StreamReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="config"></param>
        public SqlBatchReader(string inputFile, SqlBatchReaderConfig config = null) : base(inputFile)
        {
            Config = (config ?? (Config = new SqlBatchReaderConfig()));
            _regex = InitRegEx(Config.BatchDelimiter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="config"></param>
        public SqlBatchReader(Stream stream, SqlBatchReaderConfig config = null) : base(stream)
        {
            Config = (config ?? (Config = new SqlBatchReaderConfig()));
            _regex = InitRegEx(Config.BatchDelimiter);
        }

        private static Regex _regex;

        public static Regex InitRegEx(string batchDelimiter)
        {
            return new Regex("(?<batch>^GO) | (?<inline>[-][-]) | (?<blockstart>[/][*]) | (?<blockend>[*][/]) | (?<singlequote>[\\']) | (?<doublequote>[\\\"]) | (?<lbracket>[[]) | (?<rbracket>[]])",
                RegexOptions.IgnoreCase | 
                RegexOptions.IgnorePatternWhitespace | 
                RegexOptions.ExplicitCapture | 
                RegexOptions.CultureInvariant |
                RegexOptions.Compiled |
                RegexOptions.Multiline);
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlBatchReaderConfig Config { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Batches()
        {
            var stringBuffer = new StringBuilder();
            var lineChanges = new Stack<SqlBatchLineChange>();

            var insideInlineComment = false;
            var blockCommentDepth = 0;
            var startOffsetBlockComment = 0;
            var insideSingleQuoteString = false;
            var insideDoubleQuoteString = false;
            var insideSquareBrackets = false;
            var isBatch = false;

            while (Peek() > 0)
            {
                var line = ReadLine();
                Debug.WriteLine("line: [{0}]\nlength: [{1}]", line, line.Length);

                var matches =_regex.Matches(line);
                Debug.WriteLine("matches: [{0}]", matches.Count);

                foreach (Match match in matches)
                {
                    Debug.WriteLine("groupname: [{0}] offset: [{1}] length: [{2}]", ShowMatchedGroupName(_regex, match), match.Index, match.Length);
                        
                    if (match.Groups["batch"].Success)
                    {
                        if (blockCommentDepth > 0 || insideSingleQuoteString || insideDoubleQuoteString || insideSquareBrackets) continue;
                        
                        yield return stringBuffer.ToString();
                        stringBuffer.Clear();
                        line = string.Empty;
                        isBatch = true;
                        break;
                    }
                    else if (match.Groups["inline"].Success)
                    {
                        if (blockCommentDepth > 0) continue;

                        insideInlineComment = true;
                        lineChanges.Push(new SqlBatchLineChange(match.Index, line.Length - match.Index));
                    }
                    else if (match.Groups["blockstart"].Success)
                    {
                        if (insideInlineComment) continue;

                        blockCommentDepth++;
                        startOffsetBlockComment = match.Index;
                    }
                    else if (match.Groups["blockend"].Success)
                    {
                        Debug.Assert(blockCommentDepth > 0);

                        if (insideInlineComment) continue;

                        blockCommentDepth--;

                        if (blockCommentDepth == 0)
                        {
                            lineChanges.Push(new SqlBatchLineChange(startOffsetBlockComment, (match.Index + match.Length) - startOffsetBlockComment));
                        }
                    }
                    else if (match.Groups["lbracket"].Success)
                    {
                        insideSquareBrackets = true;
                    }
                    else if (match.Groups["rbracket"].Success)
                    {
                        Debug.Assert(insideSquareBrackets);
                        insideSquareBrackets = false;
                    }
                    else if (match.Groups["singlequote"].Success)
                    {
                        insideSingleQuoteString = !insideSingleQuoteString;
                    }
                    else if (match.Groups["doublequote"].Success)
                    {
                        insideDoubleQuoteString = !insideDoubleQuoteString;
                    }
                    else
                    {
                        throw new NotSupportedException("Unhandled groupname match");
                    }

                } // foreach(Match match in matches)

                insideInlineComment = false;

                if (isBatch)
                {
                    isBatch = false;
                    continue;
                }

                if (Config.RemoveComments)
                {
                    if (blockCommentDepth > 0)
                    {
                        lineChanges.Push(new SqlBatchLineChange(startOffsetBlockComment, line.Length - startOffsetBlockComment));
                        startOffsetBlockComment = 0;
                    }

                    while (lineChanges.Count > 0)
                    {
                        var lc = lineChanges.Pop();
                        line = line.Remove(lc.Offset, lc.Length);
                    }
                }

                if (Config.OmitEmptyLines && line.IsEmpty()) continue;

                stringBuffer.AppendLine(line.Trim(Config.TrimWhiteSpace));

            } // while (Peek() > 0)

            if (stringBuffer.Length != 0)
            {
                yield return stringBuffer.ToString();
            }
        }

        private static string ShowMatchedGroupName(Regex regex, Match match)
        {
            return regex.GetGroupNames()
                .Where(name => name != "0")
                .Where(name => match.Groups[name].Success)
                .ToList()
                .First();
        }
    }
}
