#nullable disable

using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public class GanacheOptions : IGanacheOptions
{
    public GanacheEnvironmentOptions GanacheEnvironmentOptions { get; set; }
    public GanacheSetupOptions GanacheSetupOptions { get; set; }
}