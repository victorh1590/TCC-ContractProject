﻿using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.Tests.Utils;
using Voting.Server.UnitTests;
using CommunityToolkit.Diagnostics;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Integration;

[Order(6)]
[TestFixture]
public class VotingDbRepositoryTests__ReadVotesByCandidateAsync : IUseBlockchainAndRepositoryProps
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
    public async Task ReadVotesByCandidateAndSectionAsync_Should_Return_Correct_Data(
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

        //Get valid random candidate
        uint expectedCandidate = seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());

        //Generate list with all sections containing the selected candidate votes.
        List<Section> expectedCandidateVotes = new();
        foreach (Section section in seedData.Sections)
        {
            List<CandidateVotes> cvList = section.CandidateVotes
                .Where(cv => cv.Candidate == expectedCandidate)
                .ToList();
            Guard.IsEqualTo(cvList.Count, 1);
            
            expectedCandidateVotes.Add(new Section(section.SectionID, cvList));
        }

        //Calls method and convert results to JSON.
        List<CandidateEventDTO> candidateEventDTOList = 
            await Repository.ReadVotesByCandidateAsync(expectedCandidate, FilterRange.FromEarliestToLatest);
        List<Section> result = Mappings.CandidateEventDTOListToSectionList(candidateEventDTOList);
        
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedJSON = JsonSerializer.Serialize(expectedCandidateVotes);

        //Assertions.
        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result.Count, Is.EqualTo(expectedCandidateVotes.Count));
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(result.Select(s => s.SectionID), 
            Is.EqualTo(expectedCandidateVotes.Select(s => s.SectionID)));
        Assert.That(result.Select(s => s.CandidateVotes), 
            Is.EqualTo(expectedCandidateVotes.Select(s => s.CandidateVotes)));
    }
    
    [Order(2)]
    [Test, Sequential]
    public async Task  ReadVotesByCandidateAsync_Should_Return_Null_When_Candidate_Doesnt_Exist(
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

        uint InvalidCandidateNumber = CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1);

        //Calls method with invalid candidate.
        List<CandidateEventDTO> resultInvalidCandidate = 
            await Repository.ReadVotesByCandidateAsync(InvalidCandidateNumber, FilterRange.FromEarliestToLatest);
        //Assertions.
        Assert.That(resultInvalidCandidate, Is.Empty);
    }
    
    [Order(3)]
    [Test]
    public void  ReadVotesByCandidateAndSectionAsync_Should_Throw_Exception_When_CandidateNum_Is_Zero_Or_No_Param()
    {
        //Assertions.
        Assert.That(async () => await Repository.ReadVotesByCandidateAndSectionAsync(), 
            Throws.InstanceOf<ArgumentException>());
        Assert.That(async () => await Repository.ReadVotesByCandidateAndSectionAsync(0), 
            Throws.InstanceOf<ArgumentException>());
    }
}