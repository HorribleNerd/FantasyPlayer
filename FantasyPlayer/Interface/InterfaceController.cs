using FantasyPlayer.Interface.Window;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Interface
{
    public class InterfaceController
    {
        private readonly PlayerWindow _player;
        private readonly SettingsWindow _settings;
        
        public InterfaceController(IPlugin plugin)
        {
            _player = new PlayerWindow(plugin);
            _settings = new SettingsWindow(plugin);
        }

        public void Draw()
        {
            _settings.WindowLoop();
            _player.WindowLoop();
        }

        public void Dispose()
        {
            _player.Dispose();
        }
        
    }
}