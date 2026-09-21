using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Lotteries;

// Мы запрашиваем список DTO, упакованный в наш Result
public record GetActiveLotteriesQuery : IRequest<Result<IReadOnlyList<LotteryDto>>>;