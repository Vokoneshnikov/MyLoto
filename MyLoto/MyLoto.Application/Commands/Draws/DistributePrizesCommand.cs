using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws;

public record DistributePrizesCommand(long DrawId) : IRequest<Result<Unit>>;