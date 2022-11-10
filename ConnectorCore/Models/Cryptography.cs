using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ConnectorCore.Cryptography
{
    public static class PasswordCryptography
    {
        public const string Salt = "4815162342";

        public static string GetHashString(string target, HashTypes hashType)
        {
            if(String.IsNullOrEmpty(target))
                return target;

            byte[] bytesWithSalt = GetStringBytes(target)
                .Concat(GetStringBytes(Salt))
                .ToArray();
            HashAlgorithm hash;
            switch (hashType)
            {
                case HashTypes.MD5:
                    hash = new MD5CryptoServiceProvider();
                    break;
                case HashTypes.SHA256:
                    hash = new SHA256Managed();
                    break;
                case HashTypes.SHA512:
                    hash = new SHA512Managed();
                    break;
                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }
            byte[] hashBytes = hash.ComputeHash(bytesWithSalt);
            return Convert.ToBase64String(hashBytes);
        }
        private static byte[] GetStringBytes(string target)
        {
            return Encoding.UTF8.GetBytes(target);
        }
        public enum HashTypes
        {
            MD5,
            SHA256,
            SHA512
        }
    }
}
