using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

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

        public static async Task<int> ModificarAsync(Usuario pUsuario)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var usuario = await dbContexto.Usuario.FirstOrDefaultAsync(
                        u => u.IdUsuario == pUsuario.IdUsuario);

                    if (usuario == null)
                        throw new Exception($"No se encontró el usuario con ID {pUsuario.IdUsuario}.");

                    usuario.Nombre = pUsuario.Nombre;
                    usuario.Correo = pUsuario.Correo;
                    usuario.IdRol = pUsuario.IdRol;
                    usuario.Activo = pUsuario.Activo;

                    if (!string.IsNullOrEmpty(pUsuario.PasswordHash))
                        usuario.PasswordHash = pUsuario.PasswordHash;

                    dbContexto.Update(usuario);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Usuario
                        .Include(u => u.Rol)
                        .FirstOrDefaultAsync(u => u.IdUsuario == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        public static async Task<List<Usuario>> ObtenerTodosAsync()
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Usuario
                        .Include(u => u.Rol)
                        .OrderBy(u => u.Nombre)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<Usuario>> ObtenerTecnicosActivosAsync()
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    int idRolTecnico = (int)RolUsuario.Tecnico;

                    return await dbContexto.Usuario
                        .Include(u => u.Rol)
                        .Where(u => u.IdRol == idRolTecnico && u.Activo)
                        .OrderBy(u => u.Nombre)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}