using FontStashSharp;
using Midtown.Classes.Utils;

namespace Midtown.Classes.Main.Game.Interface
{
    public class Label : GameplayElement
    {
        protected DynamicSpriteFont font;
        public Label(MainGame game, GameScreen scene) : base(game, scene) { }

        public override void Load()
        {
            font = Game.Fonts[Fonts.NormalSmall].GetFont(8);

            base.Load();
        }
    }
}
