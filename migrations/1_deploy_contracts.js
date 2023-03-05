const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');

module.exports = function(deployer) {
  deployer.deploy(VotingDb, seedData["votes"], seedData["candidates"], seedData["sections"], seedData["timestamp"]);
};

// module.exports = function(deployer) {
//   deployer.deploy(ConvertLib);
//   deployer.link(ConvertLib, MetaCoin);
//   deployer.deploy(MetaCoin);
// };
