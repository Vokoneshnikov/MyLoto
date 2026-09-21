using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Tickets;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Tickets;

public class BuyTicketCommandHandler : IRequestHandler<BuyTicketCommand, Result<TicketDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Draw> _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public BuyTicketCommandHandler(
        IUserRepository userRepository,
        IRepository<Draw> drawRepository,
        ILotteryRepository lotteryRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<TicketDto>> Handle(BuyTicketCommand request, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        // Бизнес-проверка 1: Существование пользователя
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return Result<TicketDto>.Failure(new Error(
                "User.NotFound",
                "Пользователь не найден"));
        }

        // Бизнес-проверка 2: Возрастное ограничение
        if (user.Age < 18)
        {
            return Result<TicketDto>.Failure(new Error(
                "User.AgeRestricted",
                "Покупка билетов доступна только пользователям старше 18 лет"));
        }

        // Бизнес-проверка 3: Существование тиража
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw is null)
        {
            return Result<TicketDto>.Failure(new Error(
                "Draw.NotFound",
                "Тираж не найден"));
        }

        // Бизнес-проверка 4: Статус тиража (открыты ли продажи)
        if (draw.Status != DrawStatus.Pending)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.PurchaseClosed",
                "Билеты можно покупать только до начала розыгрыша"));
        }

        // Бизнес-проверка 5: Доступность самой лотереи
        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery is null || lottery.IsPaused)
        {
            return Result<TicketDto>.Failure(new Error(
                "Lottery.Unavailable",
                "Лотерея не найдена или приостановлена"));
        }

        // Нормализуем числа перед проверкой дубликатов и сохранением
        var normalizedNumbers = request.ChosenNumbers
            .Distinct()
            .OrderBy(number => number)
            .ToList();

        // Бизнес-проверка 6: Уникальность комбинации в рамках этого тиража
        var isDuplicate = await _ticketRepository.ExistsWithNumbersAsync(
            request.DrawId,
            normalizedNumbers,
            ct);

        if (isDuplicate)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.DuplicateCombination",
                "Билет с такой комбинацией чисел уже зарегистрирован в этом тираже. Выберите другие числа."));
        }

        // Бизнес-проверка 7: Списание средств
        if (!user.SpendMoney(lottery.TicketPrice))
        {
            return Result<TicketDto>.Failure(new Error(
                "User.InsufficientFunds",
                "Недостаточно средств на балансе"));
        }

        // Регистрация билета
        var ticket = new Ticket
        {
            OwnerId = user.Id,
            DrawId = draw.Id,
            IsChecked = false,
            WinAmount = 0,
            SelectedNumbers = CreateTicketNumbers(lottery, normalizedNumbers)
        };

        await _ticketRepository.AddAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var dto = _mapper.Map<TicketDto>(ticket);

        return Result<TicketDto>.Success(dto);
    }

    private static List<TicketNumber> CreateTicketNumbers(Lottery lottery, List<int> normalizedNumbers)
    {
        return lottery switch
        {
            KOutOfNLottery => CreateKOutOfNTicketNumbers(normalizedNumbers),
            BingoLottery bingo => CreateBingoTicketNumbers(bingo, normalizedNumbers),
            _ => throw new InvalidOperationException("Неизвестный тип лотереи")
        };
    }

    private static List<TicketNumber> CreateKOutOfNTicketNumbers(List<int> numbers)
    {
        return numbers
            .Select((number, index) => new TicketNumber
            {
                Position = index + 1,
                Number = number,
                Row = null,
                Column = null
            })
            .ToList();
    }

    private static List<TicketNumber> CreateBingoTicketNumbers(BingoLottery lottery, List<int> numbers)
    {
        var ticketNumbers = new List<TicketNumber>();
        var index = 0;

        for (var row = 1; row <= lottery.Rows; row++)
        {
            for (var column = 1; column <= lottery.Columns; column++)
            {
                ticketNumbers.Add(new TicketNumber
                {
                    Position = index + 1,
                    Number = numbers[index],
                    Row = row,
                    Column = column
                });
                index++;
            }
        }

        return ticketNumbers;
    }
}