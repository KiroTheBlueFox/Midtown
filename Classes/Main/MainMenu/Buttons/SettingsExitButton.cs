using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Interface;
using System;

namespace Midtown.Classes.Main.MainMenu.Buttons
{
    public class SettingsExitButton : TexturedClickableElement
    {
        public static readonly int SIZE = 15;
        public Color Color { get; }

        public SettingsExitButton(MainGame game, Screen screen, int x, int y, Color color, Action onClick) : base(game, screen, x, y, SIZE, SIZE, "Textures/MainMenu/Cross", Vector2.Zero, onClick)
        {
            Color = color;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(Texture, Rectangle.Center - Texture.Bounds.Size.ToVector2() / 2 + TextureOffset, IsHovered ? Color.Red : Color);
        }
    }
}
