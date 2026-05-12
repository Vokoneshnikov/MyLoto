using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Commands.Users;

public class UpdateProfileInfoCommandHandler : IRequestHandler<UpdateProfileInfoCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProfileInfoCommand> _validator;
    private readonly IUserContext _userContext;

    public UpdateProfileInfoCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateProfileInfoCommand> validator, IUserContext userContext)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _userContext = userContext;
    }

    public async Task<Result<Unit>> Handle(UpdateProfileInfoCommand request, CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<Unit>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage)); 
        }

        var user = await _userRepository.GetWithExtraInfoAsync(userId, ct);

        if (user == null)
        {
            return Result<Unit>.Failure(new Error("User.NotFound", "Пользователь не найден"));
        }

        // Обновляем имя и фамилию в сущности User
        user.UpdateProfile(request.Name, request.Surname);

        // Проверяем и создаем, если ExtraInfo еще не существует
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
            // Если он загрузился (не null), просто обновляем поле
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