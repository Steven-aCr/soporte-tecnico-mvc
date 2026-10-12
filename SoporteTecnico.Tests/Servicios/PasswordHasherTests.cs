using System.Security.Cryptography;
using SoporteTecnico.BL.Seguridad;

namespace SoporteTecnico.Tests.Servicios;

[TestClass]
public class PasswordHasherTests
{
    // ---------- Hashear ----------

    [TestMethod]
    public void Hashear_GeneraFormatoIteraciones_Salt_Hash()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        var partes = hash.Split('.');
        Assert.HasCount(3, partes);
        Assert.AreEqual("100000", partes[0]);
        Assert.HasCount(16, Convert.FromBase64String(partes[1]));
        Assert.HasCount(32, Convert.FromBase64String(partes[2]));
    }

    [TestMethod]
    public void Hashear_MismaPassword_GeneraHashesDistintosPorSalt()
    {
        var hash1 = PasswordHasher.Hashear("Clave1234");
        var hash2 = PasswordHasher.Hashear("Clave1234");

        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void Hashear_NoContieneLaPasswordEnTextoPlano()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        Assert.DoesNotContain("Clave1234", hash);
    }

    [TestMethod]
    public void Hashear_CabeEnLaColumnaPasswordHash()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        Assert.IsLessThanOrEqualTo(SoporteTecnico.EN.LongitudesCampo.UsuarioPasswordHash, hash.Length);
    }

    [TestMethod]
    public void Hashear_PasswordNula_LanzaArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => PasswordHasher.Hashear(null!));
    }

    [TestMethod]
    public void Hashear_PasswordVacia_LanzaArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => PasswordHasher.Hashear(string.Empty));
    }

    // ---------- Verificar ----------

    [TestMethod]
    public void Verificar_PasswordCorrecta_RetornaTrue()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        Assert.IsTrue(PasswordHasher.Verificar("Clave1234", hash));
    }

    [TestMethod]
    public void Verificar_PasswordIncorrecta_RetornaFalse()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        Assert.IsFalse(PasswordHasher.Verificar("OtraClave99", hash));
    }

    [TestMethod]
    public void Verificar_DistingueMayusculasDeMinusculas()
    {
        var hash = PasswordHasher.Hashear("Clave1234");

        Assert.IsFalse(PasswordHasher.Verificar("clave1234", hash));
    }

    [TestMethod]
    public void Verificar_HashDeOtraPassword_RetornaFalse()
    {
        var hashDeOtra = PasswordHasher.Hashear("OtraClave99");

        Assert.IsFalse(PasswordHasher.Verificar("Clave1234", hashDeOtra));
    }

    [TestMethod]
    public void Verificar_UsaLasIteracionesGuardadasEnElHash()
    {
        // Hash generado "a mano" con 1000 iteraciones: debe verificarse con las iteraciones del propio hash.
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] derivado = Rfc2898DeriveBytes.Pbkdf2("Clave1234", salt, 1000, HashAlgorithmName.SHA256, 32);
        var hash = $"1000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(derivado)}";

        Assert.IsTrue(PasswordHasher.Verificar("Clave1234", hash));
        Assert.IsFalse(PasswordHasher.Verificar("Incorrecta1", hash));
    }

    [TestMethod]
    [DataRow(null, "100000.AAAA.AAAA")]
    [DataRow("", "100000.AAAA.AAAA")]
    public void Verificar_PasswordNulaOVacia_RetornaFalse(string? pPassword, string pHash)
    {
        Assert.IsFalse(PasswordHasher.Verificar(pPassword!, pHash));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void Verificar_HashNuloOVacio_RetornaFalse(string? pHash)
    {
        Assert.IsFalse(PasswordHasher.Verificar("Clave1234", pHash!));
    }

    [TestMethod]
    [DataRow("sin-puntos")]
    [DataRow("100000.soloDosPartes")]
    [DataRow("100000.a.b.c")]
    [DataRow("abc.AAAA.AAAA")]           // iteraciones no numéricas
    [DataRow("100000.@@@@.@@@@")]        // base64 inválido
    [DataRow("HASH_PENDIENTE")]          // marcador de los datos semilla
    public void Verificar_HashMalFormado_RetornaFalse(string pHash)
    {
        Assert.IsFalse(PasswordHasher.Verificar("Clave1234", pHash));
    }

    // ---------- Endurecimiento: hashes manipulados o corruptos ----------
    // Regresión de dos defectos corregidos en Verificar: iteraciones < 1 hacían lanzar
    // ArgumentOutOfRangeException, y un hash esperado vacío hacía que FixedTimeEquals devolviera true.

    [TestMethod]
    [DataRow("0.AAAA.AAAA")]
    [DataRow("-5.AAAA.AAAA")]
    public void Verificar_IteracionesNoPositivas_RetornaFalseSinLanzar(string pHash)
    {
        Assert.IsFalse(PasswordHasher.Verificar("Clave1234", pHash));
    }

    [TestMethod]
    public void Verificar_HashEsperadoVacio_RetornaFalse()
    {
        // "100000.<salt>." => tercera parte vacía
        Assert.IsFalse(PasswordHasher.Verificar("Clave1234", "100000.AAAAAAAAAAAAAAAAAAAAAA==."));
    }
}