using BooksBot.API.Models;
using BooksBot.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BooksBot.API.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        

        public TokenController(IIdentityService identityService)
        {
            _identityService = identityService;
           
        }
        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<TokenResponse> Get(TokenRequest model)
        {
            return await _identityService.Login(model);
            
        }
        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        //[HttpPost("refresh")]
        //public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        //{
        //    var response = await _identityService.GetRefreshTokenAsync(model);
        //    return Ok(response);
        //}
    }
}
