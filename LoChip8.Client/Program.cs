using ImGuiNET;
using LoChip8.Logging;
using Raylib_cs;
using rlImGui_cs;

namespace LoChip8.Client;

public static class Program
{
    public const int WindowWidth = 1280;
    public const int WindowHeight = 720;
    
    static void Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow | ConfigFlags.MaximizedWindow);
        Raylib.InitWindow(WindowWidth, WindowHeight, "LoChip8");
        Raylib.SetWindowMinSize(WindowWidth, WindowHeight);
        Raylib.SetTargetFPS(60);
        
        rlImGui.Setup(true, true);
        
        var dpi = Raylib.GetWindowScaleDPI();

        if (Math.Abs(dpi.X - dpi.Y) > 0.0001f)
            Log.Warning($"DPI.X != DPI.Y ({dpi.X}x{dpi.Y}). Using X as scaling factor", "Main");
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigWindowsMoveFromTitleBarOnly = true;
        
        if (Math.Abs(dpi.X - 1.0f) > 0.0001f)
        {
            io.FontGlobalScale = MathF.Floor(dpi.X * 1.6f); // Fix ugly fonts by flooring

            Raylib.SetWindowSize((int)(WindowWidth * (dpi.X * 2)), (int)(WindowHeight * (dpi.Y * 2)));
            Raylib.SetWindowPosition(128, 128);
        }
        
        var style = ImGui.GetStyle();
        style.ScaleAllSizes(dpi.X);

        try
        {
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(32, 32, 32));
                
                rlImGui.Begin();
                
                //ImGui.ShowDemoWindow();
                
                rlImGui.End();
                
                Raylib.EndDrawing();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message, "Main");
        }
        finally
        {
            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }
    }
}