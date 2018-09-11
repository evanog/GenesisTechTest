using System;
using System.Net;
using System.Net.Http;
using AutoMapper;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GenesisTechTest.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SignUpController(ILogger<SignUpController> logger, IMapper mapper, IUserService userService)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SignUpResponse))]
        [ProducesResponseType(409, Type = typeof(ErrorResponse))]
        public IActionResult SignUp(SignUpRequest newSignUp)
        {
            try
            {
                var user = _mapper.Map<User>(newSignUp);
                var result = _userService.CreateUser(user);
                var response = _mapper.Map<SignUpResponse>(result);
                return Ok(response);
            }
            catch (EmailAlreadyExistsException)
            {
                return Conflict(new ErrorResponse()
                {
                    message = "E-mail already exists"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing in");
                return BadRequest();
            }
        }
    }
}