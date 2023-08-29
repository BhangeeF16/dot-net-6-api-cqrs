using System.ComponentModel;

namespace Domain.Common.Extensions;

public class EnumModel
{
    public int Value { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public EnumModel(int iD, string? name, string? description)
    {
        Value = iD;
        Name = name;
        Description = description;
    }

}

public static class EnumExtensions
{
    public static IEnumerable<EnumModel> ToList<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var values = Enum.GetValues(typeof(T)).Cast<T>();
        return values.Select(x => new EnumModel(Convert.ToInt32(x), x.ToString(), x.GetDescription()));

    }
    public static T ToEnum<T>(this string enumDescription) where T : struct, IConvertible
    {
        Enum.TryParse(enumDescription, true, out T result);
        return result;
    }
    public static T ToEnum<T>(this int enumValue) where T : struct, IConvertible
    {
        return Enum.Parse<T>(enumValue.ToString(), true);
    }
    public static string GetDescription<T>(this T enumValue, bool removeSpaces = false, bool lowerCase = false) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        description = (removeSpaces ? description.Trim().Replace(' ', '_') : description.Trim());
        description = (lowerCase ? description.ToLower() : description);

        return description;
    }
}
