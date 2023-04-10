﻿using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence.ContractDefinition;
using CommunityToolkit.Diagnostics;

namespace Voting.Server.Tests.Utils.TestData;

public class SeedData
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
}