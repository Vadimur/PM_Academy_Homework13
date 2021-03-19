using DepsWebApp.Models;
using DepsWebApp.Services.CustomAuthentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Registration controller
    /// </summary>
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICustomAuthenticationService _authenticationService;

#pragma warning disable CS1591
        public AuthController(ICustomAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
#pragma warning restore CS1591
        /// <summary>
        /// Registration endpoint for Basic Authentication. Returns encoded username and password (Authentication token)
        /// </summary>
        /// <param name="registrationModel">Registration model with login and password</param>
        /// <returns>Encoded username and password (Authentication token)</returns>
        [HttpPost("register")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> Register([FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string authToken = await _authenticationService.Register(registrationModel);
            if (!string.IsNullOrWhiteSpace(authToken))
            {
                return Ok(authToken);
            }
            else
            {
                return BadRequest("Unable to register new user");
            }
        }
    }
}