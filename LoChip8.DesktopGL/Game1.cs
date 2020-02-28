using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoChip8.DesktopGL
{
    class Beeper : IBeeper
    {
        public void Beep()
        {
            Console.Beep();
        }
    }
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont _debugFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            var display = new Display();
            var keypad = new Keypad();
            
            VirtualMachine vm = new VirtualMachine(new Beeper(), keypad, display);
            vm.Initialize();
            var size = vm.LoadRom("BREAKOUT.ch8");
            Console.WriteLine($"Loaded ROM with size of {size} bytes");
            vm.DumpProgramMemory();

            Console.WriteLine();
            vm.ProceedCycle();
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _debugFont = Content.Load<SpriteFont>(Path.Combine("Fonts", "DebugFont"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.DrawString(_debugFont, "Hello!", Vector2.One, Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}