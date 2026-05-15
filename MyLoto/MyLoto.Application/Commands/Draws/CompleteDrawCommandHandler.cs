using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Enums;
using Hangfire;
using MyLoto.Application.Common.BackgroundJobs;

namespace MyLoto.Application.Commands.Draws;

public class CompleteDrawCommandHandler : IRequestHandler<CompleteDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteDrawCommandHandler(
        IDrawRepository drawRepository, 
        ILotteryRepository lotteryRepository,
        IUnitOfWork unitOfWork)
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(CompleteDrawCommand request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // Тираж полностью завершен
        draw.Status = DrawStatus.Completed;

        await _unitOfWork.SaveChangesAsync(ct);

        // Замыкаем цикл: проверяем лотерею и ставим задачу на создание нового тиража
        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        
        if (lottery != null && !lottery.IsPaused)
        {
            BackgroundJob.Enqueue<DrawJobsManager>(x => x.TriggerCreateDraw(lottery.Id));
        }

        return Result<Unit>.Success(Unit.Value);
    }
}