using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using System.Runtime.CompilerServices;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository : IVotingDbRepository
{
    private readonly IWeb3ClientsManager _clients;
    public IWeb3 Web3 { get; private set; }

    // public ContractHandler ContractHandler { get; }

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

    // public VotingDbRepository(Web3 web3, string contractAddress)
    // {
    //     Web3 = web3;
    //     ContractHandler = web3.Eth.GetContractHandler(contractAddress);
    // }
    //
    // public VotingDbRepository(IWeb3 web3, string contractAddress)
    // {
    //     Web3 = web3;
    //     ContractHandler = web3.Eth.GetContractHandler(contractAddress);
    // }
}