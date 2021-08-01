using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Main.Game.Maps;
using MonoGameTestProject.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Utils;

namespace Midtown.Classes.Main.Game.Entities
{
    public class LivingEntity : Entity
    {
        private AnimatedTexture sprite;
        private readonly string spritePath, texturePath;

        public LivingEntity(GameplayScene sceneOn, GameMap map, RectangleF bounds, string spritePath, string texturePath) : base(sceneOn, map, bounds, true)
        {
            this.spritePath = spritePath;
            this.texturePath = texturePath;
        }

        public override void Initialize()
        {
            base.Initialize();

            sprite = new AnimatedTexture(MainGame, SceneOn, spritePath, texturePath, this);
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
                this.CurrentMap = teleportationZone.DestinationMap;
                this.Position = teleportationZone.DestinationPosition;
                this._lastPosition = this.Position;
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.sprite.Draw(gameTime, spriteBatch);
        }

        public override void DrawReflection(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.sprite.DrawReflection(gameTime, spriteBatch);
        }
    }
}
