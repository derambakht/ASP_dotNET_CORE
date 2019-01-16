using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Common
{
    public static class DateUtility
    {
        public static DateTime? ShamsiToMiladi(string shamsiDate)
        {
            var pc = new PersianCalendar();
            var array = shamsiDate.Split('/');
           
            DateTime date = new DateTime(int.Parse(Persia.PersianWord.ConvertToLatinNumber(array[0])),
            int.Parse(Persia.PersianWord.ConvertToLatinNumber(array[1])),
            int.Parse(Persia.PersianWord.ConvertToLatinNumber(array[2])), pc);
            return date;
        }

        public static string MiladiToShamsi(DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}";
        }

        public static string ToShamsi(this DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}";
        }
        public static string ToShamsiForFileName(this DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date)}_{pc.GetMonth(date)}_{pc.GetDayOfMonth(date)}";
        }
        public static int ToShamsiYear(this DateTime date)
        {
            var pc = new PersianCalendar();
            return pc.GetYear(date);
        }
        public static int ToShamsiMonth(this DateTime date)
        {
            var pc = new PersianCalendar();
            return pc.GetMonth(date);
        }
    }
}
