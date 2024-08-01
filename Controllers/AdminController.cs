using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using WebPortal.Bll.Interfaces;

namespace MusicPortalLaLaFa.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;

        public AdminController(ILogger<AdminController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            _logger.LogInformation("Users fetched: {Count}", users.Count());
            return Ok(users); // Возвращаем JSON-ответ
        }

        [HttpPost("approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto != null)
            {
                userDto.IsApproved = true;
                userDto.IsActive = true;
                await _userService.UpdateUserAsync(userDto);
                _logger.LogInformation("User approved: {UserName}", userDto.UserName);
                return Ok(userDto);
            }
            return NotFound();
        }

        [HttpPost("reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto != null)
            {
                await _userService.DeleteUserAsync(id);
                _logger.LogInformation("User rejected and removed: {UserName}", userDto.UserName);
                return Ok();
            }
            return NotFound();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> AllUsers(string nameFilter, string emailFilter, string statusFilter)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                users = users.Where(u => u.UserName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(emailFilter))
            {
                users = users.Where(u => u.Email.Contains(emailFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                users = statusFilter.ToLower() switch
                {
                    "active" => users.Where(u => u.IsActive).ToList(),
                    "inactive" => users.Where(u => !u.IsActive).ToList(),
                    _ => users
                };
            }

            return Ok(users);
        }
    }
}