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

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
        
        //Generate seed data.
        _seedData = _seedDataBuilder.GenerateNew(10U, 5U);
        List<SectionEventDTO> sectionEventDTOs = new List<SectionEventDTO>();
        foreach (var section in _seedData.Sections)
        {
            SectionEventDTO dto = new SectionEventDTO()
            {
                ContractAddress = "",
                Section = section.SectionID,
                Candidates = section.CandidateVotes.Select(x => x.Candidate).ToList(),
                Votes = section.CandidateVotes.Select(x => x.Votes).ToList()
            };
            sectionEventDTOs.Add(dto);
        }

        //Mock Repository
        _mockRepository = new Mock<IVotingDbRepository>();
        _mockRepository
            .Setup(repo=> repo.ReadSectionAsync(
                It.IsIn<uint>(_seedData.Deployment.Sections), It.IsAny<FilterRange?>()))
            .Returns<uint, FilterRange>((sectionNum, _ ) => 
                Task.FromResult(sectionEventDTOs.FirstOrDefault(x => x.Section == sectionNum)));
        
        //Instantiate DomainService
        _domainService = new DomainService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _seedData = default!;
        _mockRepository = default!;
        _domainService = default!;
    }
}