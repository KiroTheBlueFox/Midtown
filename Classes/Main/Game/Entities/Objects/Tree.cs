using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Maps;
using MonoGame.Extended;

namespace Midtown.Classes.Main.Game.Entities.Objects
{
    public class Tree : Entity
    {
        public bool IsDecorative { get; private set; }

        public Tree(GameScreen screen, GameMap currentMap, Vector2 position, Vector2 textureOffset, bool isDecorative) : base(screen, currentMap, new RectangleF(position - Vector2.One * 4, Vector2.One * 8), textureOffset, new Vector2(48, 24), new Vector2(-20, -52), true, true)
        {
            IsDecorative = isDecorative;
        }
    }
}
