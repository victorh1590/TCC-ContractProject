﻿using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.UnitTests.TestData;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[Ignore("Debugging")]
[Order(3)]
[TestFixture]
public class DomainServiceTests__GetSectionRangeAsync
{
    private TestNet<Ganache> TestNet { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    private DomainService DomainService { get; set; } = default!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Config = new ConfigurationBuilder()
            .AddUserSecrets<TestNet<Ganache>>()
            .Build();
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);
        TestNet = new TestNet<Ganache>(AccountManager);
        DomainService = new DomainService(Repository);
        TestNet.SetUp();
        
        TimeSpan timeSpan = TimeSpan.FromSeconds(30); 
        var accountsTask = Repository.Web3.Personal.ListAccounts.SendRequestAsync();
        List<string> accounts = (await accountsTask.WaitAsync(timeSpan)).ToList();
        Guard.IsNotNull(accounts);
        Guard.IsNotEmpty(accounts);
        CollectionAssert.AllItemsAreNotNull(accounts);
        
        TestContext.WriteLine("Accounts in chain: ");
        accounts.ForEach(TestContext.WriteLine);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestNet.TearDown();
    }

    [Order(1)]
    [Test, Sequential]
    public async Task GetSectionRangeAsync_Should_Return_Correct_Data_When_All_SectionNums_Are_Valid(
        [Values(10U, 20U, 30U, 50U, 100U)] uint numSections,
        [Values(3U, 4U, 5U, 7U, 10U)] uint numCandidates)
    {
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(numSections, numCandidates);
        
        //Deploy Contract and test if transaction is completed.
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);
        
        //Generate a list of sections to look for.
        int sectionsLength = seedData.Sections.Count;
        List<Section> expectedSections = new();
        expectedSections
            .AddRange(seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, sectionsLength))
                .ToArray()
            );
        Guard.IsNotNull(expectedSections);
        
        uint[] sectionNumbers = expectedSections.Select(section => section.SectionID).ToArray();

        //Calls method and convert results to JSON.
        List<Section> resultSections = await DomainService.GetSectionRangeAsync(sectionNumbers);

        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSections);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSections.Count, Is.EqualTo(expectedSections.Count));
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.CandidateVotes).ToArray(), 
            expectedSections.Select(item => item.CandidateVotes).ToArray());
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.SectionID).ToArray(), 
            expectedSections.Select(item => item.SectionID).ToArray());
    }
    
    [Order(2)]
    [Test, Repeat(5)]
    public void GetSectionRangeAsync_Should_Fail_When_All_SectionNums_Are_Invalid()
    {
        //Generate a list of sections to look for and print sectionNums.
        List<uint> sectionNumbers = new();
        for (int i = 0; i < 10; i++)
        {
           sectionNumbers.Add(TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1));
        }

        //Assertions.
        Assert.That(
            async () => await DomainService.GetSectionRangeAsync(sectionNumbers.ToArray()), 
            Throws.InstanceOf<ArgumentException>());
    }
    
    [Order(3)]
    [Test, Sequential]
    public async Task GetSectionRangeAsync_Should_Return_Partial_Data_When_Part_Of_The_Sections_Are_Invalid(
        [Values(10U, 20U, 30U, 50U, 100U)] uint numSections,
        [Values(3U, 4U, 5U, 7U, 10U)] uint numCandidates,
        [Range(1, 5, 1)] int invalidDataVariance)
    {
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(numSections, numCandidates);

        //Deploy Contract and test if transaction is completed.
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);
        
        //Generate a list of sections to look for.
        List<Section> expectedSectionsWithInvalids = new();
        expectedSectionsWithInvalids
            .AddRange(seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, seedData.Sections.Count - invalidDataVariance))
                .ToArray()
            );
        Guard.IsNotNull(expectedSectionsWithInvalids);
    
        //Copies Valid Data
        List<Section> expectedSectionsValidOnly = new();
        expectedSectionsValidOnly.AddRange(expectedSectionsWithInvalids);
        
        //Add Invalid Data
        for (int i = 0; i < invalidDataVariance; i++)
        {
            expectedSectionsWithInvalids.Add(new Section(
                TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1),
                new List<CandidateVotes>()));
        }
        
        //Calls method and convert results to JSON.
        uint[] sectionNumbers = expectedSectionsWithInvalids.Select(section => section.SectionID).ToArray();
        List<Section> resultSections = await DomainService.GetSectionRangeAsync(sectionNumbers);
    
        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSectionsValidOnly);
        
        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSections.Count, Is.EqualTo(expectedSectionsValidOnly.Count));
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.CandidateVotes).ToArray(), 
            expectedSectionsValidOnly.Select(item => item.CandidateVotes).ToArray());
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.SectionID).ToArray(), 
            expectedSectionsValidOnly.Select(item => item.SectionID).ToArray());
    }
}