// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.8.13 <0.9.0;
pragma abicoder v2;

contract VotingDb 
{
    //Errors
    string CreationDataInvalidError = "CreationDataInvalid";

    //Storage
    address _Owner;
    string _Timestamp;
    string _CompressedSectionData;

    constructor(
        uint32[][] memory Votes,
        uint32[] memory Candidates,
        uint32[] memory Sections,
        string memory Timestamp,
        string memory CompressedSectionData
    ) 
    {
        //Validations
        validation__CreationData(
            Sections,
            Candidates,
            Votes, 
            Timestamp, 
            CompressedSectionData);

        //Assignments
        _Owner = msg.sender;
        _Timestamp = Timestamp;
        _CompressedSectionData = CompressedSectionData;

        emit metadata(address(this), block.number, Sections, Candidates);
        for(uint i = 0; i < Sections.length; i++) 
        {
            emit section(Sections[i], address(this), Votes[i]);
            validation__VotesForSection(Votes[i], Candidates);
            for(uint j = 0; j < Candidates.length; j++)
            {
                emit candidate(Candidates[j], Sections[i], address(this), Votes[i][j]);
            }
        }
    }

	//Events
    event candidate(
        uint32 indexed Candidate, 
        uint32 indexed Section, 
        address ContractAddress, 
        uint32 Votes);

    event section(
        uint32 indexed Section, 
        address ContractAddress, 
        uint32[] Votes);

   event metadata(
        address indexed ContractAddress, 
        uint Block, 
        uint32[] Sections,
        uint32[] Candidates);

    //Validations / Pre-Conditions / Post-Conditions / Invariants
    function validation__CreationData(
        uint32[] memory Sections,
        uint32[] memory Candidates,
        uint32[][] memory Votes,
        string memory Timestamp,
        string memory CompressedSectionData
    )
        private
        view
    {
        require(
            Sections.length   > 0     &&
            Candidates.length > 0     &&
            Sections.length   > 0     &&
            Votes.length      == Sections.length   &&
            keccak256(bytes(Timestamp)) != keccak256(bytes("")) &&
            keccak256(bytes(CompressedSectionData)) != keccak256(bytes("")),
            CreationDataInvalidError
        );
    }

    function validation__VotesForSection(
        uint32[] memory VotesForSection,
        uint32[] memory Candidates
    )
        private
        view
    {
        require(
            VotesForSection.length == Candidates.length,
            CreationDataInvalidError
        );
    }

    function GetCompressedData()
        public
        view
        returns (string memory)
    {
        return _CompressedSectionData;
    }

    function GetOwner()
        public
        view
        returns (address)
    {
        return _Owner;
    }

    function GetTimestamp()
        public
        view
        returns (string memory)
    {
        return _Timestamp;
    }
}