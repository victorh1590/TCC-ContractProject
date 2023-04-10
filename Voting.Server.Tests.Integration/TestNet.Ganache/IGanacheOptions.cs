using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public interface IGanacheOptions
{
    GanacheEnvironmentOptions GanacheEnvironmentOptions { get; set; }
    GanacheSetupOptions GanacheSetupOptions { get; set; }
}