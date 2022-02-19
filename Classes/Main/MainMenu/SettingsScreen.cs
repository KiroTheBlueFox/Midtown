using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midtown.Classes.Main.MainMenu.Buttons;
using Midtown.Classes.Main.MainMenu.Interface;
using Midtown.Classes.Utils;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Midtown.Classes.Main.MainMenu
{
    public class SettingsScreen : Screen
    {
        public static readonly int HMargin = 160, VMargin = 40, BorderWidth = 2, TextHMargin = 16, TextVMargin = 40, SettingSize = 16, CategorySize = 24, CategoryTextSize = 16, CategoryVMargin = 8;
        public static readonly Vector2 Offset = new Vector2(HMargin, VMargin + TextVMargin);
        public List<GameElement> SettingsElements;
        private DynamicSpriteFont TitleFont;
        public Screen Parent;
        private RenderTarget2D _settingsTarget;
        private int _settingsAmount = 0, _categoriesAmount = 0, _margin = 0, _scroll = 0, _maxScroll = 0;
        public bool WaitForKey = false;

        public SettingsScreen(MainGame game, Screen parent) : base(game)
        {
            Parent = parent;

            SettingsElements = new List<GameElement>();
        }

        public override void Initialize()
        {
            Elements.Add(new SettingsExitButton(Game, this, Game.MainTarget.Width - HMargin - BorderWidth - SettingsExitButton.SIZE, VMargin + BorderWidth, Color.Black, () => { Parent.ChildScreen = null; }));

            _settingsTarget = new RenderTarget2D(Game.GraphicsDevice, Game.MainTarget.Width - HMargin * 2, Game.MainTarget.Height - VMargin * 2 - TextVMargin);

            AddCategory("settingsMenu.category.game");

            AddSetting("settingsMenu.setting.windowMode", new EnumSettingValue<WindowMode>(Game.WindowMode, value => Game.SwitchMode(value)));
            AddSetting("settingsMenu.setting.language", new StringSettingValue(Game.Languages.Keys.ToArray(), Game.Settings.Language, value => Game.ChangeLanguage(value)));

            AddCategory("settingsMenu.category.controls");

            foreach (string key in Settings.RemappableKeys)
            {
                AddSetting("settingsMenu.setting.controls." + key, new KeySettingValue((Keys)Game.Settings.GetKey(key), value => Game.Settings.SetKey(key, value)));
            }

            foreach (GameElement element in SettingsElements)
                element.Initialize();

            _maxScroll -= _settingsTarget.Height;
            if (_maxScroll < 0) _maxScroll = 0;

            base.Initialize();
        }

        private void AddCategory(string name)
        {
            _margin += _categoriesAmount == 0 ? 0 : CategoryVMargin;
            SettingsElements.Add(new SettingCategory(Game, this, name, BorderWidth + TextHMargin, BorderWidth + SettingSize * _settingsAmount + CategorySize * _categoriesAmount + _margin, _settingsTarget.Width - TextHMargin * 2, CategorySize, CategoryTextSize, Color.SaddleBrown, _categoriesAmount == 0 ? 0 : 1));
            _categoriesAmount++;
            _maxScroll += CategorySize;
        }

        private void AddSetting<type>(string name, SettingValue<type> settingValue)
        {
            SettingsElements.Add(new SettingsOption<type>(Game, this, name, settingValue, BorderWidth + TextHMargin, BorderWidth + SettingSize * _settingsAmount + CategorySize * _categoriesAmount + _margin, _settingsTarget.Width - TextHMargin * 2, SettingSize));
            _settingsAmount++;
            _maxScroll += SettingSize;
        }

        public override void Load()
        {
            TitleFont = Game.Fonts[Fonts.Bold, new FontSystemSettings() { Effect = FontSystemEffect.Stroked, EffectAmount = 1 }].GetFont(32);

            foreach (GameElement element in SettingsElements)
                element.Load();

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameElement element in SettingsElements)
                element.Update(gameTime);

            _scroll += -MouseState.DeltaScrollWheelValue / 10;
            _scroll = Math.Clamp(_scroll, -_maxScroll, 0);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            SpriteBatch.FillRectangle(new Rectangle(HMargin, VMargin, Game.MainTarget.Width - HMargin * 2, Game.MainTarget.Height - VMargin * 2), Color.SaddleBrown);
            SpriteBatch.FillRectangle(new Rectangle(HMargin + BorderWidth, VMargin + BorderWidth, Game.MainTarget.Width - (HMargin + BorderWidth) * 2, Game.MainTarget.Height - (VMargin + BorderWidth) * 2), Color.SandyBrown);

            SpriteBatch.DrawString(TitleFont, "Settings", new Vector2((Game.MainTarget.Width - TitleFont.MeasureString("Settings").X) / 2, VMargin), Color.Yellow);

            SpriteBatch.End();

            base.Draw(gameTime);


            Game.SetRenderTarget(_settingsTarget);

            Game.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateTranslation(0, _scroll, 0));

            foreach (GameElement element in SettingsElements)
                element.Draw(gameTime);

            SpriteBatch.End();


            Game.SetRenderTarget(null);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            SpriteBatch.Draw(_settingsTarget, Offset, Color.White);

            SpriteBatch.End();

        }
    }
}
