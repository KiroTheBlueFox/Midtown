using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Midtown.Classes.Main.Game.MapElements
{
    public class CollisionRectangle : ICollisionActor
    {
        public IShapeF Bounds { get; }
        public GameMap CurrentMap;

        public CollisionRectangle(GameMap map, RectangleF collisionRect)
        {
            this.CurrentMap = map;
            this.Bounds = collisionRect;
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo) { }
    }
}
