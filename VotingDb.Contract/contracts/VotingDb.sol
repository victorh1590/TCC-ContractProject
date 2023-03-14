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
        uint32[] memory Sections,
        string memory Timestamp,
        string memory VotesJSON,
        string memory CompressedSectionData
    ) 
    {
        //Validations
        validation__CreationData(
            Sections, 
            Timestamp, 
            VotesJSON, 
            CompressedSectionData);

        //Assignments
        _Owner = msg.sender;
        _Timestamp = Timestamp;
        _CompressedSectionData = CompressedSectionData;

        emit heartbeat(
            msg.sender, 
            block.number, 
            true, 
            Sections, 
            VotesJSON,
            Timestamp,
            address(this));
    }

    //Events
    event heartbeat(
        address indexed Account,
        uint indexed Block, 
        bool indexed Election, 
        uint32[] Sections,
        string VotesJSON,
        string Timestamp,
        address ContractAddress);

    //Validations / Pre-Conditions / Post-Conditions / Invariants
    function validation__CreationData(
        uint32[] memory Sections,
        string memory Timestamp,
        string memory VotesJSON,
        string memory CompressedSectionData
    )
        private
        view
    {
        require(
            Sections.length > 0 &&
            keccak256(bytes(VotesJSON)) != keccak256(bytes("")) && 
            keccak256(bytes(Timestamp)) != keccak256(bytes("")) &&
            keccak256(bytes(CompressedSectionData)) != keccak256(bytes("")),
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