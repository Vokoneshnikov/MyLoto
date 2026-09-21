namespace MyLoto.Domain.Entities;

public class WinningNumber
{
    public long DrawId { get; set; }
    public int Number { get; set; }
    public int Order { get; set; } 
    
    public Draw Draw { get; set; } = null!;
}