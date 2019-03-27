using StandardBankAssessment.Web.Models;
using StandardBankAssessment.Web.Services.DateProvider;
using StandardBankAssessment.Web.Services.LuhnChecksumService;
using System;
using System.Text.RegularExpressions;

namespace StandardBankAssessment.Web.Services.IdentityNumberService
{
    public class IdentityNumberService : IIdentityNumberService
    {
        private readonly IDateProvider _dateProvider;
        private readonly ILuhnChecksumService _checksumService;

        public IdentityNumberService(IDateProvider dateProvider, ILuhnChecksumService checksumService)
        {
            _dateProvider = dateProvider;
            _checksumService = checksumService;
        }

        public IdentityParseResult Parse(string idNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(idNumber))
                {
                    throw new ArgumentNullException(nameof(idNumber), "Must have a value.");
                }

                if (idNumber.Length != 13)
                {
                    throw new ArgumentException(nameof(idNumber), "Must be 13 characters in length.");
                }

                if (Regex.IsMatch(idNumber, "[^0-9]"))
                {
                    throw new ArgumentOutOfRangeException(nameof(idNumber), "Can only contain numbers.");
                }

                
                RunLuhnCheck(idNumber);

                var dateOfBirth = ParseDateOfBirth(idNumber);
                var gender = ParseGender(idNumber);
                var citizenship = ParseCitizenship(idNumber);

                var identityDetails = new IdentityDetails(idNumber, dateOfBirth, gender, citizenship);

                return new IdentityParseResult(identityDetails);
            }
            catch (Exception ex)
            {
                return new IdentityParseResult(ex.Message);
            }
        }

        private void RunLuhnCheck(string idNumber)
        {
            if (!_checksumService.IsValid(idNumber))
            {
                throw new ArgumentException(nameof(idNumber), "The checksum is incorrect.");
            }
        }

        private string ParseCitizenship(string idNumber)
        {
            var citizen = int.Parse(idNumber.Substring(10, 1));
            if (citizen > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(idNumber), "The citizen digit can only have a value of 0 or 1.");
            }

            return citizen == 0 ? "SA Citizen" : "Permanent Resident";
        }

        private string ParseGender(string idNumber)
        {
            var gender = int.Parse(idNumber.Substring(6, 4));
            return gender < 5000 ? "female" : "male";
        }

        private DateTime ParseDateOfBirth(string idNumber)
        {
            var year = idNumber.Substring(0, 2);
            var month = idNumber.Substring(2, 2);
            var day = idNumber.Substring(4, 2);

            var thisYear = _dateProvider.GetCurrentDate().Year.ToString().Substring(2);
            var yearPrefix = int.Parse(year) > int.Parse(thisYear) ? "19" : "20";

            try
            {
                return new DateTime(int.Parse(yearPrefix + year), int.Parse(month), int.Parse(day));
            }
            catch (Exception)
            {
                throw new ArgumentException(nameof(idNumber), $"Invalid date of birth: {yearPrefix + year}/{month}/{day}.");
            }
        }
    }
}
