const VotingDb = artifacts.require("VotingDb");
const seedData = require('../contracts/SeedData.js');
const compressedData = require('../contracts/CompressedData.js');

module.exports = function(deployer) {
  deployer.deploy(VotingDb, 
    seedData["sections"],
    seedData["timestamp"], 
    compressedData["votesJSON"], 
    compressedData["compressedSectionData"]);
};