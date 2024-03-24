using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using System.Drawing;
using System;
using AALKisShared.Records;

namespace AALKisAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] User user)
        {
            try
            {
                if (_userRepository.IsValidName(user.Name) && _userRepository.IsValidName(user.Password))
                {
                    if (_userRepository.IsValidEmail(user.Email))
                    {
                        if (user.Password == user.PasswordCheck)
                        {
                            if(_userRepository.IsNameTaken(user.Name)) 
                            {
                                return StatusCode(403);
                            }
                            else
                            {
                                await _userRepository.SignIn(user.Name, user.Password, user.Email);
                                return Ok();
                            }
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return Conflict();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during sign-in");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred");
            }
        }
    


        [HttpPost]
        [Route("login")]
        public IActionResult LogIn([FromBody] User user) 
        { 
            if (_userRepository.IsValidName(user.Name) && _userRepository.IsValidName(user.Password) &&_userRepository.IsSignedIn(user.Name, user.Password)) 
            {
                var loggedUser = _userRepository.GetUser(user.Name);
                return Ok(loggedUser);
            }
            else 
            {
                return Unauthorized();
            }
        }
    }
}
