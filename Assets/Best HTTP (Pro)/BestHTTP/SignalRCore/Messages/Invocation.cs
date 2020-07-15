#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using System;

namespace BestHTTP.SignalRCore.Messages
{
    public struct Completion
    {
        public MessageTypes type;
        public string invocationId;
    }

    public struct CompletionWithResult
    {
        public MessageTypes type;
        public string invocationId;
        public object result;
    }

    public struct CompletionWithError
    {
        public MessageTypes type;
        public string invocationId;
        public string error;
    }

    public struct StreamItemMessage
    {
        public MessageTypes type;
        public string invocationId;
        public object item;
    }

    public struct InvocationMessage
    {
        public MessageTypes type;
        public string invocationId;
        public bool nonblocking;
        public string target;
        public object[] arguments;
    }

    public struct UploadInvocationMessage
    {
        public MessageTypes type;
        public string invocationId;
        public bool nonblocking;
        public string target;
        public object[] arguments;
        public int[] streamIds;
    }

    public struct CancelInvocationMessage
    {
        public MessageTypes type { get { return MessageTypes.CancelInvocation; } }
        public string invocationId;
    }

    public struct PingMessage
    {
        public MessageTypes type { get { return MessageTypes.Ping; } }
    }
}
#endif