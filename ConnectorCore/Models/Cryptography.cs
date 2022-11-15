using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ConnectorCore.Models;
using System.ComponentModel;

namespace ConnectorCore.Cryptography
{
    public static class PasswordCryptography
    {
        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(128 / 8);
        }
        public static byte[] GenerateUserSalt(string login, string password)
        {
            return Encoding.UTF8.GetBytes(GetHashString(login + password, Encoding.UTF8.GetBytes("4815162342")),0,8);
        }
        public static string GetHashString(string target, byte[] salt)
        {
            if(String.IsNullOrEmpty(target))
                throw new Exception("Target string is null or empty");
            if (salt is null || salt.Length < 1)
                throw new Exception("Salt not correct");
            
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: target!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
        }
        public static string GetUserPasswordHash(string login, string password)
        {
            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
                throw new Exception("Target credentials values is null or empty");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: GenerateUserSalt(login, password),
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
        }
    }
}
