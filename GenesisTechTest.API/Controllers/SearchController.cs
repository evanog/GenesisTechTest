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
    public class SearchController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IValidatorService _validatorService;
        private readonly IOptions<AppConfig> _config;

        public SearchController(ILogger<SearchController> logger, 
            IMapper mapper, 
            IUserService userService, 
            IValidatorService validatorService, 
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
        public IActionResult Get(Guid userId)
        {
            try
            {
                var user = _userService.GetByUserIdOrDefault(userId);
                if (user == null)
                    return NotFound();

                if (!_validatorService.IsWithinNumOfMinutes(user, _config.Value.SessionTimeOutMinutes))
                {
                    return StatusCode((int) HttpStatusCode.Unauthorized, new ErrorResponse()
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
    }
}