using System.Diagnostics;
using System.Management;
using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests;

// call
// Ganache blockchain = new("127.0.0.1", 8545, 56666, 0, "0x0", "0x87369400", "0x87369400");
// blockchain.Start();
//
// Console.WriteLine("Press Any key to stop...");
// Console.ReadKey();
// blockchain.Stop();

internal class Ganache 
{
  private IGanacheOptions Options { get; }
  private IConfiguration Config { get; }
  private AccountManager? AccountManager { get; }
  private Process? Proc { get; set; }
  
  internal Ganache(IGanacheOptions opts, IConfiguration config, AccountManager accountManager)
  {
    Options = opts;
    Config = config;
    AccountManager = accountManager;
    Guard.IsNotNull(AccountManager);
  }

  internal void Start()
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
    sb.Append($" --wallet.accounts={AccountManager?.Accounts.First().PrivateKey + ",0x3635C9ADC5DEA00000"}");
    sb.Append($" --wallet.accountKeysPath={Options.AccountKeysPath}");
    // sb.Append($" --wallet.totalAccounts={Options.TotalAccounts}");
    sb.Append($" --chain.hardfork=\"berlin\"");
    // sb.Append($" --wallet.accounts={PrivateKey},{InitialBalance}");
    
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = "cmd.exe";
    startInfo.UseShellExecute = true;
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    startInfo.Arguments = sb.ToString();
    Proc = Process.Start(startInfo) ?? throw new SystemException("Process failed to start.");
  }

  internal void Stop()
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
    if(Proc == null) return;
    KillProcessTree(Proc.Id);
    Proc = null;
    try
    {
      File.Delete(Path.Join(Directory.GetCurrentDirectory(), Options.AccountKeysPath));
    }
    catch (Exception err) 
      when (err is ArgumentException 
                or ArgumentNullException 
                or DirectoryNotFoundException 
                or IOException 
                or NotSupportedException 
                or PathTooLongException 
                or UnauthorizedAccessException)
    {
      Console.WriteLine("Failed to remove Accounts file.");
      throw;
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
    catch (Exception err) 
      when (err is InvalidOperationException or ArgumentException)
    {
      Console.WriteLine("Failed to stop process tree.");
      throw;
    }
  }
#pragma warning restore CA1416
}