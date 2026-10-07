using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.DAL
{
    public class CategoriaDAL
    {
        public static async Task<int> GuardarAsync(Categoria pCategoria)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pCategoria);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> ModificarAsync(Categoria pCategoria)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var categoria = await dbContexto.Categoria.FirstOrDefaultAsync(
                        c => c.IdCategoria == pCategoria.IdCategoria);

                    if (categoria == null)
                        throw new Exception($"No se encontró la categoría con ID {pCategoria.IdCategoria}.");

                    categoria.Nombre = pCategoria.Nombre;
                    categoria.Descripcion = pCategoria.Descripcion;
                    categoria.Activo = pCategoria.Activo;

                    dbContexto.Update(categoria);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> EliminarAsync(Categoria pCategoria)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var categoria = await dbContexto.Categoria.FirstOrDefaultAsync(
                        c => c.IdCategoria == pCategoria.IdCategoria);

                    if (categoria == null)
                        throw new Exception($"No se encontró la categoría con ID {pCategoria.IdCategoria}.");

                    categoria.Activo = false;

                    dbContexto.Update(categoria);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una categoría por su identificador.
        /// </summary>
        /// <param name="pCategoria">Objeto <see cref="Categoria"/> con el <c>IdCategoria</c> a buscar.</param>
        /// <returns>La categoría encontrada, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Categoria?> ObtenerPorIdAsync(Categoria pCategoria)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Categoria
                        .FirstOrDefaultAsync(c => c.IdCategoria == pCategoria.IdCategoria);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<bool> ExisteNombreAsync(string pNombre, int pExcluirId = 0)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Categoria.AnyAsync(
                        c => c.Nombre == pNombre && c.IdCategoria != pExcluirId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<Categoria>> ObtenerTodosAsync(Categoria pCategoria, bool pSoloActivas = false)
        {
            var result = new List<Categoria>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Categoria
                        .Where(c =>
                            (string.IsNullOrEmpty(pCategoria.Nombre) || c.Nombre.Contains(pCategoria.Nombre)) &&
                            (!pSoloActivas || c.Activo)
                        )
                        .OrderBy(c => c.Nombre)
                        .ToListAsync();
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