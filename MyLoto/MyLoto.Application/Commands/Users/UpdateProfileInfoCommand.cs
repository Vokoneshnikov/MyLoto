using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public record UpdateProfileInfoCommand(string Name, string Surname, string? Address) : IRequest<Result<Unit>>;