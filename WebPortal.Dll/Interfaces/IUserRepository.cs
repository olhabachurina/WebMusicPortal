using WebPortal.Dll.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPortal.Dll.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByNameAsync(string userName);
    }
}
