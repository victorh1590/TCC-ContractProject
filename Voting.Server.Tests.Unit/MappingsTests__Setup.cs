using Voting.Server.Tests.Utils;

namespace Voting.Server.Tests.Unit;

[TestFixture]
public partial class MappingsTests
{
    private SeedDataBuilder SeedDataBuilder { get; set; } = default!;

    [SetUp]
    public void SetUp()
    {
        SeedDataBuilder = new SeedDataBuilder();
    }
}