namespace Typeneering.Application.Sessions.Constants;

public static class SessionConsts
{
    public static class Leaderboard
    {
        public static class Results
        {
            public const string RetrievedLeaderboard = "Leaderboard has been successfully retrieved";

        }
    }

    public static class Session
    {
        public static class Errors
        {
            public const string CreatedTitle = "Error has ocurred on inserting session";
            public const string CreatedDetail =  "Failed to save the session to the database";
            public const string CreatedType =  "ErrorInsertDbSession";
        }
    }
}
