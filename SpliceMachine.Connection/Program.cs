using System;
using System.Net.Sockets;
using System.Text;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        private static readonly Encoding EbcdicEncoding = Encoding.GetEncoding(37);

        public static void Main()
        {
            using (var client = new TcpClient("localhost", 1527))
            {
                Console.WriteLine("Client connected.");

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

                    envelope.CopyTo(client.GetStream());
                }

                var reader = new DrdaReader(client.GetStream());

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

            Console.WriteLine("Client closed.");
        }
    }
}
