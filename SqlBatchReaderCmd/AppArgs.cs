using SqlBatchReaderCmd.Common;

namespace SqlBatchReaderCmd
{
    public sealed class AppArgs
    {
        [Argument(ArgumentType.AtMostOnce|ArgumentType.Required, LongName = "inputfile", ShortName = "i")]
        public string InputFile { get; private set; }

        [Argument(ArgumentType.AtMostOnce, LongName = "removecomments", ShortName = "r")]
        public bool RemoveComments { get; private set; }

        [Argument(ArgumentType.AtMostOnce, LongName = "trimwhitespace", ShortName = "t")]
        public bool TrimWhiteSpace { get; private set; }

        [Argument(ArgumentType.AtMostOnce, LongName = "omitemptylines", ShortName = "o")]
        public bool OmitEmptyLines { get; private set; }

        [Argument(ArgumentType.AtMostOnce, LongName = "version", ShortName = "v")]
        public bool Version { get; private set; }

        [Argument(ArgumentType.AtMostOnce, LongName = "help", ShortName = "?")]
        public bool Help { get; private set; }

        public AppArgs()
        {
            InputFile = null;
            RemoveComments = false;
            TrimWhiteSpace = false;
            OmitEmptyLines = false;

            Version = false;
            Help = false;
        }
    }
}
