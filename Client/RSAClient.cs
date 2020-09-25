using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Client
{
    class RSAClient
    {
        private RSAParameters parameters;
        private RSACryptoServiceProvider rsa;

        public RSAClient(int size)
        {
            this.rsa = new RSACryptoServiceProvider(size);
        }

        public string encryptMessage(string message)
        {
            return rsa.
        }

        public void setKey(byte[] key, byte[] exponent)
        {
            this.parameters = new RSAParameters();
            parameters.Modulus = key;
            parameters.Exponent = exponent;
        }

    }
}
