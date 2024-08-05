using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;


using System.Xml.Linq;
using System.Diagnostics;
using AutoMapper;
using WebPortal.Bll.DTO;
using WebPortal.Bll.Interfaces;



namespace WebMusicPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IGenreService _genreService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<SongsController> _logger;
        private readonly IMapper _mapper;

        public SongsController(
            ISongService songService,
            IGenreService genreService,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<SongsController> logger,
            IMapper mapper)
        {
            _songService = songService;
            _genreService = genreService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _songService.GetAllSongsAsync();
            var songDTOs = songs.Select(song =>
            {
                var songDTO = _mapper.Map<SongDTO>(song);
                var user = _userService.GetUserByIdAsync(song.UserId).Result;
                if (user != null)
                {
                    songDTO.UserName = user.UserName;
                }
                return songDTO;
            }).ToList();
            return Ok(songDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                _logger.LogWarning("Song with ID {Id} not found.", id);
                return NotFound("Song not found");
            }

            var songDTO = _mapper.Map<SongDTO>(song);
            return Ok(songDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong([FromForm] SongDTO songDTO)
        {
            // Проверяем пользователя
            var userName = User.Identity?.Name;
            var user = await GetUserByNameAsync(userName);
            if (user == null) return BadRequest("User is not authenticated");

            // Проверяем размер файлов
            if (songDTO.MusicFile.Length > 100_000_000 || songDTO.VideoFile.Length > 100_000_000)
            {
                return BadRequest("One or more files exceed the size limit of 100 MB.");
            }

            // Сохраняем файлы
            songDTO.MusicFilePath = await SaveFileAsync(songDTO.MusicFile, "music");
            songDTO.VideoFilePath = await SaveFileAsync(songDTO.VideoFile, "videos");
            songDTO.VideoUrl = Url.Content($"~/{songDTO.VideoFilePath}");
            songDTO.UserName = user.UserName;

            var genre = await _genreService.GetGenreByIdAsync(songDTO.GenreId);
            if (genre == null)
            {
                _logger.LogError("Invalid Genre selected.");
                return BadRequest("Invalid Genre selected.");
            }

            songDTO.Genre = genre.Name;
            await _songService.AddSongAsync(songDTO);

            _logger.LogInformation("Song created successfully.");
            return CreatedAtAction(nameof(GetSongById), new { id = songDTO.SongId }, songDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromForm] SongDTO songDTO)
        {
            if (id != songDTO.SongId)
            {
                return BadRequest("Song ID mismatch.");
            }

            var existingSong = await _songService.GetSongByIdAsync(id);
            if (existingSong == null)
            {
                return NotFound("Song not found.");
            }

            if (songDTO.MusicFile != null)
            {
                if (songDTO.MusicFile.Length > 100_000_000)
                {
                    return BadRequest("Music file exceeds the size limit of 100 MB.");
                }
                songDTO.MusicFilePath = await SaveFileAsync(songDTO.MusicFile, "music");
            }
            if (songDTO.VideoFile != null)
            {
                if (songDTO.VideoFile.Length > 100_000_000)
                {
                    return BadRequest("Video file exceeds the size limit of 100 MB.");
                }
                songDTO.VideoFilePath = await SaveFileAsync(songDTO.VideoFile, "videos");
                songDTO.VideoUrl = Url.Content($"~/{songDTO.VideoFilePath}");
            }

            var genre = await _genreService.GetGenreByIdAsync(songDTO.GenreId);
            if (genre == null)
            {
                return BadRequest("Invalid Genre selected.");
            }

            songDTO.Genre = genre.Name;
            _mapper.Map(songDTO, existingSong);
            await _songService.UpdateSongAsync(existingSong);

            _logger.LogInformation("Song updated successfully.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound("Song not found.");
            }

            await _songService.DeleteSongAsync(id);
            _logger.LogInformation("Song deleted successfully.");
            return NoContent();
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null) return null;

            var directory = Path.Combine(_webHostEnvironment.WebRootPath, $"uploads/{folder}");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var filePath = Path.Combine($"uploads/{folder}", $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        private async Task<UserDTO> GetUserByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogError("User is not authenticated.");
                return null;
            }

            var user = await _userService.GetUserByNameAsync(userName);
            if (user == null)
            {
                _logger.LogError("User not found.");
                return null;
            }

            return user;
        }
    }
}