using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence.Accounts;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

internal class TestChain<T> where T : IPrivateBlockchain, new()
{
    private IPrivateBlockchain Blockchain { get; }
    private AccountManager AccountManager { get; }
    private ITestNetOptions Options { get; } 

    internal TestChain(AccountManager accountManager)
    {
        AccountManager = accountManager;
        Blockchain = new T();
        Options = new TestNetOptions();

        string testProjectDirectory =  
            Path.GetDirectoryName(
            Path.GetDirectoryName(
            Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))) ?? "";
        Guard.IsNotNullOrEmpty(testProjectDirectory);
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(testProjectDirectory)
            .AddJsonFile(Path.Join(".", "TestNet.Ganache", "testnet_ganache_options.json"))
            .Build();
        configuration.Bind(Options);
    }
    
    public void SetUp()
    {
        Blockchain.Start(Options, AccountManager);
    }
    
    public void TearDown()
    {
        Blockchain.Stop();
    }
}