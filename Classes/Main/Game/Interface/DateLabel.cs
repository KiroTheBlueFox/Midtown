using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Midtown.Classes.Main.Game.Interface
{
    public class DateLabel : Label
    {
        public DateLabel(MainGame game, GameScreen scene) : base(game, scene) { }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(font, Screen.Date.ToString(), Vector2.Zero, Color.White);
        }
    }
}
