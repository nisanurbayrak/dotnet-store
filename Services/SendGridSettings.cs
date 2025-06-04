namespace dotnet_store.Services;

public class SendGridSettings
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
}
