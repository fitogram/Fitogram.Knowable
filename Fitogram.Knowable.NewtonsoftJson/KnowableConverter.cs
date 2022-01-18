using System;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Fitogram.Knowable.NewtonsoftJson
{
    public class KnowableConverter : JsonConverter<IKnowable>
    {
        public override IKnowable ReadJson(JsonReader reader, Type objectType, IKnowable existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            IKnowable knowable = (IKnowable)Activator.CreateInstance(objectType);

            knowable.Value = reader.Value switch
            {
                long value => (Int32)value,
                int value => value,
                string value => value,
                _ => throw new JsonException("Unexpected type.")
            };

            return knowable;
        }

        public override void WriteJson(JsonWriter writer, IKnowable value, JsonSerializer serializer)
        {
            // Allows the default enum converter to be used.
            serializer.Serialize(writer, value.Value);
        }

        public override bool CanWrite => true;
        public override bool CanRead => true;
    }
}
