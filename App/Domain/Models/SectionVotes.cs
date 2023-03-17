using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class SectionVotes 
{
  public uint Section { get; set; }
  public Dictionary<string, uint> Votes { get; set; }

  public SectionVotes() 
  {
    Votes = new Dictionary<string, uint>();
  }

  public SectionVotes(uint section, Dictionary<string, uint> votes) 
  {
    Section = section;
    Votes = votes;
  }
}