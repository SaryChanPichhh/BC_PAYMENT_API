using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BC.PAYMENT.API.Helper;
using Microsoft.Extensions.Configuration;

public static class EncryptionHelper
{

    private static readonly byte[] _key = Convert.FromBase64String("w+Vv73xKqu0q1BFlX9mHCWOr3w1jM7axPYoHhMwv92A=");
    private static readonly byte[] _iv = Convert.FromBase64String("Wm3g9eTb4D1w5kh79T8eJg==");


    public static string EncryptAES(string plainText)
    {
       
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (var msEncrypt = new MemoryStream())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string DecryptAES(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (var msDecrypt = new MemoryStream(cipherBytes))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
