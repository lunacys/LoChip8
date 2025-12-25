using LoChip8.Client.Components;
using Microsoft.Xna.Framework;

namespace LoChip8.Client.Scenes;

public class MainScene : SceneBase
{
    public override void Initialize()
    {
        base.Initialize();

        ClearColor = Color.Black;

        CreateEntity("main-scene")
            .AddComponent(new MainScreenComponent());
    }
}