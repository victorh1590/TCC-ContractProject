using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Moq;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestData;

namespace Voting.Server.UnitTests;

// [Ignore("Debugging")]
// [Order(3)]
[TestFixture]
public class DomainServiceTests__GetSectionRangeAsync
{
    // private TestNet<Ganache> TestNet { get; set; } = default!;
    // private IConfiguration Config { get; set; } = default!;
    // private AccountManager AccountManager { get; set; } = default!;
    // private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    // private IVotingDbRepository Repository { get; set; } = default!;
    // private DomainService DomainService { get; set; } = default!;
    // private Mock<IVotingDbRepository> mockRepository { get; set; } = default!;
    private SeedDataBuilder _seedDataBuilder = new();
    private SeedData _seedData = default!;
    private Mock<IVotingDbRepository> _mockRepository = default!;
    private DomainService _domainService = default!;

    // [OneTimeSetUp]
    // public async Task OneTimeSetUp()
    // {
    //     Config = new ConfigurationBuilder()
    //         .AddUserSecrets<TestNet<Ganache>>()
    //         .Build();
    //     AccountManager = new AccountManager(Config);
    //     ClientsManager = new Web3ClientsManager(AccountManager);
    //     Repository = new VotingDbRepository(ClientsManager);
    //     TestNet = new TestNet<Ganache>(AccountManager);
    //     DomainService = new DomainService(Repository);
    //     TestNet.SetUp();
    //
    //     TimeSpan timeSpan = TimeSpan.FromSeconds(30);
    //     var accountsTask = Repository.Web3.Personal.ListAccounts.SendRequestAsync();
    //     List<string> accounts = (await accountsTask.WaitAsync(timeSpan)).ToList();
    //     Guard.IsNotNull(accounts);
    //     Guard.IsNotEmpty(accounts);
    //     CollectionAssert.AllItemsAreNotNull(accounts);
    //
    //     TestContext.WriteLine("Accounts in chain: ");
    //     accounts.ForEach(TestContext.WriteLine);
    // }
    //
    // [OneTimeTearDown]
    // public void OneTimeTearDown()
    // {
    //     TestNet.TearDown();
    // }

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
        
        //Generate seed data.
        _seedData = _seedDataBuilder.GenerateNew(10U, 5U);
        List<SectionEventDTO> sectionEventDTOs = new List<SectionEventDTO>();
        foreach (var section in _seedData.Sections)
        {
            SectionEventDTO dto = new SectionEventDTO()
            {
                ContractAddress = "",
                Section = section.SectionID,
                Candidates = section.CandidateVotes.Select(x => x.Candidate).ToList(),
                Votes = section.CandidateVotes.Select(x => x.Votes).ToList()
            };
            sectionEventDTOs.Add(dto);
        }

        //Mock Repository
        _mockRepository = new Mock<IVotingDbRepository>();
        _mockRepository
            .Setup(repo=> repo.ReadSectionAsync(
                It.IsIn<uint>(_seedData.Deployment.Sections), It.IsAny<FilterRange?>()))
            .Returns<uint, FilterRange>((sectionNum, _ ) => 
                Task.FromResult(sectionEventDTOs.FirstOrDefault(x => x.Section == sectionNum)));
        
        //Instantiate DomainService
        _domainService = new DomainService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _seedData = default!;
        _mockRepository = default!;
    }

    [Order(1)]
    [Test]
    [Repeat(10)]
    public async Task GetSectionRangeAsync_Should_Return_Correct_Data_When_All_SectionNums_Are_Valid()
    {
        //Generate a list of sections to look for.
        List<Section> expectedSections = new();
        expectedSections
            .AddRange(_seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, _seedData.Sections.Count))
                .ToArray()
            );
        Guard.IsNotNull(expectedSections);

        uint[] sectionNumbers = expectedSections.Select(section => section.SectionID).ToArray();

        //Calls method and convert results to JSON.
        List<Section> resultSections = await _domainService.GetSectionRangeAsync(sectionNumbers);

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
    [Test]
    [Repeat(5)]
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
            async () => await _domainService.GetSectionRangeAsync(sectionNumbers.ToArray()),
            Throws.InstanceOf<ArgumentException>());
    }
    
    [Order(3)]
    [Test, Sequential]
    public async Task GetSectionRangeAsync_Should_Return_Partial_Data_When_Part_Of_The_Sections_Are_Invalid(
        [Range(1, 5, 1)] int invalidDataVariance)
    {
        //Generate a list of sections to look for.
        List<Section> expectedSectionsWithInvalids = new();
        expectedSectionsWithInvalids
            .AddRange(_seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, _seedData.Sections.Count - invalidDataVariance))
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
        List<Section> resultSections = await _domainService.GetSectionRangeAsync(sectionNumbers);
    
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