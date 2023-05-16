using System.IdentityModel.Tokens.Jwt;
using Brickalytics.Models;

namespace Brickalytics.Helpers
{
    public static class DateHelper
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}