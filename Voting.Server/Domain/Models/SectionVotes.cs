using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voting.Server.Domain.Models;
internal class SectionVotes 
{
  public uint Section { get; }
  public Dictionary<uint, uint> Votes { get; }

  public SectionVotes() 
  {
    Votes = new Dictionary<uint, uint>();
  }

  public SectionVotes(uint section, Dictionary<uint, uint> votes) 
  {
    Section = section;
    Votes = votes;
  }
}