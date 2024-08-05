﻿using WebPortal.Dll.Repositories;
using WebPortal.Dll.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPortal.Dll.Repositories
{
    public class SongRepository : Repository<Song>, ISongRepository
    {
        public SongRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Song> GetByNameAsync(string name)
        {
            return await _context.Set<Song>().FirstOrDefaultAsync(s => s.Title == name);
        }
    }
}
