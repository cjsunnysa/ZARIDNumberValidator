using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandardBankAssessment.Web.Services.LuhnChecksumService
{
    public interface ILuhnChecksumService
    {
        bool IsValid(string sequence);
    }
}
