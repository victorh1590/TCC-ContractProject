using System.Collections.Immutable;
using Nethereum.Web3;

namespace Voting.Server.Persistence.Clients;

internal interface IWeb3ClientsManager
{
    ImmutableList<Web3> Web3Clients { get; }
}