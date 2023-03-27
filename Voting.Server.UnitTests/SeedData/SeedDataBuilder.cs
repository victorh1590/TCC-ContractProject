using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Domain.Utils;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests.SeedData;

public static class SeedDataBuilder
{
    private static uint MaxVotes => 200;
    private static uint MaxCandidates => 30;
    private static uint MaxSections => 100;
    private static readonly Random Rand = new();

    public static SeedData GenerateNew(uint numSections, uint numCandidates)
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
        return new SeedData(deployment, sections, sectionsJSON);
    }
    
    private static void GenerateCandidates(VotingDbDeployment deployment, uint numCandidates)
    {
        deployment.Candidates = new List<uint>();
        for (int i = 0; i < numCandidates; i++)
        {
            deployment.Candidates.Add(Convert.ToUInt32(Rand.NextInt64(1, 99)));
        }
    }

    private static void GenerateSections(VotingDbDeployment deployment, uint numSections)
    {
        deployment.Sections = new List<uint>();
        for (int i = 0; i < numSections; i++)
        { 
            deployment.Sections.Add(Convert.ToUInt32(Rand.NextInt64(1, 472500)));
        }
    }
    
    private static void GenerateVotes(VotingDbDeployment deployment, uint numSections, uint numCandidates)
    {
        deployment.Votes = new List<List<uint>>();
        for (int i = 0; i < numSections; i++)
        {
            List<uint> tempVotes = new((int)numCandidates);
            for (int j = 0; j < numCandidates; j++)
            {
                tempVotes.Add(Convert.ToUInt32(Rand.NextInt64(1, MaxVotes)));
            }
            deployment.Votes.Add(tempVotes);
        }
    }
    
    private static void GenerateTimeStamp(VotingDbDeployment deployment)
    {
        DateTime currentTime = DateTime.Now;
        deployment.Timestamp = currentTime.ToString(CultureInfo.InvariantCulture);
    }

    private static List<Section> GenerateSectionsList(VotingDbDeployment deployment)
    {
        return new List<Section>(Mappings.DeploymentToSections(deployment));
    }

    private static string GenerateSectionsJSON(List<Section> sections)
    {
        Guard.IsNotNull(sections);
        Guard.IsNotEmpty(sections);
        string sectionsJSON = JsonSerializer.Serialize(sections);
        Guard.IsNotNullOrEmpty(sectionsJSON);
        return sectionsJSON;
    }
    
    private static void GenerateCompressedSectionData(string sectionsJSON, VotingDbDeployment deployment)
    {
        Compression compression = new();
        string compressedSection = compression.Compress(sectionsJSON);
        Guard.IsNotNullOrEmpty(compressedSection);
        deployment.CompressedSectionData = compressedSection;
    }

}