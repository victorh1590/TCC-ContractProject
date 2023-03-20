namespace Voting.Server.Domain.Models;

internal class Section 
{
  public uint SectionID { get; set; }
  public List<CandidateVotes> CandidateVotes { get; set; }

  public Section(uint section, List<CandidateVotes> votes) 
  {
    SectionID = section;
    CandidateVotes = votes;
  }
}