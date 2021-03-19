using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DepsWebApp.Services.CustomAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DepsWebApp.CustomAuthentication
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BasicAuthenticationSchemaHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICustomAuthenticationService _authenticationService;

        public BasicAuthenticationSchemaHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            ICustomAuthenticationService authService) : base(options, logger, encoder, clock)
        {
            _authenticationService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            
            string authenticationToken = GetBasicAuthenticationTokenFromRequest(Request);
            if (string.IsNullOrWhiteSpace(authenticationToken))
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
            
            try
            {
                string login = GetLoginFromAuthenticationToken(authenticationToken);
                string password = GetPasswordFromAuthenticationToken(authenticationToken);
                
                bool isValidAuthToken = _authenticationService.Authenticate(login, password);
                if (!isValidAuthToken)
                {
                    return AuthenticateResult.Fail("Unregistered Authorization Header");
                }
                else
                {
                    Claim[] claims = new[] {
                        new Claim(ClaimTypes.Name, login),
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    
                    return AuthenticateResult.Success(ticket);
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during authentication");
                return AuthenticateResult.Fail(ex);
            }
            
        }

        private string GetBasicAuthenticationTokenFromRequest(HttpRequest request)
        {
            try
            {
                AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
                byte[] credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var authenticationToken = Encoding.UTF8.GetString(credentialBytes);
                return authenticationToken;
            }
            catch
            {
                return null;
            }
        }

        private string GetLoginFromAuthenticationToken(string authenticationToken)
        {
            return authenticationToken.Split(new[] { ':' }, 2)[0];
        }
        
        private string GetPasswordFromAuthenticationToken(string authenticationToken)
        {
            return authenticationToken.Split(new[] { ':' }, 2)[1];
        }
        
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}