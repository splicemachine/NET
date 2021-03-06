﻿using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal readonly struct PackageSerialNumber : IDrdaMessage
    {
        private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private static readonly String CollationId = "NULLID".PadRight(18, ' ');

        private static readonly String PackageId = "SYSLH000".PadRight(18, ' ');

        private static readonly String Token = "SYSLVL01";

        private readonly UInt16 _packageSerialNumber;

        public PackageSerialNumber(
            UInt16 packageSerialNumber) => 
            _packageSerialNumber = packageSerialNumber;

        public UInt32 GetSize() =>
            BaseSize + (UInt32)WellKnownStrings.DatabaseName.Length +
            (UInt32)CollationId.Length + (UInt32)PackageId.Length + (UInt32)Token.Length +
            sizeof(UInt16);

        public void Write(
            DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint.PKGNAMCSN);

            writer.WriteBytes(Encoding.UTF8.GetBytes(WellKnownStrings.DatabaseName));
            writer.WriteBytes(Encoding.UTF8.GetBytes(CollationId));
            writer.WriteBytes(Encoding.UTF8.GetBytes(PackageId));
            writer.WriteBytes(Encoding.UTF8.GetBytes(Token));

            writer.WriteUInt16(_packageSerialNumber);
        }
    }
}