using ImGuiNET;
using System.Numerics;
using FantasyPlayer.Interfaces;
using FantasyPlayer.Manager;
using OtterGui.Widgets;

namespace FantasyPlayer.Interface.Window
{
    public class SettingsWindow
    {
        private IPlugin _plugin;

        public SettingsWindow(IPlugin plugin)
        {
            _plugin = plugin;

            _plugin.CommandManager.Commands.Add("config",
                (OptionType.Boolean, new string[] {"settings"}, "Toggles config display.", OnConfigCommand));
        }


        public void WindowLoop()
        {
            if (!_plugin.Configuration.ConfigShown)
                return;

            MainWindow();
        }

        private void MainWindow()
        {
            if (ImGui.Begin("Fantasy Player Config", ref _plugin.Configuration.ConfigShown,
                ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, InterfaceUtils.DarkenColor);
                ImGui.Text($"Type '{Plugin.Command} help' to display chat commands!");
                ImGui.PopStyleColor();

                if (ImGui.CollapsingHeader("Fantasy Player"))
                {
                    if (ImGui.Checkbox("Display chat messages", ref _plugin.Configuration.DisplayChatMessages))
                        _plugin.ConfigurationManager.Save();
                    if (Widget.DrawChatTypeSelector("Chat message output channel",
                            "To which chat channel should the fantasy player messages be echo'd?",
                            _plugin.Configuration.PlayerSettings.ChatType,
                            type => { _plugin.Configuration.PlayerSettings.ChatType = type; }))
                    {
                        _plugin.ConfigurationManager.Save();
                    }
                }

                if (!_plugin.Configuration.SpotifySettings.LimitedAccess)
                {
                    if (ImGui.CollapsingHeader("Auto-play Settings"))
                    {
                        if (ImGui.Checkbox("Auto play when entering Duty",
                            ref _plugin.Configuration.AutoPlaySettings.PlayInDuty))
                            _plugin.ConfigurationManager.Save();
                    }
                }

                if (ImGui.CollapsingHeader("Player Settings"))
                {
                    if (_plugin.Configuration.SpotifySettings.LimitedAccess)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, InterfaceUtils.DarkenColor);
                        ImGui.Text("You're not premium on Spotify. Some settings have been hidden.");
                        ImGui.PopStyleColor();
                    }

                    ImGui.Separator();

                    if (ImGui.Checkbox("Only open when logged in",
                        ref _plugin.Configuration.PlayerSettings.OnlyOpenWhenLoggedIn))
                        _plugin.ConfigurationManager.Save();

                    ImGui.Separator();

                    if (!_plugin.Configuration.SpotifySettings.LimitedAccess)
                    {
                        if (ImGui.Checkbox("Compact mode", ref _plugin.Configuration.PlayerSettings.CompactPlayer))
                        {
                            if (_plugin.Configuration.PlayerSettings.NoButtons)
                                _plugin.Configuration.PlayerSettings.NoButtons = false;
                            _plugin.ConfigurationManager.Save();
                        }

                        if (ImGui.Checkbox("Hide buttons", ref _plugin.Configuration.PlayerSettings.NoButtons))
                        {
                            if (_plugin.Configuration.PlayerSettings.CompactPlayer)
                                _plugin.Configuration.PlayerSettings.CompactPlayer = false;
                            _plugin.ConfigurationManager.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.Checkbox("Player shown", ref _plugin.Configuration.PlayerSettings.PlayerWindowShown))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    if (ImGui.Checkbox("Player locked", ref _plugin.Configuration.PlayerSettings.PlayerLocked))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    if (ImGui.Checkbox("Player input disabled",
                        ref _plugin.Configuration.PlayerSettings.DisableInput))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    if (ImGui.Checkbox("Show time elapsed",
                        ref _plugin.Configuration.PlayerSettings.ShowTimeElapsed))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    ImGui.Separator();

                    if (ImGui.SliderFloat("Player alpha", ref _plugin.Configuration.PlayerSettings.Transparency, 0f,
                        1f))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    if (ImGui.ColorEdit4("Player color", ref _plugin.Configuration.PlayerSettings.AccentColor))
                    {
                        _plugin.ConfigurationManager.Save();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Revert"))
                    {
                        _plugin.Configuration.PlayerSettings.AccentColor = InterfaceUtils.FantasyPlayerColor;
                        _plugin.ConfigurationManager.Save();
                    }

                    ImGui.Separator();

                    if (ImGui.Button("Compact mode"))
                        _plugin.Configuration.PlayerSettings.FirstRunCompactPlayer = true;

                    ImGui.SameLine();
                    if (ImGui.Button("Hide buttons"))
                        _plugin.Configuration.PlayerSettings.FirstRunSetNoButtons = true;

                    ImGui.SameLine();
                    if (ImGui.Button("Full"))
                        _plugin.Configuration.PlayerSettings.FirstRunNone = true;

                    ImGui.SameLine();
                    ImGui.Text("Revert to default size");

                    ImGui.Separator();

                    if (ImGui.Checkbox("Show debug window", ref _plugin.Configuration.PlayerSettings.DebugWindowOpen))
                    {
                        _plugin.ConfigurationManager.Save();
                    }
                }

                ImGui.Separator();

                ImGui.End();
            }
        }

        public void OnConfigCommand(string stringValue, int intValue, CallbackResponse response)
        {
            if (response == CallbackResponse.SetValue)
                _plugin.Configuration.ConfigShown = intValue == 1;

            if (response == CallbackResponse.ToggleValue)
                _plugin.Configuration.ConfigShown = !_plugin.Configuration.ConfigShown;
        }
    }
}