using BLL.Interface;

using DAL.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace BLL.Services
{
    public class ValidationHandleService : IValidationHandleService
    {
        /// <summary>
        /// Check null of a inputted field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool CheckNull(string field)
        {
            var check = true;
            if (field == null)
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckFormatUserName(string userName)
        {
            var check = true;
            if (userName.Length < 3 || userName.Length > 30 ||
                userName.Contains(' '))
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted full name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public bool CheckFormatFullName(string fullName)
        {
            var check = true;
            if (fullName.Length > 100)
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckFormatPassword(string password)
        {
            var check = true;
            Regex specialCharRegex = new Regex("[^a-zA-Z0-9]");
            if (password.Length < 8 || password.Length > 20 ||
                password.Contains(' ') ||
                !specialCharRegex.IsMatch(password))
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check password and confirm password is the same or not
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        public bool CheckConfirmPassword(string password, string confirmPassword)
        {
            var check = true;
            if (password != confirmPassword)
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckFormatEmail(string email)
        {
            var check = true;
            string pattern = @"^[a-zA-Z0-9_.+-]+@gmail\.com$";
            if (!Regex.IsMatch(email, pattern))
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public bool CheckFormatPhoneNumber(string phoneNumber)
        {
            var check = true;
            string pattern = @"^0\d{9}$";
            if (!Regex.IsMatch(phoneNumber, pattern))
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool CheckFormatAddress(string address)
        {
            var check = true;
            if (address.Length > 200)
            {
                check = false;
                return check;
            }
            return check;
        }

        /// <summary>
        /// Check format of a inputted birthday
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public bool CheckFormatBirthday(string birthday)
        {
            string pattern = @"^\d{4}/(0[1-9]|1[0-2])/(0[1-9]|[12]\d|3[01])$|^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$";
            var check = Regex.IsMatch(birthday, pattern);
            return check;
        }

        /// <summary>
        /// Check if username is already exists or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userList"></param>
        /// <returns></returns>
        public bool CheckUserNameAlreadyExists(string userName, List<User> userList)
        {
            var check = !(userList.Any(user => user.Username == userName));
            return check;
        }

        /// <summary>
        /// Check if email is already exists or not
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userList"></param>
        /// <returns></returns>
        public bool CheckEmailAlreadyExists(string email, List<User> userList)
        {
            var check = !(userList.Any(user => user.Email == email));
            return check;
        }

     
        /// <summary>
        /// Check null of a inputted field
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        public bool CheckNullToken(string token)
        {
            var check = true;
            if (token == "")
            {
                check = false;
                return check;
            }
            return check;
        }
        /// <summary>
        /// Check ShopName Already Exists
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="userList"></param>
        /// <returns></returns>

      
        
        public bool CheckGuidEmpty(Guid field)
        {
            if (field == Guid.Empty )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckQuantityEmpty(int quantity)
        {
            if (quantity is int && quantity <= 0)
            {
                return false;
            }
            return true;
        }


       
        public bool CheckUserExists(Guid userId, List<User> userList)
        => (userList.Any(a => a.Id == userId));

      
    }
}
