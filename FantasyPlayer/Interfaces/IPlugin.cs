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
    IConfigurationManager ConfigurationManager { get; }
    CommandManagerFp CommandManager { get; set; }

    public void DisplaySongTitle(string songTitle);
    public void DisplayMessage(string message);
}