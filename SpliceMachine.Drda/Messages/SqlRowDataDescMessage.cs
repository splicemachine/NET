using System;
using System.Collections.Generic;
using System.Linq;

namespace SpliceMachine.Drda
{
    internal sealed class SqlRowDataDescMessage : IDrdaRequest
    {
        private const UInt32 TripletSize = sizeof(Byte) + sizeof(Byte) + sizeof(Byte);

        private readonly IReadOnlyList<DrdaColumn> _parameters;

        private readonly struct BytesTriplet : IDrdaMessage
        {
            private readonly Byte[] _bytes;

            public BytesTriplet(
                Byte length,
                Byte type,
                Byte lid) =>
                _bytes = new[] { length, type, lid };

            public UInt32 GetSize() => TripletSize;

            public void Write(
                DrdaStreamWriter writer) =>
                writer.WriteBytes(_bytes);
        }

        private readonly struct TypeTriplet : IDrdaMessage
        {
            private readonly ColumnType _type;

            private readonly UInt16 _length;

            public TypeTriplet(
                ColumnType type,
                UInt16 length)
            {
                _type = type;
                _length = length;
            }

            public UInt32 GetSize() => TripletSize;

            public void Write(
                DrdaStreamWriter writer)
            {
                writer.WriteUInt8((Byte)_type);
                writer.WriteUInt16(_length);
            }
        }

        private readonly struct ByteMessage : IDrdaMessage
        {
            private readonly Byte _byte;

            public ByteMessage(
                Byte value) =>
                _byte = value;

            public UInt32 GetSize() => sizeof(Byte);

            public void Write(
                DrdaStreamWriter writer) =>
                writer.WriteUInt8(_byte);
        }

        private readonly struct ParameterValue : IDrdaMessage
        {
            private readonly DrdaColumn _column;

            public ParameterValue(
                DrdaColumn column)
            {
                _column = column;
            }

            public UInt32 GetSize() => (UInt32) _column.GetValueSize();

            public void Write(
                DrdaStreamWriter writer) =>
                _column.WriteColumnValue(writer);
        }

        public SqlRowDataDescMessage(
            UInt16 requestCorrelationId,
            IReadOnlyList<DrdaColumn> parameters)
        {
            _parameters = parameters;
            RequestCorrelationId = requestCorrelationId;
        }

        public UInt16 RequestCorrelationId { get; }

        public MessageFormat Format => MessageFormat.DataObject;

        public void CheckResponseType(
            DrdaResponseBase response)
        {
        }

        public CompositeCommand GetCommand() => new CompositeCommand(
            CodePoint.SQLDTA, 
            new CompositeCommand(
                CodePoint.FDODSC, GetDescriptorTriplets().ToArray()),
            new CompositeCommand(
                CodePoint.FDODTA, GetParameterValues().ToArray()));

        private IEnumerable<IDrdaMessage> GetDescriptorTriplets()
        {
            yield return new BytesTriplet(
                (Byte)(_parameters.Count * TripletSize + TripletSize), 
                118 /* NGDA_TRIPLET_TYPE */, 208 /* SQLDTAGRP_LID */);

            foreach (var parameter in _parameters)
            {
                var length = (UInt16)parameter.Length;
                var type = parameter.Db2Type.AsDrdaType(ref length);

                yield return new TypeTriplet(type, length);
            }

            yield return new BytesTriplet(0x06, 0x71, 0xE4);
            yield return new BytesTriplet(0xD0, 0x00, 0x01);
        }

        private IEnumerable<IDrdaMessage> GetParameterValues()
        {
            yield return new ByteMessage(0x00); // row indicator

            foreach (var parameter in _parameters)
            {
                yield return new ParameterValue(parameter);
            }
        }
    }
}
