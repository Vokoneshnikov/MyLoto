using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws;

public record StartDrawCommand(long DrawId) : IRequest<Result<Unit>>;