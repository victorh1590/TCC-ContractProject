using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using static Voting.Server.Protos.v1.VotingService;

namespace Voting.Client;

public static class GrpcClientBuilder
{
    private static readonly GrpcChannel _channel;
    static GrpcClientBuilder()
    {
        _channel = GrpcChannel.ForAddress("http://localhost:5205");
    }

    public static VotingServiceClient CreateClient()
    {
        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Trace);
        });
        var logger = loggerFactory.CreateLogger<TracerInterceptor>();
        return new VotingServiceClient(_channel.Intercept(new TracerInterceptor(logger)));
    }
    
    public static async void Dispose()
    {
        _channel.Dispose();
        await _channel.ShutdownAsync();
    }
}