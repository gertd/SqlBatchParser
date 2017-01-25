namespace SqlBatchReader
{
    public class SqlBatchReaderConfig
    {
        /// <summary>
        /// Batch delimiter, defaults to GO (case-incensitive)
        /// The batch delimiter has to be at the beginning of the line
        /// 
        /// Default value = GO
        /// NOTE: currently the batch delimiter cannot be changed
        /// </summary>
        public string BatchDelimiter { get; private set; }
        
        /// <summary>
        /// Remove inline and block comment text from the result batch
        /// 
        /// Default value = false
        /// </summary>
        public bool RemoveComments { get; set; }
        
        /// <summary>
        /// Trime the whitespace (leading and trailing) from each line in the batch
        /// 
        /// Default value = false
        /// </summary>
        public bool TrimWhiteSpace { get; set; }
        
        /// <summary>
        /// Remove empty lines from the resulting batch
        /// 
        /// Default value = false
        /// </summary>
        public bool OmitEmptyLines { get; set; }

        /// <summary>
        /// Configuration setting for the SqlBatchReader
        /// </summary>
        public SqlBatchReaderConfig()
        {
            BatchDelimiter = "GO";
            RemoveComments = false;
            TrimWhiteSpace = false;
            OmitEmptyLines = false;
        }
    }
}
