using MediatR;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Lotteries; // Или где у тебя лежат DTO лотерей

namespace MyLoto.Application.Queries.Lotteries;

// Запрос возвращает список всех лотерей (List<LotteryDto>)
public record GetAllLotteriesQuery() : IRequest<Result<List<LotteryDto>>>;