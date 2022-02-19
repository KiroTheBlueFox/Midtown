using FontStashSharp;
using Microsoft.Xna.Framework;
using Midtown.Classes.Utils;
using MonoGame.Extended;

namespace Midtown.Classes.Main.MainMenu.Interface
{
    public class SettingCategory : GameElement
    {
        public string Name { get; private set; }
        public int LineSize { get; private set; }
        public Color LineColor { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public RectangleF RectangleF { get => Rectangle; }
        private DynamicSpriteFont _font;
        private int _fontHeight;

        public SettingCategory(MainGame game, Screen screen, string name, int x, int y, int width, int height, int fontHeight, Color lineColor, int lineSize) : base(game, screen)
        {
            Name = name;
            Rectangle = new Rectangle(x, y, width, height);
            _fontHeight = fontHeight;
            LineColor = lineColor;
            LineSize = lineSize;
        }

        public override void Load()
        {
            _font = Game.Fonts[Fonts.Bold].GetFont(_fontHeight);

            base.Load();
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.FillRectangle(new Rectangle(Rectangle.Left, Rectangle.Top + (Rectangle.Height - _fontHeight + LineSize) / 2 /*- LineSize / 2*/, Rectangle.Width, LineSize), LineColor);

            SpriteBatch.DrawString(_font, Game.Language[Name], RectangleF.BottomLeft - _fontHeight * Vector2.UnitY, Color.Black);
        }
    }
}
