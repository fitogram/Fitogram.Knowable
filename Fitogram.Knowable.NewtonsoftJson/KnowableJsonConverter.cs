using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Fitogram.Knowable.NewtonsoftJson
{
    public class KnowableJsonConverter<T> : JsonConverter<Knowable<T>> where T : Enum
    {
        public Knowable<T> object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = (object)Activator.CreateInstance(objectType);

            JObject jObject = JObject.Load(reader);

            Dictionary<string, JProperty> jsonProperties = jObject.Properties()
                .ToDictionary(x => x.Name.ToLower());

            foreach (PropertyInfo propertyInfo in objectType.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                bool isKnowable = typeof(IKnowable<T>).IsAssignableFrom(propertyInfo.PropertyType);

                // Type propertyType = isKnowable
                //     ? ((IKnowable)propertyInfo.GetValue(result)).GetType()
                //     : propertyInfo.PropertyType;

                string jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName.ToLower()
                    ?? propertyInfo.Name.ToLower();

                if (jsonProperties.ContainsKey(jsonPropertyName))
                {
                    JProperty jsonProperty = jsonProperties[jsonPropertyName];

                    object value = null;

                    try
                    {
                        value = (int) jsonProperty.Value;
                    }
                    catch
                    {
                        try
                        {
                            value = (string) jsonProperty.Value;
                        }
                        catch
                        {
                            throw new Exception("Cannot find out enum type?");
                        }
                    }

                    if (isKnowable)
                    {
                        IKnowable<T> knowable = (IKnowable<T>)propertyInfo.GetValue(result);
                        knowable.Value = value;
                        propertyInfo.SetValue(result, knowable);
                    }
                    else
                    {
                        propertyInfo.SetValue(result, value);
                    }
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            JsonObjectContract jsonContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());

            foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
            {
                JsonProperty jsonProperty = jsonContract.Properties.First(x => x.UnderlyingName == propertyInfo.Name);

                if (jsonProperty.Ignored)
                    continue;

                string jsonPropertyName = jsonProperty.PropertyName;
                object propertyValue = propertyInfo.GetValue(value);

                //Resolve optional wrapper
                if (typeof(IKnowable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    IKnowable knowableProperty = (IKnowable)propertyValue;

                    //Skip property if no value is set
                    if (knowableProperty.IsKnown == false)
                        throw new Exception("Cannot serialize unknown enum.");

                    //Update property value and type
                    propertyValue = knowableProperty.Value;
                }

                //Write property name
                writer.WritePropertyName(jsonPropertyName);

                //Write property value
                serializer.Serialize
                (
                    jsonWriter: writer,
                    value: propertyValue
                );
            }

            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetProperties().Any(p => typeof(IKnowable).IsAssignableFrom(p.PropertyType));
        }

        public override bool CanWrite => true;
        public override bool CanRead => true;
    }
}
