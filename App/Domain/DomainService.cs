using App.Persistence;
using App.Persistence.ContractDefinition;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace App.Domain;

internal class DomainService
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
        Event<SectionEventDTO> sectionEventHandler = web3.Eth.GetEvent<SectionEventDTO>();
        // HexBigInteger sectionEventFilter = await sectionEventHandler.CreateFilterAsync(10U);
        // Console.WriteLine(sectionEventFilter);
        // if (sectionEventFilter == null) throw new Exception();
        // var logs = await sectionEventHandler.GetFilterChangesAsync(sectionEventFilter);
        var sectionEventFilter = sectionEventHandler.CreateFilterInput(10U, BlockParameter.CreateLatest(), BlockParameter.CreateLatest());
        var logs = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        Console.WriteLine(logs);
        Console.WriteLine(logs.Count);
        foreach (var log in logs)
        {
            Console.WriteLine(log.Event.Section);
            Console.WriteLine(log.Event.Block);
            Console.WriteLine(log.Event.ContractAddress);
            Console.WriteLine(log.Event.SectionJSON);
        }
    }
}