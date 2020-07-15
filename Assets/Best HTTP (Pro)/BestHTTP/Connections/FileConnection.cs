using System;
using System.Collections.Generic;

using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.FileSystem;

namespace BestHTTP
{
    internal sealed class FileConnection : ConnectionBase
    {
        public FileConnection(string serverAddress)
            :base(serverAddress)
        { }

        internal override void Abort(HTTPConnectionStates newState)
        {
            State = newState;

            switch (State)
            {
                case HTTPConnectionStates.TimedOut: TimedOutStart = DateTime.UtcNow; break;
            }

            throw new NotImplementedException();
        }

        protected override void ThreadFunc()
        {
            try
            {
                // Step 1 : create a stream with header information
                // Step 2 : create a stream from the file
                // Step 3 : create a StreamList
                // Step 4 : create a HTTPResponse object
                // Step 5 : call the Receive function of the response object

                using (System.IO.Stream fs = HTTPManager.IOService.CreateFileStream(this.CurrentRequest.CurrentUri.LocalPath, FileStreamModes.Open))
                //using (Stream fs = AndroidFileHelper.GetAPKFileStream(this.CurrentRequest.CurrentUri.LocalPath))
                    using (StreamList stream = new StreamList(new BufferPoolMemoryStream(), fs))
                    {
                        // This will write to the MemoryStream
                        stream.Write("HTTP/1.1 200 Ok\r\n");
                        stream.Write("Content-Type: application/octet-stream\r\n");
                        stream.Write("Content-Length: " + fs.Length.ToString() + "\r\n");
                        stream.Write("\r\n");

                        stream.Seek(0, System.IO.SeekOrigin.Begin);

                        base.CurrentRequest.Response = new HTTPResponse(base.CurrentRequest, stream, base.CurrentRequest.UseStreaming, false);

                        if (!CurrentRequest.Response.Receive())
                            CurrentRequest.Response = null;
                    }
            }
            catch(Exception ex)
            {
                if (CurrentRequest != null)
                {
                    // Something gone bad, Response must be null!
                    CurrentRequest.Response = null;

                    switch (State)
                    {
                        case HTTPConnectionStates.AbortRequested:
                            CurrentRequest.State = HTTPRequestStates.Aborted;
                            break;
                        case HTTPConnectionStates.TimedOut:
                            CurrentRequest.State = HTTPRequestStates.TimedOut;
                            break;
                        default:
                            CurrentRequest.Exception = ex;
                            CurrentRequest.State = HTTPRequestStates.Error;
                            break;
                    }
                }
            }
            finally
            {
                State = HTTPConnectionStates.Closed;
                if (CurrentRequest.State == HTTPRequestStates.Processing)
                {
                    if (CurrentRequest.Response != null)
                        CurrentRequest.State = HTTPRequestStates.Finished;
                    else
                        CurrentRequest.State = HTTPRequestStates.Error;
                }
            }
        }
    }
}