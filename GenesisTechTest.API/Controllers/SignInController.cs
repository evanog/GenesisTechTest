using System;
using System.Net;
using AutoMapper;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GenesisTechTest.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SignInController(ILogger<SignInController> logger, IMapper mapper, IUserService userService)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SignInResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public IActionResult SignIn(SignInRequest request)
        {
            try
            {
                var result = _userService.GetByEmailAndPasswordOrDefault(request.email, request.password);
                var response = _mapper.Map<SignInResponse>(result);
                return Ok(response);
            }
            catch (InvalidEmailAndPasswordException)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorResponse()
                {
                    message = "Invalid user and / or password"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing up user");
                return BadRequest();
            }
        }
    }
}