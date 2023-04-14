using Voting.Server.Persistence.Accounts;

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public interface IGanache
{
    IGanacheOptions Options { get; }
    AccountManager? AccountManager { get; }
    string? URL { get; }
    Task Start();
    Task Stop();
}