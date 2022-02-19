using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Midtown.Classes.Main.Game.Interface
{
    public class PositionLabel : Label
    {
        public PositionLabel(MainGame game, GameScreen scene) : base(game, scene) { }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(font, Screen.Player.Position.ToString(), new Vector2(Game.MainTarget.Width - font.MeasureString(Screen.Player.Position.ToString()).X, 0), Color.White);
        }
    }
}
