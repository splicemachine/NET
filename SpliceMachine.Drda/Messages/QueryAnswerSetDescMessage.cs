using System;
using System.IO;

namespace SpliceMachine.Drda
{
    public sealed class QueryAnswerSetDescMessage : DrdaResponseBase
    {
        private readonly struct Triple
        {
            public Triple(
                Byte type,
                UInt16 size)
            {
                Type = (ColumnType)type;
                Size = size;
            }

            public ColumnType Type { get; }

            public UInt16 Size { get; }
        }

        private readonly Byte[] _messageBytes;

        internal QueryAnswerSetDescMessage(
            ResponseMessage response)
            : base(response) =>
            _messageBytes = ((ReaderCommand) response.Command).GetMessageBytes();

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);

        internal void ProcessAndFillQueryContextData(
            QueryContext context)
        {
            using var stream = new MemoryStream(_messageBytes, false);

            var reader = new DrdaStreamReader(stream);
            while (stream.Position < stream.Length)
            {
                var length = reader.ReadUInt8();
                var type = reader.ReadUInt8();
                var id = reader.ReadUInt8();

                // TODO: olegra - store full parsing tree here later

                if (id != 0xD0)
                {
                    reader.ReadBytes(length - 3);
                    continue;
                }

                var triples = new Triple[length / 3 - 1];
                for (var index = 0; index < triples.Length; ++index)
                {
                    switch (type)
                    {
                        case 117: // GDA
                        case 118: // NGDA
                            triples[index] = new Triple(
                                reader.ReadUInt8(), 
                                reader.ReadUInt16());
                            break;

                        case 113: // RLO
                            // TODO: olegra - skip for now
                            reader.ReadUInt8();
                            reader.ReadUInt16(); 
                            break;
                    }
                }

                for (var index = 0; index < triples.Length; ++index)
                {
                    context.Columns[index].TripletType = triples[index].Type;
                    context.Columns[index].TripletDataSize = triples[index].Size;
                }
            }
        }
    }
}
