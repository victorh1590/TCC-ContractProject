#nullable disable

using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.UnitTests;

namespace Voting.Server.Tests.Integration;

[AttributeUsage(AttributeTargets.Interface)]
public class UseBlockchainAndRepository : TestActionAttribute
{
    public IGanache TestNet { get; set; }
    public IConfiguration Config { get; set; }
    public AccountManager AccountManager { get; set; }
    public IWeb3ClientsManager ClientsManager { get; set; }
    public IVotingDbRepository Repository { get; set; }

    public override void BeforeTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
            ?? throw new NullReferenceException();

        obj.Config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();
        obj.AccountManager = new AccountManager(obj.Config);
        obj.TestNet = new Ganache(obj.AccountManager);
        obj.Account = obj.AccountManager.Accounts.First().Address;
        Task.WaitAll(obj.TestNet.Start());
        Dictionary<string, string> clientConfigDict = new() { { "URL", obj.TestNet.URL } };
        IConfiguration clientConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(clientConfigDict)
            .Build();
        obj.ClientsManager = new Web3ClientsManager(obj.AccountManager, clientConfig);
        obj.Repository = new VotingDbRepository(obj.ClientsManager);
    }

    public override void AfterTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
            ?? throw new NullReferenceException();
        obj.TestNet.Stop();
    }

    public override ActionTargets Targets => ActionTargets.Suite;
}