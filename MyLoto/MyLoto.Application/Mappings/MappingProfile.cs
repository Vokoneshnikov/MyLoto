using MyLoto.Application.Queries.Draws;
using MyLoto.Application.Queries.Tickets;
using MyLoto.Application.Queries.Users;

namespace MyLoto.Application.Mappings;

using AutoMapper;
using MyLoto.Application.Queries.Lotteries;
using MyLoto.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Lottery, LotteryDto>()
            .Include<KOutOfNLottery, LotteryDto>()
            .Include<BingoLottery, LotteryDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.NumbersToChoose, opt => opt.Ignore())
            .ForMember(dest => dest.MaxNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Rows, opt => opt.Ignore())
            .ForMember(dest => dest.Columns, opt => opt.Ignore())
            .ForMember(dest => dest.MaxBallValue, opt => opt.Ignore())
            .ForMember(dest => dest.JackpotThreshold, opt => opt.Ignore());

        CreateMap<KOutOfNLottery, LotteryDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.NumbersToChoose, opt => opt.MapFrom(src => src.NumbersToChoose))
            .ForMember(dest => dest.MaxNumber, opt => opt.MapFrom(src => src.MaxNumber))
            .ForMember(dest => dest.Rows, opt => opt.Ignore())
            .ForMember(dest => dest.Columns, opt => opt.Ignore())
            .ForMember(dest => dest.MaxBallValue, opt => opt.Ignore())
            .ForMember(dest => dest.JackpotThreshold, opt => opt.Ignore());

        CreateMap<BingoLottery, LotteryDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.NumbersToChoose, opt => opt.Ignore())
            .ForMember(dest => dest.MaxNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => src.Rows))
            .ForMember(dest => dest.Columns, opt => opt.MapFrom(src => src.Columns))
            .ForMember(dest => dest.MaxBallValue, opt => opt.MapFrom(src => src.MaxBallValue))
            .ForMember(dest => dest.JackpotThreshold, opt => opt.MapFrom(src => src.JackpotThreshold));
        
        CreateMap<Ticket, TicketDto>()
            // Извлекаем ID лотереи через навигационное свойство Draw
            .ForMember(dest => dest.LotteryId, opt => opt.MapFrom(src => src.Draw.LotteryId))
            // Превращаем коллекцию объектов TicketNumber в простой список int
            .ForMember(dest => dest.Numbers, opt => opt.MapFrom(src => 
                src.SelectedNumbers.Select(sn => sn.Number).ToList()))
            // Вычисляем статус выигрыша на лету
            .ForMember(dest => dest.IsWinning, opt => opt.MapFrom(src => src.WinAmount > 0));
        
        CreateMap<Ticket, UserTicketDto>()
            // Мапим ID билета
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.Id))
            // Достаем название лотереи через цепочку Draw -> Lottery
            .ForMember(dest => dest.LotteryName, opt => opt.MapFrom(src => src.Draw.Lottery.Name))
            // Превращаем коллекцию сущностей TicketNumber в простой список цифр
            .ForMember(dest => dest.ChosenNumbers, opt => opt.MapFrom(src => 
                src.SelectedNumbers.Select(sn => sn.Number).ToList()))
            // Определяем, проверен ли билет (например, по наличию даты проверки или статусу)
            .ForMember(dest => dest.IsChecked, opt => opt.MapFrom(src => src.IsChecked))
            // Дата покупки обычно берется из базовой сущности (CreatedAt)
            .ForMember(dest => dest.PurchasedAt, opt => opt.MapFrom(src => src.CreatedAt));
        
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.TotalTicketsCount, opt => opt.MapFrom(src => src.OwnedTickets.Count))
            .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.CreatedAt));
        
        CreateMap<Draw, DrawHistoryDto>()
            .ForMember(dest => dest.WinningNumbers, opt => opt.MapFrom(src => 
                src.WinningNumbers.Select(wn => wn.Number).ToList()))
            .ForMember(dest => dest.DrawDate, opt => opt.MapFrom(src => src.ScheduledStartTime))
            .ForMember(dest => dest.WinnersCount, opt => opt.MapFrom(src => 
                src.Tickets.Count(t => t.WinAmount > 0)));
        
        CreateMap<Draw, DrawDto>()
            .ForMember(dest => dest.LotteryName, opt => opt.MapFrom(src => src.Lottery.Name))
            .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.Lottery.TicketPrice))
            .ForMember(dest => dest.Jackpot, opt => opt.MapFrom(src => src.Lottery.AccumulatedJackpot))
            .ForMember(dest => dest.SalesEndTime, opt => opt.MapFrom(src => src.ScheduledStartTime)); 
        
        CreateMap<Draw, DrawDto>()
            .ForMember(dest => dest.LotteryName, opt => opt.MapFrom(src => src.Lottery.Name))
            .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.Lottery.TicketPrice))
            .ForMember(dest => dest.Jackpot, opt => opt.MapFrom(src => src.Lottery.AccumulatedJackpot))
            .ForMember(dest => dest.SalesEndTime, opt => opt.MapFrom(src => src.ScheduledStartTime))
    
            // Используем as и тернарный оператор (это деревья выражений понимают)
            .ForMember(dest => dest.LotteryType, opt => opt.MapFrom(src => 
                (src.Lottery as BingoLottery) != null ? "Bingo" : "KOutOfN"))

            // Мапим поля KOutOfN
            .ForMember(dest => dest.NumbersToChoose, opt => opt.MapFrom(src => 
                (src.Lottery as KOutOfNLottery) != null ? (int?)((KOutOfNLottery)src.Lottery).NumbersToChoose : null))
            .ForMember(dest => dest.MaxNumber, opt => opt.MapFrom(src => 
                (src.Lottery as KOutOfNLottery) != null ? (int?)((KOutOfNLottery)src.Lottery).MaxNumber : null))

            // Мапим поля Bingo
            .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => 
                (src.Lottery as BingoLottery) != null ? (int?)((BingoLottery)src.Lottery).Rows : null))
            .ForMember(dest => dest.Columns, opt => opt.MapFrom(src => 
                (src.Lottery as BingoLottery) != null ? (int?)((BingoLottery)src.Lottery).Columns : null))
            .ForMember(dest => dest.MaxBallValue, opt => opt.MapFrom(src => 
                (src.Lottery as BingoLottery) != null ? (int?)((BingoLottery)src.Lottery).MaxBallValue : null));
    }
}