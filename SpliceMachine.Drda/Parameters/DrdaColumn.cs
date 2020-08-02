using System;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class DrdaColumn : IDrdaMessage
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public DrdaColumn(
            DrdaStreamReader reader)
        {
            Precision = reader.ReadUInt16();
            Scale = reader.ReadUInt16();
            Length = reader.ReadUInt64();
            Db2Type = reader.ReadUInt16();
            
            CcsId = reader.ReadUInt16();
            ArrayExt = reader.ReadUInt8();
            Unnamed = reader.ReadUInt16();

            Name = reader.ReadVcmVcs();
            Label = reader.ReadVcmVcs();
            Comment = reader.ReadVcmVcs();

            var hiByte = reader.ReadUInt8();
            if (hiByte != 0xFF)
            {
                TypeName = reader.ReadVcmVcs(hiByte);
                ClassName = reader.ReadVcmVcs();
            }

            // ReSharper disable once RedundantAssignment
            hiByte = reader.ReadUInt8();

            KeyMem = reader.ReadUInt16();
            Updateable = reader.ReadUInt16();
            Generated = reader.ReadUInt16();
            ParameterMode = reader.ReadUInt16();

            RdbName = reader.ReadVarString();
            CoreName = reader.ReadVcmVcs();
            BaseName = reader.ReadVcmVcs();
            Scheme = reader.ReadVcmVcs();
            DxName = reader.ReadVcmVcs();
        }

        public UInt16 Precision { get; }

        public UInt16 Scale { get;  }

        public UInt64 Length { get; }

        public UInt16 Db2Type { get; }

        public UInt16 CcsId { get; }

        public Byte ArrayExt { get; }

        public UInt16 Unnamed { get; }

        public String Name { get; }

        public String Label { get; }

        public String Comment { get; }

        public String TypeName { get; }

        public String ClassName { get; }

        public UInt16 KeyMem { get; }

        public UInt16 Updateable { get; }

        public UInt16 Generated { get; }

        public UInt16 ParameterMode { get; }

        public String RdbName { get; }

        public String CoreName { get; }

        public String BaseName { get; }

        public String Scheme { get; }

        public String DxName { get; }

        public ColumnType TripletType { get; set; }

        public UInt16 TripletDataSize { get; set; }

        public Int32 GetSize() => 0;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
