namespace email_lambda;

public class HandlerInput {
	public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public HandlerInput(string toEmail, string subject, string body)
    {
        ToEmail = toEmail;
        Subject = subject;
        Body = body;
    }
}
