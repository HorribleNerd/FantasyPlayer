using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FantasyPlayer.Config;
using FantasyPlayer.Interface;
using FantasyPlayer.Manager;

namespace FantasyPlayer.Interfaces;

public interface IPlugin
{
    InterfaceController InterfaceController { get; set; }
    Configuration Configuration { get; }
    PlayerManager PlayerManager { get; }
    ICommandManager CommandManager { get; }
    IClientState ClientState { get; }
    IConditionService ConditionService { get; }
    IConfigurationManager ConfigurationManager { get; }

    public void DisplaySongTitle(string songTitle);
    public void DisplayMessage(string message);
}