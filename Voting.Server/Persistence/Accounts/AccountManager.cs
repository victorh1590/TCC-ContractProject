using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

namespace Voting.Server.Persistence.Accounts;

public class AccountManager : IAccountManager
{
    public ImmutableList<Account> Accounts { get; }
    
    public AccountManager(IConfiguration config)
    {
        Guard.IsNotNull(config.GetSection("Accounts"));

        Accounts = config
            .GetSection("Accounts").AsEnumerable()
            .Select(item => new Account(item.Value, Chain.Private))
            .ToImmutableList();
    }
}