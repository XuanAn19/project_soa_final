using FinalProject.Models.Token;
using FinalProject.Repository.Interface;
using FinalProject.Repository.UnitOfWork;
using FinalProject.Service.Interface;

namespace FinalProject.Service
{
    public class UserTokenService : IUserTokenService
    {
        IUnitOfWork _unitOfWork;

        public UserTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SaveToken(UserToken token)
        {
            await _unitOfWork.userTokenRepository.Insert(token);
        }

        //Check value token
        public async Task<UserToken> CheckRefreshToken(string code)
        {
            return await _unitOfWork.userTokenRepository.GetUserTokenByCodeRefreshAsync(code);
        }
    }
}
