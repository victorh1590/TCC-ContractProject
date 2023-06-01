// using Grpc.Core;
// using Grpc.Core.Interceptors;
// using Microsoft.Extensions.Logging;
//
// namespace Voting.Client;
// public static class ExceptionHelpers
// {
//     public static void Handle<TRequest, TResponse>(this Exception exception, ClientInterceptorContext<TRequest, TResponse> context, ILogger<T> logger)
//         where TRequest : class
//         where TResponse : class
//     
//     {
//         switch (exception)
//         {
//             case TimeoutException timeoutException:
//                 HandleTimeoutException(timeoutException, context, logger);
//                 break;
//             case RpcException rpcException:
//                 HandleRpcException(rpcException, context, logger);
//                 break;
//             default:
//                 HandleDefault(exception, context, logger);
//                 break;
//         }
//     }
//
//     private static void HandleTimeoutException<T>(TimeoutException exception, ClientInterceptorContext context, ILogger<T> logger)
//     {   
//         logger.LogError(exception, $"A timeout occurred");
//     }
//     private static void HandleDefault<T>(Exception exception, ClientInterceptorContext<TRequest, TResponse> context, ILogger<T> logger)
//     {
//         logger.LogError(exception, $"An error occurred");
//     }
//     private static void HandleRpcException<T>(RpcException exception, ClientInterceptorContext<TRequest, TResponse> context, ILogger<T> logger)
//     {
//         logger.LogError(exception, $"An internal error occurred");
//     }
// }
