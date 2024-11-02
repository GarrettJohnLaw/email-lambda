namespace email_lambda;

public class HandlerInput {
    public string Name { get; set; }    
    public List<string> Recipients { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public HandlerInput(string name, List<string> recipients, string subject, string body) {
        Name = name;
        Recipients = recipients;
        Subject = subject;
        Body = body;
    }
}
