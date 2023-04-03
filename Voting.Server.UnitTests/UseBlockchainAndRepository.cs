using Microsoft.Extensions.Configuration;
using Nethereum.RPC.Eth.DTOs;
using NUnit.Framework.Interfaces;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence;
using Voting.Server.UnitTests.TestNet.Ganache;
using Voting.Server.UnitTests;

namespace Voting.Server.UnitTests;

[AttributeUsage(AttributeTargets.Interface)]
public class UseBlockchainAndRepository : TestActionAttribute
{
    public TestNet<Ganache> TestNet { get; set; } = default!;
    public IConfiguration Config { get; set; } = default!;
    public AccountManager AccountManager { get; set; } = default!;
    public IWeb3ClientsManager ClientsManager { get; set; } = default!;
    public IVotingDbRepository Repository { get; set; } = default!;
    public BlockParameter Latest { get; } = BlockParameter.CreateLatest();
    public BlockParameter Pending { get; } = BlockParameter.CreatePending();
    public BlockParameter Ealiest { get; } = BlockParameter.CreateEarliest();

    public UseBlockchainAndRepository()
    {
    }

    public override void BeforeTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
                                               ?? throw new ArgumentException();
        if (obj != null)
        {
            obj.Config = new ConfigurationBuilder()
                .AddUserSecrets<TestNet<Ganache>>()
                .Build();
            obj.AccountManager = new AccountManager(obj.Config);
            obj.ClientsManager = new Web3ClientsManager(obj.AccountManager);
            obj.Repository = new VotingDbRepository(obj.ClientsManager);
            obj.TestNet = new TestNet<Ganache>(obj.AccountManager);
            obj.Account = obj.AccountManager.Accounts.First().Address;
            obj.TestNet.SetUp();
        }
    }

    public override void AfterTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
                                               ?? throw new ArgumentException();
        obj?.TestNet.TearDown();
    }

    public override ActionTargets Targets
    {
        get { return ActionTargets.Suite; }
    }
}