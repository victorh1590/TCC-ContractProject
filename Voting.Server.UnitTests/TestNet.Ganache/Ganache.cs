using System.Diagnostics;
using System.Management;
using System.Text;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

// call
// Ganache blockchain = new("127.0.0.1", 8545, 56666, 0, "0x0", "0x87369400", "0x87369400");
// blockchain.Start();
//
// Console.WriteLine("Press Any key to stop...");
// Console.ReadKey();
// blockchain.Stop();

internal class Ganache : IPrivateBlockchain
{
  public ITestNetOptions Options { get; private set; }
  public AccountManager? AccountManager { get; private set;  }
  private Process? Proc { get; set; }

  public Ganache()
  {
    Options = new TestNetOptions();
    AccountManager = null;
    Proc = null;
  }

  public void Start(ITestNetOptions opts, AccountManager accountManager)
  {
    Guard.IsTrue(OperatingSystem.IsWindows());

    Options = opts;
    AccountManager = accountManager;

    ProcessStartInfo startInfo = new ProcessStartInfo
    {
      FileName = Options.TestNetEnvironmentOptions.Terminal,
      UseShellExecute = true,
      WindowStyle = ProcessWindowStyle.Normal,
      Arguments = GetExecutionString()
    };
    Proc = Process.Start(startInfo) ?? throw new SystemException("Process failed to start.");
  }

  public void Stop()
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
    if(Proc == null) return;
    KillProcessTree(Proc.Id);
    Proc = null;
    try
    {
      File.Delete(Path.Join(Directory.GetCurrentDirectory(), Options.GanacheOptions.AccountKeysPath));
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

  public void KillProcessTree(int pid)
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
#pragma warning restore CA1416
  }

  public string GetExecutionString()
  {
    StringBuilder sb = new StringBuilder();
    sb.Append($"/K ganache");
    sb.Append($" --server.host={Options.GanacheOptions.Host}");
    sb.Append($" --server.port={Options.GanacheOptions.Port}");
    sb.Append($" --chain.chainId={Options.GanacheOptions.ChainID}");
    sb.Append($" --miner.blockTime={Options.GanacheOptions.BlockTime}");
    sb.Append($" --miner.defaultGasPrice={Options.GanacheOptions.DefaultGasPrice}");
    sb.Append($" --miner.blockGasLimit={Options.GanacheOptions.BlockGasLimit}");
    sb.Append($" --miner.defaultTransactionGasLimit={Options.GanacheOptions.DefaultTransactionGasLimit}");
    sb.Append($" --wallet.accounts={AccountManager?.Accounts.First().PrivateKey + ",0x3635C9ADC5DEA00000"}");
    sb.Append($" --wallet.accountKeysPath={Options.GanacheOptions.AccountKeysPath}");
    // sb.Append($" --wallet.totalAccounts={Options.GanacheOptions.TotalAccounts}");
    sb.Append($" --chain.hardfork=\"berlin\"");
    return sb.ToString();
  }
}