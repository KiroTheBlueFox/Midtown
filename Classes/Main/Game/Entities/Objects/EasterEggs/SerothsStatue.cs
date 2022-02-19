using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Interface;
using Midtown.Classes.Main.Game.Maps;

namespace Midtown.Classes.Main.Game.Entities.Objects.EasterEggs
{
    public class SerothsStatue: ObjectEntity, IInteractable
    {

        public SerothsStatue(GameScreen scene, GameMap map, Vector2 position) : base(scene, map, new MonoGame.Extended.RectangleF(position.X, position.Y, 32, 16), new Vector2(-16, -40), "Textures/SerothsStatue")
        {

        }

        public void Interact()
        {
            Screen.OpenTextBubble(new TextBubble(Game, Screen, new string[] { "Bonjour à toutes et à tous, \nje suis Seroths !" }, "Textures/ChatBubble", "Textures/Seroths"));
        }
    }
}
