using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Domain.Common.Extensions
{
    public static class StringExtensions
    {
        public static int GenerateOrderNumber()
        {
            var init = DateTime.UtcNow.Ticks.ToString();
            var orderNumber = Convert.ToInt32(init.Remove(init.Length - 10)) + new Random().Next();
            return orderNumber < 0 ? orderNumber * -1 : orderNumber;
        }
        public static string ToCommaSeperatedString(this IEnumerable<string> theseStrings)
        {
            if (theseStrings != null && theseStrings.Any())
            {
                return string.Join(",", theseStrings.Distinct().ToList()).Trim();
            }
            return String.Empty;
        }
        public static string ToCommaSeperatedString(this IEnumerable<int> theseIntegers)
        {
            if (theseIntegers != null && theseIntegers.Any())
            {
                return string.Join(",", theseIntegers).Trim();
            }
            return String.Empty;
        }
        public static IEnumerable<string> ToEnumerable(this string thisString, char seperationChar = ',')
        {
            if (string.IsNullOrEmpty(thisString))
            {
                return new List<string>();
            }
            return thisString.Split(seperationChar).Distinct().Select(x => x.Trim()).AsEnumerable();
        }
        public static List<int> ToIntList(this string thisString, char seperationChar = ',')
        {
            if (string.IsNullOrEmpty(thisString))
            {
                return new List<int>();
            }
            var intList = new List<int>();
            foreach (var item in thisString.Split(seperationChar).Distinct().ToList())
            {
                intList.Add(Convert.ToInt32(item.Trim()));
            }
            return intList;
        }
        public static int ToInt(this string HexaDecimalTimeStamp)
        {
            return int.Parse(HexaDecimalTimeStamp, NumberStyles.HexNumber);
        }
        public static string ToLowerCase(this string QuestionItemTitle)
        {
            if (!string.IsNullOrEmpty(QuestionItemTitle))
            {
                QuestionItemTitle = QuestionItemTitle.Replace(" ", "");
                if (int.TryParse(QuestionItemTitle, out int intNumber))
                {
                    return intNumber.ToString();
                }
                else if (decimal.TryParse(QuestionItemTitle, out decimal decimalNumber))
                {
                    return decimalNumber.ToString();
                }
                else if (double.TryParse(QuestionItemTitle, out double doubleNumber))
                {
                    return doubleNumber.ToString();
                }
                else
                {
                    return QuestionItemTitle.ToLower();
                }
            }
            return string.Empty;
        }
    }
}

