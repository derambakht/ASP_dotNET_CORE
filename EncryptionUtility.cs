using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Common
{
    public static class EncryptionUtility
    {
        public static string HashSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                return hash;
            }
        }

        static string key = "56HBT2UQBUaC7FgCVABJco/kpL+uRQA1b63kU4mrKJI=";
        static readonly char[] padding = { '=' };

        public static string Encrypt(string data)
        {
            byte[] toEncryptArry = Encoding.UTF8.GetBytes(data);
            byte[] keyArry = Convert.FromBase64String(key);

            var aes = new AesCryptoServiceProvider
            {
                Key = keyArry,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.ISO10126
            };
            ICryptoTransform cTransform = aes.CreateEncryptor();
            byte[] encrypted = cTransform.TransformFinalBlock(toEncryptArry, 0, toEncryptArry.Length);
            string encryptCode = Convert.ToBase64String(encrypted);

            encryptCode = encryptCode.TrimEnd(padding).Replace('+', '-').Replace('/', '_');
            return encryptCode;
        }

        public static string Decrypt(string data)
        {
            string incoming = data.Replace('_', '/').Replace('-', '+');
            switch (data.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }


            byte[] toDecryptArry = Convert.FromBase64String(incoming);
            byte[] keyArry = Convert.FromBase64String(key);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider
            {
                Key = keyArry,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.ISO10126
            };
            ICryptoTransform cTransform = aes.CreateDecryptor();
            byte[] decrypted = cTransform.TransformFinalBlock(toDecryptArry, 0, toDecryptArry.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
        
        static string publicKey = "";
        static string privateKey = "";

        public static void GenerateRSAKey()
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                publicKey = rsa.ToXmlString(false); // Do something with the key...
                privateKey = rsa.ToXmlString(true); // Do something with the key...
            }
        }

        public static string Encryption(string strText)
        {
            var testData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                // client encrypting data with public key issued by server                    
                rsa.FromXmlString(publicKey);

                var encryptedData = rsa.Encrypt(testData, true);
                var base64Encrypted = Convert.ToBase64String(encryptedData);
                return base64Encrypted;
            }
        }

        public static string Decryption(string strText)
        {
            var testData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                var base64Encrypted = strText;

                // server decrypting data with private key                    
                rsa.FromXmlString(privateKey);

                var resultBytes = Convert.FromBase64String(base64Encrypted);
                var decryptedBytes = rsa.Decrypt(resultBytes, true);
                var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedData.ToString();
            }
        }
    }
}
