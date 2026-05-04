using MyLoto.Domain.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class Transaction : BaseEntity
{
    public long UserId { get; init; }
    public User User { get; init; } = null!;
    
    public decimal Amount { get; init; }
    public TransactionType Type { get; init; } // Deposit, Withdraw, Purchase, Win
    
    public long? TicketId { get; init; } // Для покупок и выигрышей
    public string? ExternalTransactionId { get; init; } // ID из Stripe
    public string Description { get; init; } = null!;
}