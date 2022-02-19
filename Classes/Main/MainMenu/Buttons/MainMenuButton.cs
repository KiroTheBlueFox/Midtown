using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midtown.Classes.Main.Interface;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using System;

namespace Midtown.Classes.Main.MainMenu.Buttons
{
    public class MainMenuButton : ClickableElement
    {
        public string Text { get; private set; }
        public int OffsetX { get; private set; }
        private DynamicSpriteFont _font;
        private int _fontHeight;
        public bool Right { get; private set; }
        public Texture2D Gradient { get; private set; }
        private float HoverTime = 0;
        public Color TextColor, TextHoverColor, GradientColor;
        private Vector2 _position;

        public MainMenuButton(MainGame game, MainMenuScreen screen, string text, int x, int y, int offsetX, int fontHeight, bool right, Color textColor, Color textHoverColor, Color gradientColor, Action onClick) : base(game, screen, x, y, 0, 0, onClick)
        {
            Text = text;
            _fontHeight = fontHeight;
            OffsetX = offsetX;
            Right = right;
            _position = new Vector2(x, y);
            TextColor = textColor;
            TextHoverColor = textHoverColor;
            GradientColor = gradientColor;
        }

        public override void Load()
        {
            Gradient = Game.Content.Load<Texture2D>("Textures/MainMenu/Gradient");

            _font = Game.Fonts[Fonts.Bold, new FontSystemSettings() { Effect = FontSystemEffect.Stroked, EffectAmount = 1 }].GetFont(_fontHeight);

            base.Load();
        }

        public override void LoadLanguage()
        {
            Vector2 measures = _font.MeasureString(Game.Language[Text]);

            if (Right)
                Rectangle = new RectangleF(_position - new Vector2(OffsetX + measures.X, 0), new Vector2(measures.X, _font.LineHeight));
            else
                Rectangle = new RectangleF(_position + new Vector2(OffsetX, 0), new Vector2(measures.X, _font.LineHeight));

            base.LoadLanguage();
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsHovered)
            {
                SpriteBatch.Draw(Gradient, new Rectangle((int)Rectangle.X - (Right ? 0 : OffsetX), (int)Rectangle.Y, (int)(Rectangle.Width + OffsetX), (int)Rectangle.Height), null, GradientColor * (1 - MathF.Abs(HoverTime - 1) / 2), 0, Vector2.Zero, Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            SpriteBatch.DrawString(_font, Game.Language[Text], Rectangle.Position, IsHovered ? TextHoverColor : TextColor);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsHovered)
            {
                HoverTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (HoverTime >= 2)
                {
                    HoverTime = 0;
                }
            }
            else if (HoverTime != 0)
                HoverTime = 0;

            base.Update(gameTime);
        }
    }
}
