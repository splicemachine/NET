using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal sealed class SqlStatementRequest : IDrdaRequest
    {
        private readonly String _sqlStatement;

        private readonly struct RawSqlData : IDrdaMessage
        {
            private const Int32 BaseSize = sizeof(Byte) + sizeof(Byte);

            private readonly String _sqlStatement;

            public RawSqlData(
                String sqlStatement) =>
                _sqlStatement = sqlStatement;

            public Int32 GetSize() => BaseSize + _sqlStatement.Length;

            public void Write(
                DrdaStreamWriter writer)
            {
                writer.WriteUInt8(0x00);
                writer.WriteUInt32((UInt32)_sqlStatement.Length);
                writer.WriteBytes(Encoding.UTF8.GetBytes(_sqlStatement));
                writer.WriteUInt8(0xFF);
            }
        }

        public SqlStatementRequest(
            UInt16 requestCorrelationId,
            String sqlStatement)
        {
            RequestCorrelationId = requestCorrelationId;
            _sqlStatement = sqlStatement;
        }

        public UInt16 RequestCorrelationId { get; }

        public MessageFormat Format => MessageFormat.DataObject;

        public void CheckResponseType(
            DrdaResponseBase response)
        {
        }

        public CompositeCommand GetCommand() => new CompositeCommand(
            CodePoint.SQLSTT, new RawSqlData(_sqlStatement));
    }
}
