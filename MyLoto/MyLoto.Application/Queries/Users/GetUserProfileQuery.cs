using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Users;

public record GetUserProfileQuery(long UserId) : IRequest<Result<UserProfileDto>>;