using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Auth;

public record LoginCommand(string Login, string Password) : IRequest<Result<AuthResponse>>;