﻿using System;

namespace FantasyPlayer.Spotify
{
    public static class VersionInfo
    {
#if DEBUG
        public static string Type = "DBG";
#else
        public static string Type = "REL";
#endif
        public static Version VersionNum = typeof(SpotifyState).Assembly.GetName().Version;
    }
}