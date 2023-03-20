using System.Net;
using System.Text.Json;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;
using Voting.Server.Domain;
using Nethereum.Web3.Accounts;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests;

public class DomainServiceTests
{
    public Ganache? Blockchain { get; set; }
    public string? PrivateKey { get; set; }
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EthECKey ecKey = EthECKey.GenerateKey();
        PrivateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
        PrivateKey = "0x" + PrivateKey;
        Blockchain = new Ganache(
            "127.0.0.1", 
            8545, 
            56666, 
            0, 
            "0x0", 
            "0x87369400", 
            "0x87369400",
            PrivateKey,
            "0X56BC75E2D63100000");
        Blockchain.Start();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Blockchain?.Stop();
    }

    [Test]
    public async Task DeployContractTest()
    {
        string url = "HTTP://localhost:8545";
        int chainId = 56666;
        // string? privateKey;
        IWeb3 web3 = new Web3(url);
        web3.TransactionManager.UseLegacyAsDefault = true;
        //
        // EthECKey ecKey = EthECKey.GenerateKey();
        // privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
        // Assert.IsNotNull(privateKey);
        // string? account = await web3.Personal.NewAccount.SendRequestAsync(privateKey);
        // Assert.IsNotNull(account);
        //
        var accounts = await web3.Personal.ListAccounts.SendRequestAsync();
        Assert.IsNotNull(accounts);
        Assert.IsNotEmpty(accounts);
        Console.WriteLine("Accounts in chain: ");
        foreach (var item in accounts)
        {
            Assert.IsNotNull(item);
            Console.WriteLine(item);
        }
        
        DomainService ds = new DomainService(url, chainId, PrivateKey ?? "");
        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = SeedData.Candidates,
            Votes = SeedData.Votes,
            Sections = SeedData.Sections,
            Timestamp = SeedData.Timestamp,
            CompressedSectionData = SeedData.CompressedSectionData
        };

        var service = await ds.DeployContract(deployment);
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

        string json = JsonSerializer.Serialize(sectionData);
        string jsonExp = JsonSerializer.Serialize(expectedSection);
        Console.WriteLine(json);
        Console.WriteLine("Expected: " + jsonExp);
        
        Assert.AreEqual(jsonExp, json);
        CollectionAssert.AreEqual(expectedSection.CandidateVotes, sectionData.CandidateVotes);
    }
}