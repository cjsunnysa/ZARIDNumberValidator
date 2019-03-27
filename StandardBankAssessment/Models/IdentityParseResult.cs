using System;

namespace StandardBankAssessment.Web.Models
{
    public class IdentityParseResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }
        public IdentityDetails IdDetails { get; private set; }

        public IdentityParseResult(string errorMessage) : this(false, errorMessage, null)
        {            
        }

        public IdentityParseResult(IdentityDetails details) : this(true, null, details)
        {
        }

        private IdentityParseResult(bool isValid, string errorMessage, IdentityDetails details)
        {
            if (!isValid && string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentNullException(nameof(errorMessage), "Must have a value.");
            }

            if (isValid && details == null)
            {
                throw new ArgumentNullException(nameof(details), "Must have a value.");
            }

            IsValid = isValid;
            ErrorMessage = errorMessage;
            IdDetails = details;
        }
    }
}
