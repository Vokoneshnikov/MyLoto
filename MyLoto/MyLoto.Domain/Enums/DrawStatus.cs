namespace MyLoto.Domain.Enums;

public enum DrawStatus
{
    /// <summary>
    /// Билеты продаются, розыгрыш еще не начался.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Розыгрыш идет в реальном времени (SignalR трансляция).
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Числа выпали, система рассчитывает победителей.
    /// </summary>
    Checking = 3,

    /// <summary>
    /// Розыгрыш завершен, призы начислены на балансы.
    /// </summary>
    Completed = 4
}