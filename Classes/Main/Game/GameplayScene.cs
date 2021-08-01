using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game.Entities;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Midtown.Classes.Main.Game
{
    public class GameplayScene : Scene
    {
        private readonly Random _random = new Random();
        public List<GameMap> Maps;
        private GameMap _currentMap;
        private RenderTarget2D _reflectionRenderTarget, _waterRenderTarget, _waterPixelatedRenderTarget, _backgroundRenderTarget, _middlegroundRenderTarget, _foregroundRenderTarget;
        private float maxTransitionTime, transitionWaitTime, transitionTime = 0;
        private bool _transitioning = false;
        private FadeState fadeState = FadeState.FadeIn;
        public bool Transitioning => _transitioning;
        private Action transitionAction;
        private Color fadeColor;
        private List<RectangleF> whiteLines;
        private bool Pixelated { get => MainGame.Config.Pixelisation; }

        public GameMap CurrentMap
        {
            get => _currentMap;
            set
            {
                _currentMap = value;
                _collisionComponent = new CollisionComponent(new RectangleF(0, 0, CurrentMap.Width, CurrentMap.Height));
                _collisionComponent.Initialize();
                rects = new List<RectangleF>();
                _reflectionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                _waterRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                _waterPixelatedRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                _backgroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                _middlegroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                _foregroundRenderTarget = new RenderTarget2D(Game.GraphicsDevice, CurrentMap.Width, CurrentMap.Height);
                waterReflectionEffect.Parameters["TerrainTextureSize"]?.SetValue(new Vector2(CurrentMap.Width, CurrentMap.Height));
                RefreshCollisions();
                RefreshLines();
            }
        }
        public OrthographicCamera Camera;
        private List<Entity> _entities;
        private CollisionComponent _collisionComponent;
        private List<RectangleF> rects;
        public Player Player => (Player)this._entities[0];
        private Effect waterReflectionEffect;

        public GameplayScene(MainGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            var viewportadapter = new BoxingViewportAdapter(MainGame.Window, Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            Camera = new OrthographicCamera(viewportadapter)
            {
                MaximumZoom = 3f,
                MinimumZoom = 2f
            };

            whiteLines = new List<RectangleF>();

            Maps = new List<GameMap>
            {
                new GameMap(MainGame, this, "Maps/test-map", "test-map")
            };

            _entities = new List<Entity>();

            foreach (GameMap map in Maps)
                map.Initialize();

            foreach (Entity entity in _entities)
                entity.Initialize();

            base.Initialize();
        }

        public override void Load()
        {
            waterReflectionEffect = Game.Content.Load<Effect>("Effects/WaterMovement");
            waterReflectionEffect.Parameters["Height"]?.SetValue(MainGame.Config.WaterDeepness);

            Camera.Zoom = 2 * MainGame.Config.Zoom;

            _currentMap = Maps[0];

            CurrentMap?.Load();

            CurrentMap = Maps[0];

            _entities.Add(new Player(this));
            _entities[0].Initialize();

            foreach (Entity entity in _entities)
                entity.Load();

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

            foreach (var entity in _entities.Where(entity => entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name))
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
            this._transitioning = true;
            this.transitionWaitTime = wait;
            this.fadeState = FadeState.FadeIn;
            this.maxTransitionTime = duration;
            this.transitionAction = action;
            this.fadeColor = fadeColor;
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
            if (!_transitioning)
            {
                CurrentMap?.Update(gameTime);

                foreach (Entity entity in _entities)
                    entity.Update(gameTime);

                _collisionComponent.Update(gameTime);

                if (CurrentMap != null)
                {
                    Camera.LookAt(new Vector2(
                        (CurrentMap.Width * Camera.Zoom < Game.GraphicsDevice.Viewport.Width) ? CurrentMap.Width / 2 :
                            Math.Clamp(Player.Position.X, Game.GraphicsDevice.Viewport.Width / Camera.Zoom / 2,
                            CurrentMap.Width - Game.GraphicsDevice.Viewport.Width / Camera.Zoom / 2),
                        (CurrentMap.Height * Camera.Zoom < Game.GraphicsDevice.Viewport.Height) ? CurrentMap.Height / 2 :
                            Math.Clamp(Player.Position.Y, Game.GraphicsDevice.Viewport.Height / Camera.Zoom / 2,
                            CurrentMap.Height - Game.GraphicsDevice.Viewport.Height / Camera.Zoom / 2)));
                }

                base.Update(gameTime);
            }
            else
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
                            Camera.LookAt(new Vector2(
                                (CurrentMap.Width * Camera.Zoom < Game.GraphicsDevice.Viewport.Width) ? CurrentMap.Width / 2 :
                                    Math.Clamp(Player.Position.X, Game.GraphicsDevice.Viewport.Width / Camera.Zoom / 2,
                                    CurrentMap.Width - Game.GraphicsDevice.Viewport.Width / Camera.Zoom / 2),
                                (CurrentMap.Height * Camera.Zoom < Game.GraphicsDevice.Viewport.Height) ? CurrentMap.Height / 2 :
                                    Math.Clamp(Player.Position.Y, Game.GraphicsDevice.Viewport.Height / Camera.Zoom / 2,
                                    CurrentMap.Height - Game.GraphicsDevice.Viewport.Height / Camera.Zoom / 2)));
                        }
                    }

                    switch (fadeState)
                    {
                        case FadeState.FadeIn:
                            this.fadeState = FadeState.Wait;
                            break;
                        case FadeState.Wait:
                            this.fadeState = FadeState.FadeOut;
                            break;
                        case FadeState.FadeOut:
                            this._transitioning = false;
                            break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            Game.GraphicsDevice.SetRenderTarget(_reflectionRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawReflections(gameTime, spriteBatch);

            Game.GraphicsDevice.SetRenderTarget(_backgroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawBackground(gameTime, spriteBatch);

            Game.GraphicsDevice.SetRenderTarget(_middlegroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawMiddleground(gameTime, spriteBatch);

            Game.GraphicsDevice.SetRenderTarget(_foregroundRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);
            CurrentMap?.DrawForeground(gameTime, spriteBatch);

            spriteBatch.End();

            Game.GraphicsDevice.SetRenderTarget(_waterRenderTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(_reflectionRenderTarget, Vector2.Zero, Color.White);

            foreach (Entity entity in _entities.OrderBy(entity => entity.Position.Y))
                if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name && entity.IsReflective)
                    entity.DrawReflection(gameTime, spriteBatch);

            foreach (RectangleF rect in whiteLines)
            {
                spriteBatch.DrawRectangle(rect, Color.White * 0.5f);
            }

            spriteBatch.End();


            if (Pixelated)
            {
                Game.GraphicsDevice.SetRenderTarget(_waterPixelatedRenderTarget);

                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                waterReflectionEffect.Parameters["TerrainTexture"]?.SetValue(_backgroundRenderTarget);
                waterReflectionEffect.CurrentTechnique.Passes[0].Apply();

                Game.GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Draw(_waterRenderTarget, Vector2.Zero, Color.White * 0.5f);

                spriteBatch.End();
            }


            Game.GraphicsDevice.SetRenderTarget(null);

            if (Pixelated)
            {
                spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix());

                spriteBatch.Draw(_waterPixelatedRenderTarget, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix());
                waterReflectionEffect.Parameters["TerrainTexture"]?.SetValue(_backgroundRenderTarget);
                waterReflectionEffect.CurrentTechnique.Passes[0].Apply();

                Game.GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Draw(_waterRenderTarget, Vector2.Zero, Color.White * 0.5f);

                spriteBatch.End();

                spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix());
            }

            spriteBatch.Draw(_backgroundRenderTarget, Vector2.Zero, Color.White);

            foreach (Entity entity in _entities.OrderBy(entity => entity.Position.Y))
                if (entity.CurrentMap != null && entity.CurrentMap.Name == CurrentMap.Name)
                    entity.Draw(gameTime, spriteBatch);

            //foreach (RectangleF rect in rects)
                //spriteBatch.DrawRectangle(rect, Color.Blue);

            spriteBatch.Draw(_foregroundRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

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
                spriteBatch.FillRectangle(Game.GraphicsDevice.Viewport.Bounds, color);
            }

            spriteBatch.End();
        }
    }

    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Wait
    }
}
