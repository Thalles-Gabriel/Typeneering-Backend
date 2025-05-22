namespace Typeneering.Application.UserPreferences.Contracts.Requests;

public sealed record PostUserPreferenceRequest(int PreferenceId, string Value);
