using WebPortal.Dll.Models;
using WebPortal.Bll.DTO;

namespace WebPortal.Bll.Interfaces
{
    public interface ISongService
    {
        Task<IEnumerable<SongDTO>> GetAllSongsAsync();
        Task<SongDTO> GetSongByIdAsync(int songId);
        Task AddSongAsync(SongDTO songDTO);
        Task UpdateSongAsync(SongDTO songDTO);
        Task DeleteSongAsync(int songId);
    }
}
