using System;
using System.Collections.Generic;
using System.Text;

namespace BetterJWall.Common
{
    public interface IDateTimeFunctions
    {
        /// <summary>
        /// Gives you the Date and Time as of the moment
        /// this member is executed
        /// </summary>
        /// <returns></returns>
        DateTime Now();

        /// <summary>
        /// Gives you the current date as of the moment
        /// this member is executed (always midnight)
        /// </summary>
        /// <returns></returns>
        DateTime Date();
    }

    public class DateTimeFunctions : IDateTimeFunctions
    {
        public DateTime Now() => DateTime.Now;
        public DateTime Date() => DateTime.Today;
    }
}
