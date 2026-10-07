using System;
using System.Threading.Tasks;

namespace SoporteTecnico.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> GuardarCambiosAsync();
    }
}