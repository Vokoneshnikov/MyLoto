using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common; // ИСПОЛЬЗУЕМ ТВОЙ NAMESPACE
using MyLoto.Application.Queries.Tickets;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Commands.Tickets;

public class BuyTicketCommandHandler : IRequestHandler<BuyTicketCommand, Result<TicketDto>>
{
    private readonly IUserRepository _userRepository;
    // Нам понадобится репозиторий тиражей, чтобы узнать, к какой лотерее он относится
    private readonly IRepository<Draw> _drawRepository; 
    private readonly ILotteryRepository _lotteryRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BuyTicketCommandHandler(
        IUserRepository userRepository,
        IRepository<Draw> drawRepository, // Внедряем базовый репозиторий тиражей
        ILotteryRepository lotteryRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TicketDto>> Handle(BuyTicketCommand request, CancellationToken ct)
    {
        // 1. Ищем пользователя
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null) 
            return Result<TicketDto>.Failure(new Error("User.NotFound", "Пользователь не найден"));

        // 2. Ищем тираж (чтобы получить ID лотереи и проверить статус тиража)
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null)
            return Result<TicketDto>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // 3. Ищем лотерею, чтобы узнать правила (цена, сколько чисел нужно выбрать)
        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null || lottery.IsPaused) 
            return Result<TicketDto>.Failure(new Error("Lottery.Unavailable", "Лотерея не найдена или приостановлена"));

        // 4. Валидация чисел
        if (request.ChosenNumbers.Distinct().Count() != lottery.K || 
            request.ChosenNumbers.Any(n => n < 1 || n > lottery.N))
        {
            return Result<TicketDto>.Failure(new Error("Ticket.InvalidNumbers", $"Нужно выбрать ровно {lottery.K} уникальных чисел от 1 до {lottery.N}"));
        }

        // 5. Списываем деньги
        if (!user.SpendMoney(lottery.TicketPrice))
            return Result<TicketDto>.Failure(new Error("User.InsufficientFunds", "Недостаточно средств на балансе"));

        var isDuplicate = await _ticketRepository.ExistsWithNumbersAsync(
            request.DrawId, 
            request.ChosenNumbers, 
            ct);

        if (isDuplicate)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.DuplicateCombination", 
                "Билет с такой комбинацией чисел уже зарегистрирован в этом тираже. Выберите другие числа."));
        }
        // 6. Создаем билет с учетом твоей сущности Ticket
        var ticket = new Ticket
        {
            OwnerId = user.Id,       // Используем OwnerId, как в твоей модели
            DrawId = draw.Id,        // Привязываем к тиражу
            IsChecked = false,
            WinAmount = 0,
            
            // Превращаем List<int> в ICollection<TicketNumber>
            SelectedNumbers = request.ChosenNumbers
                .Select(n => new TicketNumber { Number = n }) // <-- Проверь, как называется свойство в TicketNumber
                .ToList()
        };

        // 7. Сохраняем в базу
        // Если в твоем IRepository нет AddAsync, используй просто Add
        await _ticketRepository.AddAsync(ticket, ct);
        
        await _unitOfWork.SaveChangesAsync(ct);

        // 8. Возвращаем успешный результат
        var dto = _mapper.Map<TicketDto>(ticket);
        return Result<TicketDto>.Success(dto);
    }
}