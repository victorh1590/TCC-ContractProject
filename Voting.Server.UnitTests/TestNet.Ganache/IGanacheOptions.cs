namespace Voting.Server.UnitTests.TestNet.Ganache;

public interface IGanacheOptions
{
    GanacheEnvironmentOptions GanacheEnvironmentOptions { get; set; }
    GanacheSetupOptions GanacheSetupOptions { get; set; }
}