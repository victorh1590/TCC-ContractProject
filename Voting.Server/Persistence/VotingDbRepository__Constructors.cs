using Nethereum.Web3;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence.Clients;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository : IVotingDbRepository
{
    private readonly IWeb3ClientsManager _clients;
    public IWeb3 Web3 { get; private set; }
    
    public VotingDbRepository(IWeb3ClientsManager clients)
    {
        _clients = clients;
        Web3 = clients.Web3Clients.First();
        Guard.IsNotNull(Web3);
    }

    public void ChangeClient(int clientNumber = 0)
    {
        Guard.IsLessThan(clientNumber, _clients.Web3Clients.Count);
        Web3 = _clients.Web3Clients.ItemRef(clientNumber);
    }
}