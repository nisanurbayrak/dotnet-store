using SendGrid;
using SendGrid.Helpers.Mail;

namespace dotnet_store.Services;

public class EmailSender
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailSender(string apiKey, string fromEmail, string fromName)
    {
        _apiKey = apiKey;
        _fromEmail = fromEmail;
        _fromName = fromName;
    }

    public async Task SendConfirmationEmail(string toEmail, string confirmationLink)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);

        var to = new EmailAddress(toEmail);
        var subject = "Confirm Email";
        var htmlContent = $@"
                <h1>Merhaba!</h1>
                <p>Üyeliğinizi tamamlamak için aşağıdaki bağlantıya tıklayabilir ya da onay kodunu kullanabilirsiniz:</p>
                <p><a href='{confirmationLink}'>Üyeliği Onayla</a></p>
                <p>Teşekkürler!</p>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new Exception($"Mail gönderilemedi. StatusCode: {response.StatusCode}");
        }
    }
}
