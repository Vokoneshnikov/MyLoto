using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Users;

public class GetUserProfileQueryHandler 
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Получаем пользователя. 
        // Важно: в репозитории метод должен подгружать коллекцию Tickets для счета статистики
        var user = await _userRepository.GetByIdWithTicketsAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserProfileDto>.Failure(new Error(
                "User.NotFound", 
                $"Пользователь с ID {request.UserId} не найден"));
        }

        // 2. Маппим сущность User в UserProfileDto
        var dto = _mapper.Map<UserProfileDto>(user);

        return Result<UserProfileDto>.Success(dto);
    }
}