using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        IUserRepository userRepository, 
        IJwtProvider jwtProvider,
        IValidator<LoginCommand> validator)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _validator = validator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        // 1. Проверка валидации
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<AuthResponse>.Failure(new Error($"Validation.{firstError.PropertyName}", firstError.ErrorMessage));
        }

        // 2. Бизнес-логика
        var user = await _userRepository.GetByLoginAsync(request.Login);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidCredentials", "Неверный логин или пароль"));
        }

        var token = _jwtProvider.GenerateToken(user);
        
        return Result<AuthResponse>.Success(new AuthResponse(token, user.Login, user.Balance));
    }
}