using ImGuiNET;
using LoChip8.Client.Ui;
using LoChip8.Logging;
using Raylib_cs;
using rlImGui_cs;

namespace LoChip8.Client;

public static class Program
{
    public const int WindowWidth = 1280;
    public const int WindowHeight = 720;

    private static readonly KeyboardKey[] _keypadKeys =
    [
        KeyboardKey.One, KeyboardKey.Two, KeyboardKey.Three, KeyboardKey.Four,
        KeyboardKey.Q, KeyboardKey.W, KeyboardKey.E, KeyboardKey.R,
        KeyboardKey.A, KeyboardKey.S, KeyboardKey.D, KeyboardKey.F,
        KeyboardKey.Z, KeyboardKey.X, KeyboardKey.C, KeyboardKey.V
    ]; 
    
    static void Main(string[] args)
    {
        Raylib.SetConfigFlags( ConfigFlags.ResizableWindow | ConfigFlags.MaximizedWindow);
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

        var mainFont = Raylib.LoadFont("Content/Fonts/FiraCode-Regular.ttf");

        var interpreter = new Interpreter();
        interpreter.Ram.LoadRom("Content/ROMs/games/Space Invaders [David Winter].ch8");

        var gameScreen = new GameScreen(interpreter.Display);
        
        try
        {
            while (!Raylib.WindowShouldClose())
            {
                var fps = Raylib.GetFPS();
                
                for (byte i = 0; i < _keypadKeys.Length; i++)
                {
                    var key = _keypadKeys[i];
                    if (Raylib.IsKeyPressed(key))
                        interpreter.Keypad.SetKey(i, true);
                    else if (Raylib.IsKeyReleased(key))
                        interpreter.Keypad.SetKey(i, false);
                }

                interpreter.Clock();
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(32, 32, 32));
                
                gameScreen.Draw();
                
                rlImGui.Begin();
                
                //ImGui.ShowDemoWindow();
                
                rlImGui.End();
                
                Raylib.DrawText($"FPS: {fps}", 6, 6, 24, Color.RayWhite);
                
                Raylib.EndDrawing();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message, "Main");
        }
        finally
        {
            gameScreen.Dispose();
            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }
    }
}