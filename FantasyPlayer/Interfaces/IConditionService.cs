using Dalamud.Game.ClientState.Conditions;

namespace FantasyPlayer.Interfaces;

public interface IConditionService
{
    public bool this[ConditionFlag flag] { get; }
}