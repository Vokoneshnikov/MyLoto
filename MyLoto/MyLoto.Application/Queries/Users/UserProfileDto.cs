namespace MyLoto.Application.Queries.Users;

public record UserProfileDto
{
    public long Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string? Email { get; init; }
    public decimal Balance { get; init; }
    public int TotalTicketsCount { get; init; }
    public DateTime JoinedAt { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}