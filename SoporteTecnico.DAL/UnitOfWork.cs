using SoporteTecnico.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SoporteTecnico.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContexto _contexto;
        private bool _disposed = false;

        public UnitOfWork(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<int> GuardarCambiosAsync()
        {
            return await _contexto.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _contexto.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}