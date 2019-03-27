using Microsoft.AspNetCore.Mvc;
using StandardBankAssessment.Web.Services.IdentityNumberService;

namespace StandardBankAssessment.Web.Controllers
{
    [Route("api/identitynumber")]
    public class IdentityNumberController : ControllerBase
    {
        private readonly IIdentityNumberService _idService;

        public IdentityNumberController(IIdentityNumberService idService)
        {
            _idService = idService;
        }

        [HttpGet("verify/{idNumber}")]
        public IActionResult Verfiy(string idNumber)
        {
            try
            {
                var result = _idService.Parse(idNumber);

                return Ok(result);
            }
            catch
            {
                // log exception
                return new StatusCodeResult(500);
            }
        }
    }
}
