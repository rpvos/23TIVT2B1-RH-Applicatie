using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    class RSAClient
    {
        private RSACryptoServiceProvider sender;
        private RSACryptoServiceProvider reciever;

        public RSAClient()
        {
            this.sender = new RSACryptoServiceProvider(384);
            this.reciever = new RSACryptoServiceProvider(384);
        }

        public byte[] encryptMessage(int message)
        {
            return sender.Encrypt(Encoding.UTF8.GetBytes(message.ToString()), false);
        }

        public string decryptMessage(string message)
        {
            return Encoding.ASCII.GetString(reciever.Decrypt(Encoding.UTF8.GetBytes(message), false));
        }

        public void setKey(byte[] modulus, byte[] exponent)
        {
            var parameters = new RSAParameters();
            parameters.Modulus = modulus;
            parameters.Exponent = exponent;
            sender.ImportParameters(parameters);
        }

        internal byte[] getExponent()
        {
            var parameters = reciever.ExportParameters(false);
            return parameters.Exponent;
        }

        internal byte[] getModulus()
        {
            var parameters = reciever.ExportParameters(false);
            return parameters.Modulus;
        }

    }
}
