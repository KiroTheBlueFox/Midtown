
using System;

namespace Midtown.Classes.Utils
{
    public static class EnumExtensions
    {
        public static T Offset<T>(this T src, int offset) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(Arr, src) + offset;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        public static T Next<T>(this T src) where T : struct
        {
            return src.Offset(1);
        }

        public static T Prev<T>(this T src) where T : struct
        {
            return src.Offset(-1);
        }
    }

    public static class EnumHelper
    {
        public static T Offset<T>(T src, int offset) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = (Array.IndexOf(Arr, src) + offset)%Arr.Length;
            if (j < 0) j += Arr.Length;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        public static T Next<T>(T src) where T : struct
        {
            return Offset(src, 1);
        }

        public static T Prev<T>(T src) where T : struct
        {
            return Offset(src, -1);
        }
    }
}
