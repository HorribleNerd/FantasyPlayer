﻿using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace FantasyPlayer.Config
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public PlayerSettings PlayerSettings { get; set; } = new PlayerSettings();
        public SpotifySettings SpotifySettings { get; set; } = new SpotifySettings();
        public AutoPlaySettings AutoPlaySettings { get; set; } = new AutoPlaySettings();

        public bool DisplayChatMessages;

        [NonSerialized] public bool ConfigShown;
    }
}