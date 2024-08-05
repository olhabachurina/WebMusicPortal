using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPortal.Bll.DTO;
using System.ComponentModel.DataAnnotations;
using WebPortal.Bll.Services;
using Microsoft.AspNetCore.Authorization;
using WebPortal.Bll.Interfaces;

namespace WebMusicPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly ILogger<GenresController> _logger;

        public GenresController(IGenreService genreService, ILogger<GenresController> logger)
        {
            _genreService = genreService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all genres.");
            var genres = await _genreService.GetAllGenresAsync();
            var genreDTOs = genres.Select(g => new GenreDTO
            {
                GenreId = g.GenreId,
                Name = g.Name
            }).ToList();
            return Ok(genreDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenreDTO genreDto)
        {
            if (ModelState.IsValid)
            {
                await _genreService.AddGenreAsync(genreDto);
                _logger.LogInformation("Genre created successfully.");
                return CreatedAtAction(nameof(Details), new { id = genreDto.GenreId }, genreDto);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] GenreDTO genreDto)
        {
            if (id != genreDto.GenreId)
            {
                return BadRequest("Genre ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGenre = await _genreService.GetGenreByIdAsync(id);
            if (existingGenre == null)
            {
                return NotFound("Genre not found.");
            }

            existingGenre.Name = genreDto.Name;

            try
            {
                await _genreService.UpdateGenreAsync(existingGenre);
                _logger.LogInformation("Genre updated successfully.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GenreExists(genreDto.GenreId))
                {
                    return NotFound("Genre not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                _logger.LogWarning("Жанр не найден.");
                return NotFound("Жанр не найден.");
            }

            try
            {
                await _genreService.DeleteGenreAsync(id);
                _logger.LogInformation("Жанр успешно удален.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при удалении жанра: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при удалении жанра.");
            }
        }

        private async Task<bool> GenreExists(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            return genre != null;
        }
    }
}