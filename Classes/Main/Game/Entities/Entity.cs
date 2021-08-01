using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGameTestProject.Classes.Utils;
using System;

namespace Midtown.Classes.Main.Game.Entities
{
    public abstract class Entity : GameElement, ICollisionActor
    {
        public IShapeF Bounds { get; set; }
        protected Vector2 _lastPosition;
        public GameplayScene SceneOn;
        private GameMap _currentMap;
        private Direction _lastDirection;
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
                SceneOn.RefreshEntityCollision(this);
            }
        }
        public Vector2 Position
        {
            get => ((RectangleF)this.Bounds).Center;
            set => this.Bounds.Position = value;
        }

        protected Entity(GameplayScene sceneOn, GameMap currentMap, RectangleF bounds, bool isReflective) : base(sceneOn.MainGame, sceneOn)
        {
            this.Bounds = bounds;
            this.SceneOn = sceneOn;
            this._currentMap = currentMap;
            this._lastDirection = Direction.Down;
            this.IsReflective = isReflective;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CurrentTexture, Bounds.Position, Color.White);

            base.Draw(gameTime, spriteBatch);
        }

        public virtual void DrawReflection(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (CurrentTexture != null)
                spriteBatch.Draw(CurrentTexture, ((RectangleF)this.Bounds).BottomLeft - new Vector2(0, CurrentTexture.Height), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0f);
        }

        public void Move(Vector2 movement)
        {
            _lastPosition = this.Position;

            this.Bounds.Position += movement;
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            this.Bounds.Position -= collisionInfo.PenetrationVector;
        }
    }
}
