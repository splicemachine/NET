using System;
using System.Net.Sockets;
using SpliceMachine.Drda;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        public static void Main()
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                client.Connect("localhost", 1527);
                var stream = client.GetStream();

                Console.WriteLine("Client connected.");

                Console.WriteLine();
                Console.WriteLine("1. Handshake");
                Console.WriteLine();

                var requestCorrelationId = 0;
                
                var request1 = new ExchangeServerAttributesRequest(++requestCorrelationId);
                stream.WriteDrdaMessage(request1);
                var response1 = (ExchangeServerAttributesResponse)stream.ReadDrdaMessage();

                if (response1.RequestCorrelationId != request1.RequestCorrelationId)
                {
                    Console.Error.WriteLine("ERROR: Correlation IDs mismatch!");
                }

                Console.WriteLine();
                Console.WriteLine("2. Authentication");
                Console.WriteLine();

                var request2 =new AccessSecurityDataRequest(
                    ++requestCorrelationId);
                stream.WriteDrdaMessage(request2);
                var response2 = (AccessSecurityDataResponse)stream.ReadDrdaMessage();

                if (response2.RequestCorrelationId != request2.RequestCorrelationId)
                {
                    Console.Error.WriteLine("ERROR: Correlation IDs mismatch!");
                }

                Console.WriteLine();

                var request3 = new SecurityCheckRequest(
                    ++requestCorrelationId, 
                    "splice","admin");
                stream.WriteDrdaMessage(request3);
                var response3 = (SecurityCheckResponse)stream.ReadDrdaMessage();

                if (response3.RequestCorrelationId != request3.RequestCorrelationId)
                {
                    Console.Error.WriteLine("ERROR: Correlation IDs mismatch!");
                }

                Console.WriteLine();
                Console.WriteLine("3. Database");
                Console.WriteLine();

                var request4 = new AccessRelationalDatabaseRequest(
                        ++requestCorrelationId, 
                        client.Client.LocalEndPoint);
                stream.WriteDrdaMessage(request4);
                var response4 = (AccessRelationalDatabaseResponse)stream.ReadDrdaMessage();

                if (response4.RequestCorrelationId != request4.RequestCorrelationId)
                {
                    Console.Error.WriteLine("ERROR: Correlation IDs mismatch!");
                }
            }

            Console.WriteLine("Client closed.");
        }
    }
}
