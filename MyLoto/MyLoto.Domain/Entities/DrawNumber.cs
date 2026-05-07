namespace MyLoto.Domain.Entities;

public class DrawNumber
{
    public long DrawId { get; set; }
    public int Number { get; set; }
    public int Position { get; set; }
    
    // Навигационное свойство (опционально)
    public Draw Draw { get; set; } = null!;
}