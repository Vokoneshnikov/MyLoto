using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Users;

public record GetUserProfileQuery() : IRequest<Result<UserProfileDto>>;