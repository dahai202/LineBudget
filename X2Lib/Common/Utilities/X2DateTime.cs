using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace X2Lib.Common.Utilities
{
    public class X2DateTime
    {
        public static int GetDayOfWeek(DateTime date)
        {
            return (int)date.DayOfWeek;
        }
        public static int GetDayOfWeek()
        {
            return X2DateTime.GetDayOfWeek(DateTime.Now);
        }

        public static DateTime GetTodayStart()
        {
          return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        public static DateTime GetTodayEnd()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        }

        public static DateTime GetThisWeekStart()
        {
            DateTime date = DateTime.Now;
            date.AddDays(0 - X2DateTime.GetDayOfWeek());
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        public static DateTime GetThisWeekEnd()
        {
            DateTime date = DateTime.Now;
            date.AddDays(7 - X2DateTime.GetDayOfWeek());
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        public static DateTime GetThisMonthStart()
        {
            return new DateTime(DateTime.Now.Year,DateTime.Now.Month,1, 0, 0, 0);
        }

        public static DateTime GetThisMonthEnd()
        {
            DateTime date = DateTime.Now;
            date.AddMonths(1);
            return new DateTime(date.Year, date.Month + 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// get a culture string  //en-US//en-GB //zh-CN
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <returns></returns>
        public static string GetCultureString(DateTime sourceDate,string culturestring)
        {
            if (sourceDate == null)
	        {
		         return "";
	        }

            if (string.IsNullOrEmpty(culturestring))
	        {
        		 culturestring = "en-US";
	        }

            CultureInfo culture;
            try
            {
                culture = CultureInfo.CreateSpecificCulture(culturestring); //en-US//en-GB //zh-CN
            }
            catch (Exception ex)
            {
                culture = CultureInfo.CreateSpecificCulture("en-US"); //en-US//en-GB //zh-CN
            }
           return sourceDate.GetDateTimeFormats(culture)[0];
        }

        /// <summary>
        /// get the culture datetime
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="culturestring"></param>
        /// <returns></returns>
        public static DateTime GetCultureDate(string sourceString, string culturestring)
        {
            CultureInfo culture;
            try 
	        {	        
        		culture = CultureInfo.CreateSpecificCulture(culturestring);
	        }
	        catch (Exception)
	        {
        		culture = CultureInfo.CreateSpecificCulture("en-US");  ////default as en-US
            }

            try
            {
                return DateTime.Parse(sourceString, culture);
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        
        }

        public static long TransferToTicks(string datestring, string sourceculture)
        {
            DateTime newdate = X2DateTime.GetCultureDate(datestring, sourceculture);
            if (newdate == DateTime.MinValue)
            {
                return 0;
            }
            else
            {
                return newdate.Ticks;
            }
        }

        public static string TransferToISO(string datestring, string sourceculture)
        {
            DateTime newdate = X2DateTime.GetCultureDate(datestring, sourceculture);
            if (newdate == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                return newdate.ToString("yyyyMMddhhmmss");
            }
        }

        /// <summary>
        /// transfer date string to us standard
        /// </summary>
        /// <param name="datestring"></param>
        /// <param name="sourceculture"></param>
        /// <returns></returns>
        public static string TransferToUS(string datestring, string sourceculture)
        {
            DateTime newdate = X2DateTime.GetCultureDate(datestring, sourceculture);
            if (newdate == DateTime.MinValue)
            {
                return "";
            }
            else
            {
               return X2DateTime.GetCultureString(newdate, "en-US");            
            }
        }

    }
}
