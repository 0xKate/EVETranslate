using System;
using System.Security.Cryptography;
using System.Text;

namespace EVETranslate.Services
{
    public static class SecretsProtector
    {
        // Optional "entropy" (like an extra app-specific pepper).
        // Must be identical for protect + unprotect.
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("EVETranslate:v1");

        public static string ProtectToBase64(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return string.Empty;

            // DPAPI works on bytes
            var bytes = Encoding.UTF8.GetBytes(plaintext);

            var protectedBytes = ProtectedData.Protect(
                bytes,
                Entropy,
                DataProtectionScope.CurrentUser); // per-user :contentReference[oaicite:3]{index=3}

            return Convert.ToBase64String(protectedBytes);
        }

        public static string UnprotectFromBase64(string protectedBase64)
        {
            if (string.IsNullOrWhiteSpace(protectedBase64))
                return string.Empty;

            try
            {
                var protectedBytes = Convert.FromBase64String(protectedBase64);

                var bytes = ProtectedData.Unprotect(
                    protectedBytes,
                    Entropy,
                    DataProtectionScope.CurrentUser); // must match scope :contentReference[oaicite:4]{index=4}

                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // Corrupt/invalid/copy-from-other-user => treat as missing
                return string.Empty;
            }
        }
    }
}
