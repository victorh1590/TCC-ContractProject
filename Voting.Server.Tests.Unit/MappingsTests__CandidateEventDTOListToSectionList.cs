using Moq;
using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Utils;
using CommunityToolkit.Diagnostics;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class MappingsTests
{
    [Test, Repeat(10)]
    public void CandidateEventDTOListToSectionList_Should_Convert_CandidateEventDTO_List_to_Section_List_Correctly()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        
        //Get valid random candidate.
        uint expectedCandidate = seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());
        int expectedCandidateIndex = seedData.Deployment.Candidates.IndexOf(expectedCandidate);
        Guard.IsNotEqualTo(expectedCandidateIndex, -1);
        
        //Generate list with all sections containing the selected candidate votes.
        List<Section> expectedSectionList = new();
        foreach (Section section in seedData.Sections)
        {
            List<CandidateVotes> cvList = section.CandidateVotes
                .Where(cv => cv.Candidate == expectedCandidate)
                .ToList();
            Guard.IsEqualTo(cvList.Count, 1);
            
            expectedSectionList.Add(new Section(section.SectionID, cvList));
        }
        
        Guard.IsNotNull(expectedSectionList);
        Guard.IsNotEmpty(expectedSectionList);

        //Generate list of CandidateEventDTOs.
        List<CandidateEventDTO> candidateEventDTOList = new();
        for (int i = 0; i < seedData.Deployment.Sections.Count; i++)
        {
            Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
            candidateEventDTOMock.Setup(dto => dto.Section)
                .Returns(seedData.Deployment.Sections[i]);
            candidateEventDTOMock.Setup(dto => dto.Candidate)
                .Returns(seedData.Deployment.Candidates[expectedCandidateIndex]);
            candidateEventDTOMock.Setup(dto => dto.Votes)
                .Returns(seedData.Deployment.Votes[i][expectedCandidateIndex]);

            candidateEventDTOList.Add(candidateEventDTOMock.Object);
        }
        Guard.IsNotEmpty(candidateEventDTOList);
        
        //Act
        List<Section> result = Mappings.CandidateEventDTOListToSectionList(candidateEventDTOList);
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedSectionListJSON = JsonSerializer.Serialize(expectedSectionList);
        
        //Assertions
        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result.Count, Is.EqualTo(expectedSectionList.Count));
        Assert.That(resultJSON, Is.EqualTo(expectedSectionListJSON));
        Assert.That(result.Select(r => r.SectionID), 
            Is.EqualTo(expectedSectionList.Select(r => r.SectionID)));
        Assert.That(result.Select(r => r.CandidateVotes), 
            Is.EqualTo(expectedSectionList.Select(r => r.CandidateVotes)));
    }
    
     [Test]
    public void CandidateEventDTOListToSectionList_Should_Fail_When_List_Empty_Or_Null_Or_No_Param(
        [Random(0, 4, 10)] int randomCandidateIndex)
    {
        Assert.That(() => Mappings.CandidateEventDTOListToSectionList(null), 
            Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => Mappings.CandidateEventDTOListToSectionList(), 
            Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => Mappings.CandidateEventDTOListToSectionList(new List<CandidateEventDTO>()), 
            Throws.TypeOf<ArgumentException>());
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Invalid_CandidateEventDTO_Is_Given(
        [Random(0, 4, 10)] int randomCandidateIndex)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        
        //Get valid random candidate
        uint expectedCandidate = seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());

        //Generate list with all sections containing the selected candidate votes.
        List<Section> expectedSectionList = new();
        foreach (Section section in seedData.Sections)
        {
            List<CandidateVotes> cvList = section.CandidateVotes
                .Where(cv => cv.Candidate == expectedCandidate)
                .ToList();
            Guard.IsEqualTo(cvList.Count, 1);
            
            expectedSectionList.Add(new Section(section.SectionID, cvList));
        }
        
        Guard.IsNotNull(expectedSectionList);
        Guard.IsNotEmpty(expectedSectionList);

        //Invalid candidateEventDTOMock
        List<CandidateEventDTO> candidateEventDTOList = new();

        Mock<CandidateEventDTO> badCandidateEventDTOMock = new Mock<CandidateEventDTO>();
        badCandidateEventDTOMock.Setup(dto => dto.Section)
            .Returns(seedData.Deployment.Sections[0]); // First SectionID is valid.
        badCandidateEventDTOMock.Setup(dto => dto.Candidate)
            .Returns(0U); // Candidate zero is invalid.
        badCandidateEventDTOMock.Setup(dto => dto.Votes)
            .Returns(0U); // Zero Votes Is Valid
        
        candidateEventDTOList.Add(badCandidateEventDTOMock.Object);

        //Generate list of CandidateEventDTOs.
        for (int i = 1; i < seedData.Deployment.Sections.Count; i++)
        {
            Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
            candidateEventDTOMock.Setup(dto => dto.Section)
                .Returns(seedData.Deployment.Sections[i]);
            candidateEventDTOMock.Setup(dto => dto.Candidate)
                .Returns(seedData.Deployment.Candidates[randomCandidateIndex]);
            candidateEventDTOMock.Setup(dto => dto.Votes)
                .Returns(seedData.Deployment.Votes[i][randomCandidateIndex]);

            candidateEventDTOList.Add(candidateEventDTOMock.Object);
        }
        Guard.IsNotEmpty(candidateEventDTOList);


        //Assertions
        Assert.That(() => Mappings.CandidateEventDTOListToSectionList(candidateEventDTOList), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

}