using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class AdminLog : BaseEntity
{
    public long AdminId { get; init; }
    public AdminAction Action { get; init; }
    public string EntityName { get; init; } = null!; 
    public long EntityId { get; init; }
    public string Details { get; init; } = null!; 
}