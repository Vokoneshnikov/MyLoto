using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public class TopUpBalanceCommandHandler : IRequestHandler<TopUpBalanceCommand, Result<decimal>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TopUpBalanceCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<decimal>> Handle(TopUpBalanceCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null) return Result<decimal>.Failure(new Error("User.NotFound", "Пользователь не найден"));

        user.TopUpBalance(request.Amount);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<decimal>.Success(user.Balance);
    }
}