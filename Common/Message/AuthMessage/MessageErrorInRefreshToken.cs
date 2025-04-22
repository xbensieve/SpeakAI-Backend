namespace Common.Message.AuthMessage
{
    public static class MessageErrorInRefreshToken
    {
        public const string TokenNotExistInDB = "Token does not exist in database";

        public const string TokenInvalid = "Token is invalid by some factors";

        public const string ErrorInProcessing = "Having some error in processing";

        public const string AccessTokenHasNotExpired = "Access token has not yet expired";

        public const string RefreshTokenInvalid = "Refresh token is invalid not use please";

        public const string RefreshTokenExpired = "Refresh token has expired";

        public const string Successfully = "Refresh token successfully";

        public const string CommonError = "Error in refresh token";

    }
}
