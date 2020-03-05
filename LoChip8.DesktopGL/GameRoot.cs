using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace LoChip8.DesktopGL
{
    class Beeper : IBeeper
    {
        public void Beep(int duration)
        {
            // Console.Beep();
            Console.Beep(1000, duration);
        }
    }
    
    public class GameRoot : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont _debugFont;
        private VirtualMachine _vm;
        private Keypad _keypad;
        private Display _display;
        private Texture2D _pixel;
        private InputHandler _input;

        private Keys[] availableKeys => new Keys[]
        {
            Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.Q, Keys.W, Keys.E, Keys.R,
            Keys.A, Keys.S, Keys.D, Keys.F,
            Keys.Z, Keys.X, Keys.C, Keys.V
        };

        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 240d);
            
            // TODO: Make VM run in a separate thread 
            _display = new Display();
            _keypad = new Keypad();
            
            _vm = new VirtualMachine(new Beeper(), _keypad, _display, "BREAKOUT.ch8");
            _vm.Initialize();

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            // _vm.ProceedCycle();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _input = new InputHandler(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _debugFont = Content.Load<SpriteFont>(Path.Combine("Fonts", "DebugFont"));
            
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new []
            {
                Color.White
            });
        }

        protected override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // _vm.ProceedCycle();
            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _vm.ProceedCycle();
            }

            if (_input.WasKeyPressed(Keys.Enter))
            {
                _vm.ProceedCycle();
            }

            

            for (var i = 0; i < availableKeys.Length; i++)
            {
                var k = availableKeys[i];
                if (Keyboard.GetState().IsKeyDown(k))
                    _keypad.SendKey((byte) i, true);
                else if (Keyboard.GetState().IsKeyUp(k))
                    _keypad.SendKey((byte) i, false);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < _vm.Cpu.Registers.Length; i++)
            {
                spriteBatch.DrawString(
                    _debugFont, 
                    $"V{Convert.ToString(i, 16).ToUpper()}: {ToHexString(_vm.Cpu.Registers[i])}", 
                    new Vector2(8, 8 + 16 * i),
                    Color.Black
                );
            }
            spriteBatch.DrawString(_debugFont, $"PC: {ToHexString(_vm.Cpu.RegisterPC)}", new Vector2(128, 8),          Color.Black);
            spriteBatch.DrawString(_debugFont, $"I:  {ToHexString(_vm.Cpu.RegisterI)}",  new Vector2(128, 8 + 16),     Color.Black);
            spriteBatch.DrawString(_debugFont, $"SP: {ToHexString(_vm.Cpu.RegisterSP)}", new Vector2(128, 8 + 16 * 2), Color.Black);
            spriteBatch.DrawString(_debugFont, $"DT: {ToHexString(_vm.Cpu.RegisterDT)}", new Vector2(128, 8 + 16 * 3), Color.Black);
            spriteBatch.DrawString(_debugFont, $"ST: {ToHexString(_vm.Cpu.RegisterST)}", new Vector2(128, 8 + 16 * 4), Color.Black);

            var y = 270;
            for (int i = 0; i < _vm.RomSize; i++)
            {
                var x = i % 2 == 0 ? 8 + 24 * (i % 32) + 6 : 8 + 24 * (i % 32);
                if (i % 32 == 0)
                    y += 20;
                var pos = new Vector2(x, y);

                var color = _vm.Cpu.RegisterPC - VirtualMachine.LoadingAddress == i || _vm.Cpu.RegisterPC - VirtualMachine.LoadingAddress + 1 == i ? Color.Red : Color.Black;
                
                spriteBatch.DrawString(_debugFont, $"{ToHexStringNoPrefix(_vm.Ram[(ushort) (VirtualMachine.LoadingAddress + i)])}", pos, color);
            }

            for (int i = 0; i < _display.Height; i++)
            {
                for (int j = 0; j < _display.Width; j++)
                {
                    var state = _display.DisplayData[i, j];
                    var drawRect = new Rectangle(256 + j * 8, 8 + i * 8, 8, 8);

                    if (state == 1) // White
                    {
                        spriteBatch.Draw(_pixel, drawRect, null, Color.White);
                    }
                    else // Black
                    {
                        spriteBatch.Draw(_pixel, drawRect, null, Color.Black);
                    }
                }
            }
            
            // Stack
            for (int i = 0; i < _vm.Cpu.Stack.Length; i++)
            {
                spriteBatch.DrawString(_debugFont, $"S{ToHexStringNoPrefix((byte)i, 1)}: {ToHexString(_vm.Cpu.Stack[i])}", new Vector2(780, 8 + 16 * i), Color.Black);
            }

            y = 8 + 16 * 16;
            for (int i = 0; i < _vm.Keypad.PressedKeys.Length; i++)
            {
                var size = new Size2(32, 32);
                var key = availableKeys[i];
                var x = 780 + size.Width * (i % 4) + 6;

                var pos = new Vector2(x, y);
                

                var color = _vm.Keypad.IsKeyDown((byte) i) ? Color.Green : Color.Blue;
                
                spriteBatch.DrawRectangle(pos, size, color, 3f);
                spriteBatch.DrawString(_debugFont, key.ToString(), pos + Vector2.One * 4f, Color.Black);
                
                if ((i - 3) % 4 == 0)
                    y += (int) size.Height;
            }

            spriteBatch.End();

            base.Draw(gameTime);
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
}