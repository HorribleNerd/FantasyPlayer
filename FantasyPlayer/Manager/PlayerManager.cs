using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Plugin;
using FantasyPlayer.Interface;
using FantasyPlayer.Interfaces;
using FantasyPlayer.Provider;
using FantasyPlayer.Provider.Common;

namespace FantasyPlayer.Manager
{
    public class PlayerManager
    {
        private readonly IPlugin _plugin;
        public Dictionary<Type, IPlayerProvider> PlayerProviders;

        public IPlayerProvider CurrentPlayerProvider;

        public PlayerManager(IPlugin plugin)
        {
            _plugin = plugin;
            ResetProviders();
            InitializeProviders();
        }

        public void ReloadProviders()
        {
            DisposeProviders();
            ResetProviders();
            InitializeProviders();
        }

        private void ResetProviders()
        {
            CurrentPlayerProvider = default;
            PlayerProviders = new Dictionary<Type, IPlayerProvider>();
        }

        private void InitializeProviders()
        {
            var ppType = typeof(IPlayerProvider);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var interfaces = new List<Type> { };
            for (int i = 0; i < assemblies.Length; i++)
            {
                var potentiallyBad = assemblies[i];
                try
                {
                    interfaces.AddRange(potentiallyBad
                        .GetTypes()
                        .Where(type => ppType.IsAssignableFrom(type) && !type.IsInterface));
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    PluginLog.LogError(rtle, rtle.Message, rtle.LoaderExceptions);
                    PluginLog.LogError($"Error loading Assembly while searching for PlayerProviders: \"{potentiallyBad.FullName}\"");
                }
            }

            List<Task<IPlayerProvider>> providerTasks = new List<Task<IPlayerProvider>>();
            foreach (var playerProvider in interfaces)
            {
                PluginLog.Log("Found provider: " + playerProvider.FullName);
                providerTasks.Add(InitializeProvider(playerProvider,  (IPlayerProvider)Activator.CreateInstance(playerProvider)));
            }

            Task.WhenAll(providerTasks).ContinueWith(task =>
            {
                foreach (var playerProvider in task.Result)
                {
                    PlayerProviders.Add(playerProvider.GetType(), playerProvider);

                    if (_plugin.Configuration.PlayerSettings.DefaultProvider == playerProvider.GetType().FullName)
                        CurrentPlayerProvider ??= playerProvider;
                }
            });
        }

        private Task<IPlayerProvider> InitializeProvider(Type type, IPlayerProvider playerProvider)
        {
            return playerProvider.Initialize(_plugin);
        }

        private void Update()
        {
            foreach (var playerProvider in PlayerProviders)
                playerProvider.Value.Update();
        }

        private void DisposeProviders()
        {
            foreach (var playerProvider in PlayerProviders)
            {
                playerProvider.Value.Dispose();
            }
        }

        public void Dispose()
        {
            DisposeProviders();
        }
    }
}