using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Interface;
using Midtown.Classes.Main.Game.Maps;
using Midtown.Classes.Utils;
using System;

namespace Midtown.Classes.Main.Game.Entities.Objects.EasterEggs
{
    public class ColasBimStatue: ObjectEntity, IInteractable
    {
        private Tuple<string, int>[] AllMessages = new Tuple<string, int>[] { new Tuple<string, int>("ŒUF", 64), new Tuple<string, int>("PAIN", 64), new Tuple<string, int>("DE L'ARGENT GRATUIT!!!", 32) };

        public ColasBimStatue(GameScreen scene, GameMap map, Vector2 position) : base(scene, map, new MonoGame.Extended.RectangleF(position.X, position.Y, 32, 16), new Vector2(-16, -40), "Textures/Objects/ColasBimStatue")
        {
        }

        public void Interact()
        {
            Tuple<string, int> text = AllMessages[new Random().Next(AllMessages.Length)];
            TextBubble bubble = new TextBubble(Game, Screen, new string[] { text.Item1 }, "Textures/UI/ChatBubble", "Textures/UI/ColasBim");
            bubble.SetFont(Game.Fonts[Fonts.Normal].GetFont(text.Item2));
            Screen.OpenTextBubble(bubble);
        }
    }
}
