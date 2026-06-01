using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class Transaction : BaseEntity
{
    public long UserId { get; init; }
    public User User { get; init; } = null!;
    
    public decimal Amount { get; init; }
    public TransactionType Type { get; init; }
    
    public long? TicketId { get; init; }
    public string? ExternalTransactionId { get; init; }
    public string Description { get; init; } = null!;
}