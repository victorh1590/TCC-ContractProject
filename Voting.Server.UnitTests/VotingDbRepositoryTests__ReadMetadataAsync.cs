using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence;
using Voting.Server.UnitTests.TestNet.Ganache;
using CommunityToolkit.Diagnostics;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.UnitTests.TestData;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Voting.Server.Persistence.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Globalization;

namespace Voting.Server.UnitTests;

[Order(5)]
[TestFixture]
public class VotingDbRepositoryTests__ReadMetadataAsync
{
    private TestNet<Ganache> TestNet { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private VotingDbRepository Repository { get; set; } = default!;

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
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestNet.TearDown();
    }

    [Ignore("Debugging")]
    [Order(1)]
    [Test, Sequential]
    public async Task ReadMetadataAsync_Should_Return_Correct_Data(
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

        MetadataEventDTO? metadataEventDTO = await Repository.ReadMetadataAsync(transaction.ContractAddress);

        Assert.That(metadataEventDTO, Is.Not.Null);
        Assert.That(metadataEventDTO.ContractAddress.ToLowerInvariant(), Is.EqualTo(transaction.ContractAddress));
        Assert.That(metadataEventDTO.Block.ToString(), Is.EqualTo(transaction.BlockNumber.ToString()));
        CollectionAssert.AreEquivalent(metadataEventDTO.Sections, seedData.Deployment.Sections);
    }

    [Order(2)]
    [Test, Sequential]
    public async Task ReadMetadataAsync_Returns_Null_When_Contract_Doesnt_Exist(
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

        //Generate fake address from random string hashed to Sha3 and converted to 20 bytes address.
        string randomString = default!;
        string fakeAddress = default!;
        Sha3Keccack kck = new();

        do
        {
            randomString = "0x" + TestContext.CurrentContext.Random.GetString(50, "abcdefghijkmnopqrstuvwxyz0123456789");
            fakeAddress = kck.CalculateHash(randomString).Remove(40);
            fakeAddress = Nethereum.Web3.Web3.ToValid20ByteAddress(fakeAddress);
        } while (fakeAddress == transaction.ContractAddress);

        TestContext.WriteLine("Fake address: " + fakeAddress);
        TestContext.WriteLine("Fake address length: " + fakeAddress.Length);

        MetadataEventDTO? metadataEventDTO = await Repository.ReadMetadataAsync(fakeAddress);

        Assert.That(metadataEventDTO, Is.Null);
    }
}
