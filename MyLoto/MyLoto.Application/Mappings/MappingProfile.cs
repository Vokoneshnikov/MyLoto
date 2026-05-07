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
        CreateMap<Lottery, LotteryDto>();
        
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
            .ForMember(dest => dest.SalesEndTime, opt => opt.MapFrom(src => src.ScheduledStartTime)); // Или другое поле окончания продаж
    }
}