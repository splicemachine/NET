using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        private static readonly Encoding EbcdicEncoding = Encoding.GetEncoding(37);

        public static void Main()
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                client.Connect("localhost", 1527);

                Console.WriteLine("Client connected.");

                Console.WriteLine();
                Console.WriteLine("1. Handshake");
                Console.WriteLine();

                SendConnectRequest(client.GetStream());
                ReceiveConnectResponse(client.GetStream());

                Console.WriteLine();
                Console.WriteLine("2. Authentication");
                Console.WriteLine();

                SendLoginRequest(client.GetStream());
                ReceiveLoginResponse(client.GetStream());

                Console.WriteLine();

                SendLoginPassRequest(client.GetStream());
                ReceiveLoginPassResponse(client.GetStream());

                Console.WriteLine();
                Console.WriteLine("3. Database");
                Console.WriteLine();

                SendAccessRequest(client.GetStream(),
                    client.Client.LocalEndPoint as IPEndPoint);
                ReceiveAccessResponse(client.GetStream());
            }

            Console.WriteLine("Client closed.");
        }

        private static void ReceiveLoginPassResponse(Stream stream)
        {
            var reader = new DrdaReader(stream);

            Console.WriteLine("Envelope: Length = {0}, DDMID = 0x{1:X2}, Format = 0x{2:X2}, RCID = {3}",
                reader.Size, reader.ReadByte(), reader.ReadByte(), reader.ReadUInt16());

            Console.WriteLine("Command: Length = {0}, CodePoint = 0x{1:X4}",
                reader.ReadUInt16(), reader.ReadUInt16());

            while (reader.HasMoreData)
            {
                var parameter = new DrdaReader(reader);
                var codePoint = reader.ReadUInt16();

                Console.WriteLine("Parameter: Length = {0}, CodePoint = 0x{1:X4}",
                    parameter.Size, codePoint);
                var bytes = reader.ReadBytes(parameter.Size - 4);

                //if ((codePoint & 0xFF00) == 0x1100)
                //{
                //    Console.WriteLine(EbcdicEncoding.GetString(bytes));
                //}
            }
        }

        private static void SendLoginPassRequest(Stream stream)
        {
            using (var envelope = new DrdaWriter())
            {
                envelope.WriteByte(0xD0); // DDMID
                envelope.WriteByte(0x01); // RQSDSS_Req
                envelope.WriteUint16(0x0001); // Request Correlation ID

                using (var command = new DrdaWriter())
                {
                    command.WriteUint16(0x106E); // SECCHK

                    command.WriteUint16(0x0006);
                    command.WriteParameter(0x11A2, 0x0003); // SECMEC, USRIDPWD

                    // TODO: olegra - check if it really needed
                    //command.WriteParameter(0x2110, "splicedb".PadRight(18, ' ')); // RDBNAM

                    command.WriteParameter(0x11A1, "admin", Encoding.UTF8); // PASSWORD
                    command.WriteParameter(0x11A0, "splice", Encoding.UTF8); // USRID

                    envelope.WriteBuffer(command);
                }

                envelope.CopyTo(stream);
            }
        }

        private static void ReceiveAccessResponse(Stream stream)
        {
            var reader = new DrdaReader(stream);

            Console.WriteLine("Envelope: Length = {0}, DDMID = 0x{1:X2}, Format = 0x{2:X2}, RCID = {3}",
                reader.Size, reader.ReadByte(), reader.ReadByte(), reader.ReadUInt16());

            Console.WriteLine("Command: Length = {0}, CodePoint = 0x{1:X4}",
                reader.ReadUInt16(), reader.ReadUInt16());

            while (reader.HasMoreData)
            {
                var parameter = new DrdaReader(reader);
                var codePoint = reader.ReadUInt16();

                Console.WriteLine("Parameter: Length = {0}, CodePoint = 0x{1:X4}",
                    parameter.Size, codePoint);
                var bytes = reader.ReadBytes(parameter.Size - 4);

                //if ((codePoint & 0xFF00) == 0x1100)
                //{
                //    Console.WriteLine(EbcdicEncoding.GetString(bytes));
                //}
            }
        }

        private static void SendAccessRequest(Stream stream, IPEndPoint endPoint)
        {
            using (var envelope = new DrdaWriter())
            {
                envelope.WriteByte(0xD0); // DDMID
                envelope.WriteByte(0x01); // RQSDSS_Req
                envelope.WriteUint16(0x0001); // Request Correlation ID

                using (var command = new DrdaWriter())
                {
                    command.WriteUint16(0x2001); // ACCRDB

                    command.WriteParameter(0x2110, "splicedb".PadRight(18, ' '), Encoding.UTF8); // RDBNAM

                    command.WriteUint16(0x0006);
                    command.WriteParameter(0x210F, 0x2407); // RDBACCCL, SQLAM

                    command.WriteParameter(0x112E, "SNC10090", Encoding.UTF8); // PRDID
                    command.WriteParameter(0x002F, "QTDSQLASC", Encoding.UTF8); // TYPDEFNAM

                    // TODO: olegra - place for enabling/disbling Snappy compression support
                    command.WriteParameter(0x2104, "Splice ODBC Driver", Encoding.UTF8); // PRDDTA

                    command.WriteParameter(0x2135,
                        Encoding.ASCII.GetBytes(GetCorrelationToken(endPoint))); // CRRTKN

                    using (var encodings = new DrdaWriter())
                    {
                        encodings.WriteUint16(0x0035); // TYPDEFOVR

                        encodings.WriteUint16(0x0006);
                        encodings.WriteParameter(0x119C, 1208); // CCSIDSBC, CCSID_1208
                        encodings.WriteUint16(0x0006);
                        encodings.WriteParameter(0x119D, 1208); // CCSIDDBC, CCSID_1208
                        encodings.WriteUint16(0x0006);
                        encodings.WriteParameter(0x119E, 1208); // CCSIDMBC, CCSID_1208

                        command.WriteBuffer(encodings);
                    }

                    envelope.WriteBuffer(command);
                }

                envelope.CopyTo(stream);
            }
        }

        private static String GetCorrelationToken(IPEndPoint endPoint)
        {
            // TODO: olegra - generate token properly for both IPv4 and IPv6 modes
            var correlationToken = new StringBuilder(61);

            correlationToken.Append("9F00A8C0"); // endPoint.Address
            correlationToken.Append('.');

            var port = endPoint.Port;
            correlationToken.Append((port & 0xFF).ToString("X2", CultureInfo.InvariantCulture));
            correlationToken.Append(((port & 0xFF00) >> 8).ToString("X2", CultureInfo.InvariantCulture));

            // correlationToken.Append('.');
            correlationToken.Append("012345"); // "0123456789AB"

            return correlationToken.ToString();
        }

        private static void ReceiveLoginResponse(Stream stream)
        {
            var reader = new DrdaReader(stream);

            Console.WriteLine("Envelope: Length = {0}, DDMID = 0x{1:X2}, Format = 0x{2:X2}, RCID = {3}",
                reader.Size, reader.ReadByte(), reader.ReadByte(), reader.ReadUInt16());

            Console.WriteLine("Command: Length = {0}, CodePoint = 0x{1:X4}",
                reader.ReadUInt16(), reader.ReadUInt16());

            while (reader.HasMoreData)
            {
                var parameter = new DrdaReader(reader);
                var codePoint = reader.ReadUInt16();

                Console.WriteLine("Parameter: Length = {0}, CodePoint = 0x{1:X4}",
                    parameter.Size, codePoint);
                var bytes = reader.ReadBytes(parameter.Size - 4);
            }
        }

        private static void SendLoginRequest(Stream stream)
        {
            using (var envelope = new DrdaWriter())
            {
                envelope.WriteByte(0xD0); // DDMID
                envelope.WriteByte(0x01); // RQSDSS_Req
                envelope.WriteUint16(0x0001); // Request Correlation ID

                using (var command = new DrdaWriter())
                {
                    command.WriteUint16(0x106D); // ACCSEC

                    command.WriteParameter(0x2110, "splicedb".PadRight(18, ' ')); // RDBNAM
                    command.WriteUint16(0x0006);
                    command.WriteParameter(0x11A2, 0x0003); // SECMEC, USRIDPWD

                    envelope.WriteBuffer(command);
                }

                envelope.CopyTo(stream);
            }
        }

        private static void ReceiveConnectResponse(Stream stream)
        {
            var reader = new DrdaReader(stream);

            Console.WriteLine("Envelope: Length = {0}, DDMID = 0x{1:X2}, Format = 0x{2:X2}, RCID = {3}",
                reader.Size, reader.ReadByte(), reader.ReadByte(), reader.ReadUInt16());

            Console.WriteLine("Command: Length = {0}, CodePoint = 0x{1:X4}",
                reader.ReadUInt16(), reader.ReadUInt16());

            while (reader.HasMoreData)
            {
                var parameter = new DrdaReader(reader);
                var codePoint = reader.ReadUInt16();

                Console.WriteLine("Parameter: Length = {0}, CodePoint = 0x{1:X4}",
                    parameter.Size, codePoint);
                var bytes = reader.ReadBytes(parameter.Size - 4);

                if ((codePoint & 0xFF00) == 0x1100)
                {
                    Console.WriteLine(EbcdicEncoding.GetString(bytes));
                }
            }
        }

        private static void SendConnectRequest(Stream stream)
        {
            using (var envelope = new DrdaWriter())
            {
                envelope.WriteByte(0xD0); // DDMID
                envelope.WriteByte(0x01); // RQSDSS_Req
                envelope.WriteUint16(0x0001); // Request Correlation ID

                using (var command = new DrdaWriter())
                {
                    command.WriteUint16(0x1041); // EXCSAT

                    command.WriteParameter(0x115E, "derbydncmain"); // EXTNAM

                    using (var levels = new DrdaWriter())
                    {
                        levels.WriteUint16(0x1404); // MGRLVLLS

                        levels.WriteParameter(0x1403, 0x0007); // AGENT, DDM_Level7
                        levels.WriteParameter(0x2407, 0x0007); // SQLAM, DDM_Level7
                        levels.WriteParameter(0x240F, 0x0007); // RDB, DDM_Level7
                        levels.WriteParameter(0x1440, 0x0007); // SECMGR, DDM_Level7

                        levels.WriteParameter(0x1C08, 1208); // UNICODEMGR, CCSID_1208

                        command.WriteBuffer(levels);
                    }

                    command.WriteParameter(0x1147, "SM/WIN32"); // SRVCLSNM
                    command.WriteParameter(0x116D, "Derby"); // SRVNAM
                    command.WriteParameter(0x115A, "SNC1009-/10.9.1.0 - (1344872)"); // SRVRLSLV

                    envelope.WriteBuffer(command);
                }

                envelope.CopyTo(stream);
            }
        }
    }
}
