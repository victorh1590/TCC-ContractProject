using App;
using App.VotingDb.ContractDefinition;

DomainService ds = new DomainService();
VotingDbDeployment votingDbDeployment = new VotingDbDeployment
{
    Sections = new List<uint> 
    {
        10U, 20U, 30U, 40U, 50U, 60U, 70U, 80U, 90U, 100U, 110U, 
        120U, 130U, 140U, 150U, 160U, 170U, 180U, 190U, 200U, 210U,
        220U, 230U, 240U, 250U, 260U, 270U, 280U, 290U, 300
    },
    Timestamp = "2020-04-10T17:50:00Z",
    VotesJSON = "{\"section_votes\":{\"10\":{\"John\":3,\"Mark\":8,\"David\":5,\"Paul\":6},\"20\":{\"John\":1,\"Mark\":0,\"David\":7,\"Paul\":4},\"30\":{\"John\":2,\"Mark\":9,\"David\":0,\"Paul\":1},\"40\":{\"John\":7,\"Mark\":8,\"David\":5,\"Paul\":7},\"50\":{\"John\":3,\"Mark\":5,\"David\":4,\"Paul\":8},\"60\":{\"John\":9,\"Mark\":0,\"David\":2,\"Paul\":4},\"70\":{\"John\":2,\"Mark\":6,\"David\":4,\"Paul\":9},\"80\":{\"John\":4,\"Mark\":7,\"David\":4,\"Paul\":2},\"90\":{\"John\":7,\"Mark\":9,\"David\":5,\"Paul\":1},\"100\":{\"John\":6,\"Mark\":8,\"David\":3,\"Paul\":7},\"110\":{\"John\":3,\"Mark\":8,\"David\":9,\"Paul\":5},\"120\":{\"John\":9,\"Mark\":6,\"David\":2,\"Paul\":3},\"130\":{\"John\":4,\"Mark\":1,\"David\":8,\"Paul\":9},\"140\":{\"John\":1,\"Mark\":7,\"David\":5,\"Paul\":2},\"150\":{\"John\":9,\"Mark\":5,\"David\":4,\"Paul\":6},\"160\":{\"John\":6,\"Mark\":0,\"David\":7,\"Paul\":3},\"170\":{\"John\":2,\"Mark\":4,\"David\":6,\"Paul\":1},\"180\":{\"John\":3,\"Mark\":2,\"David\":9,\"Paul\":8},\"190\":{\"John\":1,\"Mark\":8,\"David\":7,\"Paul\":5},\"200\":{\"John\":7,\"Mark\":6,\"David\":1,\"Paul\":9},\"210\":{\"John\":8,\"Mark\":3,\"David\":0,\"Paul\":4},\"220\":{\"John\":5,\"Mark\":9,\"David\":8,\"Paul\":6},\"230\":{\"John\":4,\"Mark\":5,\"David\":1,\"Paul\":7},\"240\":{\"John\":8,\"Mark\":4,\"David\":2,\"Paul\":3},\"250\":{\"John\":6,\"Mark\":7,\"David\":3,\"Paul\":0},\"260\":{\"John\":2,\"Mark\":1,\"David\":5,\"Paul\":9},\"270\":{\"John\":9,\"Mark\":3,\"David\":6,\"Paul\":4},\"280\":{\"John\":5,\"Mark\":9,\"David\":8,\"Paul\":6},\"290\":{\"John\":4,\"Mark\":5,\"David\":1,\"Paul\":7},\"300\":{\"John\":7,\"Mark\":1,\"David\":0,\"Paul\":8}}}",
    CompressedSectionData = "G38GICwOzHO63ILBTcFILjy0ENHZQuWnT9AClJ7wSxY/5T6fXF9rEZZscnU8//3F/Aqt4Zk7f3fR0zTKM+jNThBrbCFuDMOqWQR59PXYey85m/s8EH17h/rBzePb7h5PZH/h9vGbO+/5cxAYKFUSaiIohVw6Kk3aVLYMNmCqCDpUXJZaUFiMNB18QWBcMvoAAoLTE2AHJiegBBtPABYUTrDPmQkWBiUvgsUEE5ASZycALko3IgfEjaxJpBppzWYWaYOokbVgspFRZKIRCjpuxOY4xzxhj/fFLPEWsXy+7wc="
};
string contractAddress = await ds.DeployContract(votingDbDeployment);
Console.WriteLine(contractAddress);
await ds.GetSectionAsync();