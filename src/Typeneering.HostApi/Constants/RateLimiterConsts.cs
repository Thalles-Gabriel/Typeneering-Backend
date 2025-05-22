namespace Typeneering.HostApi.Constants;

public static class RateLimiterConsts
{
    public const string UserPolicy = "user";

    public const int FixedWindowPermitLimit = 6;
    public const int FixedWindowQueueLimit = 0;
    public const int FixedWindowSeconds = 8;

    public const int TokenBucketLimit = 20;
    public const int TokenBucketQueueLimit = 0;
    public const int TokenBucketReplenishmentSeconds = 2;
    public const int TokenBucketReplenishmentTokens = 4;

    public const string ErrorTitle = "Rate limiter was triggered";
    public const string ErrorDetail = "Please wait a few seconds until your next request";
}
