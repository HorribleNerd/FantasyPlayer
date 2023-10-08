using DalaMock.Shared.Interfaces;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace FantasyPlayer
{
    public class Service : IServiceContainer
    {
        [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static ICondition Condition { get; private set; } = null!;
        public static IPluginInterfaceService Interface { get; set; } = null!;
        public IPluginInterfaceService PluginInterfaceService
        {
            get
            {
                return Interface;
            }
            set
            {
                Interface = value;
            }
        }
    }
}
