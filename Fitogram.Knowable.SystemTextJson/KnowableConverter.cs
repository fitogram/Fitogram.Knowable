using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fitogram.Knowable.SystemTextJson
{
    public class KnowableConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Knowable<>))
                return false;

            return typeToConvert.GetGenericArguments()[0].IsEnum;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            Type enumType = type.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(KnowableEnumConverterInner<>).MakeGenericType(
                    new Type[] { enumType, }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null
            );

            return converter;
        }

        private class KnowableEnumConverterInner<TEnum> : JsonConverter<Knowable<TEnum>> where TEnum : struct, Enum
        {
            public KnowableEnumConverterInner(JsonSerializerOptions options)
            {
            }

            public override Knowable<TEnum> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                Knowable<TEnum> knowable = reader.TokenType switch
                {
                    JsonTokenType.Number => reader.GetInt32(),
                    JsonTokenType.String => reader.GetString(),
                    _ => throw new JsonException("Unexpected type.")
                };

                return knowable;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Knowable<TEnum> knowable,
                JsonSerializerOptions options)
            {
                // Allows the default enum converter to be used.
                JsonSerializer.Serialize(writer, knowable.Value, options);
            }
        }
    }
}
