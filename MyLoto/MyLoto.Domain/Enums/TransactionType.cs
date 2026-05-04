namespace MyLoto.Domain.Enums;

public enum TransactionType
{
    /// <summary>
    /// Пополнение через Stripe.
    /// </summary>
    Deposit = 1,

    /// <summary>
    /// Вывод средств из системы.
    /// </summary>
    Withdraw = 2,

    /// <summary>
    /// Списание средств за покупку билета.
    /// </summary>
    Purchase = 3,

    /// <summary>
    /// Начисление выигрыша по билету.
    /// </summary>
    Win = 4
}