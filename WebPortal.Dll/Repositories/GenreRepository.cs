﻿using WebPortal.Dll.Interfaces;
using WebPortal.Dll.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPortal.Dll.Repositories
{

    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context) : base(context) { }
        public async Task<Genre> GetByNameAsync(string name)
        {
            return await _context.Set<Genre>().FirstOrDefaultAsync(g => g.Name == name);
        }
    }
}
