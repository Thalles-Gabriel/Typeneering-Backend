using Typeneering.Application.UserPreferences.Contracts.Responses;

namespace Typeneering.Application.Users.Contracts.Responses;

public sealed record UserResponse(string Name,string? GhToken, IReadOnlyList<UserPreferenceResponse> UserPreferences);
