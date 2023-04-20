using Moq;
using Voting.Server.Domain;
using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Utils;

namespace Voting.Server.Tests.Unit;

[TestFixture]
public partial class DomainServiceTests
{
    private SeedDataBuilder _seedDataBuilder = new();
    private SeedData _seedData = default!;
    private Mock<IVotingDbRepository> _mockRepository = default!;
    private DomainService _domainService = default!;
    private List<SectionEventDTO> _sectionEventDTOs = default!;
    private List<CandidateEventDTO> _candidateEventDTOs = default!;

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
        
        //Generate seed data.
        _seedData = _seedDataBuilder.GenerateNew(10U, 5U);
        
        //Generate sectionEventDTOs
        _sectionEventDTOs = new List<SectionEventDTO>();
        foreach (var section in _seedData.Sections)
        {
            SectionEventDTO dto = new SectionEventDTO
            {
                ContractAddress = "",
                Section = section.SectionID,
                Candidates = section.CandidateVotes.Select(cv => cv.Candidate).ToList(),
                Votes = section.CandidateVotes.Select(cv => cv.Votes).ToList()
            };
            _sectionEventDTOs.Add(dto);
        }

        //Generate candidateEventDTOs
        _candidateEventDTOs = new List<CandidateEventDTO>();
        foreach (var section in _seedData.Sections)
        {
            _candidateEventDTOs.AddRange(section.CandidateVotes.Select(cv => 
                new CandidateEventDTO
                {
                    Candidate = cv.Candidate, 
                    Votes = cv.Votes, 
                    Section = section.SectionID, 
                    ContractAddress = ""
                })
            );
        }

        //Mock Repository
        _mockRepository = new Mock<IVotingDbRepository>();
        _mockRepository.Setup(repo=> repo.ReadSectionAsync(
                It.IsIn<uint>(_seedData.Deployment.Sections), It.IsAny<FilterRange?>()))
            .Returns<uint, FilterRange>((sectionNum, _ ) => 
                Task.FromResult(_sectionEventDTOs.FirstOrDefault(dto => dto.Section == sectionNum)));
        _mockRepository.Setup(repo => repo.ReadVotesByCandidateAndSectionAsync(
                It.IsIn<uint>(_seedData.Deployment.Candidates), 
                It.IsIn<uint>(_seedData.Deployment.Sections), 
                It.IsAny<FilterRange?>()))
            .Returns<uint, uint, FilterRange>((candidateNum, sectionNum, _) =>
                Task.FromResult(_candidateEventDTOs.FirstOrDefault(dto => 
                    dto.Section == sectionNum && dto.Candidate == candidateNum)));
        _mockRepository.Setup(repo => repo.ReadVotesByCandidateAsync(
                It.IsIn<uint>(_seedData.Deployment.Candidates), 
                It.IsAny<FilterRange?>()))
            .Returns<uint, FilterRange>((candidateNum, _) =>
                Task.FromResult(_candidateEventDTOs.Where(dto => dto.Candidate == candidateNum).ToList()));
        //Instantiate DomainService
        _domainService = new DomainService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _seedData = default!;
        _mockRepository = default!;
        _domainService = default!;
        _sectionEventDTOs = default!;
        _candidateEventDTOs = default!;
    }
}