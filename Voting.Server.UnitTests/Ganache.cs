using System.Diagnostics;
using System.Management;

namespace Voting.Server.UnitTests;

// call
// Ganache blockchain = new("127.0.0.1", 8545, 56666, 0, "0x0", "0x87369400", "0x87369400");
// blockchain.Start();
//
// Console.WriteLine("Press Any key to stop...");
// Console.ReadKey();
// blockchain.Stop();

public class Ganache 
{
  public string Host { get; set; } = default!;
  public int Port { get; set; } = 8545;
  public int ChainID { get; set; } = 56666;
  public int BlockTime { get; set; } = 0;
  public string DefaultGasPrice { get; set; } = "0x0";
  public string BlockGasLimit { get; set; } = "0x87369400";
  public string DefaultTransactionGasLimit { get; set; } = "0x87369400";
  public Process? Proc { get; set; } = null;
  public string PrivateKey { get; set; }
  public string InitialBalance { get; set; }
  
  public Ganache()
  {
  }

  public Ganache(string host, int port, int chainID, int blockTime, string defaultGasPrice, string blockGasLimit, string defaultTransactionGasLimit, string privateKey, string initialBalance)
  {
    Host = host;
    Port = port;
    ChainID = chainID;
    BlockTime = blockTime;
    DefaultGasPrice = defaultGasPrice;
    BlockGasLimit = blockGasLimit;
    DefaultTransactionGasLimit = defaultTransactionGasLimit;
    PrivateKey = privateKey;
    InitialBalance = initialBalance;
  }

  public void Start()
  {
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = "cmd.exe";
    startInfo.UseShellExecute = true;
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    startInfo.Arguments = $"/K ganache --server.host={Host} --server.port={Port} --chain.chainId={ChainID} --miner.blockTime={BlockTime} --miner.defaultGasPrice={DefaultGasPrice} --miner.blockGasLimit={BlockGasLimit} --miner.defaultTransactionGasLimit={DefaultTransactionGasLimit} --wallet.accounts={PrivateKey},{InitialBalance} --chain.hardfork=\"berlin\"";
    // startInfo.Arguments = $"/K ganache --server.host={Host} --server.port={Port} --chain.chainId={ChainID} --miner.blockTime={BlockTime} --miner.defaultGasPrice={DefaultGasPrice} --wallet.totalAccounts=0";
    Proc = Process.Start(startInfo);
    if(Proc == null)
    {
      throw new SystemException("Process failed to start.");
    }
  }

  public void Stop()
  {
    if(Proc == null) return;
    KillProcessTree(Proc.Id);
    Proc = null;
  }

  private void KillProcessTree(int pid)
  {
    if (pid == 0) return;
    ManagementObjectSearcher lookup = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ParentProcessID=" + pid);
    ManagementObjectCollection procList = lookup.Get();
    foreach (ManagementBaseObject proc in procList)
    {
        KillProcessTree(Convert.ToInt32(proc["ProcessID"]));
    }
    try
    {
        Process.GetProcessById(pid).Kill();
    }
    catch (ArgumentException)
    {
    }
  }
}