using System;
using System.Text.Json;

namespace Fitogram.Knowable.Examples
{
    public class JsonExample
    {
        public void Run()
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new Fitogram.Knowable.SystemTextJson.KnowableJsonConverter());
            jsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

            // The JSON would come from an external application which might have enums your application does not know.
            const string json = "{\"MyEnum\":\"Bar\"}";

            MyDto myDto = JsonSerializer.Deserialize<MyDto>(json, jsonSerializerOptions);
            Console.WriteLine(myDto.MyEnum.IsKnown); // False

            if (myDto.MyEnum.IsKnown)
                Console.WriteLine(myDto.MyEnum.Value); // Use the value.
            else
                Console.WriteLine("Enum not known!"); // Handle the issue gracefully.
        }
        
        private enum MyEnum
        {
            Foo = 1,
        }

        private class MyDto
        {
            public Knowable<MyEnum> MyEnum { get; set; }
        }
    }
}
