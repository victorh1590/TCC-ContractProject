using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Nethereum.Web3;
using Voting.Server.Persistence.Accounts;

[assembly: InternalsVisibleTo("Voting.Server.Tests.Unit")]
[assembly: InternalsVisibleTo("Voting.Server.Tests.Integration")]
namespace Voting.Server.Persistence.Clients;

public class Web3ClientsManager : IWeb3ClientsManager
{
    public ImmutableList<Web3> Web3Clients { get; }

    public Web3ClientsManager(AccountManager accounts, IConfiguration configuration)
    {
        Guard.IsTrue(Uri.IsWellFormedUriString(configuration.GetSection("URL").Value ?? null, UriKind.Absolute));
        Web3Clients = accounts.Accounts
            .Select(account => new Web3(account, configuration.GetSection("URL").Value))
            .ToImmutableList();
        
        Web3Clients.ForEach(web3 => web3.TransactionManager.UseLegacyAsDefault = true);

        Guard.IsNotNull(Web3Clients);
    }
}