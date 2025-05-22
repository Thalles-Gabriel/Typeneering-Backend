namespace Typeneering.Application.Base.Constants;

public static class BaseConsts
{
    public static class Results
    {
        public const string RetrievedTitle = "Succesfully retrieved request items";

        public static string ItemsRetrieved(int itemCount)
            => $"A total of {itemCount} items were retrieved";
    }

    public static class Errors
    {
        public const string IdNotFoundTitle = "Incorrect Id";
        public const string IdNotFoundDetail = "No item was found with the provided Id";
        public const string IdNotFoundType = "ErrorIncorrectIdRequest";

        public const string InvalidRequestTitle = "Invalid request";
        public const string InvalidRequestDetail = "The request endpoint was sent an invalid object";
    }

    public static class Validations
    {
        public const string NegativeIntMessage = "Value cannot be negative";
        public const string FutureDateTimeMessage = "DateTime surpasses present";
        public const string LongStringMessage = "Text is too long";
    }
}
