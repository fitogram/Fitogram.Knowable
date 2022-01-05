# Fitogram.Knowable

## Knowable\<T>

### Basic example

The `Knowable<T>` wrapper tells us if the enum is known:

```csharp
public enum MyEnum
{
    Foo = 1,
}

Knowable<MyEnum> myKnowableEnum = new Knowable<MyEnum>();
myKnowableEnum = 1;

Console.WriteLine(myKnowableEnum.IsKnown); // True
Console.WriteLine(myKnowableEnum.Value); // Foo

myKnowableEnum = 2;

Console.WriteLine(myKnowableEnum.IsKnown); // False
Console.WriteLine(myKnowableEnum.Value); // Exception!
```

https://dotnetfiddle.net/IewleI

### Use it with JSON

This is useful when trying to deserialize a string to an enum:

```csharp
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
    throw new Exception("Unknown enum."); // Handle the issue gracefully.

public enum MyEnum
{
    Foo = 1,
}

public class MyDto
{
    public Knowable<MyEnum> MyEnum { get; set; }
}
```

### Initialization

Creating a Knowable enum is different from creating a normal enum because the Knowable default value is null rather than 0.

```cshtml
MyClassWithNormalEnum myClassWithNormalEnum = new MyClassWithNormalEnum();
Console.WriteLine(myClassWithNormalEnum.MyEnum); // 0

MyClassWithKnowableEnum myClassWithKnowableEnum = new MyClassWithKnowableEnum();
Console.WriteLine(myClassWithKnowableEnum.MyEnum.InnerValue == null); // True

public enum MyEnum
{
    // No actual values.
}

public class MyClassWithNormalEnum
{
    public MyEnum MyEnum { get; set; }
}

public class MyClassWithKnowableEnum
{
    public Knowable<MyEnum> MyEnum { get; set; }
}
```

https://dotnetfiddle.net/7uLoUv
