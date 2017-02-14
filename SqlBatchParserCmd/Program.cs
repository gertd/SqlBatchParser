using SqlBatchParser;
using SqlBatchParserCmd.Common;
using System;
using System.IO;

namespace SqlBatchParserCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = new AppArgs();
            if (Parser.ParseArguments(args, parsedArgs))
            {
                if (parsedArgs.Version || parsedArgs.Help)
                {
                    if (parsedArgs.Version)
                    {
                        Helper.DisplayVersion();
                    }
                    else if (parsedArgs.Help)
                    {
                        Console.Out.Write(Parser.ArgumentsUsage(typeof(AppArgs)));
                    }
                    return;
                }
            }

            if (!File.Exists(parsedArgs.InputFile))
            {
                Console.WriteLine("File [{0}] does not exist", parsedArgs.InputFile);
                return;
            }

            try
            {
                var config = new SqlBatchReaderConfig()
                {
                    TrimWhiteSpace = parsedArgs.TrimWhiteSpace,
                    RemoveComments = parsedArgs.RemoveComments,
                    OmitEmptyLines = parsedArgs.OmitEmptyLines
                };

                using (var batchReader = new SqlBatchParser.SqlBatchReader(parsedArgs.InputFile, config))
                {
                    var counter = 0;
                    foreach (var batch in batchReader.Batches())
                    {
                        counter++;
                        Console.WriteLine("***BATCH--[{0}]***", counter);
                        Console.WriteLine(batch);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
