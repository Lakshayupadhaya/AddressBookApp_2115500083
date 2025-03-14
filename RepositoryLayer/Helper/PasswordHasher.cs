using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // 16 bytes for salt
        private const int HashSize = 32; // 32 bytes for hash
        private const int Iterations = 10000; // Number of PBKDF2 iterations
        public static string HashPassword(string password)
        {
            try
            {
                // Generate a new random salt
                byte[] salt = new byte[SaltSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // Generate hash using PBKDF2 without explicitly setting the hash algorithm
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);

                    // Combine salt and hash
                    byte[] hashBytes = new byte[SaltSize + HashSize];
                    Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                    Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                    // Convert to Base64 for storage
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred In Business Layer Helper.", ex);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // Convert stored Base64 string back to byte array
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Extract salt (first SaltSize bytes)
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Compute hash for the input password using the same salt and iterations
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] computedHash = pbkdf2.GetBytes(HashSize);

                    // Compare computed hash with stored hash
                    for (int i = 0; i < HashSize; i++)
                    {
                        if (hashBytes[SaltSize + i] != computedHash[i])
                        {
                            return false; // Passwords don't match
                        }
                    }
                }

                return true; // Passwords match
            }
            catch(Exception ex) 
            {
                throw new Exception("An error occurred in Business Layer Helper", ex);
            }
        }

    }
}
