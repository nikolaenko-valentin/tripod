// Plugin.cs
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using HostMgd.ApplicationServices;
using Application = HostMgd.ApplicationServices.Application;

// Register plugin entry point
[assembly: ExtensionApplication(typeof(MyNanoCADPlugin.Plugin))]
// Register commands
[assembly: CommandClass(typeof(MyNanoCADPlugin.Commands))]

namespace MyNanoCADPlugin
{
    public class Plugin : IExtensionApplication
    {
        public static Plugin? Instance { get; private set; }

        public void Initialize()
        {
            Instance = this;

            try
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                doc?.Editor.WriteMessage(
                    "\n✅ MyPlugin v1.0 loaded! " +
                    "Type MYPLUGIN to open.\n"
                );
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Plugin init error: {ex.Message}"
                );
            }
        }

        public void Terminate()
        {
            try
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                doc?.Editor.WriteMessage("\nMyPlugin unloaded.\n");
            }
            catch { /* ignore on terminate */ }
        }
    }
}
