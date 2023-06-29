using System.Numerics;
using System.Text;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FantasyPlayer.Interfaces;
using ImGuiNET;
using Newtonsoft.Json;
using OtterGui.Raii;

namespace FantasyPlayer.Mock;

public class MockWindow : Window
{
    public MockWindow(string name = "FantasyPlayer Mock", ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow)
    {
        _rng = new Random();
    }
    
    public MockWindow(IPlugin plugin) : base("FantasyPlayer Mock")
    {
        _plugin = plugin;
        _rng = new Random();
    }

    public override void OnClose()
    {
        IsOpen = true;
    }

    public static string AsKey => "mock";
    private Random _rng;

    public override void Draw()
    {
        if (!ImGui.Begin("Fantasy Player Mock")) return;
        if (ImGui.BeginTabBar("MockTabs"))
        {
            DrawWindowTab();
            ImGui.EndTabBar();
        }

        ImGui.End();
    }

    private string _windowName = "";
    private readonly IPlugin _plugin;

    private void DrawWindowTab()
    {
        if (ImGui.BeginTabItem("Windows"))
        {
            if (ImGui.Button("Show Settings"))
            {
                _plugin.Configuration.ConfigShown = true;
            }
            if (ImGui.Button("Show Main Window"))
            {
                _plugin.Configuration.PlayerSettings.PlayerWindowShown = true;
            }
            ImGui.EndTabItem();
        }
    }
}