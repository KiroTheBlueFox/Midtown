using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Midtown.Classes.Main.Game.Systems
{
    public class Date
    {
        public static int DAYS_PER_SEASON = 28;
        
        public int TotalDays { get => _totalDays; }
        public MainGame Game { get; }
        public int Year { get => _year; }
        public int Day { get => _day; }
        public Season Season { get => _season; }

        private int _year, _day, _totalDays;
        private Season _season;
        private Action _dayChange, _seasonChange, _yearChange;

        public Date(MainGame game, Action dayChange, Action seasonChange, Action yearChange) : this(game, 0, 14, Season.SUMMER, dayChange, seasonChange, yearChange) { }

        public Date(MainGame game, int year, int day, Season season, Action dayChange, Action seasonChange, Action yearChange)
        {
            Game = game;
            _year = year;
            _day = day;
            _totalDays = 1;
            _season = season;
            _dayChange = dayChange;
            _seasonChange = seasonChange;
            _yearChange = yearChange;
        }

        public static Date operator +(Date date, int days)
        {
            date._day += days;
            date._totalDays += days;
            date._dayChange.Invoke();
            if (date._day > DAYS_PER_SEASON)
            {
                date._day -= DAYS_PER_SEASON;
                date._seasonChange.Invoke();
                date._season += 1;
                if ((int) date._season > Enum.GetValues(date._season.GetType()).Length)
                {
                    date._season = 0;
                    date._yearChange.Invoke();
                    date._year += 1;
                }
            }
            return date;
        }
        public static Date operator -(Date date, int days)
        {
            date._day -= days;
            date._totalDays -= days;
            if (date._day < 1)
            {
                date._day += DAYS_PER_SEASON;
                date._season -= 1;
                if ((int)date._season < 0)
                {
                    date._season = (Season) Enum.GetValues(date._season.GetType()).Length - 1;
                    date._year -= 1;
                }
            }
            return date;
        }
        public void SetDate(int year, int day, Season season)
        {
            _year = year;
            _day = day;
            _season = season;
        }

        public override string ToString()
        {
            return Game.Language["day." + ((_day - 1) % 7 + 1)].Substring(0, 3) + ". " + _day + ", " + Game.Language["season."+_season.ToString().ToLower()] + ", " + Game.Language["year"] + " " + (_year + 1);
        }
    }

    public enum Season
    {
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER
    }
}
