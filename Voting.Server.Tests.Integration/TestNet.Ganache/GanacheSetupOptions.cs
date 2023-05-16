#nullable disable

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public class GanacheSetupOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public int ChainID { get; set; }
    public int BlockTime { get; set; }
    public string DefaultGasPrice { get; set; }
    public string BlockGasLimit { get; set; }
    public string DefaultTransactionGasLimit { get; set; }
    public string AccountKeysPath { get; set; }
    public int TotalAccounts { get; set; }
}