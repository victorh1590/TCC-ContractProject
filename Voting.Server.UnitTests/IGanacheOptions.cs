namespace Voting.Server.UnitTests;

public interface IGanacheOptions
{
    string Host { get; set; }
    int Port { get; set; }
    int ChainID { get; set; }
    int BlockTime { get; set; }
    string DefaultGasPrice { get; set; }
    string BlockGasLimit { get; set; }
    string DefaultTransactionGasLimit { get; set; }
    string AccountKeysPath { get; set; }
}