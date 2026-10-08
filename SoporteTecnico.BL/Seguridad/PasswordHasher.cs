using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SoporteTecnico.BL.Seguridad
{
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

        public static bool Verificar(string pPassword, string pHashGuardado)
        {
            if (string.IsNullOrEmpty(pPassword) || string.IsNullOrEmpty(pHashGuardado))
                return false;

            var partes = pHashGuardado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out int iteraciones))
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(partes[1]);
                byte[] hashEsperado = Convert.FromBase64String(partes[2]);

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