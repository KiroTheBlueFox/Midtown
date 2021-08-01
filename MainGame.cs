using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midtown.Classes.Main;
using Midtown.Classes.Main.Game;
using Midtown.Classes.Utils;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Midtown
{
    public class MainGame : Game
    {
        public readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Config Config;
        public Scene CurrentScene;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            CurrentScene = new GameplayScene(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            CurrentScene.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var configPath = Path.Combine(Content.RootDirectory, "config.json");
            using (var stream = TitleContainer.OpenStream(configPath))
            {
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                ConfigValues configValues = JsonConvert.DeserializeObject<ConfigValues>(json);
                this.Config = new Config(this, configValues);
            }

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            CurrentScene.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Config.Update();

            CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            CurrentScene.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}
