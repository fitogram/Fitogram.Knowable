# Fitogram.Knowable

## NuGet Setup

The NuGet.config file is configured to use environment variables for some sources which need to be set
### Setting environment variables

On Windows, use the setx command to add the corresponding environment variables. It’s also possible to set machine-wide variables using the /s switch. Alternatively you can set them using the System properties window, but don’t forget to reopen the command prompt to use the new values.

> GitHub Package Registry
```
setx GPR_USER your_github_username
setx GPR_TOKEN your_github_pat
````

On Unix systems it depends on your shell program. You can use the following commands to set environment variables when using Bash.
> GitHub Package Registry
```
export GPR_USER=your_github_username
export GPR_TOKEN=your_github_pat
````

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
