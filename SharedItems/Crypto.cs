using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SharedItems
{
    public class Crypto
    {
        /// <summary>
        /// object that keeps the key
        /// </summary>
        private Rijndael rijndael;

        public CryptoStream encryptionCryptoStream { get; }
        public CryptoStream decryptionCryptoStream { get; }

        private byte[] buffer;
        private NetworkStream networkStream;
        private string totalBuffer;

        private Action<string> handleMethod;

        private MemoryStream encryptionMemoryStream;
        private MemoryStream decryptionMemoryStream;

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

            this.encryptionMemoryStream = new MemoryStream();
            this.decryptionMemoryStream = new MemoryStream();
            this.rijndael = Rijndael.Create();

            rijndael.Key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            rijndael.IV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            this.encryptionCryptoStream = new CryptoStream(encryptionMemoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            this.decryptionCryptoStream= new CryptoStream(decryptionMemoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read);

            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        #region stream dynamics

        public void WriteTextMessage(string message)
        {
            byte[] dataAsBytes = Encoding.UTF8.GetBytes(message + "\r\n\r\n");

            encryptionCryptoStream.Write(dataAsBytes,0,dataAsBytes.Length);
            encryptionCryptoStream.Clear();

            var array = encryptionMemoryStream.ToArray();
            
            networkStream.Write(array, 0, array.Length);
            networkStream.Flush();
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                // todo encrypted data has been gotten but we cant decode it
                int receivedBytes = networkStream.EndRead(ar);
                decryptionCryptoStream.Read(buffer, 0, receivedBytes);

                Console.WriteLine(receivedBytes);//todo remove

                string receivedText = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                Console.WriteLine(receivedText);//todo remove

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
