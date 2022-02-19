using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Midtown.Classes.Main.Interface;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Main.MainMenu.Buttons
{
    public class SettingsOption<type> : GameElement
    {
        public string ButtonName { get; private set; }
        public SettingValue<type> ButtonValue { get; private set; }
        public List<GameElement> Elements { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public RectangleF RectangleF { get => Rectangle; }
        public Rectangle KeyRectangle { get; private set; }
        public RectangleF KeyRectangleF { get => KeyRectangle; }
        private DynamicSpriteFont _font;
        public bool Clicked = false, WaitForKey = false;
        public new SettingsScreen Screen { get; private set; }

        public SettingsOption(MainGame game, SettingsScreen screen, string name, SettingValue<type> settingValue, int x, int y, int width, int height) : base(game, screen)
        {
            ButtonName = name;
            Screen = screen;
            ButtonValue = settingValue;
            Rectangle = new Rectangle(x, y, width, height);
            Elements = new List<GameElement>();
            if (!(settingValue is KeySettingValue))
            {
                Elements.Add(new SettingsArrowButton(game, screen, Rectangle.Right - width / 2, Rectangle.Top, () => ButtonValue.PrevValue(), false));
                Elements.Add(new SettingsArrowButton(game, screen, Rectangle.Right - SettingsArrowButton.SIZE, Rectangle.Top, () => ButtonValue.NextValue(), true));
            }
            else
            {
                KeyRectangle = new Rectangle(x + width / 2, y, width / 2, height);
            }
        }

        public override void Initialize()
        {
            foreach (GameElement element in Elements)
                element.Initialize();

            base.Initialize();
        }

        public override void Load()
        {
            foreach (GameElement element in Elements)
                element.Load();

            _font = Game.Fonts[Fonts.Normal].GetFont(Rectangle.Height);

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            bool hovered = false;

            foreach (GameElement element in Elements) {
                element.Update(gameTime);

                if (element is ClickableElement clickableElement && !Screen.WaitForKey)
                {
                    clickableElement.IsHovered = clickableElement.Rectangle.IsPointIn((MouseState.Position.ToVector2() / Game.Scale) - SettingsScreen.Offset);

                    if (clickableElement.IsHovered && !hovered)
                    {
                        hovered = true;
                        Screen.Hovered = true;
                        Mouse.SetCursor(clickableElement.CursorWhenHovered);

                        if (MouseState.IsButtonDown(MouseButton.Left))
                        {
                            if (!Clicked)
                            {
                                Clicked = true;
                                Screen.Clicked = true;
                                clickableElement.OnClick();
                            }
                        }
                        else
                        {
                            Clicked = false;
                            Screen.Clicked = false;
                        }
                    }
                }
            }

            if (ButtonValue is KeySettingValue keySettingValue && KeyRectangleF.IsPointIn((MouseState.Position.ToVector2() / Game.Scale) - SettingsScreen.Offset))
            {
                if (!Screen.WaitForKey)
                {
                    Mouse.SetCursor(MouseCursor.Hand);
                    if (MouseState.IsButtonDown(MouseButton.Left))
                    {
                        Clicked = true;
                        Screen.Clicked = true;
                        WaitForKey = true;
                        Screen.WaitForKey = true;
                    }
                }
                else
                {
                    if (!MouseState.IsButtonDown(MouseButton.Left))
                    {
                        Clicked = false;
                        Screen.Clicked = false;
                    }
                    if (WaitForKey)
                    {
                        Keys[] pressedKeys = KeyboardState.GetPressedKeys();
                        if (pressedKeys.Length > 0)
                        {
                            keySettingValue.Value = pressedKeys[0];
                            WaitForKey = false;
                            Screen.WaitForKey = false;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(_font, Game.Language[ButtonName], RectangleF.TopLeft, Color.Black);

            string text = ButtonValue.Value.ToString();
            if (ButtonValue is BoolSettingValue)
                text = Game.Language[ButtonValue.Value.ToString()];
            else if (!(ButtonValue is IntSettingValue || ButtonValue is FloatSettingValue || ButtonValue is KeySettingValue))
                text = Game.Language[ButtonName + "." + ButtonValue.Value.ToString()];
            if (ButtonValue is KeySettingValue && WaitForKey)
                text = Game.Language["settingsMenu.waitForKey"];

            Vector2 measures = _font.MeasureString(text);
            SpriteBatch.DrawString(_font, text, new Vector2(Rectangle.X + SettingsArrowButton.SIZE + (Rectangle.Width + (Rectangle.Width / 2 - SettingsArrowButton.SIZE * 2) - measures.X) / 2, Rectangle.Top), Color.Black);

            foreach (GameElement element in Elements)
                element.Draw(gameTime);

            base.Draw(gameTime);
        }
    }

    public class SettingValue<type>
    {
        protected type _value;
        public type Value
        {
            get => _value;
            set {
                _value = value;
                OnValueChange.Invoke(value);
            }
        }
        public Action<type> OnValueChange;

        public SettingValue(type value, Action<type> onValueChange)
        {
            _value = value;
            OnValueChange = onValueChange;
        }

        public virtual void PrevValue() { }
        public virtual void NextValue() { }
    }

    public class BoolSettingValue : SettingValue<bool>
    {
        public BoolSettingValue(bool value, Action<bool> onValueChange) : base(value, onValueChange) { }

        public override void NextValue() => Value = !Value;
        public override void PrevValue() => Value = !Value;
    }

    public class IntSettingValue : SettingValue<int>
    {
        public IntSettingValue(int value, Action<int> onValueChange) : base(value, onValueChange) { }

        public override void NextValue() => Value++;

        public override void PrevValue() => Value--;
    }

    public class FloatSettingValue : SettingValue<float>
    {
        public FloatSettingValue(float value, Action<float> onValueChange) : base(value, onValueChange) { }

        public override void NextValue() => Value++;

        public override void PrevValue() => Value--;
    }

    public class StringSettingValue : SettingValue<string>
    {
        public string[] Values;

        public StringSettingValue(string[] values, string value, Action<string> onValueChange) : base(value, onValueChange)
        {
            Values = values;
        }

        private void OffsetValue(int offset)
        {
            int index = (Array.IndexOf(Values, Value) + offset) % Values.Length;
            if (index < 0) index += Values.Length;
            Value = Values[index];
        }

        public override void NextValue() => OffsetValue(1);
        public override void PrevValue() => OffsetValue(-1);
    }

    public class EnumSettingValue<type> : SettingValue<type> where type : struct
    {
        public EnumSettingValue(type value, Action<type> onValueChange) : base(value, onValueChange) { }

        public override void NextValue() => Value = EnumHelper.Next(Value);

        public override void PrevValue() => Value = EnumHelper.Prev(Value);
    }

    public class KeySettingValue : SettingValue<Keys>
    {
        public KeySettingValue(Keys value, Action<Keys> onValueChange) : base(value, onValueChange) { }
    }
}
