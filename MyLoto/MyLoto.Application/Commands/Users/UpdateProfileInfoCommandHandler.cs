using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Commands.Users;

public class UpdateProfileInfoCommandHandler : IRequestHandler<UpdateProfileInfoCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    // ЧИСТОТА: Валидатор исключен из зависимостей конструктора
    public UpdateProfileInfoCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<Unit>> Handle(UpdateProfileInfoCommand request, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        // Запрашиваем пользователя сразу с дополнительной информацией
        var user = await _userRepository.GetWithExtraInfoAsync(userId, ct);

        if (user == null)
        {
            return Result<Unit>.Failure(new Error("User.NotFound", "Пользователь не найден"));
        }

        // Обновляем имя и фамилию в сущности User через инкапсулированный метод домена
        user.UpdateProfile(request.Name, request.Surname);

        // Работа со связанной сущностью 1-к-1 (ExtraInfo)
        if (user.ExtraInfo == null)
        {
            user.ExtraInfo = new UserExtraInfo 
            { 
                UserId = user.Id,
                Address = request.Address 
            };
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                user.ExtraInfo.Address = request.Address;
            }
        }

        Console.WriteLine($"User ID: {user.Id}, FirstName: {user.FirstName}, LastName: {user.LastName}, Address: {user.ExtraInfo?.Address}");
        
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}