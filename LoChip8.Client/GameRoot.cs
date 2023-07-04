using LiteLog.Logging;
using LoChip8.Client.Scenes;
using Nez;
using Nez.ImGuiTools;
using Nez.Persistence.Binary;
using Nez.UI;

namespace LoChip8.Client;

public class GameRoot : Core
{
    private readonly ILogger _logger = LoggerFactory.GetLogger("GameRoot");
    private FileDataStore _dataStore;

    public GameRoot()
    {
    }

    protected override void Initialize()
    {
        _logger.Debug("Started initializing...");

        //Input.Touch.EnableTouchSupport();

        base.Initialize();

        _dataStore = new FileDataStore(Storage.GetStorageRoot(), FileDataStore.FileFormat.Binary);
        Services.AddService(_dataStore);
        //_dataStore.Load("GameSettings.bin", _gameSettings);

        Window.AllowUserResizing = true;

#if DEBUG
        var imGuiManager = new ImGuiManager();
        imGuiManager.ShowCoreWindow = true;
        imGuiManager.ShowDemoWindow = false;
        imGuiManager.ShowMenuBar = true;
        imGuiManager.ShowSceneGraphWindow = true;
        imGuiManager.ShowSeperateGameWindow = false;
        imGuiManager.ShowStyleEditor = false;
        RegisterGlobalManager(imGuiManager);
        imGuiManager.Enabled = true;
#endif

        PauseOnFocusLost = false;
        DebugRenderEnabled = false;

        Services.AddService(Skin.CreateDefaultSkin());

        _logger.Debug("Setting Spatial hash cell size");
        Physics.SpatialHashCellSize = 40;

        _logger.Debug("Setting up scene");
        try
        {
            Scene = new TestScene();
        }
        catch (Exception e)
        {
            _logger.Log("FATAL: " + e.Message, LogLevel.Error);
            throw;
        }

        //DebugRenderEnabled = false;

        _logger.Debug("Ended initializing");
    }
}