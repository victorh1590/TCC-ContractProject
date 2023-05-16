using System.IO.Compression;
using Voting.Server.Domain;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Services;

//GRPC
var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true; // Enabling error details
    options.MaxReceiveMessageSize = 6291456;
    options.MaxSendMessageSize = 6291456;
    options.CompressionProviders = new List<Grpc.Net.Compression.ICompressionProvider>
    {
        new Grpc.Net.Compression.GzipCompressionProvider(CompressionLevel.Optimal) // gzip
    };
    options.ResponseCompressionAlgorithm = "br";
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.Interceptors.Add<ExceptionInterceptor>(); // Register custom ExceptionInterceptor interceptor
});
builder.Services.AddSingleton<ProtoService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<IAccountManager, AccountManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<VotingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//TODO configurar endpoints para o cliente poder acessar o arquivo protobuf.
app.MapGet("/protos", (ProtoService protoService) =>
{
    return Results.Ok(protoService.GetAll());
});
app.MapGet("/protos/v{version:int}/{protoName}", (ProtoService
    protoService, int version, string protoName) =>
{
    var filePath = protoService.Get(version, protoName);
    if (filePath != null)
        return Results.File(filePath);
    return Results.NotFound();
});
app.MapGet("/protos/v{version:int}/{protoName}/view", async (ProtoService
    protoService, int version, string protoName) =>
{
    var text = await protoService.ViewAsync(version, protoName);
    if (!string.IsNullOrEmpty(text))
        return Results.Text(text);
    return Results.NotFound();
});

app.Run();

//Rest
//
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
//
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<IAccountManager, AccountManager>();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();
