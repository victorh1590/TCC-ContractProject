using System.Diagnostics;
using System.Management;
using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;

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
  private IGanacheOptions Options { get; set; }
  private Process? Proc { get; set; }
  
  public Ganache(IGanacheOptions opts)
  {
    Options = opts;
  }

  public void Start()
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
    StringBuilder sb = new StringBuilder();
    sb.Append($"/K ganache");
    sb.Append($" --server.host={Options.Host}");
    sb.Append($" --server.port={Options.Port}");
    sb.Append($" --chain.chainId={Options.ChainID}");
    sb.Append($" --miner.blockTime={Options.BlockTime}");
    sb.Append($" --miner.defaultGasPrice={Options.DefaultGasPrice}");
    sb.Append($" --miner.blockGasLimit={Options.BlockGasLimit}");
    sb.Append($" --miner.defaultTransactionGasLimit={Options.DefaultTransactionGasLimit}");
    sb.Append($" --wallet.accountKeysPath={Options.AccountKeysPath}");
    sb.Append($" --chain.hardfork=\"berlin\"");
    // sb.Append($" --wallet.accounts={PrivateKey},{InitialBalance}");
    
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = "cmd.exe";
    startInfo.UseShellExecute = true;
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    startInfo.Arguments = sb.ToString();
    Proc = Process.Start(startInfo);
    if(Proc == null)
    {
      throw new SystemException("Process failed to start.");
    }
  }

  public void Stop()
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
    if(Proc == null) return;
    KillProcessTree(Proc.Id);
    Proc = null;
    try
    {
      File.Delete(Path.Join(Directory.GetCurrentDirectory(), Options.AccountKeysPath));
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      Console.WriteLine("Accounts File not removed.");
    }
  }

  private void KillProcessTree(int pid)
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
#pragma warning disable CA1416 // Guard.IsTrue(OperatingSystem.IsWindows()) ensures it's running on Windows.
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
#pragma warning restore CA1416
}