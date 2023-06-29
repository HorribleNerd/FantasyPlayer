using Dalamud.Game.ClientState.Conditions;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Mock;

public class MockConditionService : IConditionService
{
    public bool this[ConditionFlag flag] => false;
}