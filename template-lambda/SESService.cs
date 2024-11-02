namespace email_lambda;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using template_lambda;

public class SESService : IEmailService {
    private readonly IAmazonSimpleEmailService amazonSimpleEmailClient;
    private readonly NotionClient notionClient;

    public SESService(IAmazonSimpleEmailService amazonSimpleEmailClient, NotionClient notionClient) {
        this.amazonSimpleEmailClient = amazonSimpleEmailClient;
        this.notionClient = notionClient;
    }

    private string GetDaySuffix(int day) {
        return (day % 10 == 1 && day != 11) ? "st" :
               (day % 10 == 2 && day != 12) ? "nd" :
               (day % 10 == 3 && day != 13) ? "rd" : "th";
    }

    public async Task SendEmailAsync(List<string> recipients, string subject, string body) {
        var services = await notionClient.GetServicesInNext12Days("5f7984ade99949c48b2ebe3e356819bf", "Service Time");
        var pacificZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");

        var formattedServices = string.Join("<br><br>", services.Select(s => {
            DateTime serviceDate = DateTime.SpecifyKind(DateTime.Parse(s.ServiceTime), DateTimeKind.Utc);
            DateTime pacificDate = TimeZoneInfo.ConvertTimeFromUtc(serviceDate, pacificZone);

            string formattedDate = pacificDate.ToString("dddd, MMMM d") + GetDaySuffix(pacificDate.Day) + pacificDate.ToString(", h:mmtt").ToLower();
            return $"{formattedDate}<br>{s.MusicList}";
        }));

        var sendRequest = new SendEmailRequest {
            Source = "garrett@garrettjohnlaw.com",
            Destination = new Destination {
                ToAddresses = recipients
            },
            Message = new Message {
                Subject = new Content(subject),
                Body = new Body {
                    Html = new Content($"{body}<br><br>{formattedServices}<br><br>Kindly,<br>Garrett")
                }
            }
        };
        await amazonSimpleEmailClient.SendEmailAsync(sendRequest);
    }
}
