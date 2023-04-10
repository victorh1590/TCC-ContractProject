using Voting.Server.Tests.Utils.TestData;

namespace Voting.Server.Tests.Unit;

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