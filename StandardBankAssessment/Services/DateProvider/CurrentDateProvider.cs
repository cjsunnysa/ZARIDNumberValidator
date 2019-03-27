using System;

namespace StandardBankAssessment.Web.Services.DateProvider
{
    public class CurrentDateProvider : IDateProvider
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
    }
}
