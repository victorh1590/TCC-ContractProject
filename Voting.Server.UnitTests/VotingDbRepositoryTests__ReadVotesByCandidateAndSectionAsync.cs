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

[Ignore("Debugging")]
[Order(4)]
[TestFixture]
public class VotingDbRepositoryTests__ReadVotesByCandidateAndSectionAsync
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

    [Order(1)]
    [Test, Sequential]
    public async Task ReadVotesByCandidateAndSectionAsync_Should_Return_Correct_Data(
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

        //Get valid random Section
        Section? expectedSection = seedData.Sections.MinBy(_ => Guid.NewGuid());
        Guard.IsNotNull(expectedSection);
        
        //Get valid random CandidateVotes
        CandidateVotes expectedCandidateVotes = expectedSection.CandidateVotes.MinBy(_ => Guid.NewGuid());
        
        //Remove other CandidateVotes from expectedSection
        expectedSection.CandidateVotes.RemoveAll(cv=> cv.Candidate != expectedCandidateVotes.Candidate);

        //Prints SectionID and Candidate
        TestContext.WriteLine($"Trying to access contract and getting section {expectedSection.SectionID}...");
        TestContext.WriteLine($"Trying to access contract and getting section {expectedCandidateVotes.Candidate}...");

        //Calls method and convert results to JSON.
        CandidateEventDTO? candidateEventDTO = 
            await Repository.ReadVotesByCandidateAndSectionAsync(expectedCandidateVotes.Candidate, expectedSection.SectionID);
        Section sectionData = Mappings.CandidateEventDTOToSection(candidateEventDTO);
        
        string resultJSON = JsonSerializer.Serialize(sectionData);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);
        TestContext.WriteLine(resultJSON);
        TestContext.WriteLine("Expected: " + expectedJSON);

        //Assertions.
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(sectionData.SectionID, Is.EqualTo(expectedSection.SectionID));
        Assert.That(sectionData.CandidateVotes.First(), Is.EqualTo(expectedCandidateVotes));
    }

    [Order(2)]
    [Test, Sequential]
    public async Task  ReadVotesByCandidateAndSectionAsync_Should_Return_Null_When_Candidate_Or_Section_Doesnt_Exist(
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

        uint invalidSectionNumber = TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1);
        uint InvalidCandidateNumber = TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1);

        //Calls method with invalid candidate.
        CandidateEventDTO? resultInvalidCandidate = 
            await Repository.ReadVotesByCandidateAndSectionAsync(InvalidCandidateNumber, seedData.Deployment.Sections.First());

        //Calls method with invalid section.
        CandidateEventDTO? resultInvalidSection = 
            await Repository.ReadVotesByCandidateAndSectionAsync(seedData.Deployment.Candidates.First(), invalidSectionNumber);

        //Calls method with invalid candidate and section.
        CandidateEventDTO? resultInvalidCandidateAndSection = 
            await Repository.ReadVotesByCandidateAndSectionAsync(InvalidCandidateNumber, invalidSectionNumber);

        //Assertions.
        Assert.That(resultInvalidCandidate, Is.Null);
        Assert.That(resultInvalidSection, Is.Null);
        Assert.That(resultInvalidCandidateAndSection, Is.Null);
    }

    [Order(3)]
    [Test]
    public void  ReadVotesByCandidateAndSectionAsync_Should_Throw_Exception_When_SectionNum_Is_Zero_Or_No_Param()
    {
        //Assertions.
        Assert.That(async () => await Repository.ReadVotesByCandidateAndSectionAsync(), Throws.InstanceOf<ArgumentException>());
        Assert.That(async () => await Repository.ReadVotesByCandidateAndSectionAsync(0), Throws.InstanceOf<ArgumentException>());
        Assert.That(async () => await Repository.ReadVotesByCandidateAndSectionAsync(0, 0), Throws.InstanceOf<ArgumentException>());
    }
}