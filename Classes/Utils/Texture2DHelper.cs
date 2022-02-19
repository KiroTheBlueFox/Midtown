using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Midtown.Classes.Utils
{
    public static class Texture2DHelper
    {
        private static readonly Dictionary<Texture2D, Color[]> Data = new Dictionary<Texture2D, Color[]>();

        public static void PreloadTextureData(Texture2D texture)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            Data.Add(texture, colors);
        }

        public static Color GetPixel(Texture2D texture, int x, int y)
        {
            return Data[texture][x + (y * texture.Width)];
        }
    }
}
