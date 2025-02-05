﻿using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using FantasyPlayer.Config;
using FantasyPlayer.Interfaces;
using FantasyPlayer.Manager;
using FantasyPlayer.Provider;
using FantasyPlayer.Provider.Common;
using FantasyPlayer.Spotify;
using ImGuiNET;
using OtterGui;
using Swan;

namespace FantasyPlayer.Interface.Window
{
    public class PlayerWindow
    {
        private readonly IPlugin _plugin;
        private readonly PlayerManager _playerManager;

        private DateTime? _lastUpdated;
        private DateTime? _lastPaused;
        private TimeSpan _difference;
        private int _progressMs;
        private string _lastId;
        private bool _lastBoundByDuty;

        private readonly Vector2 _playerWindowSize = new Vector2(401 * ImGui.GetIO().FontGlobalScale,
            89 * ImGui.GetIO().FontGlobalScale);

        private readonly Vector2 _windowSizeNoButtons = new Vector2(401 * ImGui.GetIO().FontGlobalScale,
            62 * ImGui.GetIO().FontGlobalScale);

        private readonly Vector2 _windowSizeCompact = new Vector2(179 * ImGui.GetIO().FontGlobalScale,
            39 * ImGui.GetIO().FontGlobalScale);


        public PlayerWindow(IPlugin plugin)
        {
            _plugin = plugin;
            _playerManager = _plugin.PlayerManager;

            var cmdManager = _plugin.CommandManager;

            cmdManager.Commands.Add("display",
                (OptionType.Boolean, new string[] { }, "Toggle player display.", OnDisplayCommand));
        }

        private void CheckProvider(IPlayerProvider playerProvider)
        {
            if (playerProvider.PlayerState.ServiceName == null) return;
            if (playerProvider.PlayerState.ServiceName == _lastId) return;

            _lastId = playerProvider.PlayerState.ServiceName;
            //TODO: Add and remove command handlers based on provider settings, those need to be added too

            var cmdManager = _plugin.CommandManager;

            cmdManager.Commands.Remove("shuffle");
            cmdManager.Commands.Remove("next");
            cmdManager.Commands.Remove("back");
            cmdManager.Commands.Remove("pause");
            cmdManager.Commands.Remove("play");
            cmdManager.Commands.Remove("volume");
            cmdManager.Commands.Remove("relogin");
            cmdManager.Commands.Remove("playlist");

            cmdManager.Commands.Add("shuffle",
                (OptionType.Boolean, new string[] { }, "Toggle shuffle.", OnShuffleCommand));
            cmdManager.Commands.Add("next",
                (OptionType.None, new string[] {"skip"}, "Skip to the next track.", OnNextCommand));
            cmdManager.Commands.Add("back",
                (OptionType.None, new string[] {"previous"}, "Go back a track.", OnBackCommand));
            cmdManager.Commands.Add("pause",
                (OptionType.None, new string[] {"stop"}, "Pause playback.", OnPauseCommand));
            cmdManager.Commands.Add("play",
                (OptionType.None, new string[] { }, "Continue playback.", OnPlayCommand));
            cmdManager.Commands.Add("volume",
                (OptionType.Int, new string[] { }, "Set playback volume.", OnVolumeCommand));
            cmdManager.Commands.Add("playlist",
                (OptionType.String, new string[] { }, "Play a playlist.", OnPlaylistCommand));
            cmdManager.Commands.Add("relogin",
                (OptionType.None, new string[] {"reauth"}, "Re-opens the login window and lets you login again",
                    OnReLoginCommand));
        }

        private void CheckClientState()
        {
            var isBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
            if (_plugin.Configuration.AutoPlaySettings.PlayInDuty && isBoundByDuty &&
                !_playerManager.CurrentPlayerProvider.PlayerState.IsPlaying)
            {
                if (_lastBoundByDuty == false)
                {
                    _lastBoundByDuty = true;
                    _playerManager.CurrentPlayerProvider.SetPauseOrPlay(true);
                }
            }

            _lastBoundByDuty = isBoundByDuty;
        }

        public void WindowLoop()
        {
            if (_plugin.Configuration.PlayerSettings.OnlyOpenWhenLoggedIn &&
                Service.ClientState.LocalContentId == 0)
                return; //Do nothing

            if (_playerManager.CurrentPlayerProvider == null &&
                _plugin.Configuration.PlayerSettings.PlayerWindowShown)
                WelcomeWindow();

            if (_playerManager.CurrentPlayerProvider != null && 
                !_playerManager.ProvidersLoading &&
                _playerManager.CurrentPlayerProvider.PlayerState.RequiresLogin &&
                _plugin.Configuration.PlayerSettings.PlayerWindowShown &&
                !_playerManager.CurrentPlayerProvider.PlayerState.IsLoggedIn)
                LoginWindow(_playerManager.CurrentPlayerProvider);

            if (_playerManager.CurrentPlayerProvider != null &&
                !_playerManager.ProvidersLoading &&
                _plugin.Configuration.PlayerSettings.DebugWindowOpen)
                DebugWindow(_playerManager.CurrentPlayerProvider.PlayerState);

            if (_playerManager.CurrentPlayerProvider != null &&
                !_playerManager.ProvidersLoading &&
                _playerManager.CurrentPlayerProvider.PlayerState.IsLoggedIn &&
                _plugin.Configuration.PlayerSettings.PlayerWindowShown)
            {
                CheckProvider(_playerManager.CurrentPlayerProvider);
                MainWindow(_playerManager.CurrentPlayerProvider.PlayerState, _playerManager.CurrentPlayerProvider);
                CheckClientState();
            }
        }

        private void SetDefaultWindowSize(PlayerSettings playerSettings)
        {
            if (playerSettings.FirstRunNone)
            {
                ImGui.SetNextWindowSize(_playerWindowSize);
                _plugin.Configuration.PlayerSettings.FirstRunNone = false;
                _plugin.ConfigurationManager.Save();
            }

            if (playerSettings.CompactPlayer && playerSettings.FirstRunCompactPlayer)
            {
                ImGui.SetNextWindowSize(_windowSizeCompact);
                _plugin.Configuration.PlayerSettings.FirstRunCompactPlayer = false;
                _plugin.ConfigurationManager.Save();
            }

            if (playerSettings.NoButtons && playerSettings.FirstRunSetNoButtons)
            {
                ImGui.SetNextWindowSize(_windowSizeNoButtons);
                _plugin.Configuration.PlayerSettings.FirstRunSetNoButtons = false;
                _plugin.ConfigurationManager.Save();
            }

            if (_plugin.Configuration.SpotifySettings.LimitedAccess && playerSettings.FirstRunCompactPlayer)
            {
                ImGui.SetNextWindowSize(_windowSizeNoButtons);
                _plugin.Configuration.PlayerSettings.FirstRunCompactPlayer = false;
                _plugin.ConfigurationManager.Save();
            }
        }

        private void MainWindow(PlayerStateStruct playerState, IPlayerProvider currentProvider)
        {
            ImGui.SetNextWindowBgAlpha(_plugin.Configuration.PlayerSettings.Transparency);
            SetDefaultWindowSize(_plugin.Configuration.PlayerSettings);


            var lockFlags = (_plugin.Configuration.PlayerSettings.PlayerLocked)
                ? ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize
                : ImGuiWindowFlags.None;

            var clickThroughFlags = (_plugin.Configuration.PlayerSettings.DisableInput)
                ? ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoResize
                : ImGuiWindowFlags.None;

            var playerSettings = _plugin.Configuration.PlayerSettings;
            if (!ImGui.Begin($"Fantasy Player##C{playerSettings.CompactPlayer}&N{playerSettings.NoButtons}",
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | lockFlags |
                clickThroughFlags)) return;

            //Disable FirstRun
            if (_plugin.Configuration.PlayerSettings.FirstRunNone)
            {
                _plugin.Configuration.PlayerSettings.FirstRunNone = false;
                _plugin.ConfigurationManager.Save();
            }

            //////////////// Right click popup ////////////////

            if (ImGui.BeginPopupContextWindow())
            {
                if (_playerManager.PlayerProviders.Count > 1)
                {
                    if (ImGui.BeginMenu("Switch provider"))
                    {
                        foreach (var provider in _playerManager.PlayerProviders)
                        {
                            if (provider.Value == _playerManager.CurrentPlayerProvider) continue;
                            if (ImGui.MenuItem(provider.Key.Name.Replace("Provider", "")))
                            {
                                _playerManager.CurrentPlayerProvider = provider.Value;
                                _plugin.Configuration.PlayerSettings.DefaultProvider = provider.Key.FullName;
                                _plugin.ConfigurationManager.Save();
                            }
                        }
                        ImGui.EndMenu();
                    }
                    
                    ImGui.Separator();
                }

                if (!_plugin.Configuration.SpotifySettings.LimitedAccess)
                {
                    if (ImGui.MenuItem("Compact mode", null, ref _plugin.Configuration.PlayerSettings.CompactPlayer))
                    {
                        if (_plugin.Configuration.PlayerSettings.NoButtons)
                            _plugin.Configuration.PlayerSettings.NoButtons = false;
                    }

                    if (ImGui.MenuItem("Hide Buttons", null, ref _plugin.Configuration.PlayerSettings.NoButtons))
                    {
                        if (_plugin.Configuration.PlayerSettings.CompactPlayer)
                            _plugin.Configuration.PlayerSettings.CompactPlayer = false;
                    }

                    ImGui.Separator();
                }

                ImGui.MenuItem("Lock player", null, ref _plugin.Configuration.PlayerSettings.PlayerLocked);
                ImGui.MenuItem("Show player", null, ref _plugin.Configuration.PlayerSettings.PlayerWindowShown);
                ImGui.MenuItem("Show config", null, ref _plugin.Configuration.ConfigShown);

                ImGui.EndPopup();
            }

            //////////////// Window Basics ////////////////

            if (playerState.CurrentlyPlaying.Id == null)
            {
                InterfaceUtils.TextCentered($"Nothing is playing on {playerState.ServiceName}.");
                return;
            }

            {
                //////////////// Window Setup ////////////////

                ImGui.PushStyleColor(ImGuiCol.Button, InterfaceUtils.TransparentColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, InterfaceUtils.TransparentColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, InterfaceUtils.DarkenButtonColor);

                var track = playerState.CurrentlyPlaying;



                if (_progressMs != playerState.ProgressMs)
                {
                    _lastUpdated = DateTime.Now;
                    _progressMs = playerState.ProgressMs;
                }
                if (!playerState.IsPlaying)
                {
                    _lastUpdated = null;
                }
                float percent;
                TimeSpan actualProgress;
                TimeSpan songTotal;

                if (_lastUpdated != null)
                {
                    actualProgress = (DateTime.Now - _lastUpdated.Value).Add(TimeSpan.FromMilliseconds(playerState.ProgressMs));
                    songTotal = TimeSpan.FromMilliseconds(track.DurationMs);
                    if (actualProgress >= songTotal)
                    {
                        actualProgress = songTotal;
                    }
                    percent = (float)((double) actualProgress.Ticks / songTotal.Ticks * 100);
                }
                else
                {
                    actualProgress = TimeSpan.FromMilliseconds(playerState.ProgressMs);
                    songTotal = TimeSpan.FromMilliseconds(track.DurationMs);
                    if (actualProgress >= songTotal)
                    {
                        actualProgress = songTotal;
                    }
                    percent = (float)((double) actualProgress.Ticks / songTotal.Ticks * 100);
                }

                var artists = track.Artists.Aggregate("", (current, artist) => current + (artist + ", "));

                if (!_plugin.Configuration.PlayerSettings.NoButtons)
                {
                    //////////////// Play and Pause ////////////////

                    var stateIcon = (playerState.IsPlaying)
                        ? FontAwesomeIcon.Pause.ToIconString()
                        : FontAwesomeIcon.Play.ToIconString();

                    ImGui.PushFont(UiBuilder.IconFont);

                    if (ImGui.Button(FontAwesomeIcon.Backward.ToIconString()))
                        currentProvider.SetSkip(false);

                    if (InterfaceUtils.ButtonCentered(stateIcon))
                        currentProvider.SetPauseOrPlay(!playerState.IsPlaying);

                    //////////////// Shuffle and Repeat ////////////////

                    ImGui.SameLine(ImGui.GetWindowSize().X / 2 +
                                   (ImGui.GetFontSize() + ImGui.CalcTextSize(FontAwesomeIcon.Random.ToIconString()).X));

                    if (playerState.ShuffleState)
                        ImGui.PushStyleColor(ImGuiCol.Text, _plugin.Configuration.PlayerSettings.AccentColor);

                    if (ImGui.Button(FontAwesomeIcon.Random.ToIconString()))
                        currentProvider.SetShuffle(!playerState.ShuffleState);

                    if (playerState.ShuffleState)
                        ImGui.PopStyleColor();

                    if (playerState.RepeatState != "off")
                        ImGui.PushStyleColor(ImGuiCol.Text, _plugin.Configuration.PlayerSettings.AccentColor);

                    var buttonIcon = FontAwesomeIcon.Retweet.ToIconString();

                    if (playerState.RepeatState == "track")
                        buttonIcon = FontAwesomeIcon.Music.ToIconString();

                    ImGui.SameLine(ImGui.GetWindowSize().X / 2 -
                                   (ImGui.GetFontSize() + ImGui.CalcTextSize(buttonIcon).X +
                                    ImGui.CalcTextSize(FontAwesomeIcon.Random.ToIconString()).X));

                    if (ImGui.Button(buttonIcon))
                        currentProvider.SwapRepeatState();

                    if (playerState.RepeatState != "off")
                        ImGui.PopStyleColor();

                    ImGui.SameLine(ImGui.GetWindowSize().X -
                                   (ImGui.GetFontSize() +
                                    ImGui.CalcTextSize(FontAwesomeIcon.Forward.ToIconString()).X));
                    if (ImGui.Button(FontAwesomeIcon.Forward.ToIconString()))
                        currentProvider.SetSkip(true);

                    ImGui.PopFont();
                }

                if (!_plugin.Configuration.PlayerSettings.CompactPlayer)
                {
                    if (_plugin.Configuration.PlayerSettings.ShowTimeElapsed)
                    {
                        ImGuiUtil.Center(actualProgress.ToString("mm\\:ss", CultureInfo.InvariantCulture) + " / " +
                                         songTotal.ToString("mm\\:ss", CultureInfo.InvariantCulture) +
                                         (playerState.IsPlaying ? "" : " - Paused"));
                    }
                    //////////////// Progress Bar ////////////////

                    ImGui.PushStyleColor(ImGuiCol.PlotHistogram, _plugin.Configuration.PlayerSettings.AccentColor);
                    ImGui.ProgressBar(percent / 100f, new Vector2(-1, 2f));
                    ImGui.PopStyleColor();
                    

                    Vector2 imageSize = new Vector2(100 * ImGui.GetIO().FontGlobalScale,
                        100 * ImGui.GetIO().FontGlobalScale);

                    //////////////// Text ////////////////

                    InterfaceUtils.TextCentered(track.Name);

                    ImGui.PushStyleColor(ImGuiCol.Text, InterfaceUtils.DarkenColor);

                    ImGui.Spacing();
                    InterfaceUtils.TextCentered(artists.Remove(artists.Length - 2));

                    /*
                    ImGui.Separator();
                    
                    if (ImGui.SliderFloat("Volume", ref _playerManager.CurrentPlayerProvider.SetVolume, 0f,
                        1f))
                    {
                        _plugin.ConfigurationManager.Save();
                    }
                    */
                    ImGui.PopStyleColor();
                }

                ImGui.PopStyleColor(3);
            }

            ImGui.End();
        }

        private void WelcomeWindow()
        {
            ImGui.SetNextWindowSize(_playerWindowSize);
            if (!ImGui.Begin($"Fantasy Player: Welcome",
                ref _plugin.Configuration.PlayerSettings.PlayerWindowShown,
                ImGuiWindowFlags.NoResize)) return;
            
            if (_playerManager.ProvidersLoading)
            {
                InterfaceUtils.TextCentered($"The music providers are still being loaded.");
                return;
            }

            InterfaceUtils.TextCentered("Please select your default provider.");
            foreach (var provider in _playerManager.PlayerProviders)
            {
                ImGui.SameLine();
                if (ImGui.Button(provider.Key.Name.Replace("Provider", "")))
                {
                    _playerManager.CurrentPlayerProvider = provider.Value;
                    _plugin.Configuration.PlayerSettings.DefaultProvider = provider.Key.FullName;
                    _plugin.ConfigurationManager.Save();
                }
            }
        }

        private void LoginWindow(IPlayerProvider playerProvider)
        {
            ImGui.SetNextWindowSize(_playerWindowSize);
            if (!ImGui.Begin($"Fantasy Player: {playerProvider.PlayerState.ServiceName} Login",
                ref _plugin.Configuration.PlayerSettings.PlayerWindowShown,
                ImGuiWindowFlags.NoResize)) return;
            
            if (_playerManager.ProvidersLoading)
            {
                return;
            }

            if (!playerProvider.PlayerState.IsAuthenticating)
            {
                InterfaceUtils.TextCentered($"Please login to {playerProvider.PlayerState.ServiceName} to start.");
                if (InterfaceUtils.ButtonCentered("Login"))
                    playerProvider.StartAuth();
            }
            else
            {
                InterfaceUtils.TextCentered("Waiting for a response to login... Please check your browser.");
                if (InterfaceUtils.ButtonCentered("Re-open Url"))
                    playerProvider.RetryAuth();
            }

            ImGui.End();
        }

        private void RenderTrackStructDebug(TrackStruct track)
        {
            ImGui.Text("Id: " + track.Id);
            ImGui.Text("Name: " + track.Name);
            ImGui.Text("DurationMs: " + track.DurationMs);

            if (track.Artists != null)
                ImGui.Text("Artists: " + string.Join(", ", track.Artists));

            if (track.Album.Name != null)
                ImGui.Text("Album.Name: " + track.Album.Name);
        }

        private void DebugWindow(PlayerStateStruct currentPlayerState)
        {
            if (!ImGui.Begin("Fantasy Player: Debug Window")) return;

            if (ImGui.Button("Reload providers"))
                _playerManager.ReloadProviders();

            foreach (var provider in _playerManager.PlayerProviders
                .Where(provider => provider.Value.PlayerState.ServiceName != null))
            {
                var playerState = provider.Value.PlayerState;
                var providerText = playerState.ServiceName;

                if (playerState.ServiceName == currentPlayerState.ServiceName)
                    providerText += " (Current)";

                if (!ImGui.CollapsingHeader(providerText)) continue;
                ImGui.Text("RequiresLogin: " + playerState.RequiresLogin);
                ImGui.Text("IsLoggedIn: " + playerState.IsLoggedIn);
                ImGui.Text("IsAuthenticating: " + playerState.IsAuthenticating);
                ImGui.Text("RepeatState: " + playerState.RepeatState);
                ImGui.Text("ShuffleState: " + playerState.ShuffleState);
                ImGui.Text("IsPlaying: " + playerState.IsPlaying);
                ImGui.Text("ProgressMs: " + playerState.ProgressMs);

                if (ImGui.CollapsingHeader(providerText + ": CurrentlyPlaying"))
                    RenderTrackStructDebug(playerState.CurrentlyPlaying);

                if (playerState.ServiceName == currentPlayerState.ServiceName) continue;
                if (ImGui.Button($"Set {playerState.ServiceName} as current provider"))
                {
                    _playerManager.CurrentPlayerProvider = provider.Value;
                }
            }

            ImGui.End();
        }

        //////////////// Commands ////////////////

        private void OnPlaylistCommand(string stringValue, int intValue, CallbackResponse response)
        {
            var playerProvider = _playerManager.CurrentPlayerProvider;
            if (playerProvider is SpotifyProvider)
            {
                SpotifyProvider spotify = (SpotifyProvider)playerProvider;
                var playlists = spotify.GetPlaylists();

                foreach (var playlist in playlists.Items)
                {
                    if (playlist.Name.ToLower().StartsWith(stringValue))
                    {
                        _plugin.DisplayMessage($"Playing playlist: {playlist.Name}");
                        spotify.SetPlaylist(playlist);
                        return;
                    }
                }
            }
        }

        private void OnReLoginCommand(string stringValue, int intValue, CallbackResponse response)
        {
            var playerState = _playerManager.CurrentPlayerProvider.PlayerState;
            playerState.IsLoggedIn = false;
            _playerManager.CurrentPlayerProvider.PlayerState = playerState;
            _playerManager.CurrentPlayerProvider.ReAuth();
        }

        private void OnDisplayCommand(string stringValue, int intValue, CallbackResponse response)
        {
            _plugin.Configuration.PlayerSettings.PlayerWindowShown = response switch
            {
                CallbackResponse.SetValue => intValue == 1,
                CallbackResponse.ToggleValue => !_plugin.Configuration.PlayerSettings.PlayerWindowShown,
                _ => _plugin.Configuration.PlayerSettings.PlayerWindowShown
            };
        }

        private void OnVolumeCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            _plugin.DisplayMessage($"Set volume to: {intValue}");
            _playerManager.CurrentPlayerProvider.SetVolume(intValue);
        }

        private void OnShuffleCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            switch (response)
            {
                case CallbackResponse.SetValue:
                {
                    if (intValue == 1)
                        _plugin.DisplayMessage("Turned on shuffle.");

                    if (intValue == 0)
                        _plugin.DisplayMessage("Turned off shuffle.");

                    _playerManager.CurrentPlayerProvider.SetShuffle(intValue == 1);
                    break;
                }
                case CallbackResponse.ToggleValue:
                {
                    if (!_playerManager.CurrentPlayerProvider.PlayerState.ShuffleState)
                        _plugin.DisplayMessage("Turned on shuffle.");

                    if (_playerManager.CurrentPlayerProvider.PlayerState.ShuffleState)
                        _plugin.DisplayMessage("Turned off shuffle.");

                    _playerManager.CurrentPlayerProvider.SetShuffle(!_playerManager.CurrentPlayerProvider.PlayerState
                        .ShuffleState);
                    break;
                }
                case CallbackResponse.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(response), response, null);
            }
        }

        private void OnNextCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            _plugin.DisplayMessage("Skipping to next track.");
            _playerManager.CurrentPlayerProvider.SetSkip(true);
        }

        private void OnBackCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            _plugin.DisplayMessage("Going back a track.");
            _playerManager.CurrentPlayerProvider.SetSkip(false);
        }

        private void OnPlayCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            string displayInfo = null;
            if (_playerManager.CurrentPlayerProvider.PlayerState.CurrentlyPlaying.Id != null)
                displayInfo = _playerManager.CurrentPlayerProvider.PlayerState.CurrentlyPlaying.Name;
            _plugin.DisplaySongTitle(displayInfo); 
            _playerManager.CurrentPlayerProvider.SetPauseOrPlay(true);
        }

        private void OnPauseCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (_playerManager.CurrentPlayerProvider.PlayerState.ServiceName == null)
                return;

            _plugin.DisplayMessage("Paused playback.");
            _playerManager.CurrentPlayerProvider.SetPauseOrPlay(false);
        }

        public void Dispose()
        {
        }
    }
}