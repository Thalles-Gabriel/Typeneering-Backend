namespace Typeneering.Application.Users.Constants;

public static class UserConsts
{
    public static class Errors
    {
        public static class Login
        {
            public const string LoginTitle = "Error validating user login";
            public const string LoginDetail = "Login or password incorrect";
            public const string LoginType = "ErrorLoginPasswordInvalid";

            public const string RegisterTitle = "Registering user was unsuccessful";
            public const string RegisterDetails = "Validation issues have ocurred when registering a new user";
            public const string RegisterType = "ErrorRegisterInvalid";
        }

        public static class Token
        {
            public const string InvalidTokenTitle = "Invalid access token/refresh token";
            public const string InvalidTokenDetail = "The token provided was invalid";
            public const string InvalidTokenType = "ErrorTokenInvalid";

            public const string InvalidTokenInformationTitle = "Invalid access token/refresh token";
            public const string InvalidaTokenInformationDetail = "The token received has invalid information";
            public const string InvalidTokenInformationType = "ErrorTokenInvalidInformation";
        }

        public static class User
        {
            public const string UpdateTitle = "Error updating user";
            public const string NullDetail = "User was not found";
            public const string NullType = "ErrorUserNull";
            public const string UpdateDetail = "User values were invalid";
            public const string UpdateType = "ErrorUserUpdateInvalid";
            public const string UpdateIdType = "ErrorUserIdUpdateInvalid";


            public const string DeleteTitle = "Error deleting user";
            public const string DeleteDetail = "Errors have ocurred when deleting user";
            public const string DeleteType = "ErrorDeleteUser";
        }
    }

    public static class Results
    {
        public static class User
        {
            public const string CreatedTitle = "User registered";
            public const string CreatedMessage = "User was succesfully registered";

            public const string UpdateTitle = "User was updated";
            public const string UpdateDetail = "User was sucessfully updated";

            public const string DeleteTitle = "User was deleted";
            public const string DeleteDetail = "User was deleted successfully";
        }
    }
}
