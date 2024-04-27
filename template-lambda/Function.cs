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
            .AddSingleton<IEmailService, SESService>()
            .BuildServiceProvider(true);
    }

    public Function(IServiceProvider sp)
    {
        this.serviceProviderRoot = sp;
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<Stream> FunctionHandlerAsync(Stream inputStream, Amazon.Lambda.Core.ILambdaContext? context = null)
    {
        var inputEventString = await inputStream.ReadAsStringAsync();
        using var scope = this.serviceProviderRoot.CreateScope();
        HandlerInput input = await FunctionHelpers.DeserializeInput(inputEventString);
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        await emailService.SendEmailAsync(input.ToEmail, input.Subject, input.Body);


        return await FunctionHelpers.SerializeResult(input);

    }
}
