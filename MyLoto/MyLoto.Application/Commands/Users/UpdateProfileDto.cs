namespace MyLoto.Application.Commands.Users;

public record UpdateProfileDto(
    string Name, 
    string Surname,
    string? Address);