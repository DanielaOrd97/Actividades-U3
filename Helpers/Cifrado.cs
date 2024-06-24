using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace U3Cliente.Helpers
{
    internal class Cifrado
    {
        private readonly int tamClave = 32;
        private readonly int tamVector = 16;
        private readonly string Vector = "8b5ef91df0fe09606482f383ace81f78b0acc7fc";

        private byte[] Key;
        private byte[] IV;

        public Cifrado()
        {
            IV = UTF8Encoding.UTF8.GetBytes(Vector);
            string clave = "ebb882770bf7e389793e5ca655834c3e6ff7f7f0" + DateTime.Now.ToString("ddMMyyhh");
            Key = UTF8Encoding.UTF8.GetBytes(clave);
        }

        public string CifradoTexto(String txtPlano)
        {
            Array.Resize(ref Key, tamClave);
            Array.Resize(ref IV, tamVector);

            AesManaged aes = new AesManaged();
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                aes.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);

            byte[] txtPlanoBytes = UTF8Encoding.UTF8.GetBytes(txtPlano);

            cryptoStream.Write(txtPlanoBytes, 0, txtPlanoBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherMessageBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipherMessageBytes);
        }
        public string DescifradoTexto(String txtCifrado)
        {
            Array.Resize(ref Key, tamClave);
            Array.Resize(ref IV, tamVector);

            byte[] cipherTextBytes = Convert.FromBase64String(txtCifrado);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            AesManaged aes = new AesManaged();
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                aes.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            int readbytes = 0;
            int decryptedByteCount = 0;
            while (readbytes < plainTextBytes.Length)
            {
                decryptedByteCount = cryptoStream.Read(plainTextBytes, readbytes, plainTextBytes.Length - readbytes);
                if (decryptedByteCount == 0) break;
                readbytes += decryptedByteCount;
            }

            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.UTF8.GetString(plainTextBytes, 0, readbytes);
        }
    }
}
