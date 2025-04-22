
using BLL.Interface;
using Common.DTO;
using Common.Enum;
using Common.Message.AuthMessage;
using Common.Message.EmailMessage;
using Common.Message.ValidationMessage;
using Common.Template;
using DAL.Entities;
using DAL.UnitOfWork;
using DTO.DTO;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitofWork;
    
        private readonly IValidationHandleService _validationHandle;
        private readonly IEmailService _emailService;
     
        public UserService(IUnitOfWork unitOfWork,
           
            IValidationHandleService validationHandle,
            IEmailService emailService)
        {
            _unitofWork = unitOfWork;
           
            _validationHandle = validationHandle;
            _emailService = emailService;
          

           
        }

  

        /// <summary>
        /// Check validation for all inputted fields of sign up customer function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseDTO CheckValidationSignUpCustomer(SignUpCustomerDTOResquest model)
        {
            var userList = _unitofWork.User.GetAll().ToList();

            var checkNullUserName = _validationHandle.CheckNull(model.UserName);
            if (!checkNullUserName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullUserName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatUserName = _validationHandle.CheckFormatUserName(model.UserName);
            if (!checkFormatUserName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatUserName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkUserNameAlreadyExists = _validationHandle.CheckUserNameAlreadyExists(model.UserName, userList);
            if (!checkUserNameAlreadyExists)
            {
                var response = new ResponseDTO(ValidationErrorMessage.UserNameAlreadyExists, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullFullName = _validationHandle.CheckNull(model.FullName);
            if (!checkNullFullName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullFullName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatFullName = _validationHandle.CheckFormatFullName(model.FullName);
            if (!checkFormatFullName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatFullName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullPassword = _validationHandle.CheckNull(model.Password);
            if (!checkNullPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatPassword = _validationHandle.CheckFormatPassword(model.Password);
            if (!checkFormatPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkConfirmPassword = _validationHandle.CheckConfirmPassword(model.Password, model.ConfirmedPassword);
            if (!checkConfirmPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongConfirmPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

          

         

            var checkNullEmail = _validationHandle.CheckNull(model.Email);
            if (!checkNullEmail)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullEmail, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatEmail = _validationHandle.CheckFormatEmail(model.Email);
            if (!checkFormatEmail)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatEmail, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkEmailAlreadyExists = _validationHandle.CheckEmailAlreadyExists(model.Email, userList);
            if (!checkEmailAlreadyExists)
            {
                var response = new ResponseDTO(ValidationErrorMessage.EmailAlreadyExists, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullBirthday = _validationHandle.CheckNull(model.Birthday);
            if (!checkNullBirthday)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullBirthday, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatBirthday = _validationHandle.CheckFormatBirthday(model.Birthday);
            if (!checkFormatBirthday)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatBirthday, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullGender = _validationHandle.CheckNull(model.Gender);
            if (!checkNullGender)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullGender, StatusCodeEnum.BadRequest, false);
                return response;
            }


         

       

           

            var successfulResponse = new ResponseDTO("Check Validation Successfully", StatusCodeEnum.OK, true);
            return successfulResponse;
        }

        /// <summary>
        /// Add new Customer into DB Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SignUpCustomer(SignUpCustomerDTOResquest model)
        {
            var result = false;
            var saltBytes = GenerateSalt();
            var passwordHashedBytes = GenerateHashedPassword(model.Password, saltBytes);
        

            var checkBirthDay = DateTime.TryParse(model.Birthday, out DateTime birthday);
            if (checkBirthDay)
            {
                var userLevel = Guid.NewGuid();
                var newuser = new User
                {
                    Id = userLevel, 
                    Username = model.UserName,
                    PasswordSalt = saltBytes,
                    PasswordHash = passwordHashedBytes,
                    Email = model.Email,
                    FullName = model.FullName,
                    Birthday = birthday,
                    Gender = model.Gender,
                    CreatedAt = DateTime.UtcNow,
                    IsAdmin = false,
                    IsVerified = false,
                    IsPremium = false,
                    Status = true,
                    Otp = null,
                    OtpExpiredTime = null,
                    IsLocked = false
                
                };
                var userLevell = new UserLevel
                {

                    Point = 0, 
                    LevelName = "A0,A1", 

                    LevelId = 1,
                    UserId = newuser.Id
                
                };



                await _unitofWork.User.AddAsync(newuser);
                await _unitofWork.UserLevel.AddAsync(userLevell);
                result = await _unitofWork.SaveChangeAsync();
                _emailService.SendWelcomeEmail(model.Email, model.UserName, EmailSubject.WelcomeEmailSubject);
              
                return result;
            }
            return result;
        }

        /// <summary>
        /// Generate random salt service
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(saltBytes);
            return saltBytes;
        }

        /// <summary>
        /// Hash string of password and salt service
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltBytes"></param>
        /// <returns></returns>
        public byte[] GenerateHashedPassword(string password, byte[] saltBytes)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];

            for (int i = 0; i < passwordBytes.Length; i++)
            {
                passwordWithSaltBytes[i] = passwordBytes[i];
            }

            for (int i = 0; i < saltBytes.Length; i++)
            {
                passwordWithSaltBytes[passwordBytes.Length + i] = saltBytes[i];
            }

            var cryptoProvider = SHA512.Create();
            byte[] hashedBytes = cryptoProvider.ComputeHash(passwordWithSaltBytes);

            return hashedBytes;
        }

        /// <summary>
        /// Check validation of ForgotPasswordDTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseDTO CheckValidationForgotPassword(ForgotPasswordModelDTO model)
        {
            if (model.Email == null)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullEmail, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var user = _unitofWork.User.GetAll().FirstOrDefault(c => c.Email == model.Email);
            if (user == null)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.BadRequest, false);
            }

            var checkUserVerifiedStatus = CheckUserVerifiedStatus(user.Id);
            if (!checkUserVerifiedStatus)
            {
                return new ResponseDTO(AuthNotificationMessage.UserIsNotVerified, StatusCodeEnum.BadRequest, false);
            }

            var checkOtp = CheckOTP(user.Id, model.OTP);
            if (!checkOtp)
            {
                return new ResponseDTO(AuthNotificationMessage.OtpInccorect, StatusCodeEnum.BadRequest, false);
            }

            var checkOTPExpired = CheckOTPExpired(user.Id);
            if (checkOTPExpired)
            {
                return new ResponseDTO(AuthNotificationMessage.OtpExpired, StatusCodeEnum.BadRequest, false);
            }

            var checkNullPassword = _validationHandle.CheckNull(model.Password);
            if (!checkNullPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }
            var checkToken = _validationHandle.CheckNullToken(model.OTP);
            if (!checkToken)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullToken, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatPassword = _validationHandle.CheckFormatPassword(model.Password);
            if (!checkFormatPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var successfulResponse = new ResponseDTO("Check Validation Successfully", StatusCodeEnum.OK, true);
            return successfulResponse;
        }

        /// <summary>
        /// Reset password if user inputs correct otp  code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseDTO ResetPassword(ForgotPasswordModelDTO model)
        {
            var saltBytes = GenerateSalt();
            var passwordHashedBytes = GenerateHashedPassword(model.Password, saltBytes);
            var user = _unitofWork.User.FindAll(u => u.Email == model.Email).FirstOrDefault();
            if (user == null)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.BadRequest, false);
            }

            user.PasswordHash = passwordHashedBytes;
            user.PasswordSalt = saltBytes;
            user.Otp = null;
            user.OtpExpiredTime = null;


            _unitofWork.SaveChangeAsync();

            return new ResponseDTO(AuthNotificationMessage.PasswordUpdate, StatusCodeEnum.Created, true);
        }

        /// <summary>
        /// Send otp code by email for user changes password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ResponseDTO ForgotPassword(string email)
        {
            var user = _unitofWork.User.FindAll(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.BadRequest, false);
            }

            if (!user.IsVerified)
            {
                return new ResponseDTO(AuthNotificationMessage.UserIsNotVerified, StatusCodeEnum.BadRequest, false);
            }

            var otp = _emailService.GenerateOTP();
            user.Otp = otp.OTPCode;

            user.Otp = otp.OTPCode;
            user.OtpExpiredTime = otp.ExpiredTime;
            _unitofWork.SaveChange();

            _emailService.SendOTPEmail(email, user.Username, otp.OTPCode, EmailSubject.ResetPassEmailSubject);

            return new ResponseDTO(EmailNotificationMessage.SendOTPEmailSuccessfully + email, StatusCodeEnum.Created, true, otp);
        }

        /// <summary>
        /// Verify Password Hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedHash"></param>
        /// <param name="salt"></param>
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

        /// <summary>
        /// Compare Byte Arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>

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
        /// Check User Exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public User CheckUserExist(string email)
        {
            var result = _unitofWork.User.FindAll(x => x.Email == email).FirstOrDefault();
            return result != null ? result : null;


        }


        /// <summary>
        /// Parse UserId from string to Guid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Guid ParseUserIdToGuid(string userId)
        {
            var checkUserID = Guid.TryParse(userId, out var guidUserID);
            if (checkUserID)
            {
                return guidUserID;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Get User By userId
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User? GetUserByUserID(Guid userID)
        {
            var user = _unitofWork.User.GetAll().FirstOrDefault(c => c.Id == userID);
            return user;
        }

        /// <summary>
        /// Change status of isVerify from false to true
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyUser(Guid userId)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == userId).FirstOrDefault();
            user.IsVerified = true;
            user.Otp = null;
            user.OtpExpiredTime = null;
            var update = _unitofWork.SaveChange();
            return update;

        }
        /// <summary>
        /// Check whether a user exists or not by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserExistByUserId(Guid userId)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether a user is verified exists or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserVerifiedStatus(Guid userId)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == userId).FirstOrDefault();
            if (user.IsVerified)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether otp is expired or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CheckOTPExpired(Guid userId)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == userId).FirstOrDefault();
            if (user?.OtpExpiredTime < DateTime.Now)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set value for OTP and Expired Time
        /// </summary>
        /// <param name="otpDto"></param>
        /// <param name="parseUserId"></param>
        /// <returns></returns>
        public bool SetOtp(OtpCodeDTO otpDto, Guid parseUserId)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == parseUserId).FirstOrDefault();
            if (user != null)
            {
                user.Otp = otpDto.OTPCode;
                user.OtpExpiredTime = otpDto.ExpiredTime;
                var result = _unitofWork.SaveChange();
                return result;
            }
            return false;
        }

        /// <summary>
        /// Check the inputted otp is the same as the otp which is in database
        /// </summary>
        /// <param name="parseUserId"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public bool CheckOTP(Guid parseUserId, string otpCode)
        {
            var user = _unitofWork.User.GetAll().Where(c => c.Id == parseUserId).FirstOrDefault();
            if (user.Otp == otpCode)
            {
                return true;
            }
            return false;
        }

        

       
        /// <summary>
        /// view profile 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ViewProfileDTO ViewProfile(Guid userId)
        {
            var user = _unitofWork.User.FindAll(u => u.Id == userId)
                .Include(u => u.IsAdmin)
              
                .FirstOrDefault();



            var viewProfileDto = new ViewProfileDTO
            {
                UserName = user.Username,
                FullName = user.FullName,
            
                Email = user.Email,
                BirthDay = user.Birthday.ToString("yyyy-MM-dd"),
                Gender = user.Gender
               
       
              
            };

            return viewProfileDto;
        }
        /// <summary>
        /// Edit Profile
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public ResponseDTO EditProfile(ProfileDTO userDto)
        {
            var user = _unitofWork.User.FindAll(u => u.Id.ToString() == userDto.UserId).FirstOrDefault();
            var saltBytes = GenerateSalt();
            var passwordHashedBytes = GenerateHashedPassword(userDto.Password, saltBytes);

            if (user == null)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.BadRequest, false);
            }

            bool isChanged = false;

            if (userDto.UserName != user.Username)
            {
                user.Username = userDto.UserName;
                isChanged = true;
            }
            if (userDto.Gender != user.Gender)
            {
                user.Gender = userDto.Gender;
                isChanged = true;
            }

            if (userDto.FullName != user.FullName)
            {
                user.FullName = userDto.FullName;
                isChanged = true;
            }

          

            if (userDto.Email != user.Email)
            {
                user.Email = userDto.Email;
                user.IsVerified = false;
                isChanged = true;
            }

            if (Convert.ToDateTime(userDto.Birthday) != user.Birthday)
            {
                user.Birthday = Convert.ToDateTime(userDto.Birthday);
                isChanged = true;
            }

            if (!passwordHashedBytes.Equals(user.PasswordHash.ToString()))
            {

                user.PasswordSalt = saltBytes;
                user.PasswordHash = passwordHashedBytes;
                isChanged = true;
            }


          

            if (isChanged)
            {
                _unitofWork.SaveChangeAsync();
                return new ResponseDTO(AuthNotificationMessage.UpdateProfileSuccessfully, StatusCodeEnum.OK, true);
            }
            else
            {
                return new ResponseDTO(AuthNotificationMessage.NothingChange, StatusCodeEnum.OK, true);
            }
        }
        /// <summary>
        /// Check validation EditProfile
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public ResponseDTO CheckValidationEditProfile(ProfileDTO userDto)
        {
            var userList = _unitofWork.User.GetAll().ToList();

            var checkNullUserId = _validationHandle.CheckNull(userDto.UserId);
            if (!checkNullUserId)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatUserName, StatusCodeEnum.BadRequest, false);
                return response;
            }
            var parseUserid = ParseUserIdToGuid(userDto.UserId);
            if (parseUserid == Guid.Empty)
            {
                return new ResponseDTO(ValidationErrorMessage.WrongFormatUserId, StatusCodeEnum.BadRequest, false);
            }
            var checkUserExist = CheckUserExistByUserId(parseUserid);
            if (!checkUserExist)
            {
                return new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.BadRequest, false);
            }


            var checkNullUserName = _validationHandle.CheckNull(userDto.UserName);

            if (!checkNullUserName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullUserName, StatusCodeEnum.BadRequest, false);
                return response;
            }


            var checkFormatUserName = _validationHandle.CheckFormatUserName(userDto.UserName);
            if (!checkFormatUserName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatUserName, StatusCodeEnum.BadRequest, false);
                return response;
            }


            var checkUserNameAlreadyExists = _validationHandle.CheckUserNameAlreadyExists(userDto.UserName, userList);
            if (!checkUserNameAlreadyExists)
            {
                var response = new ResponseDTO(ValidationErrorMessage.UserNameAlreadyExists, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullFullName = _validationHandle.CheckNull(userDto.FullName);
            if (!checkNullFullName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullFullName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatFullName = _validationHandle.CheckFormatFullName(userDto.FullName);
            if (!checkFormatFullName)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatFullName, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullPassword = _validationHandle.CheckNull(userDto.Password);
            if (!checkNullPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatPassword = _validationHandle.CheckFormatPassword(userDto.Password);
            if (!checkFormatPassword)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatPassword, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var email = _validationHandle.CheckEmailAlreadyExists(userDto.Email, userList);
            if (!email)
            {
                var response = new ResponseDTO(ValidationErrorMessage.EmailAlreadyExists, StatusCodeEnum.BadRequest, false);
                return response;
            }
            var checkNullPhoneNumber = _validationHandle.CheckNull(userDto.Phone);
            if (!checkNullPhoneNumber)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullPhoneNumber, StatusCodeEnum.BadRequest, false);
                return response;
            }

          

            var checkNullEmail = _validationHandle.CheckNull(userDto.Email);
            if (!checkNullEmail)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullEmail, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatEmail = _validationHandle.CheckFormatEmail(userDto.Email);
            if (!checkFormatEmail)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatEmail, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkEmailAlreadyExists = _validationHandle.CheckEmailAlreadyExists(userDto.Email, userList);
            if (!checkEmailAlreadyExists)
            {
                var response = new ResponseDTO(ValidationErrorMessage.EmailAlreadyExists, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkNullBirthday = _validationHandle.CheckNull(userDto.Birthday.ToString());
            if (!checkNullBirthday)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullBirthday, StatusCodeEnum.BadRequest, false);
                return response;
            }

            var checkFormatBirthday = _validationHandle.CheckFormatBirthday(userDto.Birthday.ToString());
            if (!checkFormatBirthday)
            {
                var response = new ResponseDTO(ValidationErrorMessage.WrongFormatBirthday, StatusCodeEnum.BadRequest, false);
                return response;
            }


            var checkNullAdress = _validationHandle.CheckNull(userDto.Address);
            if (!checkNullAdress)
            {
                var response = new ResponseDTO(ValidationErrorMessage.NullAddress, StatusCodeEnum.BadRequest, false);
                return response;
            }

          
            var successfulResponse = new ResponseDTO("Check Validation Successfully", StatusCodeEnum.OK, true);
            return successfulResponse;
        }

        /// <summary>
        /// GetRoleSeller
        /// </summary>
        /// <returns></returns>

        public async Task<UserResponseDTO?> GetUserResponseDtoByUserId(Guid userId)
        {
            var user = await _unitofWork.User
                .FindAll(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }
            var userlevel = await _unitofWork.UserLevel.FindAll(u => u.Id == userId).FirstOrDefaultAsync();
         
            var userResponseDTO = new UserResponseDTO
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Birthday = user.Birthday,
                Gender = user.Gender,
                IsPremium = user.IsPremium,
                PremiumExpiredTime = user.PremiumExpiredTime,
                IsVerified = user.IsVerified,
                LevelName = userlevel?.LevelName ?? "0", 
                Point = userlevel?.Point ?? 0



            };

            return userResponseDTO;
        }
        public async Task<User> GetUserById(Guid userId)
        {
            return await _unitofWork.User.GetByIdAsync(userId);
        }

        public async Task UpdateUser(User user)
        {
            await _unitofWork.User.UpdateAsync(user);
            await _unitofWork.SaveChangeAsync();
        }
    }
}
