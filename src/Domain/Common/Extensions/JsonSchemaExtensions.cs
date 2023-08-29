using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

public static class JsonSchemaExtensions
{
    public static async Task<string> ToJsonAsync(this Stream stream)
    {
        using (StreamReader streamReader = new(stream))
        {
            stream.Position = 0;
            return await streamReader.ReadToEndAsync();
        }
    }

    public static JSchema GenerateSchema<T>(this object obj)
    {
        var generator = new JSchemaGenerator();
        var schema = generator.Generate(typeof(T));
        return schema;
    }
    public static bool ValidateSchema<T>(this object obj, bool IsCollection = false)
    {
        bool valid = false;
        IList<string> messages = null;
        var schema = obj.GenerateSchema<T>();
        var json = JsonConvert.SerializeObject(obj);
        if (IsCollection)
        {
            JArray jArr = JArray.Parse(json);
            valid = jArr.IsValid(schema, out messages);
        }
        else
        {
            JObject jObj = JObject.Parse(json);
            valid = jObj.IsValid(schema, out messages);
        }

        //if (!valid)
        //{
        //    throw new ClientException(string.Join("\n", messages));
        //}

        return valid;
    }
    public static bool ValidateSchema<T>(this string json, bool IsCollection = false)
    {
        bool valid = false;
        IList<string> messages = null;
        JSchema schema = JSchema.Parse(json);
        if (IsCollection)
        {
            JArray arr = JArray.Parse(json);
            valid = arr.IsValid(schema, out messages);
        }
        else
        {
            JObject obj = JObject.Parse(json);
            valid = obj.IsValid(schema, out messages);
        }

        //if (!valid)
        //{
        //    throw new ClientException(string.Join("\n", messages));
        //}

        return valid;
    }
    public static JSchemaType GetSchemaType(Type type)
    {
        // If the type is a nullable type, get the underlying type
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        // Determine the schema type based on the type of the property
        if (type == typeof(string))
        {
            return JSchemaType.String;
        }
        else if (type == typeof(int) || type == typeof(long))
        {
            return JSchemaType.Integer;
        }
        else if (type == typeof(float) || type == typeof(double))
        {
            return JSchemaType.Number;
        }
        else if (type == typeof(bool))
        {
            return JSchemaType.Boolean;
        }
        else if (type.IsEnum)
        {
            return JSchemaType.String;
        }
        else if (type.IsClass)
        {
            return JSchemaType.Object;
        }
        else
        {
            return JSchemaType.None;
        }
    }
}
