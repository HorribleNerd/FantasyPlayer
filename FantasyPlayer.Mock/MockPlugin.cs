using DalaMock.Dalamud;
using DalaMock.Interfaces;
using DalaMock.Mock;
using Dalamud.Interface.ImGuiFileDialog;
using FantasyPlayer.Config;
using FantasyPlayer.Interface;
using FantasyPlayer.Interfaces;
using FantasyPlayer.Manager;

namespace FantasyPlayer.Mock;

public class MockPlugin : IMockPlugin, IDisposable, IPlugin
{
    public InterfaceController? InterfaceController { get; set; }
    public Configuration Configuration { get; set; }

    public PlayerManager PlayerManager { get; set; }
    public IConfigurationManager ConfigurationManager { get; }
    public CommandManagerFp CommandManager { get; set; }

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
    private MockWindow? _mockWindow;
    private bool _isStarted;

    public MockPlugin()
    {
        ConfigurationManager = new MockConfigurationManager();
    }

    
    public void Draw()
    {
        InterfaceController?.Draw();
        _mockWindow?.Draw();
    }


    public void Dispose()
    {
        
    }

    public bool IsStarted => _isStarted;

    public void Start(MockProgram program, MockService mockService, DalaMock.Dalamud.MockPluginInterfaceService mockPluginInterfaceService)
    {
        Service.Interface = mockPluginInterfaceService;
        CommandManager = new CommandManagerFp(this);
        ConfigurationManager.ConfigurationFile = mockPluginInterfaceService.ConfigFile.FullName;
        ConfigurationManager.Load();
        Configuration = ConfigurationManager.Config;
        _mockWindow = new MockWindow(this);
        PlayerManager = new PlayerManager(this);
        InterfaceController = new InterfaceController(this);
        _isStarted = true;
    }

    public void Stop(MockProgram program, MockService mockService, DalaMock.Dalamud.MockPluginInterfaceService mockPluginInterfaceService)
    {
        InterfaceController?.Dispose();
        PlayerManager.Dispose();
        InterfaceController = null;
        Service.Interface.Dispose();
        _isStarted = false;
    }
}