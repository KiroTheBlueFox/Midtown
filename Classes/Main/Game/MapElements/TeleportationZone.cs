using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;

namespace Midtown.Classes.Main.Game.MapElements
{
    public class TeleportationZone : CollisionRectangle
    {
        public GameMap DestinationMap { get; }
        public Vector2 DestinationPosition { get; }
        public bool Transition { get; }

        public TeleportationZone(GameMap map, RectangleF collisionRect, GameMap destinationMap, Vector2 destinationPosition, bool transition) : base(map, collisionRect)
        {
            this.DestinationMap = destinationMap;
            this.DestinationPosition = destinationPosition;
            this.Transition = transition;
        }
    }
}
