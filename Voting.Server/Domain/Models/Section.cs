// namespace Voting.Server.Domain.Models;
//
// public class Section : ICloneable
// {
//     public uint SectionID { get; set; }
//     public List<CandidateVotes> CandidateVotes { get; set; }
//
//     public Section(uint section, List<CandidateVotes> votes)
//     {
//         SectionID = section;
//         CandidateVotes = votes;
//     }
//
//     private Section DeepClone()
//     {
//         return new Section(SectionID, new List<CandidateVotes>(CandidateVotes));
//     }
//
//     public object Clone()
//     {
//         return DeepClone();
//     }
// }