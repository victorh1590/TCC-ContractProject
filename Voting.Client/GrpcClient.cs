using Grpc.Core;
using Voting.Server.Protos.v1;
using System.Text.Json;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using static Voting.Server.Protos.v1.VotingService;

namespace Voting.Client;

public class GrpcClient : IDisposable
{
    private readonly VotingServiceClient _client;
    private readonly GrpcChannel _channel;

    // public GrpcClientCalls(VotingServiceClient client)
    // {
    //     _client = client;
    // }
    public GrpcClient()
    {
        _channel = GrpcChannel.ForAddress("https://localhost:7090");

        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Trace);
        });
        var logger = loggerFactory.CreateLogger<TracerInterceptor>();
        _client = new VotingServiceClient(_channel.Intercept(new TracerInterceptor(logger)));
    }

    public async void Dispose()
    {
        _channel.Dispose();
        await _channel.ShutdownAsync();
    }

    public async Task GetCandidateVotes(uint candidate = 0)
    {
        using var serverStreamingCall = _client.GetCandidateVotes(new CandidateIdRequest
        {
            Candidate = candidate
        });
        await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
        {
            var json = JsonSerializer.Serialize(response);
            // Console.WriteLine($"{response.SectionID}: {response.CandidateVotes}");
            Console.WriteLine(json);
        }
    }

    public async Task GetSectionVotes(uint section = 0)
    {
        var response = await _client.GetSectionVotesAsync(new SectionIdRequest
        {
            Section = section
        });
        var json = JsonSerializer.Serialize(response);
        Console.WriteLine(json);
    }

    public async Task GetTotalVotesBySection(uint section = 0)
    {
        var response = await _client.GetTotalVotesBySectionAsync(new SectionIdRequest
        {
            Section = section
        });
        var json = JsonSerializer.Serialize(response);
        Console.WriteLine(json);
    }

    public async Task GetTotalVotesByCandidate(uint candidate = 0)
    {
        var response = await _client.GetTotalVotesByCandidateAsync(new CandidateIdRequest
        {
            Candidate = candidate
        });
        var json = JsonSerializer.Serialize(response);
        Console.WriteLine(json);
    }

    public async Task CreateSection(string json)
    {
        // using var clientStreamingCall = _client.CreateSection();
        // // Write
        // List<Section> sectionToInsert = JsonSerializer.Deserialize<List<Section>>(json);
        //
        // foreach (var sectionToInsert in sectionToInsert)
        // {
        //     await clientStreamingCall.RequestStream.WriteAsync(sectionToInsert);
        // }
        //
        // // Tells server that request streaming is done
        // await clientStreamingCall.RequestStream.CompleteAsync();
        // // Finish the call by getting the response
        // var emptyResponse = await clientStreamingCall.ResponseAsync;
    }
}