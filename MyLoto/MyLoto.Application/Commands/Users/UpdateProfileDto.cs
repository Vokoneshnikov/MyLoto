namespace MyLoto.Application.Queries.Users;

public record UpdateProfileDto(
    string Name, 
    string Surname,
    string? Address);