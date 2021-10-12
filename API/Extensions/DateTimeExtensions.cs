using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var toDay = DateTime.Today;
            var age = toDay.Year - dob.Year;
            if (dob.Date > toDay.AddYears(-age)) age--;
            return age;
        }
    }
}