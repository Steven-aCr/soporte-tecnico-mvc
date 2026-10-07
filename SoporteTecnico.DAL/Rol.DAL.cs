using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.DAL
{
    public class RolDAL
    {
       
        public static async Task<Rol?> ObtenerPorIdAsync(Rol pRol)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Rol.FirstOrDefaultAsync(r => r.IdRol == pRol.IdRol);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<Rol>> ObtenerTodosAsync()
        {
            var result = new List<Rol>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Rol.OrderBy(r => r.IdRol).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}