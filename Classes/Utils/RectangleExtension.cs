using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace Midtown.Classes.Utils
{
    public static class RectangleExtension
    {
        public static bool IsPointIn(this RectangleF rect, Vector2 point)
        {
            return point.X >= rect.X && point.X <= rect.X + rect.Width && point.Y >= rect.Y && point.Y <= rect.Y + rect.Height;
        }

        public static bool IsPointIn(this RectangleF rect, Point point)
        {
            return point.X >= rect.X && point.X <= rect.X + rect.Width && point.Y >= rect.Y && point.Y <= rect.Y + rect.Height;
        }

        public static Direction? SideOfPoint(this RectangleF rect, Vector2 point)
        {
            return rect.SideOfMovement(point - (Vector2)rect.Center);
        }
        public static Direction? SideOfMovement(this RectangleF rect, Vector2 movement)
        {
            double movementAngle = Math.Atan2(movement.Y, movement.X);

            Vector2 topRightVector = rect.TopRight - rect.Center;
            double TopRightAngle = Math.Atan2(topRightVector.Y, topRightVector.X);

            Vector2 bottomRightVector = rect.BottomRight - rect.Center;
            double BottomRightAngle = Math.Atan2(bottomRightVector.Y, bottomRightVector.X);

            double BottomLeftAngle = TopRightAngle + Math.PI;

            double TopLeftAngle = -BottomLeftAngle;

            if (movementAngle > TopLeftAngle && movementAngle < TopRightAngle)
            {
                return Direction.Up;
            }
            else if (movementAngle >= TopRightAngle && movementAngle <= BottomRightAngle)
            {
                return Direction.Right;
            }
            else if (movementAngle > BottomRightAngle && movementAngle < BottomLeftAngle)
            {
                return Direction.Down;
            }
            else if ((movementAngle >= BottomLeftAngle && movementAngle <= Math.PI) || (movementAngle <= TopLeftAngle && movementAngle >= -Math.PI))
            {
                return Direction.Left;
            }
            return null;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
