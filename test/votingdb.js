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
});


// VotingDb.deployed().then(function (instance) { instance.getAllVotes(); });


// truffle(develop)> VotingDb.deployed().then(function (instance) { instance.getAllVotes(); });
// undefined
// truffle(develop)> Uncaught Error: VM Exception while processing transaction: revert
//     at evalmachine.<anonymous>
//     at processTicksAndRejections (node:internal/process/task_queues:96:5) {
//   code: -32000,
//   data: '0x4e487b710000000000000000000000000000000000000000000000000000000000000032',
//   hijackedStack: 'Error: VM Exception while processing transaction: revert\n' +
//     '    at C:\\Users\\victo\\AppData\\Roaming\\npm\\node_modules\\truffle\\build\\webpack:\\packages\\provider\\wrapper.js:25:1\n' +
//     '    at C:\\Users\\victo\\AppData\\Roaming\\npm\\node_modules\\truffle\\build\\webpack:\\packages\\provider\\wrapper.js:165:1\n' +
//     '    at C:\\Users\\victo\\AppData\\Roaming\\npm\\node_modules\\truffle\\build\\webpack:\\node_modules\\web3-providers-http\\lib\\index.js:127:1\n' +
//     '    at processTicksAndRejections (node:internal/process/task_queues:96:5)'
// }
// truffle(develop)> debug 0x4e487b710000000000000000000000000000000000000000000000000000000000000032
// Starting Truffle Debugger...


//   function testInitialBalanceWithNewMetaCoin() public {
//     MetaCoin meta = new MetaCoin();

//     uint expected = 10000;

//     Assert.equal(meta.getBalance(tx.origin), expected, "Owner should have 10000 MetaCoin initially");
//   }

// }
