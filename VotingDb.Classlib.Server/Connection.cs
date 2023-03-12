using Nethereum.Web3;

namespace VotingDb.Classlib.Server;
public static class Connection
{
  private static string Url { get; set; }
  // private string AccountAddress { get; set; }
  private static int ChainId { get; set; }
  // private string AccountPassword { get; set; }

  static Connection() 
  {
    Url = "HTTP://localhost:7545";
    ChainId = 5777;
  }

  public static Web3 Connect() 
  {
    return new Web3(Url);
  }
}
