using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public record GetDrawByIdQuery(long DrawId) : IRequest<Result<DrawDto>>;