using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Commands.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        IValidator<RegisterCommand> validator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken ct)
    {
        // 1. Проверка валидации
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result.Failure(new Error($"Validation.{firstError.PropertyName}", firstError.ErrorMessage));
        }

        // 2. Бизнес-логика
        var existingUser = await _userRepository.GetByLoginAsync(request.Login);
        if (existingUser != null)
        {
            return Result.Failure(new Error("Auth.DuplicateLogin", "Пользователь с таким логином уже существует"));
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Login = request.Login,
            PasswordHash = passwordHash,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            Balance = 0
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}