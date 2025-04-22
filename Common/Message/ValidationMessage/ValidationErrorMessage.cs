namespace Common.Message.ValidationMessage
{
    public static class ValidationErrorMessage
    {
        public const string NullUserName = "Please input username";

        public const string WrongFormatUserName = "Username must have at least 3 characters, at most 30 characters and not contain space";

        public const string UserNameAlreadyExists = "UserName already exists";

        public const string NullFullName = "Please input Full Name";

        public const string WrongFormatFullName = "The format of Full Name is incorrect";

        public const string NullPassword = "Please input password";

        public const string NullToken = "Please input token";

        public const string WrongFormatPassword = "Password must have at least 8 characters, at most 20 characters, at least 1 special character and not contain space";

        public const string WrongConfirmPassword = "Confirm password is not valid";

        public const string UserNotFound = "User not match in the system";

        public const string NullPhoneNumber = "Please input phone number";

        public const string WrongFormatPhoneNumber = "The format of phone number is incorrect";

        public const string PhoneNumberAlreadyExists = "Phone number already exists";

        public const string EmailAlreadyExists = "Email already exists";

        public const string NullEmail = "Please input email";

        public const string WrongFormatEmail = "The format of email is incorrect";

        public const string NullBirthday = "Please input birthday";

        public const string WrongFormatBirthday = "The format of birthday is incorrect";

        public const string NullGender = "Please select gender";

        public const string NullAddress = "Please input address";

        public const string WrongFormatAddress = "The format of address is incorrect";

        public const string NullDistrict = "Please select district";

        public const string NullCity = "Please select city";

        public const string WrongFormatUserId = "The format of user id is inccorect";

        public const string NullShopName = "Please input shop name";

        public const string WrongFormatShopName = "The format of shop name is incorrect";

        public const string ShopNameAlreadyExists = "Shop name already exists";

        public const string NullShippingChanel = "Please choose at least 1 shipping channel";

        public const string NullRegisterAddress = "Please input Register Address";

        public const string NullIdCardNumber = "Please input CCCD Number";

        public const string WrongFormatIdCard = "The format of number id card is incorrect ";

        public const string NullName = "Please input name";

        public const string WrongFormatName = "The format of name is incorrect ";

        public const string NullCateogry = "Category is null";

        public const string IdCartAldreadyExist = "Number id cart aldreadyExist";

        public const string FormatTax = "Tax is not format";

    }
}
