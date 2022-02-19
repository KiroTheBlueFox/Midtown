using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Midtown.Classes.Main.Game.Interface
{
    public class DaytimeLabel : Label
    {
        public DaytimeLabel(MainGame game, GameScreen scene) : base(game, scene) { }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(font, Screen.Daytime.ToString(), new Vector2(0, font.LineHeight), Color.White);
        }
    }
}
