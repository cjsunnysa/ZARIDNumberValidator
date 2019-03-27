using System;

namespace StandardBankAssessment.Web.Models
{
    public class IdentityDetails
    {
        public string IdNumber { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public string Citizenship { get; private set; }

        public IdentityDetails(string idNumber, DateTime dateOfBirth, string gender, string citizenship)
        {
            if (string.IsNullOrEmpty(idNumber))
            {
                throw new ArgumentNullException(nameof(idNumber), "Must have a value.");
            }

            if (string.IsNullOrEmpty(gender))
            {
                throw new ArgumentNullException(nameof(gender), "Must have a value.");
            }

            if (string.IsNullOrEmpty(citizenship))
            {
                throw new ArgumentNullException(nameof(citizenship), "Must have a value");
            }

            IdNumber = idNumber;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Citizenship = citizenship;
        }
    }
}