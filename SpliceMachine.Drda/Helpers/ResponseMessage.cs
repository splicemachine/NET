using System;

namespace SpliceMachine.Drda
{
    internal readonly struct ResponseMessage
    {
        public ResponseMessage(
            DrdaStreamReader reader)
        {
            Size = reader.ReadUint16();

            reader.ReadByte(); // DDMID
            reader.ReadByte(); // Format

            RequestCorrelationId = reader.ReadUint16();

            Command = (CompositeParameter)CodePointMapper.Deserialize(reader);
        }

        public Int32 RequestCorrelationId { get; }

        public CompositeParameter Command { get; }

        public Int32 Size { get; }
    }
}
