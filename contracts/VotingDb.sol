// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.8.0 <0.9.0;
pragma abicoder v2;

contract VotingDb 
{
    struct Vote {
        uint24 vote;
        bool exist;
    }

    error SectionNotFound();
    error CandidateNotFound();
    error ContractEmpty();

    //Sections (uint) > Candidates (string) > Votes (uint)
    mapping(uint24 => mapping(string => Vote)) _sectionResults;

    string[] _candidates;
    uint24[] _sections;
    string _timestamp;
    address _owner;

    constructor(
        uint24[][] memory votes,
        string[] memory candidates,
        uint24[] memory sections,
        string memory timestamp
    ) {
        _owner = msg.sender;
        _timestamp = timestamp;
        _sections = sections;
        _candidates = candidates;

        for (uint8 i = 0; i < sections.length; i++) 
        {
            for (uint8 j = 0; j < candidates.length; j++) 
            {
                _sectionResults[sections[i]][candidates[j]] = 
                Vote({
                        vote: votes[i][j],
                        exist: true
                });
            }
        }
    }

    //Functions
    function candidateExist(string memory target)
        private
        view
        returns (bool)
    {
        bytes32 targetBytes = keccak256(abi.encodePacked(target));
        for(uint8 i = 0; i < _candidates.length; i++) 
        {
            if(keccak256(abi.encodePacked(_candidates[i])) == targetBytes)
            {
                return true;
            }
        }
        return false;
    }

    //Queries
    function getVotesBySection(uint24 sectionID)
        public
        view
        returns (string[] memory, uint24[] memory)
    {
        //Post-Condition(s)
        if(!_sectionResults[sectionID][_candidates[0]].exist)
        {
            revert SectionNotFound();
        }

        uint24[] memory votes = new uint24[](_candidates.length);
        for (uint8 i = 0; i < _candidates.length; i++) 
        {
            votes[i] = _sectionResults[sectionID][_candidates[i]].vote;
        }

        return (_candidates, votes);
    }

    function getVotesOnSectionByCandidate(
        uint24 sectionID,
        string memory candidate
    ) public view returns (uint24 votes) {
        //Post-Condition(s)
        if(!candidateExist(candidate)) 
        {
            revert CandidateNotFound();
        }
        if(!_sectionResults[sectionID][candidate].exist) 
        {
            revert SectionNotFound();
        }

        return _sectionResults[sectionID][candidate].vote;
    }

    function getAllVotes()
        public
        view
        returns (
            uint24[] memory,
            string[] memory,
            uint24[][] memory
        )
    {
        //Post-Condition(s)
        if(!_sectionResults[_sections[0]][_candidates[0]].exist)
        {
            revert ContractEmpty();
        }

        uint24[][] memory votes = new uint24[][](_sections.length);

        assert(_sections.length == 30);
        for (uint8 i = 0; i < _sections.length; i++) 
        {
            uint24[] memory votesByCandidate = new uint24[](_candidates.length);
            assert(_candidates.length == 4);
            for (uint8 j = 0; j < _candidates.length; j++) 
            {
                votesByCandidate[j] = _sectionResults[_sections[i]][_candidates[j]].vote;
            }
            votes[i] = votesByCandidate;
        }

        return (_sections, _candidates, votes);
    }
}