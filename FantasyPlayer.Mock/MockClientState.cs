using Dalamud;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace FantasyPlayer.Mock;

public class MockClientState : IClientState
{
    public ClientLanguage ClientLanguage { get; } = ClientLanguage.English;
    public ushort TerritoryType { get; } = 0;
    public PlayerCharacter? LocalPlayer { get; } = null;
    public ulong LocalContentId { get; } = 0;
    public bool IsLoggedIn { get; } = false;
    public bool IsPvP { get; } = false;
    public bool IsPvPExcludingDen { get; } = false;

    public bool IsGPosing => throw new NotImplementedException();

    public event EventHandler<ushort>? TerritoryChanged;
    public event EventHandler? Login;
    public event EventHandler? Logout;
    public event Action? EnterPvP;
    public event Action? LeavePvP;
    public event EventHandler<ContentFinderCondition>? CfPop;

    event Action<ushort> IClientState.TerritoryChanged
    {
        add
        {
            throw new NotImplementedException();
        }

        remove
        {
            throw new NotImplementedException();
        }
    }

    event Action IClientState.Login
    {
        add
        {
            throw new NotImplementedException();
        }

        remove
        {
            throw new NotImplementedException();
        }
    }

    event Action IClientState.Logout
    {
        add
        {
            throw new NotImplementedException();
        }

        remove
        {
            throw new NotImplementedException();
        }
    }

    event Action<ContentFinderCondition> IClientState.CfPop
    {
        add
        {
            throw new NotImplementedException();
        }

        remove
        {
            throw new NotImplementedException();
        }
    }
}