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

    // Добавляем навигационное свойство для связи с UserExtraInfo
    public UserExtraInfo ExtraInfo { get; set; } = null!;
    
    public bool SpendMoney(decimal amount)
    {
        if (Balance < amount)
            return false;

        Balance -= amount;
        return true;
    }
    
    public void DepositMoney(decimal amount)
    {
        if (amount <= 0) return;
        Balance += amount;
    }
    public void AddDeposit(decimal amount, string externalId, string description)
    {
        if (amount <= 0) return;

        // 1. Обновляем баланс
        Balance += amount;

        // 2. Создаем транзакцию и добавляем её в коллекцию
        var transaction = new Transaction
        {
            UserId = this.Id,
            Amount = amount,
            Type = TransactionType.Deposit,
            ExternalTransactionId = externalId,
            Description = description,
            CreatedAt = DateTime.UtcNow // Хотя в BaseEntity есть значение по умолчанию
        };

        Transactions.Add(transaction);
    }
    
    public void UpdateProfile(string name, string surname)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя не может быть пустым");
        
        FirstName = name;
        LastName = surname;
    }
}