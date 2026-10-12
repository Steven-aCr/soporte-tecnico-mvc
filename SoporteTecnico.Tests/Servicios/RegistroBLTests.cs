using Microsoft.Extensions.DependencyInjection;
using SoporteTecnico.BL;

namespace SoporteTecnico.Tests.Servicios;

[TestClass]
public class RegistroBLTests
{
    [TestMethod]
    [DataRow(typeof(UsuarioBL))]
    [DataRow(typeof(CategoriaBL))]
    [DataRow(typeof(TicketBL))]
    [DataRow(typeof(ComentarioBL))]
    public void AddSoporteTecnicoBL_RegistraElServicio(Type pTipo)
    {
        var services = new ServiceCollection();
        services.AddSoporteTecnicoBL();

        using var provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        using var scope = provider.CreateScope();

        var servicio = scope.ServiceProvider.GetRequiredService(pTipo);

        Assert.IsInstanceOfType(servicio, pTipo);
    }
}