using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midtown.Classes.Main.Interface;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using System.Linq;

namespace Midtown.Classes.Main
{
    public class Screen : GameComponent
    {
        public List<GameElement> Elements;
        public new readonly MainGame Game;
        private Screen _childScreen;
        public Screen ChildScreen { get => _childScreen; set { value?.Initialize(); value?.Load(); _childScreen = value; } }
        public KeyboardStateExtended KeyboardState { get => Game.KeyboardState; }
        public MouseStateExtended MouseState { get => Game.MouseState; }
        public SpriteBatch SpriteBatch { get => Game.SpriteBatch; }
        public bool Hovered = false, Clicked = false;

        public Screen(MainGame game, Screen child = null) : base(game)
        {
            Game = game;

            ChildScreen = child;

            Elements = new List<GameElement>();
        }

        public override void Initialize()
        {
            foreach (GameElement element in Elements)
            {
                element.Initialize();
            }

            ChildScreen?.Initialize();

            base.Initialize();
        }

        public virtual void Load()
        {
            foreach (GameElement element in Elements)
            {
                element.Load();
            }

            ChildScreen?.Load();
        }

        public virtual void LoadLanguage()
        {
            foreach (GameElement element in Elements)
            {
                element.LoadLanguage();
            }

            ChildScreen?.LoadLanguage();
        }

        public override void Update(GameTime gameTime)
        {
            ChildScreen?.Update(gameTime);

            Hovered = false;

            foreach (GameElement element in Elements)
                element.Update(gameTime);
            
            if (ChildScreen == null)
            {
                foreach (GameElement element in Enumerable.Reverse(Elements))
                {
                    if (element is ClickableElement clickableElement)
                    {
                        clickableElement.IsHovered = clickableElement.Rectangle.IsPointIn(MouseState.Position.ToVector2() / Game.Scale) && ChildScreen == null;

                        if (clickableElement.IsHovered && !Hovered)
                        {
                            Hovered = true;
                            Mouse.SetCursor(clickableElement.CursorWhenHovered);

                            if (MouseState.IsButtonDown(MouseButton.Left))
                            {
                                if (!Clicked)
                                {
                                    Clicked = true;
                                    clickableElement.OnClick();
                                }
                            }
                            else
                            {
                                Clicked = false;
                            }
                        }
                    }
                }

                if (!Hovered)
                    Mouse.SetCursor(MouseCursor.Arrow);
            }
            else
            {
                Hovered = ChildScreen.Hovered;
            }

            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            try
            {
                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            catch { }

            foreach (GameElement element in Elements)
            {
                element.Draw(gameTime);
            }

            try
            {
                SpriteBatch.End();
            }
            catch { }

            if (ChildScreen != null)
            {
                try
                {
                    SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                }
                catch { }

                SpriteBatch.FillRectangle(Game.MainTarget.Bounds, Color.Black * 0.75f);

                try
                {
                    SpriteBatch.End();
                }
                catch { }

                ChildScreen.Draw(gameTime);
            }
        }

        public virtual void DrawDebug(GameTime gameTime)
        {
            try
            {
                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            catch { }

            foreach (GameElement element in Elements)
            {
                element.DrawDebug(gameTime);
            }

            try
            {
                SpriteBatch.End();
            }
            catch { }

            if (ChildScreen != null)
            {
                try
                {
                    SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                }
                catch { }

                ChildScreen.DrawDebug(gameTime);

                try
                {
                    SpriteBatch.End();
                }
                catch { }
            }
        }
    }
}
