using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SoporteTecnico.BL.Seguridad
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2-SHA256. Formato almacenado: iteraciones.salt.hash (salt y hash en Base64).
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iteraciones = 100_000;

        public static string Hashear(string pPassword)
        {
            if (string.IsNullOrEmpty(pPassword))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(pPassword));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                pPassword, salt, Iteraciones, HashAlgorithmName.SHA256, HashSize);

            return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Compara la contraseña con un hash guardado. Devuelve false (nunca lanza) si el hash
        /// está mal formado: partes faltantes, iteraciones no positivas, Base64 inválido, salt o hash vacíos.
        /// </summary>
        public static bool Verificar(string pPassword, string pHashGuardado)
        {
            if (string.IsNullOrEmpty(pPassword) || string.IsNullOrEmpty(pHashGuardado))
                return false;

            var partes = pHashGuardado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out int iteraciones) || iteraciones <= 0)
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(partes[1]);
                byte[] hashEsperado = Convert.FromBase64String(partes[2]);

                // Un hash esperado vacío haría que FixedTimeEquals compare dos arreglos vacíos (true).
                if (salt.Length == 0 || hashEsperado.Length == 0)
                    return false;

                byte[] hashActual = Rfc2898DeriveBytes.Pbkdf2(
                    pPassword, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

                return CryptographicOperations.FixedTimeEquals(hashActual, hashEsperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}