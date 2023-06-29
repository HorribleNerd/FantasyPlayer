using Dalamud.Plugin;
using FantasyPlayer.Config;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Manager;

public class ConfigurationManager : IConfigurationManager

{
    private DalamudPluginInterface _pluginInterface;
    public ConfigurationManager(DalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;
    }
    
    public Configuration Config { get; set; }
    public string ConfigurationFile
    {
        get => _pluginInterface.ConfigFile.FullName;
        set { } //No setting the configuration file
    }

    public void Load()
    {
        Config = (Configuration)_pluginInterface.GetPluginConfig();
    }

    public void Save()
    {
        _pluginInterface.SavePluginConfig(Config);
    }
}