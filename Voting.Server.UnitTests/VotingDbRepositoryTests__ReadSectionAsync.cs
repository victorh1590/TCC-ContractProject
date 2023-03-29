﻿using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestData;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[Order(2)]
[TestFixture]
public partial class VotingDbRepositoryTests__ReadSectionAsync
{
    private TestNet<Ganache> TestNet { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    // public string URL { get; set; }
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = new ConfigurationBuilder()
            .AddUserSecrets<TestNet<Ganache>>()
            .Build();
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);
        TestNet = new TestNet<Ganache>(AccountManager);
        TestNet.SetUp();
        // URL = $"HTTP://{Options.GanacheOptions.Host}:{Options.GanacheOptions.Port}";
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestNet.TearDown();
    }

    // [Order(3)]
    [Test, Sequential]
    public async Task ReadSectionAsync_Should_Return_Correct_Data(
        [Values(10U, 20U, 30U, 50U, 100U)] uint numSections,
        [Values(3U, 4U, 5U, 7U, 10U)] uint numCandidates)
    {
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(numSections, numCandidates);
        
        //Deploy Contract
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        
        //Check BYTECODE and transaction status.
        Guard.IsNotNullOrEmpty(await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress));
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);

        //Get valid random section number.
        uint sectionNumber = seedData.Deployment.Sections.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        TestContext.WriteLine($"Trying to access contract and getting section {sectionNumber}...");

        //Calls method and convert results to JSON.
        SectionEventDTO? sectionEventDTO = await Repository.ReadSectionAsync(sectionNumber);
        Section sectionData = Mappings.SectionEventDTOToSection(sectionEventDTO);
        Section? expectedSection = seedData.Sections
            .Select(section => section)
            .FirstOrDefault(section => section.SectionID == sectionNumber);
        Guard.IsNotNull(expectedSection);
        
        string resultJSON = JsonSerializer.Serialize(sectionData);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);
        TestContext.WriteLine(resultJSON);
        TestContext.WriteLine("Expected: " + expectedJSON);

        //Assertions.
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        CollectionAssert.AreEqual(expectedSection.CandidateVotes, sectionData.CandidateVotes);
    }
    
    // [Order(4)]
    [Test, Sequential]
    public async Task ReadSectionAsync_Should_Return_Null_When_Looking_For_Invalid_Section(
        [Values(10U, 20U, 30U)] uint numSections,
        [Values(3U, 4U, 5U)] uint numCandidates)
    {
        Random rand = new Random();
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(numSections, numCandidates);
        
        //Deploy Contract
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        
        //Check BYTECODE and transaction status.
        Guard.IsNotNullOrEmpty(await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress));
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);

        //Get valid random section number NOT IN seedData.
        uint sectionNumber = Convert.ToUInt32(rand.NextInt64(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1));
        TestContext.WriteLine($"Trying to access contract and getting section {sectionNumber}...");

        //Calls method and convert results to JSON.
        SectionEventDTO? sectionEventDTO = await Repository.ReadSectionAsync(sectionNumber);
        
        //Assertions.
        Assert.That(sectionEventDTO, Is.Null);
    }
}