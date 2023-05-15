using System.Text.Json;
using CommunityToolkit.Diagnostics;
// using Voting.Server.Domain.Models;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Protos;

namespace Voting.Server.Tests.Utils;

public class SeedData : ICloneable
{
    public VotingDbDeployment Deployment { get; }
    public List<Section> Sections { get; }
    public string SectionsJSON { get; }

    public SeedData(VotingDbDeployment deployment, List<Section> sections, string sectionsJson)
    {
        Deployment = deployment;
        Sections = sections;
        SectionsJSON = sectionsJson;
    }

    public string GetFormattedJSON()
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

    private SeedData DeepClone()
    {
        VotingDbDeployment? newDeployment = Deployment.Clone() as VotingDbDeployment;
        List<Section> newSectionList = new();
        foreach(Section? newSection in Sections.Select(section => section.Clone() as Section))
        {
            Guard.IsNotNull(newSection);
            newSectionList.Add(newSection);
        }
        string newSectionsJSON = new string(SectionsJSON);
        Guard.IsNotNull(newDeployment);
        Guard.IsReferenceNotEqualTo(newDeployment, Deployment);
        Guard.IsReferenceNotEqualTo(newSectionsJSON, SectionsJSON);
        return new SeedData(newDeployment, newSectionList, newSectionsJSON);
    }

    public object Clone()
    {
        return DeepClone();
    }
}