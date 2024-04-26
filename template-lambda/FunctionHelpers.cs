using System.Text.Json;

namespace template_lambda.Helpers;
internal static class FunctionHelpers
{
    internal static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true 
    };

    internal static async Task<Stream> SerializeResult(object result)
    {
        var resultStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(resultStream, result, SerializerOptions);
        resultStream.Seek(0, SeekOrigin.Begin);
        return resultStream;
    }

    internal static async Task<HandlerInput> DeserializeInput(string inputEventString)
    {
        return await Task.FromResult(JsonSerializer.Deserialize<HandlerInput>(inputEventString, SerializerOptions)!);
    }
}
