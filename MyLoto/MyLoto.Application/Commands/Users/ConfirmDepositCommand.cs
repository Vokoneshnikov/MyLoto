using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;


// Команда теперь несет в себе "сырые" данные от Stripe
public record ConfirmDepositCommand(string JsonPayload, string Signature) : IRequest<Result<Unit>>;