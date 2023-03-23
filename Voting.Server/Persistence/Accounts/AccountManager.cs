using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

namespace Voting.Server.Persistence.Accounts;

public class AccountManager : IAccountManager
{
    public ImmutableList<Account> Accounts { get; }
    
    public AccountManager(IConfiguration config)
    {
        Guard.IsNotNull(config.GetSection("Accounts"));
        
        var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
        var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();

        Accounts = config
            .GetSection("Accounts").AsEnumerable()
            .Select(item => new Nethereum.Accounts.Account(item.Value, Chain.Private))
            .ToImmutableList();
        
    }
}