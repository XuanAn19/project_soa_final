using FinalProject.Models.Token;
using FinalProject.Models;
using FinalProject.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject.Repository.Interface;
using FinalProject.DTOs;
using FinalProject.Middleware;
using FinalProject.Data;
using Microsoft.AspNetCore.Identity;
using FinalProject.Mail;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Repository.UnitOfWork;
using FinalProject.Models.CustomUser;
using Azure.Core;
using FinalProject.Repository;

namespace FinalProject.Service
{
    public class UserService : IUserService
    {
        IUserTokenService _userTokenService;
        ITokenHandler _tokenHandler;
        ISendEmail _sendEmail;
        IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IUserTokenService userTokenService,
           ITokenHandler tokenHandler, ISendEmail sendEmail)
        {
            _unitOfWork = unitOfWork;
            _tokenHandler = tokenHandler;
            _sendEmail = sendEmail;
            _userTokenService = userTokenService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.userRepository.GetUserByEmailAsync(email);
        }

        public async Task<UserDetailDTO> GetUserByIdlAsync(string id)
        {
            var user = await _unitOfWork.userRepository.GetUserByIdlAsync(id);

            if (user == null) return null;

            // Chuyển đổi sang DTO
            var userDto = new UserDetailDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId, // Lấy tên role từ liên kết
                Address = user.Addresss == null? null: $"{user.Addresss.Detail}, {user.Addresss.Ward}, {user.Addresss.District}, {user.Addresss.Provice}",
                LastLoggedIn = user.LastLoggedIn,
                CreateDateAt = user.CreateDateAt,
                Id = user.Id
            };

            return userDto;
        }

        public async Task<List<UserDetailDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.userRepository.GetAllUserAsync();
            return users.Select(u => new UserDetailDTO
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                Address = u.Addresss == null ? null : $"{u.Addresss.Detail}, {u.Addresss.Ward}, {u.Addresss.District}, {u.Addresss.Provice}",
                LastLoggedIn = u.LastLoggedIn,
                CreateDateAt = u.CreateDateAt
            }).ToList();
        }

        public async Task UpdateUserRoleAsync(string userId, string roleId)
        {
            var user = await _unitOfWork.userRepository.GetUserByIdlAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.RoleId = roleId;
            await _unitOfWork.userRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _unitOfWork.userRepository.GetUserByIdlAsync(userId);
            await _unitOfWork.userRepository.Delete(user);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<IActionResult> ConfirmAccount(ConfirmUserDTO confirm)
        {
            var user = await _unitOfWork.userRepository.GetUserByEmailAsync(confirm.Email);
            if (user == null)
            {
                return new BadRequestObjectResult("Không có tài khoản.");
            }

            if (user.CodeEmailConfirm == confirm.Code)
            {
                user.isActive = true;
                user.CodeEmailConfirm = null;
                user.EmailConfirm = true;
                await _unitOfWork.userRepository.Update(user);
                await _unitOfWork.SaveChangeAsync();

                return new OkObjectResult("Đăng kí tài khoản thành công.");
            }

            return new BadRequestObjectResult("Mã xác nhận không đúng.");
        }

        public async Task<object> Login(AccountDTO account)
        {
            var user = await _unitOfWork.userRepository.GetUserByEmailAsync(account.Email);
            if (user != null)
            {
                if (user.isActive == true)
                {

                    var verifyPassword = MD5_password.VerifyPassword(account.Password, user.Password.ToString());
                    if (verifyPassword)
                    {
                        // Tạo token
                        (string tokenAccess, DateTime exp) = await _tokenHandler.CreateAccessToken(user);
                        (string code, string tokenRefresh, DateTime expRefresh) = await _tokenHandler.CreateRefreshToken(user);

                        await _userTokenService.SaveToken(new UserToken
                        {
                            Accesstoken = tokenAccess,
                            Refreshtoken = tokenRefresh,
                            ExpiredAccess = exp,
                            CodeRefreshToken = code,
                            ExpiredRefresh = expRefresh,
                            CreateDateRefresh = DateTime.Now,
                            UserId = user.Id,
                            IsRevoked = true

                        });
                        await _unitOfWork.SaveChangeAsync();
                        var token = new TokenModels
                        {
                            AccessToken = tokenAccess,
                            RefreshToken = tokenRefresh,
                            Fullname = user.FullName,
                            Email = user.Email,
                            Role = user.role.roleName
                        };
                        return token;
                    }
                    return StatusCodes.Status400BadRequest.ToString("Mật khẩu không đúng.");
                }
                return StatusCodes.Status204NoContent.ToString("Email đã đăng kí. Nhưng chưa xác nhận. Kiểm tra email để hoàn tất đăng kí");
            }
            return StatusCodes.Status404NotFound.ToString("Tài khoản không tồn tại.");
        }

        public async Task Register(RegisterDTO register)
        {
            var checkuser = await _unitOfWork.userRepository.GetUserByEmailAsync(register.Email);
            if (checkuser == null)
            {
                string roleName = "User";

                var role = await _unitOfWork.roleRepository.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    throw new Exception("Invalid RoleId. Role does not exist.");
                }
                string confirmationCode = GenerateConfirmationCode(8);
                var hashPassword = MD5_password.GetSha256Hash(register.Password);
                var newUser = new User
                {
                    Email = register.Email,
                    Password = hashPassword,
                    FullName = register.FullName,
                    CodeEmailConfirm = confirmationCode,
                    RoleId = role.Id,
                    isActive = false

                };

                await _unitOfWork.userRepository.Insert(newUser);
                // Gửi email chứa mã xác nhận
                string subject = "Mã xác nhận đăng kí";
                string message = $"<p>Chào {newUser.FullName},</p>" +
                                 $"<p>Cảm ơn bạn đã đăng kí tài khoản của QuyNhonWork. Vui lòng sử dụng mã xác nhận sau để kích hoạt tài khoản của bạn.:</p>" +
                                 $"<h3>{confirmationCode}</h3>" +
                                 $"<p>Nếu bạn không đăng ký tài khoản này, xin vui lòng bỏ qua email này.</p>";

                await _sendEmail.SendEmailAsync(newUser.Email, subject, message);
                await _unitOfWork.SaveChangeAsync();

            }
            else
            {
                throw new Exception("Email này đã hoạt động.");
            }
           
        }

        public async Task<TokenModels> RefreshToken(RefreshTokenDTO refreshToken)
        {
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.RefreshToken))
            {
                throw new ArgumentException("Refresh token không được bỏ trống.");
            }

            // Gọi ValidateRefreshToken
            var userToken = await _tokenHandler.ValidateRefreshToken(refreshToken.RefreshToken);
            if (userToken == null || string.IsNullOrEmpty(userToken.AccessToken))
            {
                throw new InvalidOperationException("Refresh token không tìm thấy.");
            }

            return userToken;
        }

        public async Task<User> UpdateUserAsync(UpdateUserDTO user, string userId)
        {
            var exitUser = await _unitOfWork.userRepository.GetUserByIdlAsync(userId);
            if (exitUser == null) { throw new Exception("Không có tài khoản."); }

            exitUser.FullName = user.FullName ?? exitUser.FullName;
            exitUser.PhoneNumber = user.PhoneNumber ?? exitUser.PhoneNumber;
            if (user.Addresss != null)
            {
                var existingAddress = await _unitOfWork.addressRepository.GetAddressByEmailAsync(exitUser.IdAddress);
                if (existingAddress != null)
                {
                    existingAddress.Provice = user.Addresss.Provice ?? existingAddress.Provice;
                    existingAddress.District = user.Addresss.District ?? existingAddress.District;
                    existingAddress.Ward = user.Addresss.Ward ?? existingAddress.Ward;
                    existingAddress.Detail = user.Addresss.Detail ?? existingAddress.Detail;

                    await _unitOfWork.addressRepository.UpdateAddressUserAsync(existingAddress);
                }
                else
                {
                    var newAddress = new Address
                    {
                        Provice = user.Addresss.Provice,
                        District = user.Addresss.District,
                        Ward = user.Addresss.Ward,
                        Detail = user.Addresss.Detail,
                    };

                    await _unitOfWork.addressRepository.AddAddressUserAsync(newAddress);
                    exitUser.IdAddress = newAddress.Id;
                }


            }
            await _unitOfWork.userRepository.Update(exitUser);
            await _unitOfWork.SaveChangeAsync();

            return exitUser;
        }

        /// <summary>
        ///  Generate code confirm email
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateConfirmationCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }
    }
}
