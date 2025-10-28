using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    internal static class CryptClass
    {
        private static byte[] iv;
        private static byte[] key;

        public static void InitCryptClass(string keyString, string ivString)
        {
            if (keyString.Length != 48 /*0x30*/)
                throw new Exception("Key must be of 48 bytes");
            CryptClass.key = ivString.Length == 16 /*0x10*/ ? new byte[keyString.Length / 2] : throw new Exception("Iv must be of 16 bytes");
            for (int startIndex = 0; startIndex < keyString.Length; startIndex += 2)
                CryptClass.key[startIndex / 2] = Convert.ToByte(keyString.Substring(startIndex, 2), 16 /*0x10*/);
            CryptClass.iv = new byte[ivString.Length / 2];
            for (int startIndex = 0; startIndex < ivString.Length; startIndex += 2)
                CryptClass.iv[startIndex / 2] = Convert.ToByte(ivString.Substring(startIndex, 2), 16 /*0x10*/);
        }

        public static string encrypt(string clearData)
        {
            string str;
            try
            {
                str = CryptClass.internalEncrypt(clearData);
            }
            catch (Exception ex)
            {
                str = (string)null;
            }
            return str;
        }

        public static string decrypt(string encryptedData)
        {
            string str;
            try
            {
                str = CryptClass.internalDecrypt(encryptedData);
            }
            catch (Exception ex)
            {
                str = (string)null;
            }
            return str;
        }

        private static string internalEncrypt(string clearData)
        {
            ICryptoTransform encryptor = new TripleDESCryptoServiceProvider().CreateEncryptor(CryptClass.key, CryptClass.iv);
            byte[] bytes = MyTcpClient.getBytes(clearData);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return MyTcpClient.getString(memoryStream.ToArray());
        }

        private static string internalDecrypt(string encryptedData)
        {
            ICryptoTransform decryptor = new TripleDESCryptoServiceProvider().CreateDecryptor(CryptClass.key, CryptClass.iv);
            return new StreamReader((Stream)new CryptoStream((Stream)new MemoryStream(MyTcpClient.getBytes(encryptedData)), decryptor, CryptoStreamMode.Read), Encoding.Default).ReadToEnd();
        }
    }
}
