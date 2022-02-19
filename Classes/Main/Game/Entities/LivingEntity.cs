using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Midtown.Classes.Utils;

namespace Midtown.Classes.Main.Game.Entities
{
    public class LivingEntity : Entity
    {
        private AnimatedTexture2D sprite;
        private readonly string texturePath;
        public static readonly int SPEED = 50;

        public LivingEntity(GameScreen sceneOn, GameMap map, RectangleF bounds, Vector2 textureOffset, Vector2? shadowSize, Vector2? shadowOffset, string texturePath, bool collidable) : base(sceneOn, map, bounds, textureOffset, shadowSize, shadowOffset, true, collidable)
        {
            this.texturePath = texturePath;
        }

        public override void Initialize()
        {
            base.Initialize();

            sprite = new AnimatedTexture2D(Game, Screen, texturePath+".json", texturePath);
        }

        public override void Load()
        {
            base.Load();

            sprite.Load();
            sprite.Play("idleDown");
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is TeleportationZone teleportationZone)
            {
                CurrentMap = teleportationZone.DestinationMap;
                Position = teleportationZone.DestinationPosition;
                _lastPosition = Position;
            }
            else
            {
                base.OnCollision(collisionInfo);
            }
        }

        public override void Update(GameTime gameTime)
        {
            bool samePos = _lastPosition == Position;
            string animation = Direction switch
            {
                Direction.Left => samePos ? "idleLeft" : "walkLeft",
                Direction.Right => samePos ? "idleRight" : "walkRight",
                Direction.Up => samePos ? "idleUp" : "walkUp",
                _ => samePos ? "idleDown" : "walkDown",
            };

            sprite.Play(animation);
            sprite.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(gameTime, PixelPosition + TextureOffset);
        }

        public override void DrawReflection(GameTime gameTime)
        {
            sprite.DrawReflection(gameTime, PixelPosition + TextureOffset);
        }
    }
}
