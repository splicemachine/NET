using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpliceMachine.Drda
{
    internal readonly struct CompositeCommand : ICommand
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private const Int32 MaxSize = 0x7FFF - sizeof(UInt32);

        private const UInt16 SegmentFlag = 0x8008;

        private readonly IDrdaMessage[] _parameters;

        public CompositeCommand(
            CodePoint codePoint,
            params IDrdaMessage[] parameters)
        {
            CodePoint = codePoint;
            _parameters = parameters;
        }

        public CompositeCommand(
            DrdaStreamReader reader,
            Int32 sizeWithoutHeader,
            CodePoint codePoint)
            : this(
                codePoint,
                ReadParameters(reader, sizeWithoutHeader).ToArray())
        {
        }

        public Int32 GetSize() => BaseSize + checkAndAdjustForSegmentation(
            _parameters.Sum(_ => _.GetSize()));

        public CodePoint CodePoint { get; }

        public void Write(
            DrdaStreamWriter writer)
        {
            var size = GetSize();
            if (size > MaxSize)
            {
                writer.WriteUInt16(SegmentFlag);
                writer.WriteUInt16((UInt16)CodePoint);
                writer.WriteUInt32((UInt32)GetSize());
            }
            else
            {
                writer.WriteUInt16((UInt16)GetSize());
                writer.WriteUInt16((UInt16)CodePoint);
            }

            foreach (var parameter in _parameters)
            {
                parameter.Write(writer);
            }
        }

        private static IEnumerable<IDrdaMessage> ReadParameters(
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

        private Int32 checkAndAdjustForSegmentation(Int32 size) => 
            size > MaxSize ? size + sizeof(UInt32) : size;

        public IEnumerator<IDrdaMessage> GetEnumerator() => 
            _parameters.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            _parameters.GetEnumerator();
    }
}
