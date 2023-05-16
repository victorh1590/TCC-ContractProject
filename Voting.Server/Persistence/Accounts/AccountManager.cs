using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Nethereum.HdWallet;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

// [assembly: InternalsVisibleTo("Voting.Server.Tests.Unit")]
// [assembly: InternalsVisibleTo("Voting.Server.Tests.Integration")]
namespace Voting.Server.Persistence.Accounts;

public class AccountManager : IAccountManager
{
    public ImmutableList<Account> Accounts { get; }
    public ImmutableList<string> PublicKeys { get; }
    
    public AccountManager(IConfiguration config)
    {
        var mnemonics = config.GetValue("Mnemonics",string.Empty);
        var password = config.GetValue("Password", string.Empty);
        Wallet wallet = new Wallet(mnemonics, password);

        List<Account> accounts = new();
        List<string> publicKeys = new();
        for(int i = 0; i < 3; i++)
        {
            var account = wallet.GetAccount(i);
            accounts.Add( new Account(account.PrivateKey, Chain.Private));
            publicKeys.Add(account.PublicKey);
        }
        
        accounts.ForEach(acc => Guard.IsNotNull(acc));
        Guard.IsNotEmpty(accounts);
        Guard.IsNotEmpty(publicKeys);
        Guard.IsEqualTo(accounts.Count, publicKeys.Count);
        
        Accounts = accounts.ToImmutableList();
        PublicKeys = publicKeys.ToImmutableList();
    }
}