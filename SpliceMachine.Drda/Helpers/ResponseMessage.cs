using System;

namespace SpliceMachine.Drda
{
    internal readonly struct ResponseMessage
    {
        public ResponseMessage(
            DrdaStreamReader reader)
        {
            Size = reader.ReadUInt16();

            reader.ReadUInt8(); // DDMID
            Format = (MessageFormat)reader.ReadUInt8();

            RequestCorrelationId = reader.ReadUInt16();

            Command = (ICommand) CodePointMapper.Deserialize(reader);
        }

        public Int32 RequestCorrelationId { get; }

        public MessageFormat Format { get; }

        public ICommand Command { get; }

        public Int32 Size { get; }

        public Boolean IsChained => 
            (Format & MessageFormat.Chained) != 0;
    }
}
