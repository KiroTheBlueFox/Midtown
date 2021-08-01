using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Midtown.Classes.Main
{
    public class GameElement : GameComponent
    {
        public int DrawLayer { get; }
        public Texture2D CurrentTexture;
        public MainGame MainGame { get; }
        public Scene Scene { get; }
        public GameElement(MainGame game, Scene scene) : base(game)
        {
            MainGame = game;
            Scene = scene;
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Draw(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Load() { }
    }
}
