using System.Numerics;
using System.Reflection;
using DalaMock;
using DalaMock.Configuration;
using DalaMock.Mock;
using Dalamud.Interface;
using ImGuiNET;
using InventoryToolsMock;
using Lumina;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace FantasyPlayer.Mock
{
    class Program
    {
        public static Sdl2Window _window;
        public static GraphicsDevice _gd;
        private static CommandList _cl;
        public static ImGuiController _controller;
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);
        public static MockPlugin? _mockPlugin;
        private static MockSettingsWindow _mockSettingsWindow;
        private static MockProgram _program;

        static void Main(string[] args)
        {
            var yourServiceContainer = new Service();
            _program = new MockProgram(yourServiceContainer);
            _mockPlugin = new MockPlugin();
            _program.SetPlugin(_mockPlugin);
            _mockSettingsWindow = new MockSettingsWindow(_program);


            if (AppSettings.Default.AutoStart)
            {
                _program.StartPlugin();
            }

            while (_program.PumpEvents(PreUpdate, PostUpdate))
            {
    
            }

            if (_mockPlugin != null)
            {
                _mockPlugin.Dispose();
            }

            _program.Dispose();
        }

        private static void PostUpdate()
        {
            _mockPlugin?.Draw();
            _mockSettingsWindow?.Draw();
        }

        private static void PreUpdate()
        {
            _program.MockService?.MockFramework.FireUpdate();
        }            
    }
}