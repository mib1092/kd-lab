using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace IMCMS.Common.Hashing
{
    public class AESEncrypt
    {
        private static byte[] _salt = Encoding.ASCII.GetBytes("JEej2DoqcsWVnlBo");
        private static string _secret = "KqpagAFeWjwmRmC83WHl";

        public static string Encrypt(string text)
        {
            return encryptStringToBytes_AES(text);
        }

        public static string DeCrypt(string cipher)
        {
            return decryptStringFromBytes_AES(cipher);
        }

        private static string encryptStringToBytes_AES(string text)
        {
            if (text == null || text.Length <= 0)
                throw new ArgumentNullException("text");

            // Declare the Aes object
            // used to encrypt the data.
            Aes aesAlg = null;

            // Declare the bytes used to hold the
            // encrypted data.
            string encrypted = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_secret, _salt);

                // Create an Aes object
                // with the specified key and IV.
                aesAlg = Aes.Create();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                var iv = aesAlg.IV;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    //prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }
                    encrypted = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the Aes object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return encrypted;

        }

        private static string decryptStringFromBytes_AES(string cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = string.Empty;
            Aes aesAlg = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_secret, _salt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = Aes.Create();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                if (aesAlg != null) aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;

        }
    }
}
