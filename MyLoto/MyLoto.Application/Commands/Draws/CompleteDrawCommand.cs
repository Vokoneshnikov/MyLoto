using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws
{
    public record CompleteDrawCommand(long DrawId) : IRequest<Result<Unit>>;
}