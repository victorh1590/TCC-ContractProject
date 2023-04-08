using Voting.Server.UnitTests.TestData;

namespace Voting.Server.UnitTests;

[Ignore("Debugging")]
[TestFixture]
public partial class MappingsTests
{
    private SeedDataBuilder _seedDataBuilder { get; set; } = default!;

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
    }
}