using Terraria;

namespace Expeditions
{
    public static class TimeChecker
    {
        public const double mornCutOff = 16200.0;
        public const double noonCutOff = 37800.0;
        public const double eveCutOff = 54000.0;

        public const double duskCutOff = 9720.0;
        public const double midCutOff = 22680.0;
        public const double dawnCutOff = 32400.0;

        public static double RawNoonTime = 27000.0;
        public static double RawMidnightTime = 16200.0;

        public static bool TimeDayMorn
        { get { return Main.dayTime && Main.time >= 0 && Main.time < mornCutOff; } }
        public static bool TimeDayNoon
        { get { return Main.dayTime && Main.time >= mornCutOff && Main.time <= noonCutOff; } }
        public static bool TimeDayEve
        { get { return Main.dayTime && Main.time > noonCutOff && Main.time <= eveCutOff; } }

        public static bool TimeNightDusk
        { get { return !Main.dayTime && Main.time >= 0 && Main.time < duskCutOff; } }
        public static bool TimeNightMidnight
        { get { return !Main.dayTime && Main.time >= duskCutOff && Main.time <= midCutOff; } }
        public static bool TimeNightDawn
        { get { return !Main.dayTime && Main.time > midCutOff && Main.time <= dawnCutOff; } }

        public static bool WitchingHour
        { get { return !Main.dayTime && (int)ConvertedTime == 3; } }

        public static bool TimeDayPreNoon
        { get { return Main.dayTime && Main.time < RawNoonTime; } }
        public static bool TimeDayPostNoon
        { get { return Main.dayTime && Main.time >= RawNoonTime; } }
        public static bool TimeNightPreMid
        { get { return !Main.dayTime && Main.time < RawMidnightTime; } }
        public static bool TimeNightPostMid
        { get { return !Main.dayTime && Main.time >= RawMidnightTime; } }

        public static double ConvertedTime
        {
            get
            {
                double fullTime = Main.time;
                if (!Main.dayTime) fullTime += 54000.0;
                fullTime = (fullTime / 86400.0 * 24.0) - 19.75;
                if (fullTime < 0.0) fullTime += 24.0;
                return fullTime;
            }
        }

        public static bool Moon1Full { get { return Main.moonPhase == 0; } }
        public static bool Moon2WaneGibb { get { return Main.moonPhase == 1; } }
        public static bool Moon3ThirdQuart { get { return Main.moonPhase == 2; } }
        public static bool Moon4WaneCres { get { return Main.moonPhase == 3; } }
        public static bool Moon5New { get { return Main.moonPhase == 4; } }
        public static bool Moon6WaxCres { get { return Main.moonPhase == 5; } }
        public static bool Moon7FirstQuart { get { return Main.moonPhase == 6; } }
        public static bool Moon8WaxGibb { get { return Main.moonPhase == 7; } }
    }
}
