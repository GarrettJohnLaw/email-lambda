using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.SimpleEmail;
using Microsoft.Extensions.DependencyInjection;
using template_lambda.Helpers;

namespace template_lambda;
public class Function
{
    private readonly IServiceProvider serviceProviderRoot;

    public Function()
    {
        this.serviceProviderRoot = new ServiceCollection()
            .AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>()
            .BuildServiceProvider(true);
    }

    public Function(IServiceProvider sp)
    {
        this.serviceProviderRoot = sp;
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<string> FunctionHandlerAsync(Stream inputStream, Amazon.Lambda.Core.ILambdaContext? context = null)
    {
        var inputEventString = await inputStream.ReadAsStringAsync();
        HandlerInput input = await FunctionHelpers.DeserializeInput(inputEventString);
        string body = input.Body;
        return await Task.FromResult(body);

    }
}
