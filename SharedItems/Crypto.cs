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
        private byte[] key;
        private byte[] iV;


        /// <summary>
        /// Constructor wwith an method passed in to handle the incoming data
        /// </summary>
        /// <param name="networkStream">the stream from a TcpClient</param>
        /// <param name="handleMethod">method where the data is being handled</param>
        public Crypto(NetworkStream networkStream, Action<string> handleMethod)
        {
            this.buffer = new byte[1024];

            this.networkStream = networkStream;
            this.handleMethod = handleMethod;
            this.totalBuffer = new List<byte>();


            this.key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            this.iV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /// <summary>
        /// Microsoft method for using the crypto stream to decrypt the message
        /// </summary>
        /// <param name="cipherText">the message encrypted in a byte array</param>
        /// <returns>the message decrypted in string format</returns>
        private string DecryptStringFromBytes(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iV == null || iV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = key;
                rijAlg.IV = iV;


                rijAlg.Padding = PaddingMode.None;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);


                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Microsoft method for using the crypto stream to encrypt the message
        /// </summary>
        /// <param name="plainText">string of the message you want to send</param>
        /// <returns>encrypted message in the form of a byte array</returns>
        private byte[] EncryptStringToBytes(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iV == null || iV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = key;
                rijAlg.IV = iV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }


        #region stream dynamics

        /// <summary>
        /// Method to send a message to the client
        /// </summary>
        /// <param name="message">the message in string format</param>
        public void WriteTextMessage(string message)
        {
            //encrypt the message
            byte[] dataAsBytes = EncryptStringToBytes(message);

            //get the length of the message
            byte[] lengthMessage = BitConverter.GetBytes(message.Length);


            //send the message length
            networkStream.Write(lengthMessage, 0, lengthMessage.Length);
            networkStream.Flush();

            //send the message
            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
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
                byte[] temp = new byte[receivedBytes];
                Array.Copy(buffer, 0, temp, 0, receivedBytes);
                totalBuffer.AddRange(temp);

                // Get the length that the message should be
                byte[] lengthArray = totalBuffer.GetRange(0, 4).ToArray();
                int length = BitConverter.ToInt32(lengthArray, 0);
                // Make it a multiple of 8
                int totalLength = length + (8 - length % 8);


                // Check if the message has been received fully by comparing the total buffer to the length that the full message should be
                if (totalBuffer.Count >= totalLength + 4 && totalLength > 0)
                {
                    // Get the message from the total buffer minus the length (4 bytes)
                    byte[] messageInBytes = totalBuffer.GetRange(4, totalLength).ToArray();

                    // Decypher the message
                    string message = DecryptStringFromBytes(messageInBytes);

                    // cut out the encoded extra characters
                    message = message.Substring(0, length);

                    // Handle the message
                    handleMethod(message);

                    // Remove the message from the total buffer
                    totalBuffer.RemoveRange(0, length + 4);
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

        #endregion






    }
}
