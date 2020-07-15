#if CSHARP_7_OR_LATER
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BestHTTP
{
    public static class HTTPRequestAsyncExtensions
    {
        public static Task<HTTPResponse> GetHTTPResponseAsync(this HTTPRequest request, CancellationToken token = default)
        {
            return CreateTask<HTTPResponse>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        tcs.TrySetResult(resp);
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        VerboseLogging(request, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                        tcs.TrySetException(req.Exception ?? new Exception("No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        VerboseLogging(request, "Request Aborted!");

                        tcs.TrySetCanceled();
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        VerboseLogging(request, "Connection Timed Out!");

                        tcs.TrySetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        VerboseLogging(request, "Processing the request Timed Out!");

                        tcs.TrySetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
        }

        public static Task<string> GetAsStringAsync(this HTTPRequest request, CancellationToken token = default)
        {
            return CreateTask<string>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                            tcs.TrySetResult(resp.DataAsText);
                        else
                            tcs.TrySetException(new Exception(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText)));
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        VerboseLogging(request, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                        tcs.TrySetException(req.Exception ?? new Exception("No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        VerboseLogging(request, "Request Aborted!");

                        tcs.TrySetCanceled();
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        VerboseLogging(request, "Connection Timed Out!");

                        tcs.TrySetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        VerboseLogging(request, "Processing the request Timed Out!");

                        tcs.TrySetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
        }
        
        public static Task<Texture2D> GetAsTexture2DAsync(this HTTPRequest request, CancellationToken token = default)
        {
            return CreateTask<Texture2D>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                            tcs.TrySetResult(resp.DataAsTexture2D);
                        else
                            tcs.TrySetException(new Exception(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText)));
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        VerboseLogging(request, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                        tcs.TrySetException(req.Exception ?? new Exception("No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        VerboseLogging(request, "Request Aborted!");

                        tcs.TrySetCanceled();
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        VerboseLogging(request, "Connection Timed Out!");

                        tcs.TrySetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        VerboseLogging(request, "Processing the request Timed Out!");

                        tcs.TrySetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
        }

        public static Task<byte[]> GetRawDataAsync(this HTTPRequest request, CancellationToken token =  default)
        {
            return CreateTask<byte[]>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                            tcs.TrySetResult(resp.Data);
                        else
                            tcs.TrySetException(new Exception(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText)));
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        VerboseLogging(request, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                        tcs.TrySetException(req.Exception ?? new Exception("No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        VerboseLogging(request, "Request Aborted!");

                        tcs.TrySetCanceled();
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        VerboseLogging(request, "Connection Timed Out!");

                        tcs.TrySetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        VerboseLogging(request, "Processing the request Timed Out!");

                        tcs.TrySetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task<T> CreateTask<T>(HTTPRequest request, CancellationToken token, Action<HTTPRequest, HTTPResponse, TaskCompletionSource<T>> callback)
        {
            var tcs = new TaskCompletionSource<T>();

            request.Callback = (req, resp) => callback(req, resp, tcs);

            if (token.CanBeCanceled)
                token.Register((state) => (state as HTTPRequest)?.Abort(), request);

            if (request.State == HTTPRequestStates.Initial)
                request.Send();

            return tcs.Task;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void VerboseLogging(HTTPRequest request, string str)
        {
            HTTPManager.Logger.Verbose("HTTPRequestAsyncExtensions", "'" + request.CurrentUri.ToString() + "' - " + str);
        }
    }
}
#endif