#nullable disable

namespace Voting.Server.UnitTests.TestNet.Ganache;

public class TestNetOptions : ITestNetOptions
{
    public TestNetEnvironmentOptions TestNetEnvironmentOptions { get; set; }
    public GanacheOptions GanacheOptions { get; set; }
}