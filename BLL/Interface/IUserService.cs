using Common.DTO;

using DAL.Entities;
using DTO.DTO;
using Microsoft.AspNetCore.Http;


namespace BLL.Interface
{
    public interface IUserService
    {
      
        ResponseDTO CheckValidationSignUpCustomer(SignUpCustomerDTOResquest model);
        ResponseDTO CheckValidationForgotPassword(ForgotPasswordModelDTO model);
        ResponseDTO ResetPassword(ForgotPasswordModelDTO model);
        ResponseDTO ForgotPassword(string email);
        Task<bool> SignUpCustomer(SignUpCustomerDTOResquest model);
        byte[] GenerateSalt();
        byte[] GenerateHashedPassword(string password, byte[] saltBytes);
        User CheckUserExist(string email);
     
        Guid ParseUserIdToGuid(string userId);
        User? GetUserByUserID(Guid userID);
        bool VerifyUser(Guid userId);
        bool CheckUserExistByUserId(Guid userId);
        bool CheckUserVerifiedStatus(Guid userId);
        bool CheckOTPExpired(Guid userId);
        bool SetOtp(OtpCodeDTO otpDto, Guid parseUserId);
        bool CheckOTP(Guid parseUserId, string otpCode);
 
 
        ViewProfileDTO ViewProfile(Guid userId);
        ResponseDTO EditProfile(ProfileDTO userDto);
        ResponseDTO CheckValidationEditProfile(ProfileDTO userDto);
      
  
        Task<UserResponseDTO?> GetUserResponseDtoByUserId(Guid userId);
        Task<User> GetUserById(Guid userId);
        Task UpdateUser(User user);
    }
}
