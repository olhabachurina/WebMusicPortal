using WebPortal.Dll.Repositories;
using WebPortal.Dll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPortal.Dll.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenreRepository Genres { get; }
        ISongRepository Songs { get; }
        Task<int> CompleteAsync();
        Task SaveAsync();
        void Dispose();
    }
}
