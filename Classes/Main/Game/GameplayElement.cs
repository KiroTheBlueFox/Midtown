namespace Midtown.Classes.Main.Game
{
    public class GameplayElement : GameElement
    {
        public new readonly GameScreen Screen;

        public GameplayElement(MainGame game, GameScreen screen) : base(game, screen)
        {
            Screen = screen;
        }
    }
}
