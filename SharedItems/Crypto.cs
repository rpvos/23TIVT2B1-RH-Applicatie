using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SharedItems
{
    public class Crypto
    {
        private Rijndael rijndael;

        public CryptoStream sendingStream { get; }
        public CryptoStream receivingStream { get; }

        public Crypto(NetworkStream networkStream)
        {
            this.rijndael = Rijndael.Create();

            rijndael.Key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            rijndael.IV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            this.sendingStream = new CryptoStream(networkStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            this.receivingStream= new CryptoStream(networkStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read);
        }






    }
}
