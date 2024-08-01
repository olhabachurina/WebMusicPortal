using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebPortal.Bll.DTO;
using WebPortal.Bll.Interfaces;

namespace MusicPortalLaLaFa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IUserService userService, IMapper mapper, ILogger<RegistrationController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("register-admin")]
        public async Task<IActionResult> RegisterAdmin()
        {
            if ((await _userService.GetAllUsersAsync()).Any(u => u.Role == "Admin"))
            {
                return BadRequest("Admin user already exists.");
            }
            return Ok("Ready to register an Admin");
        }

        [HttpPost("register-admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(UserDTO userDto)
        {
            if (ModelState.IsValid)
            {
                userDto.Role = "Admin";
                userDto.IsApproved = true;
                userDto.IsActive = true;

                await _userService.AddUserAsync(userDto);
                _logger.LogInformation("Admin user added.");
                return Ok("Admin user successfully registered.");
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error in field {Field}: {Error}", state.Key, error.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return Ok("Ready to register a User");
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            if (ModelState.IsValid)
            {
                userDto.Role = "User";
                userDto.IsApproved = false;
                userDto.IsActive = false;

                await _userService.AddUserAsync(userDto);
                return Ok("User successfully registered.");
            }
            return BadRequest(ModelState);
        }
    }
}