
using System;
using System.Security.Cryptography;
using System.Text;

namespace Client
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

        
        public byte[] encryptMessage(byte[] message)
        {
            return sender.Encrypt(message, false);
        }

        public string decryptMessage(string message)
        {
            return Encoding.UTF8.GetString(reciever.Decrypt(Encoding.UTF8.GetBytes(message), false));
        }

        public string decryptMessage(byte[] message)
        {
            return Convert.ToBase64String(reciever.Decrypt(message, false));
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
