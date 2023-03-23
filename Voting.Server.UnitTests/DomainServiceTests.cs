using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;
using Voting.Server.Domain;
using Nethereum.Web3.Accounts;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests;

public class DomainServiceTests
{
    public Ganache? Blockchain { get; set; }
    public string URL { get; set; }
    // public string PrivateKey { get; set; }
    // public AccountAddresses Accounts { get; set; } = new();
    public IGanacheOptions Options { get; set; } = new GanacheOptions();

    private IConfiguration Config { get; set; } = default!;
    private IAccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        string testProjectDirectory =  
                Path.GetDirectoryName(
                Path.GetDirectoryName(
                Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))) ?? "";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(testProjectDirectory)
            .AddJsonFile("ganache_configs.json")
            .Build();
        configuration.Bind(Options);
        
        Config = new ConfigurationBuilder()
            .AddUserSecrets<DomainServiceTests>()
            .Build();
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);

        Blockchain = new Ganache(Options, Config);
        Blockchain.Start();
        URL = $"HTTP://{Options.Host}:{Options.Port}";


        // string accountFilePath = Path.Join(Directory.GetCurrentDirectory(), Options.AccountKeysPath);
        // int fileReadTries = 0;
        // while(fileReadTries < 3)
        // {
        //     if (!File.Exists(accountFilePath))
        //     {
        //         Thread.Sleep(3000);
        //     }
        //     fileReadTries++;
        // }
        // string text = await File.ReadAllTextAsync(accountFilePath);
        // Accounts = JsonSerializer.Deserialize<AccountAddresses>(text) ?? throw new ArgumentException("Failed to read accounts.");
        // PrivateKey = Accounts.Addresses.Values.First();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Blockchain?.Stop();
    }

    [Test]
    public async Task DeployContractTest()
    {
        // IWeb3 web3 = new Web3(URL);
        // web3.TransactionManager.UseLegacyAsDefault = true;

        DomainService ds = new DomainService(Repository);

        var accounts = await Repository.Web3.Personal.ListAccounts.SendRequestAsync();
        Assert.IsNotNull(accounts);
        Assert.IsNotEmpty(accounts);
        Console.WriteLine("Accounts in chain: ");
        foreach (var item in accounts)
        {
            Assert.IsNotNull(item);
            Console.WriteLine(item);
        }
        
        // Mappings.SectionEventDTOToSection()
        
        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = SeedData.Candidates,
            Votes = SeedData.Votes,
            Sections = SeedData.Sections,
            Timestamp = SeedData.Timestamp,
            CompressedSectionData = SeedData.CompressedSectionData
        };

        var service = await ds.CreateSectionRange(Mappings.DeploymentToSections(deployment));
        Console.WriteLine("Contract Address: " + service);

        uint sectionNumber = 190U;
        Console.WriteLine($"Trying to access contract and getting section {sectionNumber}...");

        Section sectionData = await ds.GetSectionAsync(sectionNumber);
        Assert.IsNotNull(sectionData);

        Section expectedSection = new Section(
            190U,
            new List<CandidateVotes>
            {
                new(15U, 1U),
                new(25U, 8U),
                new(35U, 7U),
                new(55U, 5U)
            }
        );

        string resultJSON = JsonSerializer.Serialize(sectionData);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);
        Console.WriteLine(resultJSON);
        Console.WriteLine("Expected: " + expectedJSON);

        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        CollectionAssert.AreEqual(expectedSection.CandidateVotes, sectionData.CandidateVotes);
    }
}