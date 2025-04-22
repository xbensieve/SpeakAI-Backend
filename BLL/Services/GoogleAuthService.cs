using Azure.Core;
using BLL.Interface;
using Common.Config;
using Common.Constants;
using Common.DTO;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using DTO.DTO;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Security.Claims;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace BLL.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GoogleAuthConfig _googleAuthConfig;
        private readonly IJwtProvider _jwtProvider;

        public GoogleAuthService(
            IUnitOfWork unitOfWork,
            IOptions<GoogleAuthConfig> googleAuthConfig,
            IJwtProvider jwtProvider)
        {
            _unitOfWork = unitOfWork;
            _googleAuthConfig = googleAuthConfig.Value;
            _jwtProvider = jwtProvider;
        }

        public async Task<ResponseDTO> GoogleSignIn(GoogleAuthTokenDTO googleAuthToken)
        {
            Payload payload;
            try
            {
                payload = await ValidateAsync(googleAuthToken.IdToken, new ValidationSettings
                {
                    Audience = new[] { "" },
                    
                });
             
            }
            catch (InvalidJwtException ex)
            {
                Console.WriteLine($"JWT validation failed: {ex.Message}");
                return new ResponseDTO("Failed to validate Google token.", StatusCodeEnum.BadRequest, false, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseDTO("Unexpected error during token validation.", StatusCodeEnum.InteralServerError, false, ex.Message);
            }

            var existingUser = await _unitOfWork.User.GetByEmailAsync(payload.Email);
            if (existingUser != null)
            {
                return await GenerateTokensForUser(existingUser);
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = $"{payload.GivenName} {payload.FamilyName}".Trim(),
                FullName = payload.Name .Trim(),
                Gender = "None",
             IsLocked = false,
             IsPremium = false,
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                Email = payload.Email,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                Birthday = DateTime.UtcNow,
                IsAdmin = false,
                LastLogin= DateTime.UtcNow,
                Status = true,
                Otp = null,
                OtpExpiredTime = null,
                PremiumExpiredTime = null,
                
              
            };
            var userLevell = new UserLevel
            {

                Point = 0,
                LevelName = "A0,A1",

                LevelId = 1,
                UserId = newUser.Id

            };
            await _unitOfWork.User.AddAsync(newUser);
            await _unitOfWork.UserLevel.AddAsync(userLevell);
            await _unitOfWork.SaveChangeAsync();

            return await GenerateTokensForUser(newUser);
        }


        private async Task<ResponseDTO> GenerateTokensForUser(User user)
        {
            // Generate tokens
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.UserId, user.Id.ToString()),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Username, user.Username),
            
            };

            var accessToken = _jwtProvider.GenerateAccessToken(claims);
            var refreshToken = _jwtProvider.GenerateRefreshToken(claims);

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Refresh_Token = refreshToken,
                UserId = user.Id,
                ExpiredAt = DateTime.UtcNow.AddDays(7),
                CreateAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.RefreshToken.AddAsync(refreshTokenEntity);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                return new ResponseDTO("Failed to save refresh token", StatusCodeEnum.InteralServerError, false, ex.Message);
            }

            return new ResponseDTO("Login successful", StatusCodeEnum.OK, true, new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
     
              
            });
        }
    }
}