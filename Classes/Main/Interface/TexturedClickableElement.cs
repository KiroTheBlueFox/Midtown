using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Midtown.Classes.Main.Interface
{
    public class TexturedClickableElement : ClickableElement
    {
        public Texture2D Texture { get; protected set; }
        public Vector2 TextureOffset { get; protected set; }
        private string TexturePath;

        public TexturedClickableElement(MainGame game, Screen screen, int x, int y, int width, int height, string texturePath, Vector2 textureOffset, Action onClick) : base(game, screen, x, y, width, height, onClick)
        {
            TexturePath = texturePath;
            TextureOffset = textureOffset;
        }

        public override void Load()
        {
            Texture = Game.Content.Load<Texture2D>(TexturePath);

            base.Load();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(Texture, Rectangle.Center - Texture.Bounds.Size.ToVector2() / 2 + TextureOffset, Color.White);

            base.Draw(gameTime);
        }
    }
}
