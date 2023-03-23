using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using Nethereum.Web3;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.Persistence.Clients;

public class Web3ClientsManager : IWeb3ClientsManager
{
    public ImmutableList<Web3> Web3Clients { get; }

    public Web3ClientsManager(IAccountManager accounts)
    {
        Web3Clients = accounts.Accounts
            .Select(account => new Web3(account))
            .ToImmutableList();
        
        Web3Clients.ForEach(web3 => web3.TransactionManager.UseLegacyAsDefault = true);

        Guard.IsNotNull(Web3Clients);
    }
}