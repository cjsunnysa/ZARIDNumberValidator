using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandardBankAssessment.Web.Services.LuhnChecksumService
{
    public class LuhnChecksumService : ILuhnChecksumService
    {
        public bool IsValid(string sequence)
        {
            var total = 0;
            var iteration = 0;

            for (int i = sequence.Length - 1; i >= 0; i--)
            {
                iteration++;

                Math.DivRem(iteration, 2, out int remainder);

                if (remainder > 0)
                {
                    total += int.Parse(sequence[i].ToString());
                    continue;
                }

                var digit = int.Parse(sequence[i].ToString()) * 2;

                if (digit > 9)
                {
                    var digitString = digit.ToString();
                    var digit1 = int.Parse(digitString[0].ToString());
                    var digit2 = int.Parse(digitString[1].ToString());

                    digit = digit1 + digit2;
                }

                total += digit;
            }

            return total % 10 == 0;
        }
    }
}
