using System.IO.Compression;
using System.Reflection;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Services;
using Voting.Server.Utils;

//GRPC
var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
var config = builder.Configuration
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();
builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddSingleton<IAccountManager, AccountManager>();
builder.Services.AddSingleton<IWeb3ClientsManager, Web3ClientsManager>();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true; // Enabling error details
    options.MaxReceiveMessageSize = 6291456;
    options.MaxSendMessageSize = 6291456;
    options.CompressionProviders = new List<Grpc.Net.Compression.ICompressionProvider>
    {
        new Grpc.Net.Compression.GzipCompressionProvider(CompressionLevel.Optimal) // gzip on grpc
    };
    options.ResponseCompressionAlgorithm = "gzip";
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.Interceptors.Add<ExceptionInterceptor>(); // Register custom ExceptionInterceptor interceptor
});
builder.Services.AddScoped<IVotingDbRepository, VotingDbRepository>();
builder.Services.AddSingleton<ProtoService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddScoped<VotingService>();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<VotingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGet("/protos", (ProtoService protoService) => Results.Ok(protoService.GetAll()));
app.MapGet("/protos/v{version:int}/{protoName}", (ProtoService
    protoService, int version, string protoName) =>
{
    var filePath = protoService.Get(version, protoName);
    return filePath != null ? Results.File(filePath) : Results.NotFound();
});
app.MapGet("/protos/v{version:int}/{protoName}/view", async (ProtoService
    protoService, int version, string protoName) =>
{
    var text = await protoService.ViewAsync(version, protoName);
    return !string.IsNullOrEmpty(text) ? Results.Text(text) : Results.NotFound();
});

app.Run();