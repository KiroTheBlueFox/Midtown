using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.MapElements;
using Midtown.Classes.Main.Game.Maps;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Main.Game.Entities
{
    public class Player : LivingEntity
    {
        public static readonly int RUNNING_SPEED = 100;
        public static readonly float INTERACTION_RANGE = 1.5f;
        public float InteractionRange { get => INTERACTION_RANGE * CurrentMap.TileSize; }
        public new GameMap CurrentMap {
            get => Screen.CurrentMap;
            set => Screen.CurrentMap = value;
        }

        public Player(GameScreen sceneOn) : base(sceneOn, sceneOn.CurrentMap, new RectangleF(184, 120, 16, 16), new Vector2(-8, -8), new Vector2(-8, 0), new Vector2(-8, -8), "Textures/Entities/Player", true)
        {
        }

        public override void Load()
        {
            base.Load();
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is TeleportationZone teleportationZone)
            {
                Screen.Transition(() => base.OnCollision(collisionInfo), 0.5f, 0.25f, Color.Black);
            }
            else
            {
                base.OnCollision(collisionInfo);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;

            if (Game.Settings.IsKeyDown(KeyboardState, Settings.MoveDownKey))
                movement += Vector2.UnitY;
            if (Game.Settings.IsKeyDown(KeyboardState, Settings.MoveUpKey))
                movement -= Vector2.UnitY;
            if (Game.Settings.IsKeyDown(KeyboardState, Settings.MoveLeftKey))
                movement -= Vector2.UnitX;
            if (Game.Settings.IsKeyDown(KeyboardState, Settings.MoveRightKey))
                movement += Vector2.UnitX;
            if (movement != Vector2.Zero)
                movement.Normalize();

            if (Game.Settings.WasKeyJustDown(KeyboardState, Settings.InteractKey))
            {
                Dictionary<IInteractable, float> interactableEntities = new Dictionary<IInteractable, float>();
                foreach (Entity entity in Screen.Entities.FindAll(Entity => Entity.CurrentMap == CurrentMap))
                {
                    if (entity is IInteractable interactable)
                    {
                        Vector2 distance = entity.Position - Position;
                        switch (Direction)
                        {
                            case Direction.Up:
                                if (Math.Abs(distance.X) < InteractionRange / 2f && distance.Y < 0 && Math.Abs(distance.Y) < InteractionRange)
                                {
                                    interactableEntities.Add(interactable, distance.Length());
                                }
                                break;
                            default:
                            case Direction.Down:
                                if (Math.Abs(distance.X) < InteractionRange / 2f && distance.Y > 0 && Math.Abs(distance.Y) < InteractionRange)
                                {
                                    interactableEntities.Add(interactable, distance.Length());
                                }
                                break;
                            case Direction.Left:
                                if (Math.Abs(distance.Y) < InteractionRange / 2f && distance.X < 0 && Math.Abs(distance.X) < InteractionRange)
                                {
                                    interactableEntities.Add(interactable, distance.Length());
                                }
                                break;
                            case Direction.Right:
                                if (Math.Abs(distance.Y) < InteractionRange / 2f && distance.X > 0 && Math.Abs(distance.X) < InteractionRange)
                                {
                                    interactableEntities.Add(interactable, distance.Length());
                                }
                                break;
                        }
                    }
                }
                KeyValuePair<IInteractable, float> closest = new KeyValuePair<IInteractable, float>();
                foreach (KeyValuePair<IInteractable, float> pair in interactableEntities)
                {
                    if (closest.Key == null || pair.Value < closest.Value)
                        closest = pair;
                }
                if (closest.Key != null)
                    closest.Key.Interact();
            }

            float speed = RUNNING_SPEED;
            if (Game.Settings.IsKeyDown(KeyboardState, Settings.WalkKey))
                speed = SPEED;

            Move(movement * (float) speed * gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }
    }
}
