using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Infrastructure.Helper
{
    public static class PasswordHashing
    {
        // Konfigurasi default — bisa diambil dari appsettings
        public const int DefaultIterations = 600_000; // PBKDF2-HMAC-SHA256
        public const int SaltSize = 16;               // 128-bit
        public const int HashSize = 32;               // 256-bit

        public static string Hash(string password, int? iterations = null)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required.", nameof(password));

            int iters = iterations ?? DefaultIterations;
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iters,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return $"{iters}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;

            // Format: {iterations}.{salt}.{hash}
            var parts = stored.Split('.', 3, StringSplitOptions.None);
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out int iterations) || iterations <= 0) return false;

            byte[] salt, hash;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                hash = Convert.FromBase64String(parts[2]);
            }
            catch
            {
                return false;
            }

            byte[] computed = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                hash.Length
            );

            // Perbandingan waktu-konstan
            return CryptographicOperations.FixedTimeEquals(hash, computed);
        }

        // Optional: untuk auto-upgrade iterasi saat login sukses
        public static bool NeedsRehash(string stored, int currentIterations = DefaultIterations)
        {
            var parts = stored.Split('.', 3, StringSplitOptions.None);
            if (parts.Length != 3) return true;
            return int.TryParse(parts[0], out int iter) ? iter < currentIterations : true;
        }

        public static string RehashIfNeeded(string password, string stored, int currentIterations = DefaultIterations)
        {
            return NeedsRehash(stored, currentIterations) ? Hash(password, currentIterations) : stored;
        }
    }
}
