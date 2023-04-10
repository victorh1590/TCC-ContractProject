using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence;
using Voting.Server.Tests.Integration;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[UseBlockchainAndRepository]
interface IUseBlockchainAndRepositoryProps
{
    IGanache TestNet { get; set; }
    IConfiguration Config { get; set; }
    AccountManager AccountManager { get; set; }
    IWeb3ClientsManager ClientsManager { get; set; }
    IVotingDbRepository Repository { get; set; }
    string Account { get; set; }
}

