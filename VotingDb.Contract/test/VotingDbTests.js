const expect = require("chai").expect;
const chaiUse = require("chai").use;
const solidity = require("ethereum-waffle").solidity;

const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');
const compressedData = require('../contracts/CompressedData.js');

const _votes = seedData["votes"];
const _candidates = seedData["candidates"];
const _sections = seedData["sections"];
const _timestamp = seedData["timestamp"]
const _compressedSectionData = compressedData["compressedSectionData"];

chaiUse(solidity);

contract('VotingDb', (accounts) => {
  it('constructor should revert when initial data is invalid', async () => {
    //Everything is invalid.
    await expect(VotingDb.new([[]], [], [], "", "")).to.be.revertedWith("CreationDataInvalid");
    //Votes length is 0.
    await expect(VotingDb.new([[]], [1, 2], [1, 2], "a", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Candidates length is 0.
    await expect(VotingDb.new([[1, 2], [3, 4]], [], [1, 2], "a", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Sections length is 0.
    await expect(VotingDb.new([[1, 2], [3, 4]], [5, 6], [], "a", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Votes has length [2][2] but candidates has length [1].
    await expect(VotingDb.new([[1, 2], [3, 4]], [1], [1, 2], "a", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Votes has length [2][2] but sections has length [1].
    await expect(VotingDb.new([[1, 2], [3, 4]], [5, 6], [1], "a", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Timestamp is ""
    await expect(VotingDb.new([[1, 2], [3, 4]], [5, 6], [1, 2], "", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
 //Compressed Data is invalid
    await expect(VotingDb.new([[1, 2]], [5, 6], [1], "a", "")).to.be.revertedWith("CreationDataInvalid");
  });
  it('constructor should emit all candidate events with correct data', async () => {
    const votingDbInstance = await VotingDb.new(_votes, _candidates, _sections, _timestamp, _compressedSectionData);
    const eventsEmitted = await votingDbInstance.getPastEvents('candidate');
    expect(eventsEmitted).to.exist;
    expect(eventsEmitted.length).to.deep.equal(_sections.length * _candidates.length);
    const mappedEvents = eventsEmitted.map(element => element.returnValues);
    const eventResults = [];
    mappedEvents.map(element =>
    (eventResults.push(
      {
        "Candidate": parseInt(element.Candidate),
        "Section": parseInt(element.Section),
        "ContractAddress": element.ContractAddress,
        "Votes": parseInt(element.Votes)
      }
    )));
    const expectedSectionCandidateUnion = [];
    _sections.map(section => {
      _candidates.forEach(candidate => {
        expectedSectionCandidateUnion.push([section, candidate])
      }
      )
    });
    const expectedVotes = [];
    _votes.forEach((element) => {
      element.forEach(votesQt => expectedVotes.push(votesQt))
    });
    // eventResults.forEach(element => {
    //   console.log("Section: " + element.Section);
    //   console.log("Candidate: " + element.Candidate);
    //   console.log("ContractAddress: " + element.ContractAddress);
    //   console.log("Block: " + element.Block);
    //   console.log("Votes: " + element.Votes);
    // });
    eventResults.forEach((element, index) => {
      expect(element.Section).to.deep.equal(expectedSectionCandidateUnion[index][0]);
      expect(element.Candidate).to.deep.equal(expectedSectionCandidateUnion[index][1]);
      expect(element.ContractAddress).to.deep.equal(votingDbInstance.address);
      expect(element.Votes).to.deep.equal(expectedVotes[index]);
    });
  });
  it('constructor should emit all section events with correct data', async () => {
    const votingDbInstance = await VotingDb.new(_votes, _candidates, _sections, _timestamp, _compressedSectionData);
    const eventsEmitted = await votingDbInstance.getPastEvents('section');
    expect(eventsEmitted).to.exist;
    expect(eventsEmitted.length).to.deep.equal(_sections.length);
    const mappedEvents = eventsEmitted.map(element => element.returnValues);
    const eventResults = [];
    mappedEvents.map(element =>
    (eventResults.push(
      {
        "Section": parseInt(element.Section),
        "ContractAddress": element.ContractAddress,
        "Votes": element.Votes,
        "Candidates": element.Candidates
      }
    )));
    eventResults.forEach((element, index) => {
      expect(element.Section).to.deep.equal(_sections[index]);
      expect(element.ContractAddress).to.deep.equal(votingDbInstance.address);
      expect(element.Votes).to.have.deep.ordered.members(element.Votes);
      expect(element.Candidates).to.have.deep.ordered.members(element.Candidates);
    });
  });
  it('constructor should emit metadata event with correct data', async () => {
    const votingDbInstance = await VotingDb.new(_votes, _candidates, _sections, _timestamp, _compressedSectionData);
    const eventsEmitted = await votingDbInstance.getPastEvents('metadata');
    expect(eventsEmitted).to.exist;
    expect(eventsEmitted.length).to.deep.equal(1);
    const eventResults = eventsEmitted[0].returnValues;
    const expectedSections = _sections.map(element => String(element));
    expect(eventResults.Block).to.deep.equal(String(eventsEmitted[0].blockNumber));
    expect(eventResults.ContractAddress).to.deep.equal(votingDbInstance.address);
    expect(eventResults.Sections).to.include.deep.ordered.members(expectedSections);
  });
  it('GetCompressedData returns correct brotli compressed data string with correct SHA3', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.GetCompressedData();
    expect(results).to.exist;
    expect(results).to.deep.equal(_compressedSectionData);
  });
  it('GetOwner should return account address', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.GetOwner();
    expect(results).to.exist;
    expect(results).to.deep.equal(accounts[0]);
    expect(results).to.not.deep.equal(accounts[1]);
  });
  it('GetTimestamp should return correct timestamp', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.GetTimestamp();
    expect(results).to.exist;
    expect(results).to.deep.equal(_timestamp);
    expect(results).to.not.deep.equal("whatever215@!#$%^&*()");
  });
}); 