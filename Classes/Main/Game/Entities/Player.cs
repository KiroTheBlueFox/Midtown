using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Midtown.Classes.Main.Game.Entities
{
    public class Player : LivingEntity
    {
        private int speed = 100;

        public Player(GameplayScene sceneOn) : base(sceneOn, sceneOn.CurrentMap, new RectangleF(184, 120, 16, 16), "Textures/player.sf", "Textures/player")
        {
        }

        public override void Load()
        {
            base.Load();

            this.CurrentMap = SceneOn.CurrentMap;
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is TeleportationZone teleportationZone)
            {
                SceneOn.Transition(() => base.OnCollision(collisionInfo), 0.5f, 0.25f, Color.Black);
            }
            else
            {
                base.OnCollision(collisionInfo);
            }
        }

        public override void Update(GameTime gameTime)
        {
            
            Vector2 movement = Vector2.Zero;

            if (MainGame.Config.GetKeyState(ControlKeys.MoveDown) == KeyStates.Holding)
            {
                movement += Vector2.UnitY;
            }

            if (MainGame.Config.GetKeyState(ControlKeys.MoveUp) == KeyStates.Holding)
            {
                movement -= Vector2.UnitY;
            }

            if (MainGame.Config.GetKeyState(ControlKeys.MoveLeft) == KeyStates.Holding)
            {
                movement -= Vector2.UnitX;
            }

            if (MainGame.Config.GetKeyState(ControlKeys.MoveRight) == KeyStates.Holding)
            {
                movement += Vector2.UnitX;
            }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }

            Move(movement * speed * gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }
    }
}
