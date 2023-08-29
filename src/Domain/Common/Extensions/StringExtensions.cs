using System.Globalization;

namespace Domain.Common.Extensions;

public static class StringExtensions
{
    public static string ReplacePlaceHolders(this string text, IReadOnlyCollection<KeyValuePair<string, string>> placeHolders)
    {
        if (text == null || !(placeHolders?.Count > 0)) return text!;
        foreach (var placeHolder in placeHolders.Where(placeHolder => text.Contains(placeHolder.Key)))
        {
            text = text.Replace(placeHolder.Key, placeHolder.Value);
        }
        return text;
    }

    //public static string CompileHandleBar(this string text, object entity)
    //{
    //    if (text == null || (entity is null)) return text!;
    //    var template = Handlebars.Compile(text);
    //    return template(entity);
    //}

    public static int GenerateOrderNumber()
    {
        var init = DateTime.UtcNow.Ticks.ToString();
        var orderNumber = Convert.ToInt32(init.Remove(init.Length - 10)) + new Random().Next();
        return orderNumber < 0 ? orderNumber * -1 : orderNumber;
    }
    public static bool HasItemLike(this IEnumerable<string> strings, string s2)
    {
        if (string.IsNullOrEmpty(s2) || string.IsNullOrWhiteSpace(s2))
        {
            return false;
        }

        return strings.Any(x => x.Like(s2));
    }
    public static bool HasItem(this IEnumerable<string> strings, string s2)
    {
        if (string.IsNullOrEmpty(s2) || string.IsNullOrWhiteSpace(s2))
        {
            return false;
        }

        return strings.Any(x => x.IsEqualTo(s2));
    }
    public static bool HasItem(this IEnumerable<int> ints, int i1)
    {
        return ints.Any(x => x == i1);
    }
    public static bool IsNotEqualTo(this string s1, string s2) => !s1.IsEqualTo(s2);
    public static bool IsEqualTo(this string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrWhiteSpace(s1) || string.IsNullOrEmpty(s2) || string.IsNullOrWhiteSpace(s2))
        {
            return false;
        }

        return s1.Replace(" ", "").ToLower().Equals(s2.Replace(" ", "").ToLower());
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
    public static string ToLowerCase(this string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            str = str.Replace(" ", "");
            if (int.TryParse(str, out int intNumber))
            {
                return intNumber.ToString();
            }
            else if (decimal.TryParse(str, out decimal decimalNumber))
            {
                return decimalNumber.ToString();
            }
            else if (double.TryParse(str, out double doubleNumber))
            {
                return doubleNumber.ToString();
            }
            else
            {
                return str.ToLower();
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// Performs equality checking using behaviour similar to that of SQL's LIKE.
    /// </summary>
    /// <param name="s">The string to check for equality.</param>
    /// <param name="match">The mask to check the string against.</param>
    /// <param name="CaseInsensitive">True if the check should be case insensitive.</param>
    /// <returns>Returns true if the string matches the mask.</returns>
    /// <remarks>
    /// All matches are case-insensitive in the invariant culture.
    /// % acts as a multi-character wildcard.
    /// * acts as a multi-character wildcard.
    /// _ acts as a single-character wildcard.
    /// Backslash acts as an escape character.  It needs to be doubled if you wish to
    /// check for an actual backslash.
    /// [abc] searches for multiple characters.
    /// [^abc] matches any character that is not a,b or c
    /// [a-c] matches a, b or c
    /// Published on CodeProject: http://www.codeproject.com/Articles/
    ///         608266/A-Csharp-LIKE-implementation-that-mimics-SQL-LIKE
    /// </remarks>
    public static bool Like(this string s, string match, bool CaseInsensitive = true)
    {
        //Nothing matches a null mask or null input string
        if (match == null || s == null)
            return false;
        //Null strings are treated as empty and get checked against the mask.
        //If checking is case-insensitive we convert to uppercase to facilitate this.
        if (CaseInsensitive)
        {
            s = s.ToUpperInvariant();
            match = match.ToUpperInvariant();
        }
        //Keeps track of our position in the primary string - s.
        int j = 0;
        //Used to keep track of multi-character wildcards.
        bool matchanymulti = false;
        //Used to keep track of multiple possibility character masks.
        string multicharmask = null;
        bool inversemulticharmask = false;
        for (int i = 0; i < match.Length; i++)
        {
            //If this is the last character of the mask and its a % or * we are done
            if (i == match.Length - 1 && (match[i] == '%' || match[i] == '*'))
                return true;
            //A direct character match allows us to proceed.
            var charcheck = true;
            //Backslash acts as an escape character.  If we encounter it, proceed
            //to the next character.
            if (match[i] == '\\')
            {
                i++;
                if (i == match.Length)
                    i--;
            }
            else
            {
                //If this is a wildcard mask we flag it and proceed with the next character
                //in the mask.
                if (match[i] == '%' || match[i] == '*')
                {
                    matchanymulti = true;
                    continue;
                }
                //If this is a single character wildcard advance one character.
                if (match[i] == '_')
                {
                    //If there is no character to advance we did not find a match.
                    if (j == s.Length)
                        return false;
                    j++;
                    continue;
                }
                if (match[i] == '[')
                {
                    var endbracketidx = match.IndexOf(']', i);
                    //Get the characters to check for.
                    multicharmask = match.Substring(i + 1, endbracketidx - i - 1);
                    //Check for inversed masks
                    inversemulticharmask = multicharmask.StartsWith("^");
                    //Remove the inversed mask character
                    if (inversemulticharmask)
                        multicharmask = multicharmask.Remove(0, 1);
                    //Unescape \^ to ^
                    multicharmask = multicharmask.Replace("\\^", "^");

                    //Prevent direct character checking of the next mask character
                    //and advance to the next mask character.
                    charcheck = false;
                    i = endbracketidx;
                    //Detect and expand character ranges
                    if (multicharmask.Length == 3 && multicharmask[1] == '-')
                    {
                        var newmask = "";
                        var first = multicharmask[0];
                        var last = multicharmask[2];
                        if (last < first)
                        {
                            first = last;
                            last = multicharmask[0];
                        }
                        var c = first;
                        while (c <= last)
                        {
                            newmask += c;
                            c++;
                        }
                        multicharmask = newmask;
                    }
                    //If the mask is invalid we cannot find a mask for it.
                    if (endbracketidx == -1)
                        return false;
                }
            }
            //Keep track of match finding for this character of the mask.
            var matched = false;
            while (j < s.Length)
            {
                //This character matches, move on.
                if (charcheck && s[j] == match[i])
                {
                    j++;
                    matched = true;
                    break;
                }
                //If we need to check for multiple charaters to do.
                if (multicharmask != null)
                {
                    var ismatch = multicharmask.Contains(s[j]);
                    //If this was an inverted mask and we match fail the check for this string.
                    //If this was not an inverted mask check and we did not match fail for this string.
                    if (inversemulticharmask && ismatch ||
                        !inversemulticharmask && !ismatch)
                    {
                        //If we have a wildcard preceding us we ignore this failure
                        //and continue checking.
                        if (matchanymulti)
                        {
                            j++;
                            continue;
                        }
                        return false;
                    }
                    j++;
                    matched = true;
                    //Consumse our mask.
                    multicharmask = null;
                    break;
                }
                //We are in an multiple any-character mask, proceed to the next character.
                if (matchanymulti)
                {
                    j++;
                    continue;
                }
                break;
            }
            //We've found a match - proceed.
            if (matched)
            {
                matchanymulti = false;
                continue;
            }

            //If no match our mask fails
            return false;
        }
        //Some characters are left - our mask check fails.
        if (j < s.Length)
            return false;
        //We've processed everything - this is a match.
        return true;
    }
}

