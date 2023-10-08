using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Services;

public class ConditionService : IConditionService
{
    private ICondition _condition;
    public ConditionService(ICondition condition)
    {
        _condition = condition;
    }
    public bool this[ConditionFlag flag] => _condition[flag];
}