using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Validators.Users;

namespace MyLoto.Application.Commands.Users;

public class TopUpBalanceCommandHandler : IRequestHandler<TopUpBalanceCommand, Result<decimal>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<TopUpBalanceCommand> _validator;
    private readonly IUserContext _userContext;

    public TopUpBalanceCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<TopUpBalanceCommand> validator,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _userContext = userContext;
    }

    public async Task<Result<decimal>> Handle(TopUpBalanceCommand request, CancellationToken ct)
    {
        var userId = _userContext.UserId;
        // Проверка валидации
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<decimal>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage)); 
        }

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            return Result<decimal>.Failure(new Error("User.NotFound", "Пользователь не найден"));
        }

        user.TopUpBalance(request.Amount);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<decimal>.Success(user.Balance);
    }
}