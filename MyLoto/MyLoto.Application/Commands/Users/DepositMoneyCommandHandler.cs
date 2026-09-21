using MediatR;
using Microsoft.Extensions.Configuration;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using Stripe;
using Stripe.Checkout;

namespace MyLoto.Application.Commands.Users;

public class DepositMoneyCommandHandler : IRequestHandler<DepositMoneyCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly string _stripeSecretKey;

    public DepositMoneyCommandHandler(
        IUserRepository userRepository,
        IUserContext userContext,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _stripeSecretKey = configuration["Stripe:SecretKey"] 
                           ?? throw new ArgumentNullException("Stripe Secret Key is missing");
        
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<Result<string>> Handle(DepositMoneyCommand request, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        // 1. Проверка существования пользователя
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            return Result<string>.Failure(new Error("User.NotFound", "Пользователь не найден"));
        }

        // 2. Создание сессии Stripe
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(request.Amount * 100), // Stripe работает в копейках
                        Currency = "rub",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Пополнение игрового баланса MyLoto",
                            Description = $"Для пользователя {user.Email}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            // Передаем UserId в Metadata, чтобы Webhook понимал, кому зачислять средства
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() }
            }
        };

        var service = new SessionService();
        try
        {
            Session session = await service.CreateAsync(options, cancellationToken: ct);
            
            // Баланс не меняем — ждем вебхука от Stripe
            return Result<string>.Success(session.Url);
        }
        catch (StripeException ex)
        {
            return Result<string>.Failure(new Error("Stripe.SessionError", ex.Message));
        }
    }
}