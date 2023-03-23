using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Domain.Models.Mappings;

internal class Mappings
{
    public static Section SectionEventDTOToSection(SectionEventDTO sectionDTO)
    {
        Guard.IsEqualTo(sectionDTO.Candidates.Count, sectionDTO.Votes.Count);
        List<CandidateVotes> candidateVotes = sectionDTO.Candidates
            .Select((t, i) => new CandidateVotes(t, sectionDTO.Votes[i]))
            .ToList();

        return new Section(sectionDTO.Section, candidateVotes);
    }

    public static Section CandidateEventDTOToSection(CandidateEventDTO candidateDTO)
    {
        List<CandidateVotes> candidateVotes = new()
        {
            new CandidateVotes
            {
                Candidate = candidateDTO.Candidate,
                Votes = candidateDTO.Votes
            }
        };
        return new Section(candidateDTO.Section, candidateVotes);
    }

    public static List<Section> DeploymentToSections(VotingDbDeployment deployment)
    {
        Guard.IsNotEmpty(deployment.Candidates);
        Guard.IsNotEmpty(deployment.Sections);
        Guard.IsNotEmpty(deployment.Votes);
        Guard.IsNotNullOrEmpty(deployment.Timestamp);
        Guard.IsNotNullOrEmpty(deployment.CompressedSectionData);
        Guard.IsEqualTo(deployment.Votes.Count, deployment.Sections.Count);
        Guard.IsEqualTo(deployment.Votes.First().Count, deployment.Candidates.Count);
        
        List<Section> sections = new();
        // List<CandidateVotes> candidateVotesList = new();
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
            sections.Add(new Section(deployment.Sections[i], candidateVotesList));
        }
        
        Guard.IsNotEmpty(sections);
        Guard.IsEqualTo(sections.Count, deployment.Sections.Count);
        return sections;
    }
}