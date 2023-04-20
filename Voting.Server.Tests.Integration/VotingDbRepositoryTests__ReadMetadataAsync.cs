using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.Tests.Utils;
using Voting.Server.UnitTests;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Integration;

// [Ignore("Debug")]
[Order(3)]
[TestFixture]
public class VotingDbRepositoryTests__ReadMetadataAsync : IUseBlockchainAndRepositoryProps
{
    public IGanache TestNet { get; set; } = default!;
    public IConfiguration Config { get; set; } = default!;
    public AccountManager AccountManager { get; set; } = default!;
    public IWeb3ClientsManager ClientsManager { get; set; } = default!;
    public IVotingDbRepository Repository { get; set; } = default!;
    public string Account { get; set; } = default!;
    private readonly SeedDataBuilder _seedDataBuilder = new();

    [Order(1)]
    [Test, Sequential]
    public async Task ReadMetadataAsync_Should_Return_Correct_Data(
        [Values(10U, 20U, 30U, 50U, 100U)] uint numSections,
        [Values(3U, 4U, 5U, 7U, 10U)] uint numCandidates)
    {
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(numSections, numCandidates);

        //Deploy Contract
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);

        //Check BYTECODE and transaction status.
        Guard.IsNotNullOrEmpty(await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress));
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);

        //Calls method passing valid address.
        MetadataEventDTO? resultMetadataEventDTO = await Repository.ReadMetadataAsync(transaction.ContractAddress, FilterRange.FromLatestToLatest);

        //Assertions.
        Assert.That(resultMetadataEventDTO, Is.Not.Null);
        Assert.That(resultMetadataEventDTO.ContractAddress.ToLowerInvariant(), Is.Not.SameAs(transaction.ContractAddress));
        Assert.That(resultMetadataEventDTO.ContractAddress.ToLowerInvariant(), Is.EqualTo(transaction.ContractAddress));
        Assert.That(resultMetadataEventDTO.Block.ToString(), Is.EqualTo(transaction.BlockNumber.ToString()));
        Assert.That(resultMetadataEventDTO.Sections, Is.Not.SameAs(seedData.Deployment.Sections));
        Assert.That(resultMetadataEventDTO.Sections, Is.EquivalentTo(seedData.Deployment.Sections));
    }

    [Order(2)]
    [Test, Sequential]
    public async Task ReadMetadataAsync_Returns_Null_When_Contract_Doesnt_Exist(
    [Values(10U, 20U, 30U, 50U, 100U)] uint numSections,
    [Values(3U, 4U, 5U, 7U, 10U)] uint numCandidates)
    {
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(numSections, numCandidates);

        //Deploy Contract
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);

        //Check BYTECODE and transaction status.
        Guard.IsNotNullOrEmpty(await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress));
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);

        //Generate fake address from random string hashed to Sha3 and converted to 20 bytes address.
        string fakeAddress;
        Sha3Keccack keccack = new();

        do
        {
            string randomString = "0x" + CurrentContext.Random.GetString(50, "abcdefghijkmnopqrstuvwxyz0123456789");
            fakeAddress = keccack.CalculateHash(randomString).Remove(40);
            fakeAddress = Nethereum.Web3.Web3.ToValid20ByteAddress(fakeAddress);
        } while (fakeAddress == transaction.ContractAddress);

        WriteLine("Fake address: " + fakeAddress);
        WriteLine("Fake address length: " + fakeAddress.Length);

        //Calls method passing fake address.
        MetadataEventDTO? metadataEventDTO = await Repository.ReadMetadataAsync(fakeAddress, FilterRange.FromLatestToLatest);

        //Assertions
        Assert.That(metadataEventDTO, Is.Null);
    }
}
