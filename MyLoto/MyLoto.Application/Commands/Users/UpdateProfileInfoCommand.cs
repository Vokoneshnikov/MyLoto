using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Users;

public record UpdateProfileInfoCommand(
    long UserId, 
    string Name, 
    string Surname) : IRequest<Result<Unit>>;