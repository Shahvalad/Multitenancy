using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace firstApi.Cryptography
{
    public static class CryptographyHelper
    {

        public static string Encrypt(string plainText, string key, string iv)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(nameof(plainText), "Wrong plain text!");
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        public static string Decrypt(string cipherText, string key, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(nameof(cipherText), "Wrong cipher text!");
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }


    public interface ICryptographyService
    {
        string Encrypt(string plaintText);
        string Decrypt(string cipherText);
    }

    public class CryptographyService : ICryptographyService
    {
        private const string Key = "eg7bwo3dPWE4fZQeVnMqW0k8bMBGtQjd";
        private const string Iv = "FG325OBXDW55Ab2n";
        public string Encrypt(string plaintText)
        {
            string cipherText = CryptographyHelper.Encrypt(plaintText, Key, Iv);
            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            var plainText = CryptographyHelper.Decrypt(cipherText, Key, Iv);
            return plainText;
        }
    }
}
