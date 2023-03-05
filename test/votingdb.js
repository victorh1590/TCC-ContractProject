const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');

const _votes = seedData["votes"];
const _candidates = seedData["candidates"];
const _sections = seedData["sections"];
const _timestamp = seedData["timestamp"]


contract('VotingDb', (accounts) => {
  it('should return votes by section', async () => {
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
  it('should return votes by section and candidate', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = (await votingDbInstance.getVotesOnSectionByCandidate(300, "David")).toNumber();
    expect(results).to.exist;

    console.log(results);
    expect(results).to.deep.equal(0);
  });
  it('should return all sections, candidates and votes', async () => {
    const votingDbInstance = await VotingDb.deployed();
    const results = await votingDbInstance.getAllVotes();
    console.log(results);
    expect(results).to.exist;

    const sections = results[0].map(element => element.toNumber());
    const candidates = results[1];
    const votes = results[2].map(element => element.toNumber());
    console.log("Sections: \n" + sections);
    console.log("Candidates: \n" + candidates);
    console.log("Votes: \n" + votes);

    expect(votes).to.include.deep.ordered.members(_votes);
    expect(candidates).to.include.deep.ordered.members(_candidates);
    expect(sections).to.include.deep.ordered.members(_sections);
  });
});


//   function testInitialBalanceWithNewMetaCoin() public {
//     MetaCoin meta = new MetaCoin();

//     uint expected = 10000;

//     Assert.equal(meta.getBalance(tx.origin), expected, "Owner should have 10000 MetaCoin initially");
//   }

// }
