using Hangfire;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Commands.Draws;
using MyLoto.Domain.Enums;

namespace MyLoto.Application
{
    public class HangfireJobs
    {
        private readonly IDrawRepository _drawRepository;
        private readonly IMediator _mediator;

        public HangfireJobs(IDrawRepository drawRepository, IMediator mediator)
        {
            _drawRepository = drawRepository;
            _mediator = mediator;
        }

        // Настроим задачу на выполнение через Hangfire (здесь проверяем один розыгрыш)
        public void ConfigureJobs(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate("check-prizes", () => CheckPrizesForSingleDraw(1), Cron.Daily(3));  // Пример с DrawId = 1
        }

        // Метод для проверки выигрыша для одного тиража
        public async Task CheckPrizesForSingleDraw(long drawId)
        {
            var draw = await _drawRepository.GetByIdAsync(drawId);  // Получаем конкретный тираж по ID

            if (draw == null)
            {
                // Тираж не найден
                return;
            }

            if (draw.Status == DrawStatus.Checking)
            {
                // Если статус тиража "Checking", то проверяем выигрыш
                await _mediator.Send(new CheckPrizesCommand(draw.Id));
            }
        }
    }
}