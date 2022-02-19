using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Main.Game.Entities
{
    public class NPC : LivingEntity, IInteractable
    {
        private readonly List<Tuple<string, int, Action>> Schedule;

        public NPC(GameScreen sceneOn, GameMap map, RectangleF bounds, Vector2 textureOffset, Vector2? shadowSize, Vector2? shadowOffset, string texturePath) : base(sceneOn, map, bounds, textureOffset, shadowSize, shadowOffset, texturePath, true)
        {
            Schedule = new List<Tuple<string, int, Action>>();
        }

        public Tuple<string, int, Action> GetScheduledEvent(string id)
        {
            foreach (Tuple<string, int, Action> scheduleEvent in Schedule) {
                if (scheduleEvent.Item1 == id)
                    return scheduleEvent;
            }
            return null;
        }

        public void AddScheduledEvent(string id, int time, Action action)
        {
            Schedule.Add(new Tuple<string, int, Action>(id, time, action));
        }

        public void Interact()
        {
        }
    }
}
