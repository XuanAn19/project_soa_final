using FinalProject.Data;
using FinalProject.Models.Token;
using FinalProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Repository
{
    public class UserTokenRepository : IUserTokenRepository
    {
        DataContext _dataContext;

        public UserTokenRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<UserToken> GetUserTokenByCodeRefreshAsync(string codeRefresh)
        {
            return await _dataContext.UserTokens.FirstOrDefaultAsync(x => x.CodeRefreshToken == codeRefresh);
        }

        public async Task Insert(UserToken user)
        {
           await _dataContext.UserTokens.AddAsync(user);
        }
    }
}
