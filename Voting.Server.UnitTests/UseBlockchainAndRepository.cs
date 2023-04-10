﻿#nullable disable

using Microsoft.Extensions.Configuration;
using Nethereum.RPC.Eth.DTOs;
using NUnit.Framework.Interfaces;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[AttributeUsage(AttributeTargets.Interface)]
public class UseBlockchainAndRepository : TestActionAttribute
{
    public TestNet<Ganache> TestNet { get; set; }
    public IConfiguration Config { get; set; }
    public AccountManager AccountManager { get; set; }
    public IWeb3ClientsManager ClientsManager { get; set; }
    public IVotingDbRepository Repository { get; set; }

    public UseBlockchainAndRepository()
    {
    }

    public override void BeforeTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
            ?? throw new NullReferenceException();
    
        obj.Config = new ConfigurationBuilder()
            .AddUserSecrets<TestNet<Ganache>>()
            .Build();
        obj.AccountManager = new AccountManager(obj.Config);
        obj.TestNet = new TestNet<Ganache>(obj.AccountManager);
        obj.Account = obj.AccountManager.Accounts.First().Address;
        string URL = obj.TestNet.SetUp().Result;
        obj.ClientsManager = new Web3ClientsManager(obj.AccountManager, URL);
        obj.Repository = new VotingDbRepository(obj.ClientsManager);
    }

    public override void AfterTest(ITest details)
    {
        IUseBlockchainAndRepositoryProps obj = details.Fixture as IUseBlockchainAndRepositoryProps
            ?? throw new NullReferenceException();
        obj.TestNet.TearDown().Wait();
    }

    public override ActionTargets Targets
    {
        get { return ActionTargets.Suite; }
    }
}