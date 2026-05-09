using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws
{
    public class CheckPrizesCommand : IRequest<Result<Unit>>
    {
        public long DrawId { get; }

        public CheckPrizesCommand(long drawId)
        {
            DrawId = drawId;
        }
    }
}