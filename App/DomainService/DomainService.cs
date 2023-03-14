﻿using App.VotingDb;
using App.VotingDb.ContractDefinition;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Math;

namespace App;

public class DomainService
{
    private string URL { get; }
    private int ChainID { get; }
    private string PrivateKey { get; }

    public DomainService()
    {
        URL = "HTTP://localhost:7545";
        ChainID = 5777;
        PrivateKey = "d1d45d0629c5a5fe5ac563c89d759ca057cb7e2936f6c78351bb93bb2f58eb99";
    }

    private Web3 GetWeb3Client()
    {
        Account account = new Account(PrivateKey, 5777);
        return new Web3(account, URL);
    }

    public async Task<string> DeployContract(VotingDbDeployment votingDbDeployment)
    {
        Web3 web3 = GetWeb3Client();
        web3.TransactionManager.UseLegacyAsDefault = true; 
        TransactionReceipt transactionReceipt = await web3.Eth
            .GetContractDeploymentHandler<VotingDbDeployment>()
            .SendRequestAndWaitForReceiptAsync(votingDbDeployment);
        return transactionReceipt.ContractAddress;
    }

    private VotingDbService GetVotingDbService(string contractAdress)
    {
        Web3 web3 = GetWeb3Client();
        return new VotingDbService(web3, contractAdress);
    }

    public async Task GetSectionAsync()
    {
        Web3 web3 = GetWeb3Client();
        Event<HeartbeatEventDTO> heartbeatEventHandler = web3.Eth.GetEvent<HeartbeatEventDTO>();
        HexBigInteger? heartbeatEventFilter = await heartbeatEventHandler
            .CreateFilterAsync<string?, BigInteger?, bool?>(null, null, true);
        if (heartbeatEventFilter == null) throw new Exception();
        var logs = await heartbeatEventHandler.GetFilterChangesAsync(heartbeatEventFilter);
        Console.WriteLine(logs);
        Console.WriteLine(logs.Count);
        foreach (var log in logs)
        {
            Console.WriteLine(log.Log.IndexedVal1);
            Console.WriteLine(log.Event.VotesJSON);
        }
    }
}