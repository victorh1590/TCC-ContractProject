// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.8.0 <0.9.0;
pragma abicoder v2;

contract VotingDb 
{
    //Sections (uint) > Candidates (string) > Votes (uint)
    mapping(uint => mapping(string => uint)) _sectionResults;
    
    string[] _candidates;
    uint[] _sections;
    string _timestamp;
    address _owner;

    constructor
    (
        uint[][] memory votes,
        string[] memory candidates,
        uint[] memory sections,
        string memory timestamp
    ) 
    {
        _owner = msg.sender;
        _timestamp = timestamp;
        _sections = sections;
        _candidates = candidates;

        for(uint i = 0; i < sections.length; i++) 
        {
            for(uint j = 0; j < candidates.length; j++)
            {
                _sectionResults[sections[i]][candidates[j]] = votes[i][j];
            }
        }
    }

    //Queries
    function getVotesBySection(uint sectionID) 
    public 
    view 
    returns (string[] memory, uint[] memory) 
    {
        uint[] memory votes = new uint[](_candidates.length);
        for(uint i = 0; i < _candidates.length; i++) 
        {
            votes[i] = _sectionResults[sectionID][_candidates[i]];
        }

        return (_candidates, votes);
    }

    function getVotesOnSectionByCandidate(uint sectionID, string memory candidate) 
    public
    view
    returns (uint votes)
    {
        return _sectionResults[sectionID][candidate];
    }

    function getAllVotes()
    public
    view
    returns (uint[] memory, string[] memory, uint[][] memory)
    {
        uint[][] memory votes = new uint[][](_sections.length);

        assert(_sections.length == 30);
        for(uint i = 0; i < _sections.length; i++)
        {
            uint[] memory votesByCandidate = new uint[](_candidates.length);
            assert(_candidates.length == 4);
            for(uint j = 0; j < _candidates.length; j++) 
            {
                votesByCandidate[j] = _sectionResults[_sections[i]][_candidates[j]];
            }
            votes[i] = votesByCandidate;
        }

        return (_sections, _candidates, votes);
    }
}