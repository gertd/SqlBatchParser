namespace SqlBatchReader
{
    internal class SqlBatchLineChange
    {
        public SqlBatchLineChange(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public int Offset { get; private set; }
        public int Length { get; private set; }
    }
}
