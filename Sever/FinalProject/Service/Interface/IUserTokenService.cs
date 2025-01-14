using FinalProject.Models.Token;

namespace FinalProject.Service.Interface
{
    public interface IUserTokenService
    {

        Task<UserToken> CheckRefreshToken(string code);
        Task SaveToken(UserToken token);
    }
}
