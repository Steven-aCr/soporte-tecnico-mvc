using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.DAL
{
    public class UsuarioDAL
    {
        public static async Task<int> GuardarAsync(Usuario pUsuario)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pUsuario);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Usuario
                        .Include(u => u.Rol)
                        .FirstOrDefaultAsync(u => u.Correo == correo);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}