using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;

namespace Midtown.Classes.Main
{
    public class GameElement : GameComponent
    {
        public readonly int DrawLayer;
        public Texture2D CurrentTexture;
        public new readonly MainGame Game;
        public readonly Screen Screen;
        public KeyboardStateExtended KeyboardState { get => Game.KeyboardState; }
        public MouseStateExtended MouseState { get => Game.MouseState; }
        public SpriteBatch SpriteBatch { get => Game.SpriteBatch; }
        public GameElement(MainGame game, Screen screen) : base(game)
        {
            Game = game;
            Screen = screen;
        }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void Draw(Matrix matrix, GameTime gameTime) { }
        public virtual void DrawDebug(GameTime gameTime) { }
        public virtual void DrawDebug(Matrix matrix, GameTime gameTime) { }
        public virtual void Load() { }
        public virtual void LoadLanguage() { }
    }
}
