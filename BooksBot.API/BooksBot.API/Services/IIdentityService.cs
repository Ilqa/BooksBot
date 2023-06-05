using BooksBot.API.Models;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public interface IIdentityService
    {

        Task<TokenResponse> Login(TokenRequest model);

        //Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}