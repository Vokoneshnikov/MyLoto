using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public record DepositMoneyCommand(
    decimal Amount, 
    string SuccessUrl, 
    string CancelUrl) : IRequest<Result<string>>;