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
  it('getVotesByCandidate should return votes by candidate', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.getVotesByCandidate("David");
    expect(results).to.exist;

    const expectedVotes = [5,7,0,5,4,2,4,4,5,3,9,2,8,5,4,7,6,9,7,1,0,8,1,2,3,5,6,8,7,0];
    const sections = results[0].map(element => element.toNumber());
    const votes = results[1].map(element => element.toNumber());

    console.log("Sections: \n" + JSON.stringify(sections));
    console.log("Votes: \n" + JSON.stringify(votes));

    expect(votes).to.include.deep.ordered.members(expectedVotes);
    expect(sections).to.include.deep.ordered.members(_sections);
  });
  it('getVotesBySection should revert when sectionID is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();

    await expect(votingDbInstance.getVotesBySection(0)).to.be.revertedWith("SectionNotFound");
    await expect(votingDbInstance.getVotesBySection(500)).to.be.revertedWith("SectionNotFound");
    await expect(votingDbInstance.getVotesBySection(15455646)).to.be.revertedWith("SectionNotFound");
  });
  it('getVotesOnSectionByCandidate should revert when sectionID or candidate is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();
    //Valid candidate but invalid sectionID.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(0, "John")).to.be.revertedWith("SectionNotFound");;
    //Valid sectionID but invalid candidate.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(20, "4848468")).to.be.revertedWith("CandidateNotFound");;
    //candidate and sectionID invalid.
    await expect(votingDbInstance.getVotesOnSectionByCandidate(1500, "@!#?dasd")).to.be.revertedWith("CandidateNotFound");;
  });
  it('getVotesByCandidate should revert when candidate is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();
    //Valid candidate is not reverted.
    await expect(votingDbInstance.getVotesByCandidate("Mark")).to.not.be.reverted;
    //Invalid candidate is reverted.
    await expect(votingDbInstance.getVotesByCandidate("Mark8")).to.be.revertedWith("CandidateNotFound");
  });
  it('constructor should revert when initial data is invalid', async () => {
    //Everything is invalid.
    expect(VotingDb.new([[]], [], [], "")).to.be.revertedWith("InitialDataInvalid");
    //Votes length is 0.
    expect(VotingDb.new([[]], ["a"], [1,2], "a")).to.be.revertedWith("InitialDataInvalid");
    //Candidates length is 0.
    expect(VotingDb.new([[1,2],[3,4]], [], [1,2], "a")).to.be.revertedWith("InitialDataInvalid");
    //Sections length is 0.
    expect(VotingDb.new([[1,2],[3,4]], ["a"], [], "a")).to.be.revertedWith("InitialDataInvalid");
    //Votes has length [2][2] but candidates has length [1].
    expect(VotingDb.new([[1,2],[3,4]], ["a"], [1,2], "a")).to.be.revertedWith("InitialDataInvalid");
    //Votes has length [2][2] but sections has length [1].
    expect(VotingDb.new([[1,2],[3,4]], ["a", "b"], [1], "a")).to.be.revertedWith("InitialDataInvalid");
    //Timestamp is ""
    expect(VotingDb.new([[1,2],[3,4]], ["a", "b"], [1,2], "")).to.be.revertedWith("InitialDataInvalid");
    //Candidates[0] is ""
    expect(VotingDb.new([[1,2],[3,4]], ["", "b"], [1,2], "a")).to.be.revertedWith("InitialDataInvalid");
  });
  it('getTotalVotesInBlock should return the sum of all votes', async () => {
    const votingDbInstance = await VotingDb.deployed();

    const results = await votingDbInstance.getTotalVotesInBlock();
    expect(results).to.exist;
    const sum = results.toNumber();
    
    console.log("Total Votes in Block: \n" + sum);
    expect(sum).to.deep.equal(570);
  });
  it('getTotalVotesBySection should return the sum of all votes in a given section', async () => {
    const votingDbInstance = await VotingDb.deployed();

    const results = await votingDbInstance.getTotalVotesBySection(140);
    expect(results).to.exist;
    const sum = results.toNumber();

    console.log("Total Votes in Section: \n" + sum);
    expect(sum).to.deep.equal(15);
  });
  it('getTotalVotesByCandidate should return the sum of all votes by candidate on a block', async () => {
    const votingDbInstance = await VotingDb.deployed();

    const results = await votingDbInstance.getTotalVotesByCandidate("Paul");
    expect(results).to.exist;
    const sum = results.toNumber();

    console.log("Total Votes by candidate: \n" + sum);
    expect(sum).to.deep.equal(144);
  });
  it('getTotalVotesBySection should revert when sectionID is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();
    //Valid sectionID is not reverted.
    await expect(votingDbInstance.getTotalVotesBySection(290)).to.not.be.reverted;
    //Invalid sectionID is reverted.
    await expect(votingDbInstance.getTotalVotesBySection(500)).to.be.revertedWith("SectionNotFound");
  });
  it('getTotalVotesByCandidate should revert when candidate is invalid', async () => {
    const votingDbInstance = await VotingDb.deployed();
    //Valid candidate is not reverted.
    await expect(votingDbInstance.getTotalVotesByCandidate("John")).to.not.be.reverted;
    //Invalid candidate is reverted.
    await expect(votingDbInstance.getTotalVotesByCandidate("@#!$_")).to.be.revertedWith("CandidateNotFound");
  });
}); 