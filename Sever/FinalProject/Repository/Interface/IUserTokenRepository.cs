using FinalProject.Models;
using FinalProject.Models.Token;

namespace FinalProject.Repository.Interface
{
    public interface IUserTokenRepository
    {
        Task Insert(UserToken user);
        Task<UserToken> GetUserTokenByCodeRefreshAsync(string codeRefresh);
    }
}
