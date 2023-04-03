using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

public interface IGanache
{
    IGanacheOptions Options { get; }
    AccountManager? AccountManager { get; }
    void Start(IGanacheOptions opts, AccountManager accountManager);
    void Stop();
    void KillProcessTree(int pid);
    string GetExecutionString();
}