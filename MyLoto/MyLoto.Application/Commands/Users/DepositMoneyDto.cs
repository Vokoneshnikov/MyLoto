namespace MyLoto.Application.Commands.Users;

// Этот объект будет десериализован из Body
// В слое Application или Web
public record DepositMoneyDto(decimal Amount, string SuccessUrl, string CancelUrl);