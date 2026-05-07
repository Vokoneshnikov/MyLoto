using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public record TopUpBalanceCommand(long UserId, decimal Amount) : IRequest<Result<decimal>>;