namespace Typeneering.Application.Users.Contracts.Requests;

public sealed record UserLoginRequest(string UserName, string Password);
