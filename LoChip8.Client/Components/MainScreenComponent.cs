using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace LoChip8.Client.Components;

[StructLayout(LayoutKind.Explicit, Size = 8)]
struct TestWord
{
    [FieldOffset(0)]
    public char Unused;
    [FieldOffset(0)]
    public byte Sw2;
    [FieldOffset(0)]
    public byte Sw1;
    [FieldOffset(0)]
    public byte State;
    [FieldOffset(0)]
    public byte Value;
}

public class MainScreenComponent : RenderableComponent, IUpdatable
{
    public override float Width { get; }
    public override float Height { get; }

    private IFont _silkScreenFont16 = null!;

    private Keys[] _availableKeys => new Keys[]
    {
        Keys.D1, Keys.D2, Keys.D3, Keys.D4,
        Keys.Q, Keys.W, Keys.E, Keys.R,
        Keys.A, Keys.S, Keys.D, Keys.F,
        Keys.Z, Keys.X, Keys.C, Keys.V
    };

    private Display _display;
    private Keypad _keypad;
    private VirtualMachine _vm;

    private Texture2D _pixelTexture;

    public bool IsPaused = false;

    public MainScreenComponent()
    {
        Width = Screen.Width;
        Height = Screen.Height;

        _display = new Display();
        _keypad = new Keypad();
        _vm = new VirtualMachine(new ConsoleBeeper(), _keypad, _display, "BREAKOUT.ch8");
        _vm.Initialize();

        Core.Schedule(1 / 60f, true, (timer) =>
        {
            if (!IsPaused)
                _vm.DelayTimerTick();
        });

        Core.Schedule(1 / 60f, true, (timer) =>
        {
            if (!IsPaused)
                _vm.SoundTimerTick();
        });

        Core.Schedule(1 / 240f, true, (timer) =>
        {
            if (!IsPaused)
                _vm.ProceedCycle();
        });
    }

    public override void OnAddedToEntity()
    {
        _silkScreenFont16 = Entity.Scene.Content.LoadBitmapFont(Nez.Content.Fonts.Silkscreen16);

        _pixelTexture = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[]
        {
            Color.White
        });
    }

    public void Update()
    {
        if (Input.IsKeyPressed(Keys.Space))
            IsPaused = !IsPaused;

        if (IsPaused && Input.IsKeyPressed(Keys.Enter))
            _vm.ProceedCycle();

        for (int i = 0; i < _availableKeys.Length; i++)
        {
            var k = _availableKeys[i];
            if (Input.CurrentKeyboardState.IsKeyDown(k))
                _keypad.SendKey((byte) i, true);
            else if (Input.CurrentKeyboardState.IsKeyUp(k))
                _keypad.SendKey((byte)i, false);
        }
    }

    public override void Render(Batcher batcher, Camera camera)
    {
        var drawColor = Color.White;

        // Draw Register Values
        for (int i = 0; i < _vm.Cpu.Registers.Length; i++)
        {
            batcher.DrawString(
                _silkScreenFont16,
                $"V{Convert.ToString(i, 16).ToUpper()}: {ToHexString(_vm.Cpu.Registers[i])}",
                new Vector2(16, 16 + 16 * i),
                drawColor
            );
        }
        batcher.DrawString(_silkScreenFont16, $"PC: {ToHexString(_vm.Cpu.RegisterPC)}", new Vector2(158, 16), drawColor);
        batcher.DrawString(_silkScreenFont16, $"I:  {ToHexString(_vm.Cpu.RegisterI)}", new Vector2(158, 16 + 16), drawColor);
        batcher.DrawString(_silkScreenFont16, $"SP: {ToHexString(_vm.Cpu.RegisterSP)}", new Vector2(158, 16 + 16 * 2), drawColor);
        batcher.DrawString(_silkScreenFont16, $"DT: {ToHexString(_vm.Cpu.RegisterDT)}", new Vector2(158, 16 + 16 * 3), drawColor);
        batcher.DrawString(_silkScreenFont16, $"ST: {ToHexString(_vm.Cpu.RegisterST)}", new Vector2(158, 16 + 16 * 4), drawColor);


        // Draw Display
        for (int i = 0; i < _display.Height; i++)
        {
            for (int j = 0; j < _display.Width; j++)
            {
                var state = _display.DisplayData[i, j];
                var drawRect = new Rectangle(300 + j * 8, 8 + i * 8, 8, 8);

                if (state == 1) // White
                {
                    batcher.Draw(_pixelTexture, drawRect, null, Color.White);
                }
                else // Black
                {
                    batcher.Draw(_pixelTexture, drawRect, null, Color.Black);
                }
            }
        }
    }

    private string ToHexString(byte value)
    {
        return "0x" + Convert.ToString(value, 16).ToUpper().PadLeft(2, '0');
    }

    private string ToHexString(ushort value)
    {
        return "0x" + Convert.ToString(value, 16).ToUpper().PadLeft(4, '0');
    }

    private string ToHexStringNoPrefix(byte value, int padLeft = 2)
    {
        return Convert.ToString(value, 16).ToUpper().PadLeft(padLeft, '0');
    }
}