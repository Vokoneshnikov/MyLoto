using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Auth;

public record RegisterCommand(
    string Login, 
    string Password, 
    string Email, 
    string FirstName, 
    string LastName, 
    int Age) : IRequest<Result>;