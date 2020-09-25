using Client;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace GameClient
{
    class Client
    {
        static void Main(string[] args)
        {
            Client client = new Client();

        }

        private String username;
        private TcpClient server;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private RSAClient rsaClient;


        public Client()
        {
            this.rsaClient = new RSAClient(2048);

            Console.Write("Username: ");
            this.username = Console.ReadLine();

            this.server = new TcpClient("127.0.0.1", 1330);

            this.streamWriter = new StreamWriter(server.GetStream(), Encoding.ASCII, -1, true);
            this.streamReader = new StreamReader(server.GetStream(), Encoding.ASCII);

            Console.WriteLine("Type 'exit' to end connection");

            WriteTextMessage(server, username);

            Thread receiveMessages = new Thread(ReadTextMessage);
            receiveMessages.Start();

            while (true)
            {
                string message = Console.ReadLine();

                WriteTextMessage(server, message);

                if (message.Equals("exit"))
                {
                    server.Close();
                    break;
                }

            }
        }

        public void WriteTextMessage(TcpClient client, string message)
        {
            try
            {
                this.streamWriter.WriteLine(message);
                this.streamWriter.Flush();
            }
            catch { }

        }

        public void ReadTextMessage()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(this.streamReader.ReadLine());
                }
                catch
                {
                    break;
                }
            }
        }

        public string getUpdateMessageString(int session, int heartrate, double accDistance, double speed, double instPower, double accPower)
        {
            int checksum = 0;
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    HeartRate = heartrate,
                    AccumulatedDistance = accDistance,
                    Speed = speed,
                    InstantaniousPower = instPower
                },
                Checksum = checksum
            };

            return JsonSerializer.Serialize(json);
        }

        public string getMessageString(int session, string message)
        {
            int checksum = 0;
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    Message = message
                },
                Checksum = checksum
            };

            return JsonSerializer.Serialize(json);
        }

        public string getRequestMessageString(byte[] publicKey, byte[] exponent)
        {
            int checksum = 0;
            dynamic json = new
            {
                Type = "request",
                Data = new
                {
                    PublicKey = publicKey,
                    Exponent = exponent
                },
                Checksum = checksum
            };

            return JsonSerializer.Serialize(json);
        }
    }
}
