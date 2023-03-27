using System;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using FantasyPlayer.Dalamud.Config;
using FantasyPlayer.Dalamud.Interface;
using FantasyPlayer.Dalamud.Manager;
using CommandManager = FantasyPlayer.Dalamud.Manager.CommandManager;

namespace FantasyPlayer.Dalamud
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "FantasyPlayer";
        public const string Command = "/pfp";

        private InterfaceController InterfaceController { get; set; }
        public DalamudPluginInterface PluginInterface { get; private set; }
        public Configuration Configuration { get; set; }

        public PlayerManager PlayerManager { get; set; }
        public CommandManager CommandManager { get; set; }
        public RemoteManager RemoteConfigManager { get; set; }

        public string Version { get; private set; }

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
            PluginInterface.Create<Service>();

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(pluginInterface);
            
            RemoteConfigManager = new RemoteManager(this);
            var config = RemoteConfigManager.Config;

            Version =
                $"FP{VersionInfo.VersionNum}{VersionInfo.Type}_SP{Spotify.VersionInfo.VersionNum}{Spotify.VersionInfo.Type}_HX{config.ApiVersion}";

            Service.CommandManager.AddHandler(Command, new CommandInfo(OnCommand)
            {
                HelpMessage = "Run commands for Fantasy Player"
            });

            //Setup player
            PlayerManager = new PlayerManager(this);

            CommandManager = new CommandManager(pluginInterface, this);

            InterfaceController = new InterfaceController(this);

            PluginInterface.UiBuilder.Draw += InterfaceController.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;
        }

        private void OnCommand(string command, string arguments)
        {
            CommandManager.ParseCommand(arguments);
        }

        public void DisplayMessage(string message)
        {
            if (!Configuration.DisplayChatMessages)
                return;

            Service.ChatGui.Print(message);
        }

        public void OpenConfig()
        {
            Configuration.ConfigShown = true;
        }

        public void Dispose()
        {
            Service.CommandManager.RemoveHandler(Command);
            PluginInterface.UiBuilder.Draw -= InterfaceController.Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;

            InterfaceController.Dispose();
            PlayerManager.Dispose();
        }
    }
}