using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game.Entities;
using Midtown.Classes.Main.Game.Entities.Objects.EasterEggs;
using Midtown.Classes.Main.Game.Interface;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Main.Game.Maps;
using Midtown.Classes.Main.Game.Systems;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Midtown.Classes.Main.Game
{
    public class GameScreen : Screen
    {
        private readonly Random _random = new Random();
        public List<GameMap> Maps;
        private GameMap _currentMap;
        private RenderTarget2D _reflectionRenderTarget, _waterRenderTarget, _waterPixelatedRenderTarget, _backgroundRenderTarget, _middlegroundRenderTarget, _foregroundRenderTarget, _shadowRenderTarget;
        private float maxTransitionTime, transitionWaitTime, transitionTime = 0;
        private bool _transitioning = false, _paused = false, textBubbleShown = false;
        private TextBubble textBubble;
        private FadeState fadeState = FadeState.FadeIn;
        public bool Transitioning => _transitioning;
        private Action transitionAction;
        private Color fadeColor;
        private List<RectangleF> whiteLines;
        private bool Pixelated { get => Game.Settings.Pixelisation; }
        private float Zoom { get => Game.Settings.Zoom; }
        private Matrix Camera;
        public Daytime Daytime { get => _daytime; }
        private Daytime _daytime;
        public Date Date { get => _date; }
        private Date _date;

        public GameMap CurrentMap
        {
            get => _currentMap;
            set
            {
                _currentMap = value;
                _collisionComponent = new CollisionComponent(new RectangleF(0, 0, CurrentMap.Width, CurrentMap.Height));
                _collisionComponent.Initialize();
                rects = new List<RectangleF>();
                _reflectionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _waterRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _waterPixelatedRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _backgroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _middlegroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _foregroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _shadowRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                waterReflectionEffect.Parameters["TerrainTextureSize"]?.SetValue(new Vector2(CurrentMap.Width, CurrentMap.Height));
                RefreshCollisions();
                RefreshLines();
            }
        }
        public List<Entity> Entities { get; private set; }
        private CollisionComponent _collisionComponent;
        private List<RectangleF> rects;
        public Player Player => (Player)Entities[0];
        private Effect waterReflectionEffect;
        private Texture2D daylightCycleGradient;

        public GameScreen(MainGame game) : base(game)
        {
            _date = new Date(Game, () => { }, () => { }, () => { });
            _daytime = new Daytime(720, () => { _date += 1; });
        }

        public override void Initialize()
        {
            whiteLines = new List<RectangleF>();

            Maps = new List<GameMap>
            {
                new GameMap(Game, this, "Maps/test-map", "test-map")
            };

            Entities = new List<Entity>();

            foreach (GameMap map in Maps)
                map.Initialize();

            foreach (Entity entity in Entities)
                entity.Initialize();

            Elements.Add(new DaytimeLabel(Game, this));
            Elements.Add(new DateLabel(Game, this));
            Elements.Add(new PositionLabel(Game, this));

            base.Initialize();
        }

        public override void Load()
        {
            waterReflectionEffect = Game.Content.Load<Effect>("Effects/WaterMovement");
            waterReflectionEffect.Parameters["Height"]?.SetValue(Game.Settings.WaterDeepness);
            waterReflectionEffect.Parameters["EquationValueA"]?.SetValue(25f);
            waterReflectionEffect.Parameters["EquationValueB"]?.SetValue(32f);
            waterReflectionEffect.Parameters["EquationValueC"]?.SetValue(2f);
            waterReflectionEffect.Parameters["EquationValueD"]?.SetValue(24f);

            daylightCycleGradient = Game.Content.Load<Texture2D>("Textures/System/DayLightCycle");
            Texture2DHelper.PreloadTextureData(daylightCycleGradient);

            _currentMap = Maps[0];

            CurrentMap?.Load();

            CurrentMap = Maps[0];

            Entities.Add(new Player(this));
            //Entities.Add(new NPC(this, CurrentMap, new RectangleF(184, 88, 16, 16), Vector2.Zero, "Textures/Entities/Player"));
            Entities.Add(new ColasBimStatue(this, CurrentMap, new Vector2(184, 56)));
            Entities.Add(new JustRealxStatue(this, CurrentMap, new Vector2(136, 56)));
            Entities.Add(new SerothsStatue(this, CurrentMap, new Vector2(232, 56)));

            foreach (Entity entity in Entities)
            {
                entity.Initialize();
                entity.Load();
            }

            RefreshCollisions();

            base.Load();
        }

        private void RefreshCollisions()
        {
            foreach (CollisionRectangle collision in CurrentMap.Collisions)
            {
                _collisionComponent.Insert(collision);
                rects.Add((RectangleF)collision.Bounds);
            }

            foreach (TeleportationZone teleportationZone in CurrentMap.TeleportationZones)
            {
                _collisionComponent.Insert(teleportationZone);
                rects.Add((RectangleF)teleportationZone.Bounds);
            }

            foreach (var entity in Entities.Where(entity => entity.CurrentMap != null && entity.CurrentMap.Name.Equals(CurrentMap.Name)))
            {
                _collisionComponent.Insert(entity);
                rects.Add((RectangleF)entity.Bounds);
            }
        }

        private void RefreshLines()
        {
            Random random = new Random();

            int amountOfLines = CurrentMap.WidthInTiles * CurrentMap.HeightInTiles / 5;

            for (int i = 0; i < amountOfLines; i++)
            {
                whiteLines.Add(new RectangleF(random.Next(-16, CurrentMap.Width), random.Next(0, CurrentMap.Height), random.Next(4,16), 1));
            }
        }

        public void RefreshEntityCollision(Entity entity)
        {
            if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name)
            {
                if (!_collisionComponent.Contains(entity))
                    _collisionComponent.Insert(entity);
            }
            else
            {
                if (_collisionComponent.Contains(entity))
                    _collisionComponent.Remove(entity);
            }
        }
        public void Transition(Action action, float duration, float wait, Color fadeColor)
        {
            _transitioning = true;
            transitionWaitTime = wait;
            fadeState = FadeState.FadeIn;
            maxTransitionTime = duration;
            transitionAction = action;
            this.fadeColor = fadeColor;
        }

        public void OpenTextBubble(TextBubble bubble)
        {
            if (!textBubbleShown)
            {
                textBubbleShown = true;
                _paused = true;
                textBubble = bubble;
                Elements.Add(textBubble);
            }
        }

        public void CloseTextBubble()
        {
            if (textBubbleShown)
            {
                textBubbleShown = false;
                _paused = false;
                Elements.Remove(textBubble);
            }
        }

        public override void Update(GameTime gameTime)
        {
            waterReflectionEffect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            Random random = new Random();
            if (random.Next(0,10) == 0)
            {
                whiteLines.RemoveAt(random.Next(0, whiteLines.Count));
                whiteLines.Add(new RectangleF(random.Next(0, CurrentMap.Width - 1), random.Next(0, CurrentMap.Height - 1), random.Next(4, 16), 1));
            }
            if (!_transitioning && !_paused)
            {
                _daytime += (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (_daytime >= 1560)
                {
                    _daytime.SetTime(420f);
                }

                CurrentMap?.Update(gameTime);

                foreach (Entity entity in Entities)
                    entity.Update(gameTime);

                _collisionComponent.Update(gameTime);

                if (CurrentMap != null)
                {
                    LookAt(Player.PixelPosition);
                }

                base.Update(gameTime);
            }
            else if (_paused)
            {
                if (Game.Settings.WasKeyJustDown(KeyboardState, Settings.ConfirmKey))
                {
                    if (!textBubble.NextText())
                    {
                        CloseTextBubble();
                    }
                }

                base.Update(gameTime);
            }
            else if(_transitioning)
            {
                transitionTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
                float currentTransitionTime = maxTransitionTime;
                if (fadeState == FadeState.Wait)
                    currentTransitionTime = transitionWaitTime;
                if (transitionTime >= maxTransitionTime)
                {
                    transitionTime = 0;
                    if (fadeState == FadeState.FadeIn)
                    {
                        transitionAction.Invoke();

                        if (CurrentMap != null)
                        {
                            LookAt(Player.PixelPosition);
                        }
                    }

                    switch (fadeState)
                    {
                        case FadeState.FadeIn:
                            fadeState = FadeState.Wait;
                            break;
                        case FadeState.Wait:
                            fadeState = FadeState.FadeOut;
                            break;
                        case FadeState.FadeOut:
                            _transitioning = false;
                            break;
                    }
                }
            }
        }

        public void LookAt(Vector2 position)
        {
            float positionX;
            if (CurrentMap.Width < Game.MainTarget.Width / Zoom)
                positionX = (CurrentMap.Width - Game.MainTarget.Width / Zoom) / 2;
            else
                positionX = Math.Clamp(position.X - Game.MainTarget.Width / Zoom / 2, 0, CurrentMap.Width - Game.MainTarget.Width / Zoom);

            float positionY;
            if (CurrentMap.Height < Game.MainTarget.Height / Zoom)
                positionY = (CurrentMap.Height - Game.MainTarget.Height / Zoom) / 2;
            else
                positionY = Math.Clamp(position.Y - Game.MainTarget.Height / Zoom / 2, 0, CurrentMap.Height - Game.MainTarget.Height / Zoom);

            Camera = Matrix.CreateTranslation(-new Vector3(positionX, positionY, 0) * Zoom);
            Camera.M11 = Zoom;
            Camera.M22 = Zoom;
            Camera.M33 = Zoom;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            Game.SetRenderTarget(_reflectionRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawReflections(gameTime);

            Game.SetRenderTarget(_backgroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawBackground(gameTime);

            Game.SetRenderTarget(_middlegroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawMiddleground(gameTime);

            Game.SetRenderTarget(_foregroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawForeground(gameTime);

            SpriteBatch.End();

            Game.SetRenderTarget(_waterRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            SpriteBatch.Draw(_reflectionRenderTarget, Vector2.Zero, Color.White);

            foreach (Entity entity in Entities.OrderBy(entity => entity.Position.Y))
                if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name && entity.IsReflective)
                    entity.DrawReflection(gameTime);

            foreach (RectangleF rect in whiteLines)
            {
                SpriteBatch.DrawRectangle(rect, Color.White * 0.5f);
            }

            SpriteBatch.End();


            if (Pixelated)
            {
                Game.SetRenderTarget(_waterPixelatedRenderTarget);

                SpriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                waterReflectionEffect.Parameters["TerrainTexture"]?.SetValue(_backgroundRenderTarget);
                waterReflectionEffect.CurrentTechnique.Passes[0].Apply();

                Game.GraphicsDevice.Clear(Color.CornflowerBlue);

                SpriteBatch.Draw(_waterRenderTarget, Vector2.Zero, Color.White * 0.5f);

                SpriteBatch.End();
            }


            Game.SetRenderTarget(_shadowRenderTarget);

            Game.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            foreach (Entity entity in Entities.OrderBy(entity => entity.Position.Y))
                if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name && entity.HasShadow)
                    entity.DrawShadow(gameTime);

            SpriteBatch.End();


            Game.SetRenderTarget(null);

            if (Pixelated)
            {
                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera);

                SpriteBatch.Draw(_waterPixelatedRenderTarget, Vector2.Zero, Color.White);
            }
            else
            {
                SpriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera);
                waterReflectionEffect.Parameters["TerrainTexture"]?.SetValue(_backgroundRenderTarget);
                waterReflectionEffect.CurrentTechnique.Passes[0].Apply();

                Game.GraphicsDevice.Clear(Color.CornflowerBlue);

                SpriteBatch.Draw(_waterRenderTarget, Vector2.Zero, Color.White * 0.5f);

                SpriteBatch.End();

                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera);
            }

            SpriteBatch.Draw(_backgroundRenderTarget, Vector2.Zero, Color.White);

            SpriteBatch.Draw(_shadowRenderTarget, Vector2.Zero, Color.White * 0.25f);

            foreach (Entity entity in Entities.OrderBy(entity => entity.Position.Y))
                if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name)
                    entity.Draw(gameTime);

            //foreach (RectangleF rect in rects)
            //SpriteBatch.DrawRectangle(rect, Color.Blue);

            SpriteBatch.Draw(_foregroundRenderTarget, Vector2.Zero, Color.White);

            SpriteBatch.End();

            SpriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: CustomBlendStates.Multiply);

            SpriteBatch.FillRectangle(Game.GraphicsDevice.Viewport.Bounds, Texture2DHelper.GetPixel(daylightCycleGradient, (int)_daytime.Time, _date.Day + Date.DAYS_PER_SEASON * (int)_date.Season));

            SpriteBatch.End();

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            if (_transitioning)
            {
                Color color = fadeColor;
                switch (fadeState)
                {
                    case FadeState.FadeIn:
                        color.A = (byte)Math.Round(this.transitionTime / this.maxTransitionTime * 255);
                        break;
                    case FadeState.FadeOut:
                        color.A = (byte)Math.Round((1 - this.transitionTime / this.maxTransitionTime) * 255);
                        break;
                }
                SpriteBatch.FillRectangle(Game.GraphicsDevice.Viewport.Bounds, color);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera);

            foreach (RectangleF rectangle in rects)
            {
                SpriteBatch.DrawRectangle(rectangle, Color.White);
            }

            foreach (Entity entity in Entities)
            {
                entity.DrawDebug(gameTime);
            }

            SpriteBatch.End();
        }
    }

    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Wait
    }
}
