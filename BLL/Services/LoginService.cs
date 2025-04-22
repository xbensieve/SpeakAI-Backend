
using Azure.Core;
using BLL.Interface;
using Common.DTO;
using Common.Enum;
using Common.Message.AuthMessage;
using Common.Message.ValidationMessage;
using DAL.Entities;

using DAL.UnitOfWork;
using DTO.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(IUnitOfWork unitOfWork, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IGoogleAuthService googleAuthService)
        {
            _unitofWork = unitOfWork;

            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _googleAuthService=googleAuthService;
        }

        /// <summary>
        /// this check unique user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsUniqueUser(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var result = _unitofWork.User.FindAll(x => x.Username == userName).FirstOrDefault();
                return result != null;
            }
            return false;
        }

        /// <summary>
        /// This is login function ( create accessToken and refreshToken )
        /// </summary>
        /// <param name="loginRequestDTO"></param>
        /// <returns></returns>
        public LoginResponseDTO? Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _unitofWork.User.FindAll(x => x.Username == loginRequestDTO.UserName).FirstOrDefault();
            if (user == null) return null;
            if (VerifyPasswordHash(loginRequestDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                string jwtTokenId = $"JTI{Guid.NewGuid()}";

                string refreshToken = CreateNewRefreshToken(user.Id, jwtTokenId);

                var accessToken = CreateToken(user, jwtTokenId);

            

                // Trả về đối tượng LoginResponseDTO
                return new LoginResponseDTO
                {
                    AccessToken = accessToken,
  
                    RefreshToken = refreshToken
                };
            }

            return null;
        }

        /// <summary>
        /// This is Create new accessToken with Refresh Token
        /// </summary>
        /// <param name="tokenDTO"></param>
        /// <returns></returns>
        public TokenDTO RefreshAccessToken(RequestTokenDTO tokenDTO)
        {
            // Find an existing refresh token
            var existingRefreshToken = _unitofWork.RefreshToken.FindAll(r => r.Refresh_Token == tokenDTO.RefreshToken).FirstOrDefault();
            if (existingRefreshToken == null)
            {
                return new TokenDTO()
                {
                    Message = MessageErrorInRefreshToken.TokenNotExistInDB
                };
            }

            // Compare data from exixsting refresh and access token provided and if there is any missmatch then consider it as fraud
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtId);
            if (!isTokenValid)
            {
                existingRefreshToken.IsValid = false;
                _unitofWork.SaveChange();
                return new TokenDTO()
                {
                    Message = MessageErrorInRefreshToken.TokenInvalid
                };
            }

            // Check accessToken expire ?
            var tokenHandler = new JwtSecurityTokenHandler();
            var test = tokenHandler.ReadJwtToken(tokenDTO.AccessToken);
            if (test == null) return new TokenDTO()
            {
                Message = MessageErrorInRefreshToken.ErrorInProcessing
            };

            var accessExpiredDateTime = test.ValidTo;
            // Sử dụng accessExpiredDateTime làm giá trị thời gian hết hạn

            if (accessExpiredDateTime > DateTime.UtcNow)
            {
                return new TokenDTO()
                {
                    Message = MessageErrorInRefreshToken.AccessTokenHasNotExpired
                };
            }
            // When someone tries to use not valid refresh token, fraud possible

            if (!existingRefreshToken.IsValid)
            {
                var chainRecords = _unitofWork.RefreshToken.FindAll(u => u.UserId == existingRefreshToken.UserId && u.JwtId == existingRefreshToken.JwtId).ToList();
                var a = chainRecords;
                foreach (var item in chainRecords)
                {
                    item.IsValid = false;
                }
                _unitofWork.RefreshToken.UpdateRange(chainRecords);
                _unitofWork.SaveChange();
                return new TokenDTO { Message = MessageErrorInRefreshToken.RefreshTokenInvalid };
            }

            // If it just expired then mark as invalid and return empty

            if (existingRefreshToken.ExpiredAt < DateTime.Now)
            {
                existingRefreshToken.IsValid = false;
                _unitofWork.SaveChange();
                return new TokenDTO() { Message = MessageErrorInRefreshToken.RefreshTokenExpired };
            }

            // Replace old refresh with a new one with updated expired date
            var newRefreshToken = ReNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtId);

            // Revoke existing refresh token
            existingRefreshToken.IsValid = false;
            _unitofWork.SaveChange();
            // Generate new access token
            var user = _unitofWork.User.FindAll(a => a.Id == existingRefreshToken.UserId).FirstOrDefault();
            if (user == null)
            {
                return new TokenDTO();
            }
            var newAccessToken = CreateToken(user, existingRefreshToken.JwtId);

            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = MessageErrorInRefreshToken.Successfully
            };
        }

        /// <summary>
        /// This is CreateToken function to create new token when login successfull
        /// </summary>
        /// <param name="user"></param>
        /// <param name="jwtId"></param>
        /// <returns></returns>
        private string CreateToken(User user, string jwtId)
        {
          


            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.FullName.ToString()),
              
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(45).ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString()),
                new Claim (JwtRegisteredClaimNames.GivenName, user.Username.ToString()),

            };
            var key = _configuration.GetSection("ApiSetting")["Secret"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddMinutes(1),
               signingCredentials: credentials
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// This is Verify Password because password is hash and salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] salt)
        {
            // Compute the hash of the provided password using the provided salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + salt.Length];

            for (int i = 0; i < passwordBytes.Length; i++)
            {
                passwordWithSaltBytes[i] = passwordBytes[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                passwordWithSaltBytes[passwordBytes.Length + i] = salt[i];
            }

            using (var cryptoProvider = SHA512.Create())
            {
                byte[] hashedPassword = cryptoProvider.ComputeHash(passwordWithSaltBytes);

                // Compare the computed hash with the stored hash
                return CompareByteArrays(hashedPassword, storedHash);
            }
        }

        private bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1 == null || array2 == null || array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This is to read the data (claim) in Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="expectedUserId"></param>
        /// <param name="expectedTokenId"></param>
        /// <returns></returns>
        private bool GetAccessTokenData(string accessToken, Guid expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtId = jwt.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userId = jwt.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Sub)?.Value;
                userId = userId ?? string.Empty;
                return Guid.Parse(userId) == expectedUserId && jwtId == expectedTokenId;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This is create random token to put into refresh token
        /// </summary>
        /// <returns></returns>
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        /// <summary>
        /// This is create new Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jwtId"></param>
        /// <returns></returns>
        private string CreateNewRefreshToken(Guid userId, string jwtId)
        {
            var time = _unitofWork.RefreshToken.FindAll(a => a.JwtId == jwtId).FirstOrDefault();
            RefreshToken refreshAccessToken = new()
            {
                UserId = userId,
                JwtId = jwtId,
                ExpiredAt = DateTime.Now.AddHours(1),
                IsValid = true,
                Refresh_Token = CreateRandomToken(),
            };
            _unitofWork.RefreshToken.Add(refreshAccessToken);
            _unitofWork.SaveChange();
            return refreshAccessToken.Refresh_Token;
        }

        /// <summary>
        /// This function is also create new refresh token but it just create new refresh token not expired time in refresh token ok ?
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jwtId"></param>
        /// <returns></returns>
        private string ReNewRefreshToken(Guid userId, string jwtId)
        {
            var time = _unitofWork.RefreshToken.FindAll(a => a.JwtId == jwtId).FirstOrDefault();
            RefreshToken refreshAccessToken = new()
            {
                UserId = userId,
                JwtId = jwtId,
                ExpiredAt = time?.ExpiredAt != null ? time.ExpiredAt : DateTime.Now.AddMinutes(15),
                IsValid = true,
                Refresh_Token = CreateRandomToken(),
            };
            _unitofWork.RefreshToken.Add(refreshAccessToken);
            _unitofWork.SaveChange();
            return refreshAccessToken.Refresh_Token;
        }
        /// <summary>
        /// Find token and revoke token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public ResponseDTO Logout(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.NotFound, false);
            }
            if (!Guid.TryParse(userId, out Guid userIdGuid))
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.NotFound, false);
            }

            var user = _unitofWork.User.FindAll(u => u.Id == userIdGuid);
            if (user == null)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.NotFound, false);
            }

            var refreshTokenList = _unitofWork.RefreshToken.GetAll().Where(c => c.UserId == userIdGuid).ToList(); ;

            _unitofWork.RefreshToken.RemoveRange(refreshTokenList);
            _unitofWork.SaveChange();

            return new ResponseDTO(AuthNotificationMessage.LogOut, StatusCodeEnum.OK, true);
        }

        /// <summary>
        /// Find token and revoke token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public ResponseDTO GetUserByAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ResponseDTO(AuthNotificationMessage.AccessTokenEmpty, StatusCodeEnum.NotFound, false);
            }
            var userId = ExtractUserIdFromToken(accessToken);
            if (userId == null)
            {
                return new ResponseDTO(AuthNotificationMessage.TokenExpired, StatusCodeEnum.NotFound, false);
            }
            if (userId == AuthNotificationMessage.InvalidToken)
            {
                return new ResponseDTO(AuthNotificationMessage.InvalidToken, StatusCodeEnum.NotFound, false);
            }
            var user = GetUserByUserId(userId);
            if (user == null)
            {
                return new ResponseDTO(AuthNotificationMessage.UserNotFound, StatusCodeEnum.NotFound, false);
            }
            return new ResponseDTO(AuthNotificationMessage.GetUserByTokenSuccess, StatusCodeEnum.OK, true, user);
        }

        public LocalUserDTO GetUserByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }
            Guid.TryParse(userId, out Guid userIdGuid);
            var user = _unitofWork.User.FindAll(a => a.Id == userIdGuid).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            // Ánh xạ thủ công từ User sang LocalUserDTO
            var localUserDTO = new LocalUserDTO
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.Username,
                FullName = user.FullName,
              
                Status = user.Status
            };

            return localUserDTO;
        }
        private string ExtractUserIdFromToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                //token expired
                var expiration = jwtToken.ValidTo;

                if (expiration < DateTime.UtcNow)
                {
                    return null;
                }
                else
                {
                    //  take userId in claim of token
                    string userId = jwtToken.Subject;
                    return userId;
                }
            }
            catch (Exception ex)
            {
                return AuthNotificationMessage.InvalidToken;
            }
        }
        public async Task<ResponseDTO> SignInWithGoogle(GoogleAuthTokenDTO googleAuthToken)
        {
            var response = await _googleAuthService.GoogleSignIn(googleAuthToken);
            return response;
        }
    }
}

