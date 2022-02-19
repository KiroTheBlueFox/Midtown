﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;

namespace Midtown.Classes.Utils
{
    public class Settings
    {
        public static readonly string
            MoveUpKey = "MoveUp",
            MoveDownKey = "MoveDown",
            MoveLeftKey = "MoveLeft",
            MoveRightKey = "MoveRight",
            InteractKey = "Interact",
            WalkKey = "Walk",
            ConfirmKey = "Confirm",
            CancelKey = "Cancel";
        public static readonly string[] RemappableKeys =
        {
            MoveUpKey, MoveDownKey, MoveLeftKey, MoveRightKey,
            InteractKey, WalkKey
        };

        public string Language { get; set; } = "fr";
        public bool Fullscreen { get; set; } = false;
        public bool Borderless { get; set; } = false;
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 360;
        public Dictionary<string, string> Controls { get; set; } = new Dictionary<string, string>()
        {
            { MoveUpKey, "Z" },
            { MoveDownKey, "S" },
            { MoveLeftKey, "Q" },
            { MoveRightKey, "D" },
            { InteractKey, "E" },
            { WalkKey, "LeftShift" },
            { ConfirmKey, "Enter" },
            { CancelKey, "Escape" }
        };
        public float Zoom { get; set; } = 1f;
        public bool Pixelisation { get; set; } = true;
        public int WaterDeepness { get; set; } = 16;

        public Keys? GetKey(string name)
        {
            if (Controls.ContainsKey(name))
                return (Keys)Enum.Parse(typeof(Keys), Controls[name]);
            else
                return null;
        }

        public void SetKey(string name, Keys key)
        {
            if (Controls.ContainsKey(name))
                Controls[name] = key.ToString();
        }

        public bool IsKeyDown(KeyboardStateExtended state, string controlName)
        {
            Keys? key = GetKey(controlName);
            if (key == null)
                return false;
            else
                return state.IsKeyDown((Keys)key);
        }

        public bool IsKeyUp(KeyboardStateExtended state, string controlName)
        {
            Keys? key = GetKey(controlName);
            if (key == null)
                return false;
            else
                return state.IsKeyUp((Keys)key);
        }

        public bool WasKeyJustDown(KeyboardStateExtended state, string controlName)
        {
            Keys? key = GetKey(controlName);
            if (key == null)
                return false;
            else
                return state.WasKeyJustDown((Keys)key);
        }

        public bool WasKeyJustUp(KeyboardStateExtended state, string controlName)
        {
            Keys? key = GetKey(controlName);
            if (key == null)
                return false;
            else
                return state.WasKeyJustUp((Keys)key);
        }
    }
}
