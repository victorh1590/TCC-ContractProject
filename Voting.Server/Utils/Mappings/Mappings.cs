using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Protos.v1;

namespace Voting.Server.Utils.Mappings;

internal class Mappings
{
    public static Section SectionEventDTOToSection(SectionEventDTO? sectionDTO = null)
    {
        Guard.IsNotNull(sectionDTO);
        Guard.IsGreaterThan(sectionDTO.Section, 0);
        Guard.IsEqualTo(sectionDTO.Candidates.Count, sectionDTO.Votes.Count);
        List<CandidateVotes> candidateVotes = sectionDTO.Candidates
            .Select((candidateNum, index) => new CandidateVotes
            {
                Candidate = candidateNum,
                Votes = sectionDTO.Votes[index]
            })
            .ToList();
        
        Guard.IsNotEmpty(candidateVotes);
        Section section = new();
        section.SectionID = sectionDTO.Section;
        section.CandidateVotes.AddRange(candidateVotes);
        return section;
    }

    public static Section CandidateEventDTOToSection(CandidateEventDTO? candidateDTO = null)
    {
        Guard.IsNotNull(candidateDTO);
        Guard.IsGreaterThan(candidateDTO.Section, 0);
        Guard.IsGreaterThan(candidateDTO.Candidate, 0);
        List<CandidateVotes> candidateVotes = new()
        {
            new CandidateVotes
            {
                Candidate = candidateDTO.Candidate,
                Votes = candidateDTO.Votes
            }
        };
        
        Guard.IsNotEmpty(candidateVotes);
        
        Guard.IsNotEmpty(candidateVotes);
        Section section = new();
        section.SectionID = candidateDTO.Section;
        section.CandidateVotes.AddRange(candidateVotes);
        return section;
    }
    
    public static List<Section> CandidateEventDTOListToSectionList(List<CandidateEventDTO>? candidateDTOList = null)
    {
        Guard.IsNotNull(candidateDTOList);
        Guard.IsNotEmpty(candidateDTOList);
        List<Section> sections = new();
        candidateDTOList.ForEach(dto =>
        {
            sections.Add(CandidateEventDTOToSection(dto));
        });
        
        Guard.IsNotEmpty(sections);
        Guard.IsEqualTo(sections.Count, candidateDTOList.Count);
        return sections;
    }

    public static List<Section> DeploymentToSections(VotingDbDeployment deployment)
    {
        Guard.IsNotEmpty(deployment.Candidates);
        Guard.IsNotEmpty(deployment.Sections);
        Guard.IsNotEmpty(deployment.Votes);
        Guard.IsEqualTo(deployment.Votes.Count, deployment.Sections.Count);
        Guard.IsEqualTo(deployment.Votes.First().Count, deployment.Candidates.Count);
        
        List<Section> sections = new();
        for (int i = 0; i < deployment.Votes.Count; i++)
        {
            List<CandidateVotes> candidateVotesList = deployment.Votes[i]
                .Select(
                    (votes, j) => new CandidateVotes 
                    { 
                        Candidate = deployment.Candidates[j], 
                        Votes = votes 
                    })
                .ToList();
            
            Section section = new();
            section.SectionID = deployment.Sections[i];
            section.CandidateVotes.AddRange(candidateVotesList);
            sections.Add(section);
        }

        Guard.IsNotEmpty(sections);
        Guard.IsEqualTo(sections.Count, deployment.Sections.Count);
        return sections;
    }

    public static VotingDbDeployment SectionsListToDeployment(List<Section> sections)
    {
        Guard.IsNotEmpty(sections);
        Guard.IsFalse(sections.Any(section => section.SectionID == 0));
        Guard.IsFalse(sections.Any(section => section.CandidateVotes.Count == 0));

        List<uint> candidates = sections.First().CandidateVotes.Select(x => x.Candidate).ToList();
        Guard.IsNotEmpty(candidates);
        Guard.IsFalse(sections.Any(section => section.CandidateVotes.Count != candidates.Count));


        List<List<uint>> votes = new();
        foreach (var section in sections)
        {
            votes.Add(section.CandidateVotes.Select(x => x.Votes).ToList());
        }

        string sectionJSON = JsonSerializer.Serialize(sections);
        Guard.IsNotNullOrEmpty(sectionJSON);

        Compression compression = new();
        string compressedSection = compression.Compress(sectionJSON);
        Guard.IsNotNullOrEmpty(compressedSection);

        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString(CultureInfo.InvariantCulture);

        return new VotingDbDeployment
        {
            Votes = votes,
            Candidates = candidates,
            Sections = sections.Select(section => section.SectionID).ToList(),
            Timestamp = timestamp,
            CompressedSectionData = compressedSection
        };
    }
}