using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Users;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetUserProfileQuery> _validator; // Вставлен валидатор

    public GetUserProfileQueryHandler(IUserRepository userRepository, IMapper mapper, IValidator<GetUserProfileQuery> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<UserProfileDto>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        var user = await _userRepository.GetByIdWithTicketsAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure(new Error("User.NotFound", $"Пользователь с ID {request.UserId} не найден"));
        }

        // Создаем DTO, включая дополнительные данные из ExtraInfo
        var dto = _mapper.Map<UserProfileDto>(user);

        return Result<UserProfileDto>.Success(dto);
    }
}