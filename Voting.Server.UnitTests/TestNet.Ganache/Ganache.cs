using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.UnitTests.TestNet.Ganache;

public class Ganache : IGanache
{
    public IGanacheOptions Options { get; private set; }
    public AccountManager? AccountManager { get; private set; }
    public IContainer Container { get; private set; }

    public Ganache()
    {
        Options = new GanacheOptions();
        AccountManager = null;
        Container = default!;
    }

    public async Task<string> Start(IGanacheOptions opts, AccountManager accountManager)
    {
        Options = opts;
        AccountManager = accountManager;
        Container = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage(@"trufflesuite/ganache")
            .WithExposedPort(Options.GanacheSetupOptions.Port)
            .WithPortBinding(Options.GanacheSetupOptions.Port, true)
            .WithCommand(GetExecutionString())
            // .WithEnvironment("NODE_ENV", "production")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilMessageIsLogged(new Regex(@"RPC Listening on .*",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)))
            .Build();

        await Container.StartAsync();

        UriBuilder URL = new UriBuilder(
            "http", 
            Container.Hostname, 
            Container.GetMappedPublicPort(Options.GanacheSetupOptions.Port));
        return URL.ToString();
    }

    public async Task Stop()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }

    private string[] GetExecutionString()
    {
        List<string> args = new();
        args.AddRange(new[]{"--server.port", $"{Options.GanacheSetupOptions.Port}"});
        args.AddRange(new[] { "--miner.blockTime", $"{Options.GanacheSetupOptions.BlockTime}" });
        args.AddRange(new[] { "--miner.defaultGasPrice", $"{Options.GanacheSetupOptions.DefaultGasPrice}" });
        args.AddRange(new[] { "--miner.blockGasLimit", $"{Options.GanacheSetupOptions.BlockGasLimit}" });
        args.AddRange(new[]
            { "--miner.defaultTransactionGasLimit", $"{Options.GanacheSetupOptions.DefaultTransactionGasLimit}" });
        args.AddRange(new[]
            { "--wallet.accounts", $"{AccountManager?.Accounts.First().PrivateKey + ",0x3635C9ADC5DEA00000"}" });
        args.AddRange(new[] { "--miner.instamine", $"eager" });
        args.AddRange(new[] { "--chain.hardfork", $"\"berlin\"" });
        // args.AddRange(new[]{"--wallet.totalAccounts", $"{Options.GanacheSetupOptions.TotalAccounts}"});
        // args.AddRange(new[] { "--wallet.accountKeysPath", $"{Options.GanacheSetupOptions.AccountKeysPath}" });
        // args.AddRange(new[]{"--chain.chainId", $"{Options.GanacheSetupOptions.ChainID}"});
        // args.AddRange(new[]{"--server.host", $"{Options.GanacheSetupOptions.Host}"});

        return args.ToArray();
    }
}