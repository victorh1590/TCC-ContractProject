using System.Collections.Immutable;
using Nethereum.Web3.Accounts;

namespace Voting.Server.Persistence.Accounts;

internal interface IAccountManager
{ 
    ImmutableList<string> PublicKeys { get; }
}