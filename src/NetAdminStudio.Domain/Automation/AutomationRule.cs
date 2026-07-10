using NetAdminStudio.Domain.Common;

namespace NetAdminStudio.Domain.Automation;

public sealed class AutomationRule : Entity
{
    public required string Name { get; set; }
    public required string TriggerType { get; set; }
    public required string ConditionJson { get; set; }
    public required string ActionType { get; set; }
    public required string ActionJson { get; set; }
    public bool Enabled { get; set; } = true;
}
