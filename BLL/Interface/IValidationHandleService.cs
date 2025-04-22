using DAL.Entities;

namespace BLL.Interface
{
    public interface IValidationHandleService
    {
        public bool CheckNull(string field);

        public bool CheckFormatUserName(string userName);
        public bool CheckNullToken(string token);
        public bool CheckFormatFullName(string fullName);

        public bool CheckFormatPassword(string password);

        public bool CheckConfirmPassword(string password, string confirmPassword);

        public bool CheckFormatEmail(string email);

 



        public bool CheckFormatBirthday(string birthday);

        public bool CheckUserNameAlreadyExists(string userName, List<User> userList);

        public bool CheckEmailAlreadyExists(string email, List<User> userList);

 

        public bool CheckGuidEmpty(Guid field);

        public bool CheckUserExists(Guid UserId, List<User> userList);
       



    }
}
