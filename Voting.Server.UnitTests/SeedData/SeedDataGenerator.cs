using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Domain.Utils;
using Voting.Server.Persistence.ContractDefinition;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Voting.Server.UnitTests.SeedData;

public static class SeedDataGenerator
{
    private static readonly Random Rand = new();
    private static uint MaxVotes => 200;
    private static uint MaxCandidates => 30;
    private static uint MaxSections => 100;
    public static uint NumSections { get; private set; }
    public static uint NumCandidates { get; private set; }
    public static VotingDbDeployment Deployment { get; private set; } = default!;
    public static List<Section> Sections { get; private set; } = default!;
    public static string SectionsJSON { get; private set; } = default!;

    static SeedDataGenerator()
    {
    }
    
    public static void GenerateNew(uint numSections, uint numCandidates)
    {
        Guard.IsGreaterThan(numSections, 0);
        Guard.IsGreaterThan(numCandidates, 0);
        Guard.IsLessThanOrEqualTo(numSections, MaxSections);
        Guard.IsLessThanOrEqualTo(numCandidates, MaxCandidates);

        NumSections = numSections;
        NumCandidates = numCandidates;

        Deployment = new VotingDbDeployment();
        GenerateCandidates(numCandidates);
        GenerateSections(numSections);
        GenerateVotes(numSections, numCandidates);
        GenerateTimeStamp();
        GenerateSectionsList();
        GenerateSectionsJSON();
        GenerateCompressedSectionData();
    }

    private static void GenerateCandidates(uint numCandidates)
    {
        Deployment.Candidates = new List<uint>();
        for (int i = 0; i < numCandidates; i++)
        {
            Deployment.Candidates.Add(Convert.ToUInt32(Rand.NextInt64(1, 99)));
        }
    }

    private static void GenerateSections(uint numSections)
    {
        Deployment.Sections = new List<uint>();
        for (int i = 0; i < numSections; i++)
        { 
            Deployment.Sections.Add(Convert.ToUInt32(Rand.NextInt64(1, 472500)));
        }
    }
    
    private static void GenerateVotes(uint numSections, uint numCandidates)
    {
        Deployment.Votes = new List<List<uint>>();
        for (int i = 0; i < numSections; i++)
        {
            List<uint> tempVotes = new((int)numCandidates);
            for (int j = 0; j < numCandidates; j++)
            {
                tempVotes.Add(Convert.ToUInt32(Rand.NextInt64(1, MaxVotes)));
            }
            Deployment.Votes.Add(tempVotes);
        }
    }
    
    private static void GenerateTimeStamp()
    {
        DateTime currentTime = DateTime.Now;
        Deployment.Timestamp = currentTime.ToString(CultureInfo.InvariantCulture);
    }

    private static void GenerateSectionsList()
    {
        Sections = Mappings.DeploymentToSections(Deployment);
    }

    private static void GenerateSectionsJSON()
    {
        Guard.IsNotNull(Sections);
        Guard.IsNotEmpty(Sections);
        string sectionsJSON = JsonSerializer.Serialize(Sections);
        Guard.IsNotNullOrEmpty(sectionsJSON);
        SectionsJSON = sectionsJSON;
    }
    
    private static void GenerateCompressedSectionData()
    {
        Compression compression = new();
        string compressedSection = compression.Compress(SectionsJSON);
        Guard.IsNotNullOrEmpty(compressedSection);
        Deployment.CompressedSectionData = compressedSection;
    }

    public static string GetFormattedJSON()
    {
        Guard.IsNotNull(Sections);
        Guard.IsNotEmpty(Sections);
        Guard.IsNotNullOrEmpty(SectionsJSON);

        JsonSerializerOptions opts = new()
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(Sections, opts);
    }
}