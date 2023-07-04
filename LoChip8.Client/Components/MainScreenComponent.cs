using Microsoft.Xna.Framework;
using Nez;

namespace LoChip8.Client.Components;

public class MainScreenComponent : RenderableComponent, IUpdatable
{
    public override float Width { get; }
    public override float Height { get; }

    private IFont _silkScreenFont16 = null!;

    public MainScreenComponent()
    {
        Width = Screen.Width;
        Height = Screen.Height;
    }

    public override void OnAddedToEntity()
    {
        _silkScreenFont16 = Entity.Scene.Content.LoadBitmapFont(Nez.Content.Fonts.Silkscreen32);
    }

    public override void Render(Batcher batcher, Camera camera)
    {
        batcher.DrawString(_silkScreenFont16, "Hello!", new Vector2(128, 128), Color.White);
    }

    public void Update()
    {
    
    }
}