using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class BingoLottery : Lottery
{
    public BingoLottery()
    {
        Type = LotteryType.Bingo;
    }
    public int Rows { get; set; }
    
    public int Columns { get; set; }
    
    public int MaxBallValue { get; set; }
    
    public int JackpotThreshold { get; set; }
    
    public int TicketNumbersCount => Rows * Columns;
}