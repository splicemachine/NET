using System;
using System.Collections.Generic;
using System.Linq;

namespace SpliceMachine.Drda
{
    internal readonly struct CompositeParameter : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private readonly IDrdaMessage[] _parameters;

        public CompositeParameter(
            CodePoint codePoint,
            params IDrdaMessage[] parameters)
        {
            CodePoint = codePoint;
            _parameters = parameters;
        }

        public CompositeParameter(
            DrdaStreamReader reader, 
            Int32 sizeWithoutHeader, 
            CodePoint codePoint)
            : this(
                codePoint) =>
            _parameters = ReadParameters(reader, sizeWithoutHeader).ToArray();

        public IReadOnlyCollection<IDrdaMessage> Parameters => _parameters;

        public Int32 GetSize() => BaseSize + _parameters.Sum(_ => _.GetSize());
        
        public CodePoint CodePoint { get; }

        public void Write(
            DrdaStreamWriter writer)
        {
            writer.WriteUint16((UInt16)GetSize());
            writer.WriteUint16((UInt16)CodePoint);

            foreach (var parameter in _parameters)
            {
                parameter.Write(writer);
            }
        }

        private IEnumerable<IDrdaMessage> ReadParameters(
            DrdaStreamReader reader,
            Int32 size)
        {
            while (size > BaseSize)
            {
                var parameter = CodePointMapper.Deserialize(reader);
                size -= parameter.GetSize();
                yield return parameter;
            }
        }
    }
}
