using System;
using System.Text.Json;
using Fitogram.Knowable;

// Knowable<MyEnum> myKnowableEnum = new Knowable<MyEnum>();
// myKnowableEnum = 1;
// Console.WriteLine(myKnowableEnum.IsKnown); // True
// Console.WriteLine(myKnowableEnum.Value); // Foo
// myKnowableEnum = 2;
// Console.WriteLine(myKnowableEnum.IsKnown); // False
// Console.WriteLine(myKnowableEnum.Value); // Exception!
//
// public enum MyEnum
// {
//     Foo = 1,
// }

JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
jsonSerializerOptions.Converters.Add(new Fitogram.Knowable.SystemTextJson.KnowableJsonConverter<MyEnum>());
jsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

// The JSON would come from an external application which might have enums your application does not know.
const string json = "{\"MyEnum\":\"Bar\"}";

MyDto myDto = JsonSerializer.Deserialize<MyDto>(json, jsonSerializerOptions);
Console.WriteLine(myDto.MyEnum.IsKnown); // False

if (myDto.MyEnum.IsKnown)
    Console.WriteLine(myDto.MyEnum.Value); // Use the value.
else
    throw new Exception("Unknown enum."); // Handle the issue gracefully.

public enum MyEnum
{
    Foo = 1,
}

public class MyDto
{
    public Knowable<MyEnum> MyEnum { get; set; }
}
