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
        string memory CompressedSectionData,
        string[] memory SectionJSON
    ) 
    {
        //Validations
        validation__CreationData(
            Sections, 
            Timestamp, 
            CompressedSectionData,
            SectionJSON);

        //Assignments
        _Owner = msg.sender;
        _Timestamp = Timestamp;
        _CompressedSectionData = CompressedSectionData;

        for(uint i = 0; i < Sections.length; i++) 
        {
            validate_SectionJSON(SectionJSON[i]);
            emit section(Sections[i], address(this), block.number, SectionJSON[i]);
        }
    }

	//Events
    event section(uint32 indexed Section, address indexed ContractAddress, uint indexed Block, string SectionJSON);

    //Validations / Pre-Conditions / Post-Conditions / Invariants
    function validation__CreationData(
        uint32[] memory Sections,
        string memory Timestamp,
        string memory CompressedSectionData,
        string[] memory SectionJSON
    )
        private
        view
    {
        require(
            Sections.length > 0 &&
            SectionJSON.length > 0 &&
            SectionJSON.length == Sections.length &&
            keccak256(bytes(Timestamp)) != keccak256(bytes("")) &&
            keccak256(bytes(CompressedSectionData)) != keccak256(bytes("")),
            CreationDataInvalidError
        );
    }

    function validate_SectionJSON(string memory SectionJSON)
        private
        view
    {        
        require(keccak256(bytes(SectionJSON)) != keccak256(bytes("")), CreationDataInvalidError);
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