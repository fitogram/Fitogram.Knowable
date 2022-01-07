using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fitogram.Knowable.SystemTextJson
{
    public class KnowableJsonConverter : JsonConverter<IKnowable>
    {
        public override IKnowable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IKnowable result = (IKnowable)Activator.CreateInstance(typeToConvert);

            result.Value = reader.GetString();

            return result;
        }

        public override void Write(Utf8JsonWriter writer, IKnowable value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.InnerValue.ToString());
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetProperties().Any(p => typeof(IKnowable).IsAssignableFrom(p.PropertyType));
        }
    }
}
