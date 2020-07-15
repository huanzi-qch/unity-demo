#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using BestHTTP;
using BestHTTP.Futures;
using BestHTTP.SignalRCore.Messages;

namespace BestHTTP.SignalRCore
{

    public sealed class UploadItemController<TResult> : IFuture<TResult>, IDisposable
    {
        public readonly long invocationId;
        public readonly int[] streamingIds;
        public readonly HubConnection hubConnection;
        public readonly Futures.IFuture<TResult> future;

        public FutureState state { get { return this.future.state; } }
        public TResult value { get { return this.future.value; } }
        public Exception error { get { return this.future.error; } }

        private object[] streams;
        private bool isFinished;

        public UploadItemController(HubConnection hub, long iId, int[] sIds, IFuture<TResult> future)
        {
            this.hubConnection = hub;
            this.invocationId = iId;
            this.streamingIds = sIds;
            this.streams = new object[this.streamingIds.Length];
            this.future = future;
        }

        public UploadChannel<TResult, T> GetUploadChannel<T>(int paramIdx)
        {
            var stream = this.streams[paramIdx] as UploadChannel<TResult, T>;
            if (stream == null)
                this.streams[paramIdx] = stream = new UploadChannel<TResult, T>(this, paramIdx);

            return stream;
        }

        public void UploadParam<T>(int streamId, T item)
        {
            if (streamId == 0)
                return;

            var message = new Message
            {
                type = MessageTypes.StreamItem,
                invocationId = streamId.ToString(),
                item = item,
            };

            this.hubConnection.SendMessage(message);
        }

        public void Finish()
        {
            if (!this.isFinished)
            {
                this.isFinished = true;

                for (int i = 0; i < this.streamingIds.Length; ++i)
                    if (this.streamingIds[i] > 0)
                    {
                        var message = new Message
                        {
                            type = MessageTypes.Completion,
                            invocationId = this.streamingIds[i].ToString()
                        };

                        this.hubConnection.SendMessage(message);
                    }
            }
        }

        public void Cancel()
        {
            if (!this.isFinished)
            {
                this.isFinished = true;

                var message = new Message
                {
                    type = MessageTypes.CancelInvocation,
                    invocationId = this.invocationId.ToString(),
                };

                this.hubConnection.SendMessage(message);

                // Zero out the streaming ids, disabling any future message sending
                Array.Clear(this.streamingIds, 0, this.streamingIds.Length);

                // If it's also a down-stream, set it canceled.
                var itemContainer = (this.future.value as StreamItemContainer<TResult>);
                if (itemContainer != null)
                    itemContainer.IsCanceled = true;
            }
        }

        void IDisposable.Dispose()
        {
            Finish();
        }

        public IFuture<TResult> OnItem(FutureValueCallback<TResult> callback) { return this.future.OnItem(callback); }
        public IFuture<TResult> OnSuccess(FutureValueCallback<TResult> callback) { return this.future.OnSuccess(callback); }
        public IFuture<TResult> OnError(FutureErrorCallback callback) { return this.future.OnError(callback); }
        public IFuture<TResult> OnComplete(FutureCallback<TResult> callback) { return this.future.OnComplete(callback); }
    }

    /// <summary>
    /// An upload channel that represents one prameter of a client callable function. It implements the IDisposable
    /// interface and calls Finish from the Dispose method.
    /// </summary>
    public sealed class UploadChannel<TResult, T> : IDisposable
    {
        /// <summary>
        /// The associated upload controller
        /// </summary>
        public UploadItemController<TResult> Controller { get; private set; }

        /// <summary>
        /// What parameter is bound to.
        /// </summary>
        public int ParamIdx { get; private set; }

        /// <summary>
        /// Returns true if Finish() or Cancel() is already called.
        /// </summary>
        public bool IsFinished
        {
            get { return this.Controller.streamingIds[this.ParamIdx] == 0; }
            private set
            {
                if (value)
                    this.Controller.streamingIds[this.ParamIdx] = 0;
            }
        }

        /// <summary>
        /// The unique generated id of this parameter channel.
        /// </summary>
        public int StreamingId { get { return this.Controller.streamingIds[this.ParamIdx]; } }

        internal UploadChannel(UploadItemController<TResult> ctrl, int paramIdx)
        {
            this.Controller = ctrl;
            this.ParamIdx = paramIdx;
        }

        /// <summary>
        /// Uploads a parameter value to the server.
        /// </summary>
        public void Upload(T item)
        {
            int streamId = this.StreamingId;
            if (streamId > 0)
                this.Controller.UploadParam(streamId, item);
        }

        /// <summary>
        /// Calling this function cancels the call itself, not just a parameter upload channel.
        /// </summary>
        public void Cancel()
        {
            if (!this.IsFinished)
            {
                // Cancel all upload stream, cancel will also set streaming ids to 0.
                this.Controller.Cancel();
            }
        }

        /// <summary>
        /// Finishes the channel by telling the server that no more uplode items will follow.
        /// </summary>
        public void Finish()
        {
            if (!this.IsFinished)
            {
                int streamId = this.StreamingId;
                if (streamId > 0)
                {
                    // this will set the streaming id to 0
                    this.IsFinished = true;

                    var message = new Message
                    {
                        type = MessageTypes.Completion,
                        invocationId = streamId.ToString()
                    };

                    this.Controller.hubConnection.SendMessage(message);
                }
            }
        }

        void IDisposable.Dispose()
        {
            if (!this.IsFinished)
                Finish();
            GC.SuppressFinalize(this);
        }
    }
}
#endif