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

    // ЧИСТОТА: Убрали валидатор из зависимостей
    public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken ct)
    {
        // 1. Бизнес-чек: проверяем уникальность логина
        var existingUser = await _userRepository.GetByLoginAsync(request.Login);
        if (existingUser != null)
        {
            return Result.Failure(new Error("Auth.DuplicateLogin", "Пользователь с таким логином уже существует"));
        }

        // 2. Хешируем пароль
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Создаем сущность
        var user = new User
        {
            Login = request.Login,
            PasswordHash = passwordHash,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            Balance = 0 // Новый пользователь всегда начинает с нулевым балансом
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}