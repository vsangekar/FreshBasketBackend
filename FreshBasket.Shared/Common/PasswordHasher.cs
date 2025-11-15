using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Shared.Common
{
    public interface IPasswordHasher
    {
        (byte[] hash, byte[] salt) HashPassword(string password);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 32;
        private const int HashSize = 64;
        private const int Iterations = 100000;

        public (byte[] hash, byte[] salt) HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Password cannot be null or empty.");

                var salt = RandomNumberGenerator.GetBytes(SaltSize);

                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA512);

                var hash = pbkdf2.GetBytes(HashSize);

                return (hash, salt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error hashing password: {ex.Message}");
                throw;
            }
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Password cannot be null or empty.");
                if (storedHash == null || storedHash.Length != HashSize)
                    throw new ArgumentException("Invalid stored hash.");
                if (storedSalt == null || storedSalt.Length != SaltSize)
                    throw new ArgumentException("Invalid stored salt.");

                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    storedSalt,
                    Iterations,
                    HashAlgorithmName.SHA512);

                var hash = pbkdf2.GetBytes(HashSize);

                return CryptographicOperations.FixedTimeEquals(hash, storedHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                return false;
            }
        }
    }

}