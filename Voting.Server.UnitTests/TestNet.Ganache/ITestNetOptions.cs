namespace Voting.Server.UnitTests.TestNet.Ganache;

public interface ITestNetOptions
{
    TestNetEnvironmentOptions TestNetEnvironmentOptions { get; set; }
    GanacheOptions GanacheOptions { get; set; }
}