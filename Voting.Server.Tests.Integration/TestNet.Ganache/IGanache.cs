using Voting.Server.Persistence.Accounts;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public interface IGanache
{
    IGanacheOptions Options { get; }
    AccountManager? AccountManager { get; }
    Task<string> Start();
    Task Stop();
}