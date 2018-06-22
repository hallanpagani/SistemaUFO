using System;

namespace BaseModelo.classes
{
    public static class Fuso
    {
        public const int TIMEZONE = -3; // -3 horario normal,  -2 verão

        public static DateTime GetDateNow()
        {
            return DateTime.Now.ToUniversalTime().AddHours(TIMEZONE); 
        }

        public static TimeSpan GetTimeNow()
        {
            DateTime agora = GetDateNow();
            return new TimeSpan(agora.Hour, agora.Minute, agora.Second);
        }

        public static DateTime GetDateComAjusteFuso(DateTime dh)
        {
            return dh.ToUniversalTime().AddHours(TIMEZONE);  
        }
    }
}