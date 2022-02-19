using System;

namespace Midtown.Classes.Main.Game.Systems
{
    public class Daytime
    {
        public float Time { get; private set; }
        public float TotalTime { get; private set; }
        private readonly Action _dayChange;

        public Daytime(float startTime, Action DayChange)
        {
            Time = startTime;
            TotalTime = startTime;
            _dayChange = DayChange;
        }

        public int Hours
        {
            get => (int)Time / 60;
        }
        public int Minutes
        {
            get => (int)Time % 60;
        }

        public override string ToString()
        {
            return string.Format("{0,0:D2}", Hours) + ":" + string.Format("{0,0:D2}", Minutes);
        }

        public static Daytime operator +(Daytime daytime, float time)
        {
            daytime.Time += time;
            daytime.TotalTime += time;
            if (daytime.Time >= 1440)
            {
                daytime.Time = 0;
                daytime._dayChange.Invoke();
            }
            return daytime;
        }
        public static Daytime operator -(Daytime daytime, float time)
        {
            daytime.Time -= time;
            return daytime;
        }
        public void SetTime(float time)
        {
            Time = time;
        }

        public static bool operator >(Daytime daytime, float time) => daytime.Time > time;
        public static bool operator <(Daytime daytime, float time) => daytime.Time < time;
        public static bool operator >=(Daytime daytime, float time) => daytime.Time >= time;
        public static bool operator <=(Daytime daytime, float time) => daytime.Time <= time;
        public static bool operator ==(Daytime daytime, float time) => daytime.Time == time;
        public static bool operator !=(Daytime daytime, float time) => daytime.Time != time;
        public override bool Equals(object obj)
        {
            if (obj is Daytime daytime)
            {
                return daytime.Time == Time;
            }
            else
            {
                return Time.Equals(obj);
            }
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
