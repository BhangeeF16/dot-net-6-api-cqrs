﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Common.Extensions;

public class DateTimeJsonConverter : JsonConverter
{
    public DateTimeJsonConverter()
    {

    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        // implement in case you're serializing it back
    }

    //public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => reader.Value is long ticks ? new DateTime(ticks) : Convert.ToDateTime(reader.Value);
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        DateTime date;
        if (reader.Value is long ticks)
        {
            date = new DateTime(ticks);
        }
        else
        {
            date = Convert.ToDateTime(reader.Value);
        }

        return date;
    }

    public override bool CanConvert(Type objectType) => true;
}

public sealed class JsonTypeConverter<TModel> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TModel);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var result = default(TModel);

        switch (reader.TokenType)
        {
            case JsonToken.Integer:
            case JsonToken.Float:
            case JsonToken.String:
            case JsonToken.Boolean:
            case JsonToken.Date:
                result = (TModel)Convert.ChangeType(reader.Value, JsonTypeConverter<TModel>.GetUnderlyingType());
                break;
            case JsonToken.StartObject:
                JsonTypeConverter<TModel>.GetObject(reader, out result);
                break;
        }
        return result;
    }

    private static void GetObject(JsonReader reader, out TModel result)
    {
        var tags = JObject.Load(reader);
        result = tags != null && tags.Count > 0
            ? (TModel)Convert.ChangeType((string)tags[0], JsonTypeConverter<TModel>.GetUnderlyingType())
            : default;
    }

    private static Type GetUnderlyingType()
    {
        var type = typeof(TModel);
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
