using System;

namespace SpliceMachine.Drda
{
    internal readonly struct RequestMessage : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(Byte) + sizeof(Byte) + sizeof(UInt16);

        private readonly UInt16 _requestCorrelationId;

        private readonly CompositeCommand _command;

        private readonly MessageFormat _format;

        public RequestMessage(
            UInt16 requestCorrelationId,
            CompositeCommand command,
            MessageFormat format)
        {
            _requestCorrelationId = requestCorrelationId;
            _command = command;
            _format = format;
        }

        public Int32 GetSize() => BaseSize + _command.GetSize();

        public void Write(
            DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt8(0xD0); // DDMID
            writer.WriteUInt8((Byte)_format);
            writer.WriteUInt16(_requestCorrelationId);

            _command.Write(writer);
        }
    }
}
