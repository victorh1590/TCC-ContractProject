const expect = require("chai").expect;
const chaiUse = require("chai").use;
const solidity = require("ethereum-waffle").solidity;

const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');

const _votes = seedData["votes"];
const _candidates = seedData["candidates"];
const _sections = seedData["sections"];
const _timestamp = seedData["timestamp"]

const {
  BN,           // Big Number support
  constants,    // Common constants, like the zero address and largest integers
  expectEvent,  // Assertions for emitted events
  expectRevert, // Assertions for transactions that should fail
} = require('@openzeppelin/test-helpers');

chaiUse(solidity);

contract('VotingDb', (accounts) => {
  it('getVotesBySection should return votes by section', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.getVotesBySection(30);
    
    expect(results).to.exist;

    const candidates = results[0];
    const votes = results[1].map(element => element.toNumber());
    
    console.log(candidates);
    console.log(votes);

    expect(votes).to.include.deep.ordered.members(_votes[2]);
    expect(candidates).to.include.deep.ordered.members(_candidates);
  });
  it('getVotesOnSectionByCandidate should return votes by section and candidate', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = (await votingDbInstance.getVotesOnSectionByCandidate(300, "David")).toNumber();
    
    expect(results).to.exist;

    console.log(results);

    expect(results).to.deep.equal(0);
  });
  it('getAllVotes should return all sections, candidates and votes', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.getAllVotes();
    expect(results).to.exist;

    const sections = results[0].map(element => element.toNumber());
    const candidates = results[1];
    const votes = [];
    results[2].forEach( array => {
      let numArray = array.map(element => element.toNumber());
      votes.push(numArray);
    });

    console.log("Sections: \n" + JSON.stringify(sections));
    console.log("Candidates: \n" + JSON.stringify(candidates));
    console.log("Votes: \n" + JSON.stringify(votes));

    expect(votes).to.include.deep.ordered.members(_votes);
    expect(candidates).to.include.deep.ordered.members(_candidates);
    expect(sections).to.include.deep.ordered.members(_sections);
  });
  it('getVotesBySection should revert when sectionID is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();

    await expect(votingDbInstance.getVotesBySection(0)).to.be.revertedWith("SectionNotFound");
    await expect(votingDbInstance.getVotesBySection(500)).to.be.revertedWith("SectionNotFound");
    await expect(votingDbInstance.getVotesBySection(15455646)).to.be.revertedWith("SectionNotFound");
  });
  it('getOnSectionByCandidate should revert when sectionID or candidate is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();

    //Valid candidate but invalid sectionID.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(0, "John")).to.be.revertedWith("SectionNotFound");;

    //Valid sectionID but invalid candidate.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(20, "4848468")).to.be.revertedWith("CandidateNotFound");;

    //candidate and sectionID invalid.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(1500, "@!#?dasd")).to.be.revertedWith("CandidateNotFound");;
  });
  it('getAllVotes should revert when contract is empty', async () => {
    expect(VotingDb.new([[]], [], [], "")).to.be.revertedWith("InitialDataInvalid");
  });
});