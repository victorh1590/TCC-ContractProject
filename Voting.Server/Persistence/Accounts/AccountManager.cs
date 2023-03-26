using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Nethereum.HdWallet;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

[assembly: InternalsVisibleTo("Voting.Server.UnitTests")]
namespace Voting.Server.Persistence.Accounts;

internal class AccountManager : IAccountManager
{
    internal ImmutableList<Account> Accounts { get; }
    public ImmutableList<string> PublicKeys { get; }
    
    internal AccountManager(IConfiguration config)
    {
        Guard.IsNotNull(config.GetSection("Accounts"));

        Wallet wallet = new Wallet(config.GetSection("Mnemonics").Value, config.GetSection("Password").Value);

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