using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence.Accounts;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

public class TestNet<T> where T : IGanache, new()
{
    private IGanache Blockchain { get; }
    private IGanacheOptions Options { get; } 
    private AccountManager AccountManager { get; }

    public TestNet(AccountManager accountManager)
    {
        AccountManager = accountManager;
        Blockchain = new T();
        Options = new GanacheOptions();

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