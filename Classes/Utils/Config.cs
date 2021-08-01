using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Utils
{
    public class Config
    {
        public Dictionary<string, Keys> Controls { get; set; }
        public float Zoom { get; set; }
        public bool Pixelisation { get; set; }
        public int WaterDeepness { get; set; }
        public KeyboardState KeyboardState;
        public KeyboardStateExtended KeyboardStateExtended;
        public MouseState MouseState;
        public MouseStateExtended MouseStateExtended;
        public Point PositionOnClick, PositionOnRelease;
        public Game Game;

        public Config(Game game, ConfigValues values)
        {
            this.Game = game;
            Controls = new Dictionary<string, Keys>();
            foreach (KeyValuePair<string, string> control in values.Controls)
            {
                Controls.Add(control.Key, (Keys)Enum.Parse(typeof(Keys), control.Value));
            }
            Zoom = values.Zoom;
            Pixelisation = values.Pixelisation;
            WaterDeepness = values.WaterDeepness;
        }

        public Keys GetKey(string keyName)
        {
            return Controls[keyName];
        }

        public KeyStates GetKeyState(string keyName)
        {
            if (Array.Exists<string>(MouseKeys.MouseKeyList, key => key == keyName))
            {
                return GetMouseState(keyName);
            }

            Keys key = Controls[keyName];

            KeyStates stateToReturn;

            if (KeyboardStateExtended.WasKeyJustDown(key))
            {
                stateToReturn = KeyStates.JustReleased;
            }
            else if (KeyboardStateExtended.WasKeyJustUp(key))
            {
                stateToReturn = KeyStates.JustPressed;
            }
            else if (KeyboardState.IsKeyDown(key))
            {
                stateToReturn = KeyStates.Holding;
            }
            else
            {
                stateToReturn = KeyStates.Up;
            }

            return stateToReturn;
        }

        public KeyStates GetMouseState(string key)
        {
            MouseButton button;
            ButtonState buttonState;

            switch (key)
            {
                default:
                case MouseKeys.Left:
                    button = MouseButton.Left;
                    buttonState = MouseState.LeftButton;
                    break;
                case MouseKeys.Middle:
                    button = MouseButton.Middle;
                    buttonState = MouseState.MiddleButton;
                    break;
                case MouseKeys.Right:
                    button = MouseButton.Right;
                    buttonState = MouseState.RightButton;
                    break;
            }

            KeyStates stateToReturn;

            if (MouseStateExtended.WasButtonJustDown(button))
            {
                stateToReturn = KeyStates.JustReleased;
                PositionOnClick = MouseState.Position;
            }
            else if (MouseStateExtended.WasButtonJustUp(button))
            {
                stateToReturn = KeyStates.JustPressed;
                PositionOnRelease = MouseState.Position;
            }
            else if (buttonState == ButtonState.Pressed)
            {
                stateToReturn = KeyStates.Holding;
            }
            else
            {
                stateToReturn = KeyStates.Up;
            }

            return stateToReturn;
        }

        public void Update()
        {
            KeyboardState = Keyboard.GetState();
            KeyboardStateExtended = KeyboardExtended.GetState();
            MouseState = Mouse.GetState();
            MouseStateExtended = MouseExtended.GetState();
        }
    }

    public class ConfigValues
    {
        public Dictionary<string, string> Controls { get; set; }
        public float Zoom { get; set; }
        public bool Pixelisation { get; set; }
        public int WaterDeepness { get; set; }
    }

    public class ControlKeys
    {
        public const string MoveUp = "MoveUp";
        public const string MoveDown = "MoveDown";
        public const string MoveLeft = "MoveLeft";
        public const string MoveRight = "MoveRight";
    }

    public enum KeyStates
    {
        JustPressed,
        Holding,
        JustReleased,
        Up
    }

    public class MouseKeys
    {
        public const string Left = "lmb";
        public const string Middle = "mmb";
        public const string Right = "rmb";
        public static string[] MouseKeyList = { Left, Middle, Right };
    }
}
