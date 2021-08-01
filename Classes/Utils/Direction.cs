using Microsoft.Xna.Framework;
using System;

namespace MonoGameTestProject.Classes.Utils
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public static class DirectionVectorHelper
    {
        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            return new Vector2(
                vector.X * (float)Math.Cos(angle) - vector.Y * (float)Math.Sin(angle),
                vector.X * (float)Math.Sin(angle) + vector.Y * (float)Math.Cos(angle));
        }
    }
}
