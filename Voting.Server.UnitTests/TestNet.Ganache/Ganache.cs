using System.Diagnostics;
using System.Management;
using System.Text;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

public class Ganache : IGanache
{
  public IGanacheOptions Options { get; private set; }
  public AccountManager? AccountManager { get; private set;  }
  private Process? Proc { get; set; }

  public Ganache()
  {
    Options = new GanacheOptions();
    AccountManager = null;
    Proc = null;
  }

  public void Start(IGanacheOptions opts, AccountManager accountManager)
  {
    Guard.IsTrue(OperatingSystem.IsWindows());

    Options = opts;
    AccountManager = accountManager;

    ProcessStartInfo startInfo = new ProcessStartInfo
    {
      FileName = Options.GanacheEnvironmentOptions.Terminal,
      UseShellExecute = true,
      WindowStyle = ProcessWindowStyle.Normal,
      Arguments = GetExecutionString()
    };
    Proc = Process.Start(startInfo) ?? throw new SystemException("Process failed to start.");
  }

  public void Stop()
  {
    Guard.IsTrue(OperatingSystem.IsWindows());
    // TestContext.WriteLine("Press enter to stop...");
    // TestContext.ReadLine();
    if(Proc == null) return;
    KillProcessTree(Proc.Id);
    Proc = null;
    try
    {
      File.Delete(Path.Join(Directory.GetCurrentDirectory(), Options.GanacheSetupOptions.AccountKeysPath));
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
    sb.Append($" --server.host={Options.GanacheSetupOptions.Host}");
    sb.Append($" --server.port={Options.GanacheSetupOptions.Port}");
    sb.Append($" --chain.chainId={Options.GanacheSetupOptions.ChainID}");
    sb.Append($" --miner.blockTime={Options.GanacheSetupOptions.BlockTime}");
    sb.Append($" --miner.defaultGasPrice={Options.GanacheSetupOptions.DefaultGasPrice}");
    sb.Append($" --miner.blockGasLimit={Options.GanacheSetupOptions.BlockGasLimit}");
    sb.Append($" --miner.defaultTransactionGasLimit={Options.GanacheSetupOptions.DefaultTransactionGasLimit}");
    sb.Append($" --wallet.accounts={AccountManager?.Accounts.First().PrivateKey + ",0x3635C9ADC5DEA00000"}");
    sb.Append($" --wallet.accountKeysPath={Options.GanacheSetupOptions.AccountKeysPath}");
    sb.Append($" --miner.instamine=eager");
    // sb.Append($" --wallet.totalAccounts={Options.GanacheOptions.TotalAccounts}");
    sb.Append($" --chain.hardfork=\"berlin\"");
    return sb.ToString();
  }
}