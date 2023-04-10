using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

public interface IGanache
{
    IGanacheOptions Options { get; }
    AccountManager? AccountManager { get; }
    Task<string> Start(IGanacheOptions opts, AccountManager accountManager);
    Task Stop();
}