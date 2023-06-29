using FantasyPlayer.Config;

namespace FantasyPlayer.Interfaces;

public interface IConfigurationManager
{
    public Configuration Config { get; set; }
    public string ConfigurationFile { get; set; }
    public void Load();
    public void Save();
}