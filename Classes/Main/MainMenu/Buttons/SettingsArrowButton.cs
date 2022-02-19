using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Interface;
using System;

namespace Midtown.Classes.Main.MainMenu.Buttons
{
    public class SettingsArrowButton : TexturedClickableElement
    {
        public static readonly int SIZE = 15;
        public bool Right;

        public SettingsArrowButton(MainGame game, Screen screen, int x, int y, Action onClick, bool right) : base(game, screen, x, y, SIZE, SIZE, "Textures/MainMenu/Arrow", Vector2.Zero, onClick)
        {
            Right = right;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(Texture, new Rectangle(new Point((int)Rectangle.Center.X, (int)Rectangle.Center.Y) - (Texture.Bounds.Size.ToVector2() / 2).ToPoint() + TextureOffset.ToPoint(), Texture.Bounds.Size), null, IsHovered ? Color.Yellow : Color.Black, 0, Vector2.Zero, Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
