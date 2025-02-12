using System;

namespace MainScreen.HabbitPlane
{
    [Serializable]
    public class DailyProgress
    {
        public DateTime Date;
        public int Progress;
        public int TargetNumber;
    }
}