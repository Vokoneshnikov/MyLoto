using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Users;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetUserProfileQueryHandler(IUserRepository userRepository, IMapper mapper, IUserContext userContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        // Берем ID текущего авторизованного пользователя из контекста
        var currentUserId = _userContext.UserId;

        var user = await _userRepository.GetByIdWithTicketsAsync(currentUserId, ct);

        // Бизнес-чек: если профиля нет в БД, возвращаем доменную ошибку
        if (user == null)
            return Result<UserProfileDto>.Failure(new Error("User.NotFound", "Профиль не найден"));

        return Result<UserProfileDto>.Success(_mapper.Map<UserProfileDto>(user));
    }
}