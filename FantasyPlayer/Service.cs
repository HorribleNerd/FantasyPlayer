using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Libc;
using Dalamud.Game.Network;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace FantasyPlayer
{
    public class Service
    {
        [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
        [PluginService] public static ClientState ClientState { get; private set; } = null!;
        [PluginService] public static CommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static Condition Condition { get; private set; } = null!;
    }
}
