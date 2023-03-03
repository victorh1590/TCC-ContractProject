const VotingDb = artifacts.require("VotingDb");

contract( 'VotingDb', (accounts) => {
  it('should return votes by section', async () => {
    const votingDbInstance = await VotingDb.deployed();
    let result = await votingDbInstance.getVotesBySection(30);
    let resultString = result[1];
    let resultsMap =
      resultString.map(element => element.toNumber());
    console.log(resultsMap);
  });
});