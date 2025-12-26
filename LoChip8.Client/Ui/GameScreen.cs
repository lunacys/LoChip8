using System.Numerics;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace LoChip8.Client.Ui;

public class GameScreen : IDisposable
{
    public int X = 64;
    public int Y = 64;
    public float Scale = 16f;
    
    public Display Display { get; }

    private Color[] _screenTextureData;
    
    private Texture2D _screenTexture;
    public Texture2D ScreenTexture => _screenTexture;

    public GameScreen(Display display)
    {
        Display = display;
        
        _screenTextureData = new Color[Display.Width * Display.Height];
        for (int i = 0; i < _screenTextureData.Length; i++)
        {
            _screenTextureData[i] = Color.Black;
        }

        var screenImage = Raylib.GenImageColor(
            Display.Width,
            Display.Height,
            Color.Black
        );
        _screenTexture = Raylib.LoadTextureFromImage(screenImage);
        Raylib.UnloadImage(screenImage);
        
        Display.Changed += (_, _) => UpdateScreenTexture();
    }

    public void UpdateScreenTexture()
    {
        for (int i = 0; i < Display.Width * Display.Height; i++)
        {
            if (Display.Data[i] == 1)
                _screenTextureData[i] = Color.RayWhite;
            else 
                _screenTextureData[i] = Color.Black;
        }
        
        Raylib.UpdateTexture(_screenTexture, _screenTextureData);
    }

    public void Draw()
    {
        Raylib.DrawTextureEx(_screenTexture, new Vector2(X, Y), 0, Scale, Color.White);
    }

    public void Dispose()
    {
        Raylib.UnloadTexture(_screenTexture);
    }
}