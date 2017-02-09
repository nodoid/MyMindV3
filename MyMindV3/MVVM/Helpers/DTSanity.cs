using System;
using System.Collections.Generic;
namespace MvvmFramework
{
    public static class DTSanity
    {
        public static DateTime CleanDate(this string dt)
        {
            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(dt);
            }
            catch (Exception)
            {
                // does it contain a time?
                var time = dt.Split(' ');
                var times = new List<int>() { -1, -1, 0 };
                if (time.Length != 1)
                {
                    var tr = time[1].Split(':');
                    times[0] = Convert.ToInt32(tr[0]);
                    times[1] = Convert.ToInt32(tr[1]);
                }
                // get the date bit
                var bitsFull = dt.Split(' ');
                var bits = bitsFull[0].Split('/');
                if (times[0] != -1)
                {
                    dateTime = new DateTime(Convert.ToInt32(bits[2]), Convert.ToInt32(bits[1]), Convert.ToInt32(bits[0]), times[0], times[1], times[2]);
                }
                else
                {
                    dateTime = new DateTime(Convert.ToInt32(bits[2]), Convert.ToInt32(bits[1]), Convert.ToInt32(bits[0]));
                }
            }
            return dateTime;
        }
    }
}
