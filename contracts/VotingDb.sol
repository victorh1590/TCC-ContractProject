// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.8.13 <0.9.0;
pragma abicoder v2;

contract VotingDb 
{
    struct Vote {
        uint24 vote;
        bool exists;
    }

    //Errors
    string SectionNotFoundError = "SectionNotFound";
    string CandidateNotFoundError = "CandidateNotFound";
    string ContractEmptyError = "ContractEmpty";
    string InitialDataInvalidError = "InitialDataInvalid";

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
    ) 
    {
        //Validations
        require(
            votes.length      > 0     && 
            candidates.length > 0     &&
            sections.length   > 0     &&
            votes.length      == sections.length   &&
            votes[0].length   == candidates.length &&
            keccak256(bytes(timestamp))     != keccak256(bytes("")) &&
            keccak256(bytes(candidates[0])) != keccak256(bytes("")),
            InitialDataInvalidError
        );

        //Assignments
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
                        exists: true
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
        //Pre-Condition(s)
        if(!_sectionResults[sectionID][_candidates[0]].exists)
        {
            revert(SectionNotFoundError);
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
    ) 
        public 
        view 
        returns (uint24 votes) 
    {
        //Pre-Condition(s)
        if(!candidateExist(candidate)) 
        {
            revert(CandidateNotFoundError);
        }
        if(!_sectionResults[sectionID][candidate].exists) 
        {
            revert(SectionNotFoundError);
        }

        return _sectionResults[sectionID][candidate].vote;
    }

    function getVotesByCandidate(string memory candidate)
    public
    view
    returns (uint24[] memory, uint24[] memory)
    {
        //Pre-Condition(s)
        if(!candidateExist(candidate)) 
        {
            revert(CandidateNotFoundError);
        }

        uint24[] memory votes = new uint24[](_sections.length);
        for(uint8 i = 0; i < _sections.length; i++) 
        {
            votes[i] = _sectionResults[_sections[i]][candidate].vote;
        }

        return(_sections, votes);
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
        //Pre-Condition(s)
        if( _sections.length == 0 || _candidates.length == 0 ||
            !_sectionResults[_sections[0]][_candidates[0]].exists)
        {
            revert(ContractEmptyError);
        }

        uint24[][] memory votes = new uint24[][](_sections.length);
        for (uint8 i = 0; i < _sections.length; i++) 
        {
            uint24[] memory votesByCandidate = new uint24[](_candidates.length);
            for (uint8 j = 0; j < _candidates.length; j++) 
            {
                votesByCandidate[j] = _sectionResults[_sections[i]][_candidates[j]].vote;
            }
            votes[i] = votesByCandidate;
        }
        return (_sections, _candidates, votes);
    }

    function getTotalVotesInBlock()
        public
        view
        returns(uint24)
    {
        uint24 total = 0;
        for(uint8 i = 0; i < _sections.length; i++)
        {
            for(uint8 j = 0; j < _candidates.length; j++)
            {
                total += _sectionResults[_sections[i]][_candidates[j]].vote;
            }
        }
        return total;
    }

    function getTotalVotesBySection(uint24 sectionID)
        public
        view
        returns(uint24)
    {
        //Pre-Condition(s)
        if(!_sectionResults[sectionID][_candidates[0]].exists)
        {
            revert(SectionNotFoundError);
        }

        uint24 total = 0;
        for(uint8 i = 0; i < _candidates.length; i++)
        {
            total += _sectionResults[sectionID][_candidates[i]].vote;
        }
        return total;
    }

    function getTotalVotesByCandidate(string memory candidate)
        public
        view
        returns(uint24)
    {
        //Pre-Condition(s)
        if(!candidateExist(candidate)) 
        {
            revert(CandidateNotFoundError);
        }

        uint24 total = 0;
        for(uint8 i = 0; i < _sections.length; i++)
        {
            total += _sectionResults[_sections[i]][candidate].vote;
        }
        return total;
    }
}