using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws;

public record GetDrawLiveStatusQuery(long DrawId) : IRequest<Result<DrawLiveStatusResponse>>;
