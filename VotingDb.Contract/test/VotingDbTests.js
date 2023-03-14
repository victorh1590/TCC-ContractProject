const expect = require("chai").expect;
const chaiUse = require("chai").use;
const solidity = require("ethereum-waffle").solidity;

const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');
const compressedData = require('../contracts/CompressedData.js');

const _sections = seedData["sections"];
const _timestamp = seedData["timestamp"]
const _compressedSectionData = compressedData["compressedSectionData"];
const _votesJSON = compressedData["votesJSON"];

chaiUse(solidity);

contract('VotingDb', (accounts) => {
  it('constructor should revert when initial data is invalid', async () => {
    //Everything is invalid.
    await expect(VotingDb.new([], "", "", "")).to.be.revertedWith("CreationDataInvalid");
    //Sections length is 0.
    await expect(VotingDb.new([], "a", _votesJSON, _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Timestamp is ""
    await expect(VotingDb.new([1,2], "", _votesJSON, _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //VotesJSON is invalid
    await expect(VotingDb.new([1,2], "a", "", _compressedSectionData)).to.be.revertedWith("CreationDataInvalid");
    //Compressed Data is invalid
    await expect(VotingDb.new([1,2], "a", _votesJSON, "")).to.be.revertedWith("CreationDataInvalid");
  });
  it('constructor should emit heartbeat event with correct data', async () => {
    const votingDbInstance = await VotingDb.new(_sections, _timestamp, _votesJSON, _compressedSectionData);
    const eventsEmitted = await votingDbInstance.getPastEvents('heartbeat');
    expect(eventsEmitted.length).to.deep.equal(1);
    const eventResults = eventsEmitted[0].returnValues;
    const expectedSections = _sections.map(element => String(element));
    console.log(eventResults);
    expect(eventResults.Block).to.deep.equal(String(eventsEmitted[0].blockNumber));
    expect(eventResults.ContractAddress).to.deep.equal(votingDbInstance.address);
    expect(eventResults.Election).to.deep.equal(true);
    expect(eventResults.Sections).to.include.deep.ordered.members(expectedSections);
    expect(eventResults.Account).to.deep.equal(accounts[0]);
    expect(eventResults.Timestamp).to.deep.equal(_timestamp);
    expect(eventResults.VotesJSON).to.deep.equal(_votesJSON);
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