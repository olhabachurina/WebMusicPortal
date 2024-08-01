using WebPortal.Dll.Interfaces;
using WebPortal.Dll.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebPortal.Dll.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IGenreRepository genreRepository, ISongRepository songRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Genres = genreRepository ?? throw new ArgumentNullException(nameof(genreRepository));
            Songs = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
        }

        public IUserRepository Users { get; private set; }
        public IGenreRepository Genres { get; private set; }
        public ISongRepository Songs { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}