using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public class UpdateProfileInfoCommandHandler : IRequestHandler<UpdateProfileInfoCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileInfoCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(UpdateProfileInfoCommand request, CancellationToken ct)
    {
        // 1. Получаем пользователя из плоской таблицы
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        
        if (user == null) 
            return Result<Unit>.Failure(new Error("User.NotFound", "Пользователь не найден"));

        // 2. Вызываем метод обновления в самой сущности
        user.UpdateProfile(request.Name, request.Surname);

        // 3. Сохраняем изменения
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}