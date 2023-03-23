using System.Collections.Immutable;
using Nethereum.Web3.Accounts;

namespace Voting.Server.Persistence.Accounts;

public interface IAccountManager
{ 
    ImmutableList<Account> Accounts { get; }
}