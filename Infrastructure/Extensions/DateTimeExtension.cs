using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static DayOfWeekRus GetDayOfWeekRus(this DateTime dt)
        {
            DayOfWeekRus result = DayOfWeekRus.Понедельник;
            var date = dt.DayOfWeek;
            switch (date)
            {
                case DayOfWeek.Monday:
                    result = DayOfWeekRus.Понедельник;
                    break;
                case DayOfWeek.Tuesday:
                    result = DayOfWeekRus.Вторник;
                    break;
                case DayOfWeek.Wednesday:
                    result = DayOfWeekRus.Среда;
                    break;
                case DayOfWeek.Thursday:
                    result = DayOfWeekRus.Четверг;
                    break;
                case DayOfWeek.Friday:
                    result = DayOfWeekRus.Пятница;
                    break;
                case DayOfWeek.Saturday:
                    result = DayOfWeekRus.Суббота;
                    break;
                case DayOfWeek.Sunday:
                    result = DayOfWeekRus.Воскресенье;
                    break;
            }
            return result;
        }
    }
}
