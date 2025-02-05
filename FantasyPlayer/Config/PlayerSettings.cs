﻿using System.Numerics;
using Dalamud.Game.Text;

namespace FantasyPlayer.Config
{
    public class PlayerSettings
    {
        public Vector4 AccentColor = Interface.InterfaceUtils.FantasyPlayerColor;
        public float Transparency = 1f;
        
        public bool PlayerWindowShown = true;
        public string DefaultProvider;
        
        public bool CompactPlayer;
        public bool NoButtons;

        public bool FirstRunNone;
        public bool FirstRunCompactPlayer;
        public bool FirstRunSetNoButtons;
        
        public bool DisableInput;
        public bool PlayerLocked;
        public bool DebugWindowOpen;
        public bool ShowTimeElapsed;
        
        public bool OnlyOpenWhenLoggedIn = true;
        public XivChatType ChatType = XivChatType.Echo;

        public PlayerSettings()
        {
            
        }
    }
}