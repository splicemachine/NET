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
            UInt32 sizeWithoutHeader,
            CodePoint codePoint)
            : this(
                codePoint,
                ReadParameters(reader, sizeWithoutHeader).ToArray())
        {
        }

        public UInt32 GetSize() => BaseSize + checkAndAdjustForSegmentation(
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
                writer.WriteUInt32(size);
                //var extendedLengthByteCount = calculateExtendedLengthByteCount(size);
                //if (extendedLengthByteCount != 0 && CodePoint == CodePoint.FDODTA)
                //{
                //    uint extendedLength = (size - 8);
                //    size = (uint)extendedLengthByteCount + 4;
                //    size |= 0x8000;
                //    writer.WriteUInt32(size);
                //    // shift the data to the right by the number of extended length bytes needed.
                //    // write the extended length
                //    int shiftSize = (extendedLengthByteCount - 1) * 8;
                //    for (int i = 0; i < extendedLengthByteCount; i++)
                //    {
                //        writer.WriteUInt8((byte)((uint)(extendedLength >> shiftSize)));
                //        shiftSize -= 8;
                //    }
                //}
            }
            else
            {
                writer.WriteUInt16((UInt16)size);
                writer.WriteUInt16((UInt16)CodePoint);
            }

            foreach (var parameter in _parameters)
            {
                parameter.Write(writer);
            }
        }

        private static IEnumerable<IDrdaMessage> ReadParameters(
            DrdaStreamReader reader,
            UInt32 size)
        {
            while (size > BaseSize)
            {
                var parameter = CodePointMapper.Deserialize(reader);
                size -= parameter.GetSize();
                yield return parameter;
            }
        }

        private UInt32 checkAndAdjustForSegmentation(Int64 size) => 
            size > MaxSize ? (UInt32)size + sizeof(UInt32) : (UInt32)size;

        public IEnumerator<IDrdaMessage> GetEnumerator() => 
            _parameters.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            _parameters.GetEnumerator();

        private int calculateExtendedLengthByteCount(long ddmSize) //throws SqlException
        {
            // according to Jim and some tests perfomred on Lob data,
            // the extended length bytes are signed.  Assume that
            // if this is the case for Lobs, it is the case for
            // all extended length scenarios.
            if (ddmSize <= 0x7FFF)
            {
                return 0;
            }
            else if (ddmSize <= 0x7FFFFFFFL)
            {
                return 4;
            }
            else if (ddmSize <= 0x7FFFFFFFFFFFL)
            {
                return 6;
            }
            else
            {
                return 8;
            }
        }
    }
}
