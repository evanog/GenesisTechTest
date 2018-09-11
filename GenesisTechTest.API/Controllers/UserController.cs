using System;
using System.Net;
using AutoMapper;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenesisTechTest.API.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IValidationService _validatorService;
        private readonly IOptions<AppConfig> _config;

        public UserController(ILogger<UserController> logger,
            IMapper mapper,
            IUserService userService,
            IValidationService validatorService,
            IOptions<AppConfig> config)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _validatorService = validatorService;
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(SearchUserResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var userid))
                    return NotFound();

                var user = _userService.GetByUserIdOrDefault(userid);
                if (user == null)
                    return NotFound();

                var accessToken = HttpContext.Request.Headers["Authorization"];
                if (!_validatorService.IsTokenMatching(user, accessToken))
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorResponse()
                    {
                        message = "Unauthorized"
                    });
                }

                if (!_validatorService.IsWithinNumOfMinutes(user, _config.Value.SessionTimeOutMinutes))
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorResponse()
                    {
                        message = "Invalid Session"
                    });
                }

                var response = _mapper.Map<SearchUserResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for user");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
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

        [HttpPost]
        [Route("signin")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(SignInResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public IActionResult SignIn(SignInRequest request)
        {
            try
            {
                var result = _userService.SignIn(request.email, request.password);
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