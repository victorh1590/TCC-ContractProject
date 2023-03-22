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
}