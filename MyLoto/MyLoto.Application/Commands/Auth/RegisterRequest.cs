namespace MyLoto.Application.Commands.Auth;

public record RegisterRequest(string Login, string Password, string Email, string FirstName, string LastName, int Age);