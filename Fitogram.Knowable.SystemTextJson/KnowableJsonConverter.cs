using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fitogram.Knowable.SystemTextJson
{
    public class KnowableJsonConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object result = (object)Activator.CreateInstance(typeToConvert);

            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                Dictionary<string, JsonProperty> jsonProperties = document.RootElement.EnumerateObject()
                    .ToDictionary(x => x.Name.ToLower());

                foreach (PropertyInfo propertyInfo in typeToConvert.GetProperties())
                {
                    if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                        continue;

                    bool isKnowable = typeof(IKnowable).IsAssignableFrom(propertyInfo.PropertyType);

                    string jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.ToLower()
                        ?? propertyInfo.Name.ToLower();

                    if (jsonProperties.ContainsKey(jsonPropertyName))
                    {
                        JsonProperty jsonProperty = jsonProperties[jsonPropertyName];

                        if (jsonProperty.Value.ValueKind != JsonValueKind.Undefined)
                        {
                            object value = jsonProperty.Value.ValueKind switch
                            {
                                // It will only be a string if using JsonStringEnumConverter.
                                JsonValueKind.String => jsonProperty.Value.GetString(),
                                // Otherwise enums will generally be integers.
                                JsonValueKind.Number => jsonProperty.Value.GetInt32(),
                                _ => null
                            };

                            if (isKnowable)
                            {
                                IKnowable knowable = (IKnowable)propertyInfo.GetValue(result);
                                knowable.Value = value;
                                propertyInfo.SetValue(result, knowable);
                            }
                            else
                            {
                                propertyInfo.SetValue(result, value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                string jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                    ?? options.PropertyNamingPolicy?.ConvertName(propertyInfo.Name)
                    ?? propertyInfo.Name;

                object propertyValue = propertyInfo.GetValue(value);
                Type propertyType = propertyInfo.PropertyType;

                //Resolve optional wrapper
                if (typeof(IKnowable).IsAssignableFrom(propertyType))
                {
                    IKnowable knowableProperty = (IKnowable)propertyValue;

                    //Skip property if no value is set
                    if (knowableProperty.IsKnown == false)
                        throw new Exception("Cannot serialize unknown enum.");

                    //Update property value and type
                    propertyValue = knowableProperty.Value;
                    propertyType = knowableProperty.Value.GetType();
                }

                //Write property name
                writer.WritePropertyName(jsonPropertyName);

                //Write property value
                JsonSerializer.Serialize
                (
                    writer: writer,
                    value: propertyValue,
                    inputType: propertyType,
                    options: options
                );
            }

            writer.WriteEndObject();
        }


        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetProperties().Any(p => typeof(IKnowable).IsAssignableFrom(p.PropertyType));
        }
    }
}
