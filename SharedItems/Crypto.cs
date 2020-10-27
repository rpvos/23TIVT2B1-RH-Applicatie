using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SharedItems
{
    public class Crypto
    {
        private byte[] buffer;
        private NetworkStream networkStream;
        private List<byte> totalBuffer;

        private Action<string> handleMethod;

        private EncyptionService encyptionService;

        /// <summary>
        /// Constructor wwith an method passed in to handle the incoming data
        /// </summary>
        /// <param name="networkStream">the stream from a TcpClient</param>
        /// <param name="handleMethod">method where the data is being handled</param>
        public Crypto(NetworkStream networkStream, Action<string> handleMethod)
        {
            this.buffer = new byte[1024];
            this.encyptionService = new EncyptionService();
            this.networkStream = networkStream;
            this.handleMethod = handleMethod;
            this.totalBuffer = new List<byte>();

            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        


        #region stream dynamics

        /// <summary>
        /// Method to send a message to the client
        /// </summary>
        /// <param name="message">the message in string format</param>
        public void WriteTextMessage(string message)
        {
            //encrypt the message
            byte[] dataAsBytes = encyptionService.EncryptStringToBytes(message);

            //get the length of the message
            byte[] lengthMessage = BitConverter.GetBytes(message.Length);


            byte[] fullMessage = new byte[dataAsBytes.Length + lengthMessage.Length];
            Array.Copy(lengthMessage, fullMessage, lengthMessage.Length);
            Array.Copy(dataAsBytes,0, fullMessage,lengthMessage.Length, dataAsBytes.Length);

       
            //send the message
            networkStream.Write(fullMessage, 0, fullMessage.Length);
            networkStream.Flush();
        }

        /// <summary>
        /// Method to read the messages sent from the server
        /// </summary>
        /// <param name="ar">the async result</param>
        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = networkStream.EndRead(ar);

                // Add the content of the buffer to the total buffer
                byte[] newMessage = new byte[receivedBytes];

                // Get the message from the buffer to the newMessage array
                Array.Copy(buffer, 0, newMessage, 0, receivedBytes);

                // Add the newMessage to the total buffer
                totalBuffer.AddRange(newMessage);

                // Calculate length and totalLength
                int length = 0;
                int totalLength = 0;
                if (totalBuffer.Count > 4)
                    totalLength = calculateTotalLength(out length);

                // Check if the message has been received fully by comparing the total buffer to the length that the full message should be
                while (totalBuffer.Count >= totalLength + 4 && totalLength > 0)
                {

                    // Get the message from the total buffer minus the length (4 bytes)
                    byte[] messageInBytes = totalBuffer.GetRange(4, totalLength).ToArray();

                    // Decypher the message
                    string message = encyptionService.DecryptStringFromBytes(messageInBytes);

                    // cut out the encoded extra characters                  
                        if (length != totalLength)
                            message = message.Substring(0, length);
                   

                    // Handle the message
                    handleMethod(message);

                    // Remove the message from the total buffer
                    totalBuffer.RemoveRange(0, totalLength + 4);

                    // If the buffer contains more then 4 bytes there can be a next message

                    // Calculate length and totalLength with new message
                    if (totalBuffer.Count > 4)
                        totalLength = calculateTotalLength(out length);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Disconnected");
                return;
            }

            // Listen for more messages 
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /// <summary>
        /// Method to calculate the length of the message
        /// </summary>
        /// <param name="length"> The actual length of the message unencoded</param>
        /// <returns></returns>
        private int calculateTotalLength(out int length)
        {
            // Get the length that the message should be
     
            byte[] lengthArray = totalBuffer.GetRange(0, 4).ToArray();
            length = BitConverter.ToInt32(lengthArray, 0);
            // Make it a multiple of 16
            return length + (16 - length % 16);
        }

        #endregion






    }
}
