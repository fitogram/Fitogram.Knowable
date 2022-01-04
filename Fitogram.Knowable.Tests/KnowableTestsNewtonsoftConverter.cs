using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Fitogram.Knowable.Tests
{
    public class KnowableTestsNewtonsoftConverter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public KnowableTestsNewtonsoftConverter()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Converters.Add(new Fitogram.Knowable.NewtonsoftJson.KnowableJsonConverter());
        }

        /// <summary>
        /// Tests should work with or without the string enum converter.
        /// </summary>
        private void AddStringEnumConverter() =>
            // The real issue is with the StringEnumConverter - https://github.com/JamesNK/Newtonsoft.Json/issues/1361#issuecomment-339222662
            // If there's an unexpected value, it will throw an exception.
            _jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

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

            string json = JsonConvert.SerializeObject(unknowableDummyWithBar, _jsonSerializerSettings);;

            if (willThrow)
            {
                Should.Throw<JsonSerializationException>(()
                    => JsonConvert.DeserializeObject<UnknowableDummyWithoutBar>(json, _jsonSerializerSettings)
                );
            }
            else
            {
                UnknowableDummyWithoutBar unknowableDummyWithoutBar =
                    JsonConvert.DeserializeObject<UnknowableDummyWithoutBar>(json, _jsonSerializerSettings);

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

            string json = JsonConvert.SerializeObject(knowableDummyWithBar, _jsonSerializerSettings);

            KnowableDummyWithoutBar knowableDummyWithoutBar =
                JsonConvert.DeserializeObject<KnowableDummyWithoutBar>(json, _jsonSerializerSettings);

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

            string json = JsonConvert.SerializeObject(input, _jsonSerializerSettings);

            DummyWithStandardProperty output =
                JsonConvert.DeserializeObject<DummyWithStandardProperty>(json, _jsonSerializerSettings);

            // [Assert]

            output.MyProperty.ShouldBe(input.MyProperty);
        }

        // Custom serializer is not used on single properties. I think that's just not supported.
        // [Theory]
        // [InlineData(EnumWithBar.Foo, true)]
        // [InlineData(EnumWithBar.Bar, false)]
        // public void Wrapped(Knowable<EnumWithBar> dummyEnum2, bool shouldBeKnown)
        // {
        //     string json = JsonConvert.SerializeObject(dummyEnum2, _jsonSerializerSettings);
        //
        //     Knowable<EnumWithoutBar> enum2 = JsonConvert.DeserializeObject<Knowable<EnumWithoutBar>>(json);
        //
        //     enum2.IsKnown.ShouldBe(shouldBeKnown);
        //
        //     if (shouldBeKnown)
        //         enum2.Value.ToString().ShouldBe(dummyEnum2.Value.ToString());
        // }
        //
        // [Theory]
        // [InlineData(EnumWithBar.Foo, true)]
        // [InlineData(EnumWithBar.Bar, false)]
        // public void NotWrapped(EnumWithBar dummyEnum2, bool shouldBeKnown)
        // {
        //     string json = JsonConvert.SerializeObject(dummyEnum2, _jsonSerializerSettings);
        //
        //     EnumWithoutBar enum2 = JsonConvert.DeserializeObject<EnumWithoutBar>(json);
        //
        //     // enum2.ShouldBe(shouldBeKnown);
        //
        //     if (shouldBeKnown)
        //         enum2.ToString().ShouldBe(dummyEnum2.ToString());
        // }
    }
}
