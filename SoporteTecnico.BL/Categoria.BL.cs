using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.DAL;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.BL
{
    public class CategoriaBL
    {
        private const int NombreMaxLength = LongitudesCampo.CategoriaNombre;
        private const int DescripcionMaxLength = LongitudesCampo.CategoriaDescripcion;

        public async Task<int> GuardarAsync(Categoria pCategoria)
        {
            Validar(pCategoria);
            Normalizar(pCategoria);

            if (await CategoriaDAL.ExisteNombreAsync(pCategoria.Nombre))
                throw new ReglaNegocioException($"Ya existe una categoría con el nombre '{pCategoria.Nombre}'.");

            pCategoria.Activo = true;
            return await CategoriaDAL.GuardarAsync(pCategoria);
        }

        public async Task<int> ModificarAsync(Categoria pCategoria)
        {
            ValidarId(pCategoria);
            Validar(pCategoria);
            Normalizar(pCategoria);

            if (await CategoriaDAL.ExisteNombreAsync(pCategoria.Nombre, pCategoria.IdCategoria))
                throw new ReglaNegocioException($"Ya existe otra categoría con el nombre '{pCategoria.Nombre}'.");

            return await CategoriaDAL.ModificarAsync(pCategoria);
        }

        public async Task<int> EliminarAsync(Categoria pCategoria)
        {
            ValidarId(pCategoria);
            return await CategoriaDAL.EliminarAsync(pCategoria);
        }

        public async Task<Categoria?> ObtenerPorIdAsync(Categoria pCategoria)
        {
            ValidarId(pCategoria);
            return await CategoriaDAL.ObtenerPorIdAsync(pCategoria);
        }

        public async Task<List<Categoria>> ObtenerTodosAsync(Categoria? pCategoria = null, bool pSoloActivas = false)
        {
            pCategoria ??= new Categoria();
            pCategoria.Nombre = pCategoria.Nombre?.Trim() ?? string.Empty;
            return await CategoriaDAL.ObtenerTodosAsync(pCategoria, pSoloActivas);
        }

        private static void ValidarId(Categoria pCategoria)
        {
            if (pCategoria == null)
                throw new ReglaNegocioException("La categoría no puede ser nula.");

            if (pCategoria.IdCategoria <= 0)
                throw new ReglaNegocioException("El identificador de la categoría no es válido.");
        }

        private static void Validar(Categoria pCategoria)
        {
            if (pCategoria == null)
                throw new ReglaNegocioException("La categoría no puede ser nula.");

            if (string.IsNullOrWhiteSpace(pCategoria.Nombre))
                throw new ReglaNegocioException("El nombre de la categoría es obligatorio.");

            if (pCategoria.Nombre.Trim().Length > NombreMaxLength)
                throw new ReglaNegocioException($"El nombre no puede superar los {NombreMaxLength} caracteres.");

            if (pCategoria.Descripcion != null && pCategoria.Descripcion.Trim().Length > DescripcionMaxLength)
                throw new ReglaNegocioException($"La descripción no puede superar los {DescripcionMaxLength} caracteres.");
        }

        private static void Normalizar(Categoria pCategoria)
        {
            pCategoria.Nombre = pCategoria.Nombre.Trim();
            pCategoria.Descripcion = string.IsNullOrWhiteSpace(pCategoria.Descripcion)
                ? null
                : pCategoria.Descripcion.Trim();
        }
    }
}