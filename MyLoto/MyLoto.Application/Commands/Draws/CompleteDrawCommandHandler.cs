using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;


public class CompleteDrawCommandHandler : IRequestHandler<CompleteDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteDrawCommandHandler(IDrawRepository drawRepository, IUnitOfWork unitOfWork)
    {
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(CompleteDrawCommand request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // Тираж полностью завершен
        draw.Status = DrawStatus.Completed;

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}