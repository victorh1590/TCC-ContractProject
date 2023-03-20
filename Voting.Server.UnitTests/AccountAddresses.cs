#nullable disable

using System.Text.Json.Serialization;

namespace Voting.Server.UnitTests;

public class AccountAddresses
{
    [JsonPropertyName("addresses")]
    public Dictionary<string, string> Addresses { get; set; } 

    public AccountAddresses() 
    {
        Addresses = new Dictionary<string, string>();
    }
}