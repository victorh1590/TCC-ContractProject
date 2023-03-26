using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

internal interface IPrivateBlockchain
{
    ITestNetOptions Options { get; }
    AccountManager? AccountManager { get; }
    void Start(ITestNetOptions opts, AccountManager accountManager);
    void Stop();
    void KillProcessTree(int pid);
    string GetExecutionString();
}