using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Midtown.Classes.Main
{
    public class Scene : GameComponent
    {
        public List<GameElement> Elements;
        public MainGame MainGame { get; }

        public Scene(MainGame game) : base(game)
        {
            MainGame = game;

            this.Elements = new List<GameElement>();
        }

        public override void Initialize()
        {
            foreach (GameElement element in Elements)
            {
                element.Initialize();
            }

            base.Initialize();
        }

        public virtual void Load()
        {
            foreach (GameElement element in Elements)
            {
                element.Load();
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameElement element in Elements)
            {
                element.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameElement element in Elements)
            {
                element.Draw(gameTime, spriteBatch);
            }
        }
    }
}
