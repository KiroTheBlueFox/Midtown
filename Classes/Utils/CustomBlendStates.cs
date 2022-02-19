using Microsoft.Xna.Framework.Graphics;

namespace Midtown.Classes.Utils
{
    public static class CustomBlendStates
    {
        public static BlendState Multiply = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };
    }
}
