using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using Xunit;

namespace Fitogram.Knowable.Tests
{
    public class KnowableTestsJsonNetConverter
    {
        private readonly JsonSerializerOptions _jsonSerializerSettings;

        public KnowableTestsJsonNetConverter()
        {
            _jsonSerializerSettings = new JsonSerializerOptions();
            _jsonSerializerSettings.Converters.Add(new Fitogram.Knowable.SystemTextJson.KnowableJsonConverter());
        }

        /// <summary>
        /// Tests should work with or without the string enum converter.
        /// </summary>
        private void AddStringEnumConverter() =>
            // The real issue is with the StringEnumConverter - https://github.com/JamesNK/Newtonsoft.Json/issues/1361#issuecomment-339222662
            // If there's an unexpected value, it will throw an exception.
            _jsonSerializerSettings.Converters.Add(new JsonStringEnumConverter());

        private enum EnumWithoutBar
        {
            Foo = 1,
        }

        public enum EnumWithBar
        {
            Foo = 1,
            Bar = 2,
        }

        private class UnknowableDummyWithoutBar
        {
            public EnumWithoutBar UnknowableEnum { get; set; }
        }

        private class UnknowableDummyWithBar
        {
            public EnumWithBar UnknowableEnum { get; set; }
        }

        private class KnowableDummyWithoutBar
        {
            public Knowable<EnumWithoutBar> KnowableEnum { get; set; }
        }

        private class KnowableDummyWithBar
        {
            public Knowable<EnumWithBar> KnowableEnum { get; set; }
        }

        [Theory]
        [InlineData(EnumWithBar.Foo, true, false, false)]
        [InlineData(EnumWithBar.Bar, false, false, false)]
        [InlineData(EnumWithBar.Foo, true, false, true)]
        [InlineData(EnumWithBar.Bar, false, true, true)]
        public void UnknowableTest(EnumWithBar enumWithBar, bool shouldBeKnown, bool willThrow, bool useStringEnumConverter)
        {
            // [Arrange]

            if (useStringEnumConverter)
                AddStringEnumConverter();

            UnknowableDummyWithBar unknowableDummyWithBar = new UnknowableDummyWithBar
            {
                UnknowableEnum = enumWithBar,
            };

            // [Act]

            string json = JsonSerializer.Serialize(unknowableDummyWithBar, _jsonSerializerSettings);;

            if (willThrow)
            {
                Should.Throw<JsonException>(()
                    => JsonSerializer.Deserialize<UnknowableDummyWithoutBar>(json, _jsonSerializerSettings)
                );
            }
            else
            {
                UnknowableDummyWithoutBar unknowableDummyWithoutBar =
                    JsonSerializer.Deserialize<UnknowableDummyWithoutBar>(json, _jsonSerializerSettings);

                // [Assert]

                if (shouldBeKnown)
                    unknowableDummyWithoutBar.UnknowableEnum.ToString().ShouldBe(enumWithBar.ToString());
            }
        }

        [Theory]
        [InlineData(EnumWithBar.Foo, true, false)]
        [InlineData(EnumWithBar.Bar, false, false)]
        [InlineData(EnumWithBar.Foo, true, true)]
        [InlineData(EnumWithBar.Bar, false, true)]
        public void KnowableTest(EnumWithBar enumWithBar, bool shouldBeKnown, bool useStringEnumConverter)
        {
            // [Arrange]

            if (useStringEnumConverter)
                AddStringEnumConverter();

            KnowableDummyWithBar knowableDummyWithBar = new KnowableDummyWithBar
            {
                KnowableEnum = enumWithBar,
            };

            // [Act]

            string json = JsonSerializer.Serialize(knowableDummyWithBar, _jsonSerializerSettings);

            KnowableDummyWithoutBar knowableDummyWithoutBar =
                JsonSerializer.Deserialize<KnowableDummyWithoutBar>(json, _jsonSerializerSettings);

            // [Assert]

            knowableDummyWithoutBar.KnowableEnum.IsKnown.ShouldBe(shouldBeKnown);

            if (shouldBeKnown)
                knowableDummyWithoutBar.KnowableEnum.Value.ToString().ShouldBe(enumWithBar.ToString());
        }

        public class DummyWithStandardProperty
        {
            public string MyProperty { get; set; }
        }

        [Fact]
        public void Knowable_ShouldNotTouchOtherProperties()
        {
            // [Arrange]

            AddStringEnumConverter();

            DummyWithStandardProperty input = new DummyWithStandardProperty
            {
                MyProperty = "Foo",
            };

            // [Act]

            string json = JsonSerializer.Serialize(input, _jsonSerializerSettings);

            DummyWithStandardProperty output =
                JsonSerializer.Deserialize<DummyWithStandardProperty>(json, _jsonSerializerSettings);

            // [Assert]

            output.MyProperty.ShouldBe(input.MyProperty);
        }
    }
}
