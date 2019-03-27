using StandardBankAssessment.Web.Models;

namespace StandardBankAssessment.Web.Services.IdentityNumberService
{
    public interface IIdentityNumberService
    {
        IdentityParseResult Parse(string idNumber);
    }
}
