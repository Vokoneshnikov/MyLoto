using MediatR;
using Microsoft.Extensions.Configuration;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using Stripe;
using Stripe.Checkout;

namespace MyLoto.Application.Commands.Users;

public class ConfirmDepositCommandHandler : IRequestHandler<ConfirmDepositCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository; // Новый репозиторий
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _webhookSecret;

    public ConfirmDepositCommandHandler(
        IUserRepository userRepository, 
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork, 
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? throw new Exception("Webhook Secret is missing");
    }

    public async Task<Result<Unit>> Handle(ConfirmDepositCommand request, CancellationToken ct)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                request.JsonPayload,
                request.Signature,
                _webhookSecret,
                throwOnApiVersionMismatch: false
            );

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                var paymentIntentId = session?.PaymentIntentId;

                // 1. Идемпотентность: проверяем, не обрабатывали ли мы этот платеж ранее
                if (paymentIntentId != null && await _transactionRepository.ExistsByExternalIdAsync(paymentIntentId, ct))
                {
                    Console.WriteLine($"[SKIP] Платеж {paymentIntentId} уже был обработан.");
                    return Result<Unit>.Success(Unit.Value);
                }

                // 2. Извлекаем данные
                if (session?.Metadata != null && 
                    session.Metadata.TryGetValue("UserId", out var userIdStr) && 
                    long.TryParse(userIdStr, out var userId))
                {
                    var user = await _userRepository.GetByIdAsync(userId, ct);
                    if (user == null) return Result<Unit>.Failure(new Error("User.NotFound", "Пользователь не найден"));

                    var amount = (decimal)session.AmountTotal!.Value / 100;

                    // 3. Используем новый метод домена
                    user.AddDeposit(
                        amount, 
                        paymentIntentId ?? session.Id, 
                        $"Пополнение баланса через Stripe (Session: {session.Id})");
                    
                    // 4. Сохраняем всё одним транзакционным пакетом
                    await _unitOfWork.SaveChangesAsync(ct);
                    
                    Console.WriteLine($"[SUCCESS] Баланс юзера {userId} пополнен на {amount}. Транзакция создана.");
                }
            }

            return Result<Unit>.Success(Unit.Value);
        }
        catch (StripeException ex)
        {
            return Result<Unit>.Failure(new Error("Stripe.WebhookError", ex.Message));
        }
    }
}