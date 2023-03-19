using App;
using Voting.Server.Domain;
using Voting.Server.Persistence.ContractDefinition;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using System.Linq;
using System.Text;
using Nethereum.Util;

DomainService ds = new DomainService();
VotingDbDeployment deployment = new VotingDbDeployment()
{
    Votes = SeedData.Votes,
    Candidates = SeedData.Candidates,
    Sections = SeedData.Sections,
    Timestamp = SeedData.Timestamp,
    CompressedSectionData = SeedData.CompressedSectionData
};

await ds.DeployContract(deployment);
await ds.GetSectionRangeAsync(25);
// await ds.GetSectionAsync(60);


//VotingDbDeployment votingDbDeployment = new VotingDbDeployment
//{
//    Sections = new List<uint>
//    {
//        10U, 20U, 30U, 40U, 50U, 60U, 70U, 80U, 90U, 100U, 110U,
//        120U, 130U, 140U, 150U, 160U, 170U, 180U, 190U, 200U, 210U,
//        220U, 230U, 240U, 250U, 260U, 270U, 280U, 290U, 300
//    },
//    Timestamp = "2020-04-10T17:50:00Z",
//    //VotesJSON = "{\"section_votes\":{\"10\":{\"John\":3,\"Mark\":8,\"David\":5,\"Paul\":6},\"20\":{\"John\":1,\"Mark\":0,\"David\":7,\"Paul\":4},\"30\":{\"John\":2,\"Mark\":9,\"David\":0,\"Paul\":1},\"40\":{\"John\":7,\"Mark\":8,\"David\":5,\"Paul\":7},\"50\":{\"John\":3,\"Mark\":5,\"David\":4,\"Paul\":8},\"60\":{\"John\":9,\"Mark\":0,\"David\":2,\"Paul\":4},\"70\":{\"John\":2,\"Mark\":6,\"David\":4,\"Paul\":9},\"80\":{\"John\":4,\"Mark\":7,\"David\":4,\"Paul\":2},\"90\":{\"John\":7,\"Mark\":9,\"David\":5,\"Paul\":1},\"100\":{\"John\":6,\"Mark\":8,\"David\":3,\"Paul\":7},\"110\":{\"John\":3,\"Mark\":8,\"David\":9,\"Paul\":5},\"120\":{\"John\":9,\"Mark\":6,\"David\":2,\"Paul\":3},\"130\":{\"John\":4,\"Mark\":1,\"David\":8,\"Paul\":9},\"140\":{\"John\":1,\"Mark\":7,\"David\":5,\"Paul\":2},\"150\":{\"John\":9,\"Mark\":5,\"David\":4,\"Paul\":6},\"160\":{\"John\":6,\"Mark\":0,\"David\":7,\"Paul\":3},\"170\":{\"John\":2,\"Mark\":4,\"David\":6,\"Paul\":1},\"180\":{\"John\":3,\"Mark\":2,\"David\":9,\"Paul\":8},\"190\":{\"John\":1,\"Mark\":8,\"David\":7,\"Paul\":5},\"200\":{\"John\":7,\"Mark\":6,\"David\":1,\"Paul\":9},\"210\":{\"John\":8,\"Mark\":3,\"David\":0,\"Paul\":4},\"220\":{\"John\":5,\"Mark\":9,\"David\":8,\"Paul\":6},\"230\":{\"John\":4,\"Mark\":5,\"David\":1,\"Paul\":7},\"240\":{\"John\":8,\"Mark\":4,\"David\":2,\"Paul\":3},\"250\":{\"John\":6,\"Mark\":7,\"David\":3,\"Paul\":0},\"260\":{\"John\":2,\"Mark\":1,\"David\":5,\"Paul\":9},\"270\":{\"John\":9,\"Mark\":3,\"David\":6,\"Paul\":4},\"280\":{\"John\":5,\"Mark\":9,\"David\":8,\"Paul\":6},\"290\":{\"John\":4,\"Mark\":5,\"David\":1,\"Paul\":7},\"300\":{\"John\":7,\"Mark\":1,\"David\":0,\"Paul\":8}}}",
//    CompressedSectionData = "G38GICwOzHO63ILBTcFILjy0ENHZQuWnT9AClJ7wSxY/5T6fXF9rEZZscnU8//3F/Aqt4Zk7f3fR0zTKM+jNThBrbCFuDMOqWQR59PXYey85m/s8EH17h/rBzePb7h5PZH/h9vGbO+/5cxAYKFUSaiIohVw6Kk3aVLYMNmCqCDpUXJZaUFiMNB18QWBcMvoAAoLTE2AHJiegBBtPABYUTrDPmQkWBiUvgsUEE5ASZycALko3IgfEjaxJpBppzWYWaYOokbVgspFRZKIRCjpuxOY4xzxhj/fFLPEWsXy+7wc=",
//    SectionJSON = new List<string>() {
//        "{ \"Section\": \"10\", \"Votes\": { \"John\": 3, \"Mark\": 8, \"David\": 5, \"Paul\": 6 } }",
//        "{ \"Section\": \"20\", \"Votes\": { \"John\": 1, \"Mark\": 0, \"David\": 7, \"Paul\": 4 } }",
//        "{ \"Section\": \"30\", \"Votes\": { \"John\": 2, \"Mark\": 9, \"David\": 0, \"Paul\": 1 } }",
//        "{ \"Section\": \"40\", \"Votes\": { \"John\": 7, \"Mark\": 8, \"David\": 5, \"Paul\": 7 } }",
//        "{ \"Section\": \"50\", \"Votes\": { \"John\": 3, \"Mark\": 5, \"David\": 4, \"Paul\": 8 } }",
//        "{ \"Section\": \"60\", \"Votes\": { \"John\": 9, \"Mark\": 0, \"David\": 2, \"Paul\": 4 } }",
//        "{ \"Section\": \"70\", \"Votes\": { \"John\": 2, \"Mark\": 6, \"David\": 4, \"Paul\": 9 } }",
//        "{ \"Section\": \"80\", \"Votes\": { \"John\": 4, \"Mark\": 7, \"David\": 4, \"Paul\": 2 } }",
//        "{ \"Section\": \"90\", \"Votes\": { \"John\": 7, \"Mark\": 9, \"David\": 5, \"Paul\": 1 } }",
//        "{ \"Section\": \"100\", \"Votes\": { \"John\": 6, \"Mark\": 8, \"David\": 3, \"Paul\": 7 } }",
//        "{ \"Section\": \"110\", \"Votes\": { \"John\": 3, \"Mark\": 8, \"David\": 9, \"Paul\": 5 } }",
//        "{ \"Section\": \"120\", \"Votes\": { \"John\": 9, \"Mark\": 6, \"David\": 2, \"Paul\": 3 } }",
//        "{ \"Section\": \"130\", \"Votes\": { \"John\": 4, \"Mark\": 1, \"David\": 8, \"Paul\": 9 } }",
//        "{ \"Section\": \"140\", \"Votes\": { \"John\": 1, \"Mark\": 7, \"David\": 5, \"Paul\": 2 } }",
//        "{ \"Section\": \"150\", \"Votes\": { \"John\": 9, \"Mark\": 5, \"David\": 4, \"Paul\": 6 } }",
//        "{ \"Section\": \"160\", \"Votes\": { \"John\": 6, \"Mark\": 0, \"David\": 7, \"Paul\": 3 } }",
//        "{ \"Section\": \"170\", \"Votes\": { \"John\": 2, \"Mark\": 4, \"David\": 6, \"Paul\": 1 } }",
//        "{ \"Section\": \"180\", \"Votes\": { \"John\": 3, \"Mark\": 2, \"David\": 9, \"Paul\": 8 } }",
//        "{ \"Section\": \"190\", \"Votes\": { \"John\": 1, \"Mark\": 8, \"David\": 7, \"Paul\": 5 } }",
//        "{ \"Section\": \"200\", \"Votes\": { \"John\": 7, \"Mark\": 6, \"David\": 1, \"Paul\": 9 } }",
//        "{ \"Section\": \"210\", \"Votes\": { \"John\": 8, \"Mark\": 3, \"David\": 0, \"Paul\": 4 } }",
//        "{ \"Section\": \"220\", \"Votes\": { \"John\": 5, \"Mark\": 9, \"David\": 8, \"Paul\": 6 } }",
//        "{ \"Section\": \"230\", \"Votes\": { \"John\": 4, \"Mark\": 5, \"David\": 1, \"Paul\": 7 } }",
//        "{ \"Section\": \"240\", \"Votes\": { \"John\": 8, \"Mark\": 4, \"David\": 2, \"Paul\": 3 } }",
//        "{ \"Section\": \"250\", \"Votes\": { \"John\": 6, \"Mark\": 7, \"David\": 3, \"Paul\": 0 } }",
//        "{ \"Section\": \"260\", \"Votes\": { \"John\": 2, \"Mark\": 1, \"David\": 5, \"Paul\": 9 } }",
//        "{ \"Section\": \"270\", \"Votes\": { \"John\": 9, \"Mark\": 3, \"David\": 6, \"Paul\": 4 } }",
//        "{ \"Section\": \"280\", \"Votes\": { \"John\": 5, \"Mark\": 9, \"David\": 8, \"Paul\": 6 } }",
//        "{ \"Section\": \"290\", \"Votes\": { \"John\": 4, \"Mark\": 5, \"David\": 1, \"Paul\": 7 } }",
//        "{ \"Section\": \"300\", \"Votes\": { \"John\": 7, \"Mark\": 1, \"David\": 0, \"Paul\": 8 } }"
//    }
//};

//var johnHash = "0x" + Nethereum.Web3.Web3.Sha3("John"); // sha3Hash