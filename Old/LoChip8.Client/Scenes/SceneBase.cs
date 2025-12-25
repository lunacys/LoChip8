using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace LoChip8.Client.Scenes;


public abstract class SceneBase : Scene, IFinalRenderDelegate
{
    public const int ScreenSpaceRenderLayer = 999;
    private readonly ScreenSpaceRenderer _screenSpaceRenderer = null!;

    static bool _needsFullRenderSizeForUi;

    private float _scalingFactor = 1f;

    public float ScalingFactor
    {
        get => _scalingFactor;
        set
        {
            _scalingFactor = value;
            UpdateResolution();
        }
    }

    protected SceneBase(bool addExcludeRenderer = true, bool needsFullRenderSizeForUi = false)
    {
        _needsFullRenderSizeForUi = needsFullRenderSizeForUi;

        // setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
        if (needsFullRenderSizeForUi)
        {
            // dont actually add the renderer since we will manually call it later
            _screenSpaceRenderer = new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer);
            _screenSpaceRenderer.ShouldDebugRender = false;
            FinalRenderDelegate = this;
        }
        else
        {
            AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
        }

        if (addExcludeRenderer)
            AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));
    }

    public override void Initialize()
    {
        UpdateResolution();

        base.Initialize();
    }

    public void UpdateResolution()
    {
        var screenSizeW = Screen.MonitorWidth;
        var screenSizeH = Screen.MonitorHeight - 128;

        Screen.SetSize(screenSizeW, screenSizeH);

        SetDesignResolution(
            (int)(screenSizeW / ScalingFactor),
            (int)(screenSizeH / ScalingFactor),
            SceneResolutionPolicy.ShowAllPixelPerfect);
    }


    private Scene? _scene;

    public void OnAddedToScene(Scene scene) => _scene = scene;

    public void OnSceneBackBufferSizeChanged(int newWidth, int newHeight)
        => _screenSpaceRenderer.OnSceneBackBufferSizeChanged(newWidth, newHeight);

    public void HandleFinalRender(RenderTarget2D finalRenderTarget, Color letterboxColor, RenderTarget2D source,
        Rectangle finalRenderDestinationRect, SamplerState samplerState)
    {
        Core.GraphicsDevice.SetRenderTarget(null);
        Core.GraphicsDevice.Clear(letterboxColor);
        Graphics.Instance.Batcher.Begin(BlendState.Opaque, samplerState, DepthStencilState.None, RasterizerState.CullNone, null);
        Graphics.Instance.Batcher.Draw(source, finalRenderDestinationRect, Color.White);
        Graphics.Instance.Batcher.End();

        _screenSpaceRenderer.Render(_scene);
    }
}