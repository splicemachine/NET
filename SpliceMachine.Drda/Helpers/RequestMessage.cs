using System;

namespace SpliceMachine.Drda
{
    internal readonly struct RequestMessage : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(Byte) + sizeof(Byte) + sizeof(UInt16);

        private readonly Int32 _requestCorrelationId;

        private readonly CompositeParameter _command;

        public RequestMessage(
            Int32 requestCorrelationId,
            CompositeParameter command)
        {
            _requestCorrelationId = requestCorrelationId;
            _command = command;
        }

        public Int32 GetSize() => BaseSize + _command.GetSize();

        public void Write(
            DrdaStreamWriter writer)
        {
            writer.WriteUint16((UInt16)GetSize());
            writer.WriteByte(0xD0); // DDMID
            writer.WriteByte(0x01); // RQSDSS_Req
            writer.WriteUint16((UInt16)_requestCorrelationId);

            _command.Write(writer);
        }
    }
}
