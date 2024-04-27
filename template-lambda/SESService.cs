namespace template_lambda;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

public class SESService : IEmailService {
    private readonly IAmazonSimpleEmailService amazonSimpleEmailClient;

    public SESService(IAmazonSimpleEmailService amazonSimpleEmailClient) {
        this.amazonSimpleEmailClient = amazonSimpleEmailClient;
    }

    public async Task SendEmailAsync(string toAddress, string subject, string body) {
        var sendRequest = new SendEmailRequest
        {
            Source = "garrett@garrettjohnlaw.com",
            Destination = new Destination
            {
                ToAddresses = new List<string> { toAddress }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content(body)
                }
            }
        };
        await amazonSimpleEmailClient.SendEmailAsync(sendRequest);
    }
}
