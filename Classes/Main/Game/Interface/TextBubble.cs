using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Utils;

namespace Midtown.Classes.Main.Game.Interface
{
    public class TextBubble : GameplayElement
    {
        private readonly string[] texts;
        private readonly string texturePath, characterImagePath;
        private Texture2D bubbleTexture, characterImageTexture;
        public readonly static Vector2 DEFAULT_SIZE = new Vector2(24, 5), DEFAULT_POSITION = new Vector2(0.5f, 9.5f), DEFAULT_TEXT_OFFSET = new Vector2(0.5f, 0.5f), DEFAULT_TEXT_OFFSET_WITH_CHARACTER_IMAGE = new Vector2(5, 0.5f), DEFAULT_CHARACTER_IMAGE_OFFSET = new Vector2(0.5f,0.5f);
        private readonly Vector2 size, position, textOffset, characterImageOffset;
        public readonly static float UI_SIZE = 1f;
        private DynamicSpriteFont font;
        private int text;

        public TextBubble(MainGame game, GameScreen Screen, string[] texts, string texturePath, string characterImagePath) : this(game, Screen, texts, texturePath, characterImagePath, new Vector2(game.MainTarget.Width / Screen.CurrentMap.TileSize - 1, 5), new Vector2(0.5f, game.MainTarget.Height / Screen.CurrentMap.TileSize - 5.5f), (characterImagePath == null) ? DEFAULT_TEXT_OFFSET : DEFAULT_TEXT_OFFSET_WITH_CHARACTER_IMAGE, DEFAULT_CHARACTER_IMAGE_OFFSET) { }

        public TextBubble(MainGame game, GameScreen Screen, string[] texts, string texturePath, string characterImagePath, Vector2 size, Vector2 position, Vector2 textOffset, Vector2 characterImageOffset) : base(game, Screen)
        {
            this.texts = texts;
            this.texturePath = texturePath;
            this.characterImagePath = characterImagePath;
            this.size = size;
            this.position = position;
            this.textOffset = textOffset;
            this.characterImageOffset = characterImageOffset;
            Load();
        }

        public void SetFont(DynamicSpriteFont font)
        {
            this.font = font;
        }

        public override void Load()
        {
            bubbleTexture = Game.Content.Load<Texture2D>(texturePath);
            if (characterImagePath != null)
                characterImageTexture = Game.Content.Load<Texture2D>(characterImagePath);
            font = Game.Fonts[Fonts.Normal].GetFont(16);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 1; i < size.X - 1; i++)
            {
                SpriteBatch.Draw(bubbleTexture, (position + new Vector2(i, 0)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(16, 0, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
                SpriteBatch.Draw(bubbleTexture, (position + new Vector2(i, size.Y - 1)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(16, 32, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
                for (int j = 1; j < size.Y - 1; j++)
                {
                    SpriteBatch.Draw(bubbleTexture, (position + new Vector2(i, j)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(16, 16, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
                }
            }
            for (int i = 1; i < size.Y - 1; i++)
            {
                SpriteBatch.Draw(bubbleTexture, (position + new Vector2(0, i)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(0, 16, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
                SpriteBatch.Draw(bubbleTexture, (position + new Vector2(size.X - 1, i)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(32, 16, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
            }

            SpriteBatch.Draw(bubbleTexture, position * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(0, 0, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
            SpriteBatch.Draw(bubbleTexture, (position + new Vector2(size.X - 1, 0)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(32, 0, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
            SpriteBatch.Draw(bubbleTexture, (position + new Vector2(size.X - 1, size.Y - 1)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(32, 32, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);
            SpriteBatch.Draw(bubbleTexture, (position + new Vector2(0, size.Y - 1)) * Screen.CurrentMap.TileSize * UI_SIZE, new Rectangle(0, 32, 16, 16), Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);

            if (characterImageTexture != null)
                SpriteBatch.Draw(characterImageTexture, (position + characterImageOffset) * Screen.CurrentMap.TileSize * UI_SIZE, characterImageTexture.Bounds, Color.White, 0, Vector2.Zero, UI_SIZE, SpriteEffects.None, 0);

            if (texts[text] != null)
                SpriteBatch.DrawString(font, texts[text], (position + textOffset) * Screen.CurrentMap.TileSize * UI_SIZE, Color.Black);
            //font.DrawText(SpriteBatch, texts[text], (position + textOffset) * Screen.CurrentMap.TileSize * UI_SIZE);
        }

        public bool NextText()
        {
            if (text + 1 >= texts.Length)
                return false;
            else
                text++;
            return true;
        }
    }
}
