using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace Midtown.Classes.Main.Interface
{
    public abstract class ClickableElement : GameElement
    {
        public RectangleF Rectangle { get; set; }
        public Action OnClick { get; protected set; }
        public bool IsHovered;
        public readonly MouseCursor CursorWhenHovered = MouseCursor.Hand;

        public ClickableElement(MainGame game, Screen screen, int x, int y, int width, int height, Action onClick) : base(game, screen)
        {
            Rectangle = new RectangleF(x, y, width, height);
            OnClick = onClick;
        }
    }
}
