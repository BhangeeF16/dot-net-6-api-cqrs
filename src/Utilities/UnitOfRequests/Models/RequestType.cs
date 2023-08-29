using Newtonsoft.Json;

namespace Utilities.UnitOfRequests.Models;

public class RequestType
{
    public object ItemInstance { get; set; }

    /// <summary>
    /// use this method 
    /// if (configuration.BodyParameterType == BodyParameterType.FormUrlEncoded || configuration.BodyParameterType == BodyParameterType.FormData)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public List<KeyValuePair<string, string>> GetUrlEncodedParams()
    {
        var pairs = ItemInstance.GetType()
                                .GetProperties()
                                .Select(p => new
                                {
                                    Property = p,
                                    Attribute = p.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
                                                 .Cast<JsonPropertyAttribute>()
                                                 .FirstOrDefault()
                                })
                                .ToList();

        var objProps = pairs.Where(x => !IsNullOrEmpty(x.Property.GetValue(ItemInstance)))
                            .ToDictionary(x => x.Attribute != null && string.IsNullOrEmpty(x.Attribute.PropertyName) ? x.Property.Name : x.Attribute.PropertyName, y => y.Property.GetValue(ItemInstance).ToString()
                                         );

        return objProps.ToList();
    }

    private static bool IsNullOrEmpty(object value)
    {
        if (value is null)
            return true;

        var type = value.GetType();

        if (type == typeof(string))
        {
            return string.IsNullOrEmpty(value.ToString());
        }

        return type.IsValueType && Equals(value, Activator.CreateInstance(type));
    }

}
