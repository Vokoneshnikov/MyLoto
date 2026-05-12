namespace MyLoto.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(Domain.Entities.User user);
}