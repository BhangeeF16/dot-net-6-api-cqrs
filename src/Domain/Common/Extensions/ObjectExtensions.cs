using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Domain.Common.Extensions;

public static class ObjectExtensions
{
    public static bool AreRequiredNullOrEmpty(this object obj)
    {
        if (obj is null)
            return true;

        return obj.GetType().GetProperties()
                            .Select(p => new
                            {
                                Property = p,
                                Attribute = p.GetCustomAttributes(typeof(RequiredAttribute), true)
                                             .Cast<RequiredAttribute>()
                                             .FirstOrDefault()
                            })
                            .Any(x => (x.Attribute != null) && IsNullOrEmpty(x.Property.GetValue(obj)));
    }
    public static bool IsAnyNullOrEmpty(this object obj)
    {
        if (obj is null)
            return true;

        return obj.GetType().GetProperties()
            .Any(x => IsNullOrEmpty(x.GetValue(obj)));
    }

    public static KeyValuePair<string, string>[] ToKeyValuePairs(this object entity)
    {
        var json = JsonConvert.SerializeObject(entity);
        var jobj = JObject.Parse(json);
        var dict = jobj.Children().Cast<JProperty>().ToList().Where(x => !IsNullOrEmpty(x.Value)).ToDictionary(x => x.Name, x => (string)x.Value);
        return dict.ToArray();
    }
    public static List<KeyValuePair<string, string>> ToKeyValuePairList(this object obj)
    {
        var pairs = obj.GetType()
                       .GetProperties()
                       .Select(p => new
                       {
                           Property = p,
                           Attribute = p.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
                                        .Cast<JsonPropertyAttribute>()
                                        .FirstOrDefault()
                       })
                       .ToList();

        var objProps = pairs.Where(x => !IsNullOrEmpty(x.Property.GetValue(obj)))
                            .ToDictionary(x => (x.Attribute != null && string.IsNullOrEmpty(x.Attribute.PropertyName)) ? x.Property.Name : x.Attribute.PropertyName, y => y.Property.GetValue(obj).ToString()
                                         );

        return objProps.ToList();
    }

    public static bool IsValueNullOrEmpty(this object value) => IsNullOrEmpty(value);

    private static bool IsNullOrEmpty(object value)
    {
        if (value is null)
            return true;

        var type = value.GetType();

        if (type == typeof(string))
        {
            return string.IsNullOrEmpty(value.ToString());
        }

        return type.IsValueType && Object.Equals(value, Activator.CreateInstance(type));
    }

    public static bool IsNull(this object obj) => obj == null;
    public static bool IsNotNull(this object obj) => obj != null;
}
