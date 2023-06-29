using Dalamud.Game.ClientState.Conditions;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Services;

public class ConditionService : IConditionService
{
    private Condition _condition;
    public ConditionService(Condition condition)
    {
        _condition = condition;
    }
    public bool this[ConditionFlag flag] => _condition[flag];
}