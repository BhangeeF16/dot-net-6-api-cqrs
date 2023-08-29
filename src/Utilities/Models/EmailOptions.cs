using System.Text;

namespace Utilities.Models;
public class EmailOptions
{
    public string Subject { get; set; }

    public List<string> ToEmails { get; set; }
    public List<string>? CCEmails { get; set; }
    public List<string>? BCCEmails { get; set; }

    public string Body { get; set; }
    public List<KeyValuePair<string, string>> PlaceHolders { get; set; }

    public bool IsBodyHtml { get; set; }
    public Encoding BodyEncoding { get; set; } = Encoding.Default;
}
