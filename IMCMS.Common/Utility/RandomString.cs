using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMCMS.Common.Utility
{
    public class RandomString
    {
        //create constant strings for each type of characters
        static string alphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
        static string alphaLow = "qwertyuiopasdfghjklzxcvbnm";
        static string numerics = "1234567890";
        static string special = "$@()";
        //create another string which is a concatenation of all above
        static string allChars = alphaCaps + alphaLow + numerics + special;
        static Random r = new Random();

        public static string Generate(int length)
        {
            String generatedString = "";

            if (length < 4)
                throw new Exception("Number of characters should be greater than 4.");

            // Generate four repeating random numbers are postions of lower,
            // upper, numeric and special characters
            // By filling these positions with corresponding characters,
            // we can ensure the password has atleast one
            // character of those types
            int pLower, pUpper, pNumber, pSpecial;
            string posArray = "0123456789";
            if (length < posArray.Length)
                posArray = posArray.Substring(0, length);
            string randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble() * posArray.Length)].ToString();
            pLower = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");
            randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble() * posArray.Length)].ToString();
            pUpper = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");
            randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble() * posArray.Length)].ToString();
            pNumber = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");
            randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble() * posArray.Length)].ToString();
            pSpecial = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");


            for (int i = 0; i < length; i++)
            {
                double rand = r.NextDouble();
                if (i == pLower)
                    generatedString += alphaCaps.ToCharArray()[(int)Math.Floor(rand * alphaCaps.Length)];
                else if (i == pUpper)
                    generatedString += alphaLow.ToCharArray()[(int)Math.Floor(rand * alphaLow.Length)];
                else if (i == pNumber)
                    generatedString += numerics.ToCharArray()[(int)Math.Floor(rand * numerics.Length)];
                else if (i == pSpecial)
                    generatedString += special.ToCharArray()[(int)Math.Floor(rand * special.Length)];
                else
                    generatedString += allChars.ToCharArray()[(int)Math.Floor(rand * allChars.Length)];
            }
            return generatedString;
        }
    }
}
