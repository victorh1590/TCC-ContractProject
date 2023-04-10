using System.Reflection;
using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence.Accounts;

namespace Voting.Server.Tests.Integration.TestNet.Ganache;

public class Ganache : IGanache
{
    public IGanacheOptions Options { get; private set; }
    public AccountManager? AccountManager { get; private set; }
    public IContainer Container { get; private set; }

    public Ganache(AccountManager accountManager)
    {
        AccountManager = accountManager;
        Options = new GanacheOptions();
        Container = default!;

        string? testProjectDirectory = Assembly.GetExecutingAssembly().Location
            .Split("\\bin\\Debug\\")
            .FirstOrDefault();
        Guard.IsNotNullOrEmpty(testProjectDirectory);
        Guard.IsTrue(Path.Exists(testProjectDirectory));
        testProjectDirectory = Path.Join(testProjectDirectory, "TestNet.Ganache");
        testProjectDirectory = Path.GetFullPath(testProjectDirectory); //Will throw exception if path is invalid.
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(testProjectDirectory)
            .AddJsonFile(Path.Join(".", "testnet_ganache_options.json"))
            .Build();
        configuration.Bind(Options);
    }

    public async Task<string> Start()
    {
        Container = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage(@"trufflesuite/ganache")
            .WithExposedPort(Options.GanacheSetupOptions.Port)
            .WithPortBinding(Options.GanacheSetupOptions.Port, true)
            .WithCommand(GetExecutionString())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged(new Regex(@"RPC Listening on .*", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
            .Build();

        await Container.StartAsync();

        return 
            new UriBuilder("http", Container.Hostname, Container.GetMappedPublicPort(Options.GanacheSetupOptions.Port))
            .ToString();
    }

    public async Task Stop()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }

    private string[] GetExecutionString()
    {
        List<string> args = new();
        args.AddRange(new[] { "--server.port", $"{Options.GanacheSetupOptions.Port}"});
        args.AddRange(new[] { "--miner.blockTime", $"{Options.GanacheSetupOptions.BlockTime}" });
        args.AddRange(new[] { "--miner.defaultGasPrice", $"{Options.GanacheSetupOptions.DefaultGasPrice}" });
        args.AddRange(new[] { "--miner.blockGasLimit", $"{Options.GanacheSetupOptions.BlockGasLimit}" });
        args.AddRange(new[] { "--miner.defaultTransactionGasLimit", $"{Options.GanacheSetupOptions.DefaultTransactionGasLimit}" });
        args.AddRange(new[] { "--wallet.accounts", $"{AccountManager?.Accounts.First().PrivateKey + ",0x3635C9ADC5DEA00000"}" });
        args.AddRange(new[] { "--miner.instamine", "eager" });
        args.AddRange(new[] { "--chain.hardfork", "\"berlin\"" });
        // args.AddRange(new[]{"--wallet.totalAccounts", $"{Options.GanacheSetupOptions.TotalAccounts}"});
        // args.AddRange(new[] { "--wallet.accountKeysPath", $"{Options.GanacheSetupOptions.AccountKeysPath}" });
        // args.AddRange(new[]{"--chain.chainId", $"{Options.GanacheSetupOptions.ChainID}"});
        // args.AddRange(new[]{"--server.host", $"{Options.GanacheSetupOptions.Host}"});

        return args.ToArray();
    }
}