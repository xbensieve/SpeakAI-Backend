using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Message.AuthMessage
{
    public static class AuthNotificationMessage
    {
        public const string LoginSuccessfully = "Login successfully";

        public const string LoginFailed = "Login failed";

        public const string SignUpSuccessfully = "Sign Up Successfully";

        public const string SignUpUnsuccessfully = "Sign Up Unsuccessfully";

        public const string LogOut = "Log Out Successfully";

        public const string LogOutFailed = "Log out error ";

        public const string PasswordUpdate = "Your password have been changed";

        public const string ResetPassword = "Failed when reset your password";

        public const string VerifySuccessfully = "Verify successfully";

        public const string VerifyUnsuccessfully = "Verify unsuccessfully";

        public const string UserIsVerified = "User is verified";

        public const string UserIsNotVerified = "User is not verified";

        public const string OtpInccorect = "OTP code is incorrect";

        public const string OtpExpired = "OTP code is expired";

        public const string SignUpForSellerSuccessfully = "Sign Up Successfully.Please wait for checking";

        public const string NoFileUpload = "No file is uploaded";

        public const string InValidFile = "Invalid file";

        public const string NotFound = "The image is not found";

        public const string GetUserByTokenSuccess = "Get user by access token successfully";

        public const string TokenExpired = "Fail because token is expired";

        public const string AccessTokenEmpty = "Access token is empty";

        public const string InvalidToken = "Token is invalid";

        public const string GetUserByTokenUnsuccess = "Get user by access token unsuccessfully";

        public const string UserNotFound = "User Not Found";

        public const string UpdateProfileSuccessfully = "Update Profile Successfully";

        public const string UpdateProfileFailed = "Update Profile Failed";

        public const string NothingChange = "No Made Change";

        public const string AddProductToCartSuccess = "Add product to cart successfully";

        public const string AddProductToCartFail = "Add product to cart fail";

        public const string ValidateUnSuccess = "Validate unsuccessfully";

        public const string UpdateProductSuccess = "Update quantity product of cart successfully";

        public const string UpdateProductFail = "Update quantity product of cart fail";

        public const string DeleteProductSuccess = "Delete product of cart successfully";

        public const string DeleteProductFail = "Delete product of cart fail";


    }
}
