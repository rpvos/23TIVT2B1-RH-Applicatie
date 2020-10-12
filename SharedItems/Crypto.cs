using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SharedItems
{
    public class Crypto
    {


        public CryptoStream encryptionCryptoStream { get; }
        public CryptoStream decryptionCryptoStream { get; }

        private byte[] buffer;
        private NetworkStream networkStream;
        private string totalBuffer;

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


            this.key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            this.iV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

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

        public void WriteTextMessage(string message)
        {
            message += "\r\n\r\n";
            byte[] dataAsBytes = EncryptStringToBytes(message);

            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            networkStream.Flush();
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = networkStream.EndRead(ar);

                string receivedText = DecryptStringFromBytes(buffer);

                Console.WriteLine(receivedText);

                totalBuffer += receivedText;
            }
            catch (IOException)
            {
                Console.WriteLine("Disconnected");
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));


                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                handleMethod(packet);
            }
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        #endregion






    }
}
