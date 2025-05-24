namespace Typeneering.Application.Users.Contracts.Requests;

public sealed record UserRegisterRequest(string Username, string? Email, string Password, string? PreferredUsername);
