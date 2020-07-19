namespace SpliceMachine.Drda
{
    internal static class CodePointMapper
    {
        public static IDrdaMessage Deserialize(
            DrdaStreamReader reader)
        {
            var size = reader.ReadUint16();

            var codePoint = (CodePoint)reader.ReadUint16();
            
            // TODO: olegra - find the better way for handling code points here

            switch (codePoint)
            {
                case CodePoint.EXSATRM:
                case CodePoint.ACCSECRM:
                case CodePoint.SECCHKRM:
                case CodePoint.ACCRDBRM:
                    return new CompositeParameter(reader, size, codePoint);

                case CodePoint.EXTNAM:
                case CodePoint.RDBNAM:
                    return new BytesParameter(reader, size, codePoint);

                case CodePoint.SECMEC:
                    return new UInt16Parameter(reader, codePoint);

                default:
                    return new BytesParameter(reader, size, codePoint);
            }
        }
    }
}
