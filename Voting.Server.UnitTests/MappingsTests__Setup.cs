using Voting.Server.UnitTests.TestData;

namespace Voting.Server.UnitTests;

public partial class MappingsTests
{
    private SeedDataBuilder _seedDataBuilder { get; set; } = default!;

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
    }
}