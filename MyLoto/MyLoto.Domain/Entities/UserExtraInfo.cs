namespace MyLoto.Domain.Entities;

public class UserExtraInfo : BaseEntity
{
    public long UserId { get; set; } 
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    public User User { get; set; } = null!;
}