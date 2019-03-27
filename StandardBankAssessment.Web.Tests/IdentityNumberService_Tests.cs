using FakeItEasy;
using NUnit.Framework;
using StandardBankAssessment.Web.Models;
using StandardBankAssessment.Web.Services.DateProvider;
using StandardBankAssessment.Web.Services.IdentityNumberService;
using StandardBankAssessment.Web.Services.LuhnChecksumService;
using System;

namespace Tests
{
    public class IdentitiyNumberService_Tests
    {
        private static string InvalidBirthDateError(string yearPrefix, string year, string month, string day) => new ArgumentException("idNumber", $"Invalid date of birth: {yearPrefix + year}/{month}/{day}.").Message;
        private static readonly string NullOrEmptyError = new ArgumentNullException("idNumber", "Must have a value.").Message;
        private static readonly string LengthNot13Error = new ArgumentException("idNumber", "Must be 13 characters in length.").Message;
        private static readonly string ContainsNonNumericError = new ArgumentOutOfRangeException("idNumber", "Can only contain numbers.").Message;
        private static readonly string InvalidCitizenshipError = new ArgumentOutOfRangeException("idNumber", "The citizen digit can only have a value of 0 or 1.").Message;
        private static readonly string ChecksumFailedError = new ArgumentException("idNumber", "The checksum is incorrect.").Message;

        private IDateProvider _dateProvider;
        private ILuhnChecksumService _checksumService;
        private IdentityNumberService _idService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var testDate = new DateTime(2015, 01, 01);

            _dateProvider =
                A.Fake<IDateProvider>();

            A.CallTo(() => _dateProvider.GetCurrentDate()).Returns(testDate);

            _checksumService =
                A.Fake<ILuhnChecksumService>();

            A.CallTo(_checksumService)
                .Where(call => call.Method.Name == "IsValid")
                .WithReturnType<bool>()
                .Returns(true);

            _idService = new IdentityNumberService(_dateProvider, _checksumService);
        }

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Should_BeInvalid_When_Null()
        {
            var result = _idService.Parse(null);

            AssertInvalid(result);
        }

        [Test]
        public void Should_HaveCorrectErrorMessage_When_Null()
        {
            var result = _idService.Parse(null);

            AssertCorrectMessage(result, NullOrEmptyError);
        }

        [Test]
        public void Should_BeInvalid_When_Empty()
        {
            var result = _idService.Parse("");

            AssertInvalid(result);
        }

        [Test]
        public void Should_HaveCorrectErrorMessage_When_Empty()
        {
            var result = _idService.Parse("");

            AssertCorrectMessage(result, NullOrEmptyError);
        }

        [TestCase("1")]
        [TestCase("123456789112")]
        [TestCase("12345678911234")]
        public void Should_BeInvalid_When_LengthNot13(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            AssertInvalid(result);
        }

        [TestCase("1")]
        [TestCase("123456789112")]
        [TestCase("12345678911234")]
        public void Should_HaveCorrectErrorMessage_When_LengthNot13(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            AssertCorrectMessage(result, LengthNot13Error);
        }

        [TestCase("8293A38296238")]
        [TestCase("8293382@96238")]
        [TestCase("a293238296238")]
        public void Should_BeInvalid_When_ContainsNonNumeric(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            AssertInvalid(result);
        }

        [TestCase("8293A38296238")]
        [TestCase("8293382@96238")]
        [TestCase("a293238296238")]
        public void Should_HaveCorrectErrorMessage_When_ContainsNonNumeric(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            AssertCorrectMessage(result, ContainsNonNumericError);
        }

        [TestCase("8503915055089")]
        [TestCase("8502295055089")]
        [TestCase("8513015055089")]
        public void Should_BeInvalid_When_ContainsInvalidBirthDate(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            AssertInvalid(result);
        }

        [TestCase("8503915055089")]
        [TestCase("8502295055089")]
        [TestCase("8513015055089")]
        public void Should_HaveCorrectErrorMessage_When_ContainsInvalidBirthDate(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            var (year, month, day) = ExtractYearMonthDay(testNumber);

            AssertCorrectMessage(result, InvalidBirthDateError("19", year, month, day));
        }

        [TestCase("1401015055089", "20")]
        [TestCase("1501015055089", "20")]
        [TestCase("1601015055089", "19")]
        public void Should_Assume20thCentury_When_IdNumberYearGreaterThanCurrentYear(string testNumber, string expectedPrefix)
        {
            // CurrentDate: 01/01/2015
            var result = _idService.Parse(testNumber);

            var birthDatePrefix = result.IdDetails.DateOfBirth.Year.ToString().Substring(0, 2);

            Assert.AreEqual(expectedPrefix, birthDatePrefix);
        }

        [TestCase("9006104000086")]
        [TestCase("9006104999089")]
        [TestCase("9006104800089")]
        public void Should_SetGenderFemale_When_GenderDigitsLessThan5000(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            Assert.That(result.IdDetails.Gender == "female", $"Expected: female\nActual: {result.IdDetails.Gender}");
        }

        [TestCase("9006105999088")]
        [TestCase("9006105561086")]
        [TestCase("9006105000085")]
        public void Should_SetGenderMale_When_GenderDigitsGreaterOrEqual5000(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            Assert.That(result.IdDetails.Gender == "male", $"Expected: male\nActual: {result.IdDetails.Gender}");
        }

        [TestCase("9006105999088")]
        [TestCase("7411274984089")]
        [TestCase("5901144254087")]
        public void Should_SetCitizen_When_CitizenDigitEquals0(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            Assert.That(result.IdDetails.Citizenship == "SA Citizen", $"Expected: SA Citizen\nActual: {result.IdDetails.Citizenship}");
        }

        [TestCase("9110034266187")]
        [TestCase("9808174503182")]
        [TestCase("5901144254186")]
        public void Should_SetResident_When_CitizenDigitEquals1(string testNumber)
        {
            var result = _idService.Parse(testNumber);

            Assert.That(result.IdDetails.Citizenship == "Permanent Resident", $"Expected: Permanent Resident\nActual: {result.IdDetails.Citizenship}");
        }

        [Test]
        public void Should_BeInvalid_When_CitizenDigitGreaterThan1()
        {
            var result = _idService.Parse("5901144254286");

            AssertInvalid(result);
        }

        [Test]
        public void Should_HaveCorrectErrorMessage_When_CitizenDigitGreaterThan1()
        {
            var result = _idService.Parse("5901144254286");

            AssertCorrectMessage(result, InvalidCitizenshipError);
        }

        [TestCase("9110034266186")]
        [TestCase("9808174503183")]
        [TestCase("5901144254185")]
        public void Should_BeInvalid_When_ChecksumFailed(string testNumber)
        {
            var checksumService = A.Fake<ILuhnChecksumService>();

            A.CallTo(checksumService)
                .Where(call => call.Method.Name == "IsValid")
                .WithReturnType<bool>()
                .Returns(false);

            var idService = new IdentityNumberService(_dateProvider, checksumService);

            var result = idService.Parse(testNumber);

            AssertInvalid(result);
        }

        [TestCase("9110034266186")]
        [TestCase("9808174503183")]
        [TestCase("5901144254185")]
        public void Should_HaveCorrectErrorMessage_When_ChecksumFailed(string testNumber)
        {
            var checksumService = A.Fake<ILuhnChecksumService>();

            A.CallTo(checksumService)
                .Where(call => call.Method.Name == "IsValid")
                .WithReturnType<bool>()
                .Returns(false);

            var idService = new IdentityNumberService(_dateProvider, checksumService);

            var result = idService.Parse(testNumber);

            AssertCorrectMessage(result, ChecksumFailedError);
        }

        private static (string year, string month, string day) ExtractYearMonthDay(string testNumber)
        {
            var year = testNumber.Substring(0, 2);
            var month = testNumber.Substring(2, 2);
            var day = testNumber.Substring(4, 2);

            return (year, month, day);
        }

        private static void AssertCorrectMessage(IdentityParseResult result, string correctMessage)
        {
            Assert.That(result.ErrorMessage == correctMessage, $"\nShould be: {correctMessage}\nIs: {result.ErrorMessage}");
        }

        private static void AssertInvalid(IdentityParseResult result)
        {
            Assert.That(!result.IsValid, "Id number is not invalid.");
        }
    }
}