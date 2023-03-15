#region Imports

using System.Security.Cryptography;
using System.Text;

#endregion

namespace Domain.Common.Extensions
{
    public static class PasswordHasher
    {
        #region Salts/Keys

        private static readonly string key = "s0l@ra!b)AzP"; //Encryption key

        #endregion

        #region Encryption/Decryption

        public static string Encrypt(string clearText)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            var pdb = new PasswordDeriveBytes(key,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            var encryptedData = Encrypt(clearBytes, pdb.GetBytes(16), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            var pdb = new PasswordDeriveBytes(key,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            var decryptedData = Decrypt(cipherBytes, pdb.GetBytes(16), pdb.GetBytes(16));
            return Encoding.Unicode.GetString(decryptedData);
        }

        #region Private Methods

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream that is going to accept the decrypted bytes 
            var ms = new MemoryStream();
            var alg = Aes.Create();
            alg.Key = Key;
            alg.IV = IV;

            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            // Write the data and make it do the decryption 
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();

            var decryptedData = ms.ToArray();
            return decryptedData;
        }
        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream that is going to accept the encrypted bytes 
            var ms = new MemoryStream();
            var alg = Aes.Create();
            alg.Key = Key;
            alg.IV = IV;

            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            // Write the data and make it do the encryption 
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();

            var encryptedData = ms.ToArray();
            return encryptedData;
        }

        #endregion

        #endregion

        #region Hashing

        public static bool VerifyHash(string plainText, string hashValue)
        {
            try
            {
                // Convert base64-encoded hash value into a byte array.
                var hashWithSaltBytes = Convert.FromBase64String(hashValue);

                // We must know size of hash (without salt).
                int hashSizeInBits, hashSizeInBytes;

                // Size of hash is based on the specified algorithm.
                hashSizeInBits = 256;
                // Convert size of hash from bits to bytes.
                hashSizeInBytes = hashSizeInBits / 8;

                // Make sure that the specified hash value is long enough.
                if (hashWithSaltBytes.Length < hashSizeInBytes)
                    return false;

                // Allocate array to hold original salt bytes retrieved from hash.
                var saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

                // Copy salt from the end of the hash to the new array.
                for (var i = 0; i < saltBytes.Length; i++)
                    saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

                // Compute a new hash string.
                var expectedHashString = ComputeHash(plainText, saltBytes);

                // If the computed hash matches the specified hash,
                // the plain text value must be correct.
                return hashValue == expectedHashString;
            }
            catch
            {
            }

            return false;
        }
        public static string GeneratePasswordHash(string plainPasswordtext)
        {
            var passwordHash = string.Empty;
            try
            {
                var saltToAdd = GenerateSalt();
                passwordHash = ComputeHash(plainPasswordtext, saltToAdd);
            }
            catch (Exception)
            {
            }

            return passwordHash;
        }

        #region Private methods

        private static byte[] GenerateSalt()
        {
            var minSaltSize = 4;
            var maxSaltSize = 8;
            // Generate a random number for the size of the salt.
            var random = new Random();
            var saltSize = random.Next(minSaltSize, maxSaltSize);

            // Allocate a byte array, which will hold the salt.
            var saltBytes = new byte[saltSize];

            // Initialize a random number generator.
            var rng = new RNGCryptoServiceProvider();
            // Fill the salt with cryptographically strong byte values.
            rng.GetNonZeroBytes(saltBytes);
            return saltBytes;
        }
        private static string ComputeHash(string plainText, byte[] saltBytes)
        {
            // Convert plain text into a byte array.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            var plainTextWithSaltBytes =
                new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (var i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (var i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Initialize appropriate hashing algorithm class.
            HashAlgorithm hash = new SHA256Managed();

            // Compute hash value of our plain text with appended salt.
            var hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            var hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (var i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (var i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            var hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        #endregion

        #endregion
    }
}
