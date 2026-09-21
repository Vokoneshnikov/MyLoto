using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class KOutOfNLottery : Lottery
{
    public KOutOfNLottery()
    {
        Type = LotteryType.K_Out_Of_N;
    }
    
    public int NumbersToChoose { get; set; }
    
    public int MaxNumber { get; set; }
}