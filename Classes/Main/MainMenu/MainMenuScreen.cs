using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game;
using Midtown.Classes.Main.MainMenu.Buttons;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Main.MainMenu
{
    public class MainMenuScreen : Screen
    {
        public Texture2D BackgroundSky { get; private set; }
        public Texture2D BackgroundClouds { get; private set; }
        public Texture2D BackgroundBack { get; private set; }
        public Texture2D BackgroundMiddle { get; private set; }
        public Texture2D BackgroundFront { get; private set; }
        public Texture2D BackgroundWater { get; private set; }
        public Texture2D BackgroundWaterMask { get; private set; }
        public Texture2D BackgroundCar { get; private set; }
        public Texture2D TitleTexture { get; private set; }
        private Effect waterReflectionEffect;
        private RenderTarget2D WaterTarget, CloudReflectionTarget;
        private List<Car> Cars;
        private Random Random;
        private double Time = 0, TimeBeforeAddingCar = 0.25;

        public MainMenuScreen(MainGame game) : base(game) { }

        public override void Initialize()
        {
            Elements.Add(new MainMenuButton(Game, this, "mainMenu.buttons.newGame", 0, 240, 8, 32, false, Color.White, Color.CornflowerBlue, Color.CornflowerBlue, () => Game.SetScreen(new GameScreen(Game))));
            Elements.Add(new MainMenuButton(Game, this, "mainMenu.buttons.continue", 0, 280, 8, 32, false, Color.White, Color.Lime, Color.Lime, () => { }));
            Elements.Add(new MainMenuButton(Game, this, "mainMenu.buttons.settings", 0, 320, 8, 32, false, Color.White, Color.Yellow, Color.Yellow, () => ChildScreen = new SettingsScreen(Game, this)));
            Elements.Add(new MainMenuButton(Game, this, "mainMenu.buttons.exit", 640, 320, 8, 32, true, Color.White, Color.Red, Color.Red, () => Game.Exit()));

            Cars = new List<Car>();
            Random = new Random();

            WaterTarget = new RenderTarget2D(Game.GraphicsDevice, Game.MainTarget.Width, Game.MainTarget.Height);
            CloudReflectionTarget = new RenderTarget2D(Game.GraphicsDevice, Game.MainTarget.Width, Game.MainTarget.Height);

            base.Initialize();
        }

        public override void Load()
        {
            waterReflectionEffect = Game.Content.Load<Effect>("Effects/WaterMovement");
            waterReflectionEffect.Parameters["Height"]?.SetValue(0);
            waterReflectionEffect.Parameters["TerrainTextureSize"]?.SetValue(new Vector2(Game.MainTarget.Width, Game.MainTarget.Height));
            waterReflectionEffect.Parameters["EquationValueA"]?.SetValue(25f);
            waterReflectionEffect.Parameters["EquationValueB"]?.SetValue(32f);
            waterReflectionEffect.Parameters["EquationValueC"]?.SetValue(4f);
            waterReflectionEffect.Parameters["EquationValueD"]?.SetValue(24f);

            BackgroundSky = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundSky");
            BackgroundClouds = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundClouds");
            BackgroundBack = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundBack");
            BackgroundMiddle = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundMiddle");
            BackgroundFront = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundFront");
            BackgroundWater = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundWater");
            BackgroundWaterMask = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundWaterMask");
            BackgroundCar = Game.Content.Load<Texture2D>("Textures/MainMenu/BackgroundCar");
            TitleTexture = Game.Content.Load<Texture2D>("Textures/MainMenu/Title");

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            waterReflectionEffect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            Time += gameTime.ElapsedGameTime.TotalSeconds;
            if (Time >= TimeBeforeAddingCar)
            {
                if (Random.Next(15) == 0)
                {
                    Time = 0;
                    bool left = Random.Next(2) == 0;
                    Vector2 position;
                    if (!left)
                        position = new Vector2(228,177);
                    else
                        position = new Vector2(406,181);

                    int red = (int)Math.Clamp(Random.Next(0, 0xFF + 1) + 0x99 * 0.2, 0, 0xFF);
                    int green = (int)Math.Clamp(Random.Next(0, 0xFF + 1) + 0xFF * 0.2, 0, 0xFF);
                    int blue = (int)Math.Clamp(Random.Next(0, 0xFF + 1) + 0xFF * 0.2, 0, 0xFF);
                    Cars.Add(new Car()
                    {
                        Color = new Color(red, green, blue),
                        Left = left,
                        Position = position
                    });
                }
            }

            List<Car> carsToRemove = new List<Car>();
            for (int i = 0; i < Cars.Count; i++)
            {
                Vector2 motion = Vector2.UnitX;
                if (Cars[i].Left)
                    Cars[i].Position -= motion;
                else
                    Cars[i].Position += motion;

                if ((!Cars[i].Left && Cars[i].Position.X > 424) || (Cars[i].Left && Cars[i].Position.X < 238))
                    carsToRemove.Add(Cars[i]);
            }

            for (int i = 0; i < carsToRemove.Count; i++)
                Cars.Remove(carsToRemove[i]);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            int cloudX = (int) Math.Round((gameTime.TotalGameTime.TotalSeconds * 8) % 640);

            Game.SetRenderTarget(CloudReflectionTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            SpriteBatch.Draw(BackgroundClouds, new Rectangle(cloudX, 0, BackgroundClouds.Width, BackgroundClouds.Height), null, Color.White * 0.1f, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            SpriteBatch.Draw(BackgroundClouds, new Rectangle(cloudX - BackgroundClouds.Width, 0, BackgroundClouds.Width, BackgroundClouds.Height), null, Color.White * 0.1f, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);

            SpriteBatch.End();

            SpriteBatch.Begin(blendState: new BlendState() { AlphaSourceBlend = Blend.DestinationAlpha, ColorDestinationBlend = Blend.SourceAlpha }, samplerState: SamplerState.PointClamp);

            SpriteBatch.Draw(BackgroundWaterMask, Vector2.Zero, Color.White);

            SpriteBatch.End();


            waterReflectionEffect.Parameters["EquationValueA"]?.SetValue(10f);
            waterReflectionEffect.Parameters["EquationValueB"]?.SetValue(64f);
            waterReflectionEffect.Parameters["EquationValueC"]?.SetValue(2f);
            waterReflectionEffect.Parameters["EquationValueD"]?.SetValue(24f);


            Game.SetRenderTarget(WaterTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            waterReflectionEffect.Parameters["TerrainTexture"]?.SetValue(BackgroundFront);
            waterReflectionEffect.CurrentTechnique.Passes[0].Apply();

            SpriteBatch.Draw(BackgroundWater, Vector2.Zero, Color.White);
            SpriteBatch.Draw(CloudReflectionTarget, Vector2.Zero, Color.White);

            SpriteBatch.End();


            Game.SetRenderTarget(null);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            SpriteBatch.Draw(BackgroundSky, Vector2.Zero, Color.White);
            SpriteBatch.Draw(BackgroundClouds, new Vector2(cloudX, 0), Color.White);
            SpriteBatch.Draw(BackgroundClouds, new Vector2(cloudX - BackgroundClouds.Width, 0), Color.White);
            SpriteBatch.Draw(BackgroundBack, Vector2.Zero, Color.White);

            foreach (Car car in Cars)
                SpriteBatch.Draw(BackgroundCar, car.Position - new Vector2(2, 3), car.Color);

            SpriteBatch.Draw(BackgroundMiddle, Vector2.Zero, Color.White);
            SpriteBatch.Draw(WaterTarget, Vector2.Zero, Color.White);
            SpriteBatch.Draw(BackgroundFront, Vector2.Zero, Color.White);

            SpriteBatch.Draw(TitleTexture, new Vector2((Game.MainTarget.Width - TitleTexture.Width) / 2, 40 - TitleTexture.Height / 2), Color.White);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
    
    class Car
    {
        public Vector2 Position;
        public Color Color;
        public bool Left;
    }
}
