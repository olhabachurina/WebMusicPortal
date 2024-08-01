using WebPortal.Dll.Models;
using WebPortal.Bll.DTO;

namespace WebPortal.Bll.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<UserDTO> GetUserByNameAsync(string userName);
        Task AddUserAsync(UserDTO userDTO);
        Task UpdateUserAsync(UserDTO userDTO);
        Task DeleteUserAsync(int userId);
    }
}
