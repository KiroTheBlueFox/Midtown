using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game.Maps;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;

namespace Midtown.Classes.Main.Game.Entities
{
    public abstract class Entity : GameElement, ICollisionActor
    {
        public IShapeF Bounds { get; set; }
        protected Vector2 _lastPosition;
        public Vector2 TextureOffset { get; }
        public new GameScreen Screen;
        private GameMap _currentMap;
        public readonly bool Collidable;
        private Direction _lastDirection;
        public Texture2D CircleTexture { get; private set; }
        public bool HasShadow { get; protected set; } = true;
        public Vector2 ShadowSize { get; }
        public Vector2 ShadowOffset { get; }
        public bool IsReflective { get; }
        public Direction Direction
        {
            get
            {
                if (Position != _lastPosition)
                {
                    Vector2 lastMovement = Position - _lastPosition;
                    float angle = MathHelper.ToDegrees((float)Math.Atan2(lastMovement.X, lastMovement.Y));
                    if (angle > 45 && angle < 135)
                    {
                        _lastDirection = Direction.Right;
                        return Direction.Right;
                    }
                    else if (angle > -135 && angle < -45)
                    {
                        _lastDirection = Direction.Left;
                        return Direction.Left;
                    }
                    else if (angle > 135 || angle < -135)
                    {
                        _lastDirection = Direction.Up;
                        return Direction.Up;
                    }
                    else if (angle > -45 && angle < 45)
                    {
                        _lastDirection = Direction.Down;
                        return Direction.Down;
                    }
                    else
                    {
                        return _lastDirection;
                    }
                }
                else
                {
                    return _lastDirection;
                }
            }
        }

        public GameMap CurrentMap
        {
            get => _currentMap;
            set
            {
                _currentMap = value;
                Screen.RefreshEntityCollision(this);
            }
        }
        public Vector2 Position
        {
            get => ((RectangleF)Bounds).Center;
            set => Bounds.Position = value;
        }

        public Vector2 PixelPosition
        {
            get => new Vector2(MathF.Round(Position.X), MathF.Round(Position.Y));
        }

        protected Entity(GameScreen screen, GameMap currentMap, RectangleF bounds, Vector2 textureOffset, Vector2? shadowSize, Vector2? shadowOffset, bool isReflective, bool collidable) : base(screen.Game, screen)
        {
            Bounds = bounds;
            ShadowSize = shadowSize ?? Vector2.Zero;
            ShadowOffset = shadowOffset ?? Vector2.Zero;
            TextureOffset = textureOffset;
            Screen = screen;
            _currentMap = currentMap;
            _lastDirection = Direction.Down;
            IsReflective = isReflective;
            Collidable = collidable;
        }

        public override void Load()
        {
            CircleTexture = Game.Content.Load<Texture2D>("Textures/System/Circle");

            base.Load();
        }

        public virtual void DrawShadow(GameTime gameTime)
        {
            SpriteBatch.Draw(CircleTexture, new Rectangle((PixelPosition + new Vector2(0, ((RectangleF) Bounds).Height ) - ShadowSize / 2f + ShadowOffset).ToPoint(), ShadowSize.ToPoint()), CircleTexture.Bounds, Color.Black);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(CurrentTexture, PixelPosition - new Vector2(CurrentTexture.Width / 2, CurrentTexture.Height / 2) + TextureOffset, Color.White);

            base.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            SpriteBatch.DrawRectangle((RectangleF)Bounds, Color.Blue);

            base.Draw(gameTime);
        }

        public virtual void DrawReflection(GameTime gameTime)
        {
            if (CurrentTexture != null)
                SpriteBatch.Draw(CurrentTexture, new Vector2(MathF.Round(((RectangleF)Bounds).BottomLeft.X), MathF.Round(((RectangleF)Bounds).BottomLeft.Y)) - new Vector2(0, CurrentTexture.Height), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0f);
        }

        public void Move(Vector2 movement)
        {
            _lastPosition = Position;

            Point2 newPosition = Bounds.Position + movement;

            if (newPosition.X < 0)
                newPosition.X = 0;
            else if (newPosition.X > CurrentMap.Width - ((RectangleF)Bounds).Width)
                newPosition.X = CurrentMap.Width - ((RectangleF)Bounds).Width;

            if (newPosition.Y < 0)
                newPosition.Y = 0;
            else if (newPosition.Y > CurrentMap.Height - ((RectangleF)Bounds).Height)
                newPosition.Y = CurrentMap.Height - ((RectangleF)Bounds).Height;

            Bounds.Position = newPosition;
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is Entity entity)
            {
                if (Collidable && entity.Collidable)
                {
                    Bounds.Position -= collisionInfo.PenetrationVector;
                }
            }
            else
            {
                Bounds.Position -= collisionInfo.PenetrationVector;
            }
        }
    }
}
