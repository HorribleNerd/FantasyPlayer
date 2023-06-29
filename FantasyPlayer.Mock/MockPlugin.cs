using System.Diagnostics;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FantasyPlayer.Config;
using FantasyPlayer.Interface;
using FantasyPlayer.Interfaces;
using FantasyPlayer.Manager;
using ImGuiNET;
using Lumina;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Veldrid;

namespace FantasyPlayer.Mock;

public class MockPlugin : IDisposable, IPlugin
{
    public InterfaceController InterfaceController { get; set; }
    public Configuration Configuration { get; set; }

    public PlayerManager PlayerManager { get; set; }
    public ICommandManagerFP CommandManager { get; set; }
    
    public IClientState ClientState { get; set; }
    public IConditionService ConditionService { get; }
    public IConfigurationManager ConfigurationManager { get; }

    public void DisplaySongTitle(string songTitle)
    {
        Console.WriteLine("Now playing: " + songTitle);
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine("Displaying message: " + message);
    }

    private MockPluginInterfaceService _mockPluginInterfaceService;
    private FileDialogManager _fileDialogManager;
    private MockWindow _mockWindow;

    public MockPlugin(string configPath)
    {
        ConfigurationManager = new MockConfigurationManager();
        ConfigurationManager.ConfigurationFile = configPath;
        ConfigurationManager.Load();
        Configuration = ConfigurationManager.Config;
        ClientState = new MockClientState();
        ConditionService = new MockConditionService();
        _mockWindow = new MockWindow(this);
        PlayerManager = new PlayerManager(this);

        CommandManager = new MockCommandManagerFp();

        InterfaceController = new InterfaceController(this);
    }

    
    public void Draw()
    {
        InterfaceController.Draw();
        _mockWindow.Draw();
    }


    public void Dispose()
    {
        
    }
}