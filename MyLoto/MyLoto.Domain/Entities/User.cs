using MyLoto.Domain.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class User : BaseEntity
{
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public decimal Balance { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    // Навигационные свойства
    public ICollection<Ticket> OwnedTickets { get; set; } = new List<Ticket>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}