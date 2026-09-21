using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Lotteries;

public record PrizeTierDto(
    PrizeTierRuleType RuleType,
    int ConditionValue,
    decimal RewardMultiplier
);