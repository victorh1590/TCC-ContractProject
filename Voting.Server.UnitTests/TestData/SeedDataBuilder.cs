using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Domain.Utils;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests.TestData;

public class SeedDataBuilder
{
    private static uint MaxVotes => 200;
    private static uint MaxCandidates => 10;
    private static uint MaxSections => 100;
    public static uint MaxSectionID => 472500;
    public static uint MaxCandidateNumber => 1000;
    private static uint MaxDeploymentBuffer => 30;
    private readonly Random _rand = new();
    private List<VotingDbDeployment> _deploymentsGenerated = new();

    public SeedData GenerateNew(uint numSections, uint numCandidates)
    {
        Guard.IsGreaterThan(numSections, 0);
        Guard.IsGreaterThan(numCandidates, 0);
        Guard.IsLessThanOrEqualTo(numSections, MaxSections);
        Guard.IsLessThanOrEqualTo(numCandidates, MaxCandidates);

        VotingDbDeployment deployment = new();
        
        GenerateCandidates(deployment, numCandidates);
        GenerateSections(deployment, numSections);
        GenerateVotes(deployment, numSections, numCandidates);
        GenerateTimeStamp(deployment);
        List<Section> sections = GenerateSectionsList(deployment);
        string sectionsJSON = GenerateSectionsJSON(sections);
        GenerateCompressedSectionData(sectionsJSON, deployment);
        _deploymentsGenerated.Add(deployment);
        if (_deploymentsGenerated.Count >= MaxDeploymentBuffer) _deploymentsGenerated = new List<VotingDbDeployment>();
        return new SeedData(deployment, sections, sectionsJSON);
    }
    
    private void GenerateCandidates(VotingDbDeployment deployment, uint numCandidates)
    {
        List<uint> candidatesToAdd = new();
        while (candidatesToAdd.Count < numCandidates)
        {
            uint candidate = Convert.ToUInt32(_rand.NextInt64(1, MaxCandidateNumber));
            bool pastDeploymentContainsCandidate =
                _deploymentsGenerated?.Any(pastDeployment => pastDeployment.Candidates.Contains(candidate)) ?? false;
            if (candidatesToAdd.Contains(candidate) || pastDeploymentContainsCandidate) continue; 
            candidatesToAdd.Add(candidate);
        }

        deployment.Candidates = candidatesToAdd;
    }

    private void GenerateSections(VotingDbDeployment deployment, uint numSections)
    {
        List<uint> sectionsToAdd = new();
        while (sectionsToAdd.Count < numSections)
        {
            uint section = Convert.ToUInt32(_rand.NextInt64(1, MaxSectionID));
            bool pastDeploymentContainSection = 
                _deploymentsGenerated?.Any(pastDeployment => pastDeployment.Sections.Contains(section)) ?? false;
            if (sectionsToAdd.Contains(section) || pastDeploymentContainSection) continue;
            sectionsToAdd.Add(section);
        }

        deployment.Sections = sectionsToAdd;
    }
    
    private void GenerateVotes(VotingDbDeployment deployment, uint numSections, uint numCandidates)
    {
        deployment.Votes = new List<List<uint>>();
        for (int i = 0; i < numSections; i++)
        {
            List<uint> tempVotes = new((int)numCandidates);
            for (int j = 0; j < numCandidates; j++)
            {
                tempVotes.Add(Convert.ToUInt32(_rand.NextInt64(1, MaxVotes)));
            }
            deployment.Votes.Add(tempVotes);
        }
    }
    
    private void GenerateTimeStamp(VotingDbDeployment deployment)
    {
        DateTime currentTime = DateTime.Now;
        deployment.Timestamp = currentTime.ToString(CultureInfo.InvariantCulture);
    }

    private List<Section> GenerateSectionsList(VotingDbDeployment deployment)
    {
        return new List<Section>(Mappings.DeploymentToSections(deployment));
    }

    private string GenerateSectionsJSON(List<Section> sections)
    {
        Guard.IsNotNull(sections);
        Guard.IsNotEmpty(sections);
        string sectionsJSON = JsonSerializer.Serialize(sections);
        Guard.IsNotNullOrEmpty(sectionsJSON);
        return sectionsJSON;
    }
    
    private void GenerateCompressedSectionData(string sectionsJSON, VotingDbDeployment deployment)
    {
        Compression compression = new();
        string compressedSection = compression.Compress(sectionsJSON);
        Guard.IsNotNullOrEmpty(compressedSection);
        deployment.CompressedSectionData = compressedSection;
    }

}