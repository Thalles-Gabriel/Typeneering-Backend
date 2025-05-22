namespace Typeneering.Application.Users.Contracts.Requests;

public sealed record PatchUserRequest(string? Username, string? Email, string? GithubToken);
