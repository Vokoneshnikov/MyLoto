using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    // ЧИСТОТА: Убрали валидатор из конструктора
    public LoginCommandHandler(
        IUserRepository userRepository, 
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        // 1. Ищем пользователя по логину
        var user = await _userRepository.GetByLoginAsync(request.Login);

        // 2. Бизнес-чек: сверяем пароль (безопасно объединяем проверки, чтобы не выдавать, что именно неверно)
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(new Error(
                "Auth.InvalidCredentials", 
                "Неверный логин или пароль"));
        }

        // 3. Генерация токена доступа
        var token = _jwtProvider.GenerateToken(user);
        
        return Result<AuthResponse>.Success(new AuthResponse(
            token, 
            user.Login, 
            user.Balance, 
            user.Role.ToString()));
    }
}