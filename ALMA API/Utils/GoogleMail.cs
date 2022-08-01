using System.Reflection;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ALMA_API.Utils;

public static class GoogleMail
{
    static string[] Scopes = { GmailService.Scope.GmailSend };
    static string ApplicationName = "ALMA_API";
    private static UserCredential? _credentials = GetCredentials();
    private static GmailService _service = GetService();
    
    
    private static string Base64UrlEncode(string input)
    {
        var data = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(data).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
    
    public static (bool, string?) SendMessage(string emailTo, string subject, string body)
    {
        var message = $"To: {emailTo}\r\nSubject: {subject}\r\nContent-Type: text/html;charset=utf-8\r\n\r\n<h1>{body}</h1>";
               
        var msg = new Message
        {
            Raw = Base64UrlEncode(message)
        };
        try
        {
            var response = _service.Users.Messages.Send(msg, "me").Execute();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.ToString());
        }
    }

    private static GmailService GetService()
    {
        return new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = _credentials,
            ApplicationName = ApplicationName
        });
    }

    private static UserCredential? GetCredentials()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("ALMA_API.credentials.json");
        if (stream is null) return null;
        using var reader = new StreamReader(stream);
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        path = Path.Combine(path, ".credentials/gmail-dotnet-quickstart.json");
        return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(path, true)).Result;
    }
}