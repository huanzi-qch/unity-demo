#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using System;

namespace BestHTTP.SignalRCore
{
    public static class HubConnectionExtensions
    {
        public static UploadItemController<StreamItemContainer<TResult>> UploadStreamWithDownStream<TResult, T1>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.UploadStreamWithDownStream<TResult>(target, 1);
        }

        public static UploadItemController<StreamItemContainer<TResult>> UploadStreamWithDownStream<TResult, T1, T2>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.UploadStreamWithDownStream<TResult>(target, 2);
        }

        public static UploadItemController<StreamItemContainer<TResult>> UploadStreamWithDownStream<TResult, T1, T2, T3>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.UploadStreamWithDownStream<TResult>(target, 3);
        }

        public static UploadItemController<StreamItemContainer<TResult>> UploadStreamWithDownStream<TResult, T1, T2, T3, T4>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.UploadStreamWithDownStream<TResult>(target, 4);
        }

        public static UploadItemController<StreamItemContainer<TResult>> UploadStreamWithDownStream<TResult, T1, T2, T3, T4, T5>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.UploadStreamWithDownStream<TResult>(target, 5);
        }

        public static UploadItemController<TResult> Upload<TResult, T1>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.Upload<TResult>(target, 1);
        }

        public static UploadItemController<TResult> UploadStream<TResult, T1, T2>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.Upload<TResult>(target, 2);
        }

        public static UploadItemController<TResult> Upload<TResult, T1, T2, T3>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.Upload<TResult>(target, 3);
        }

        public static UploadItemController<TResult> Upload<TResult, T1, T2, T3, T4>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.Upload<TResult>(target, 4);
        }

        public static UploadItemController<TResult> Upload<TResult, T1, T2, T3, T4, T5>(this HubConnection hub, string target)
        {
            if (hub.State != ConnectionStates.Connected)
                return null;

            return hub.Upload<TResult>(target, 5);
        }
    }

    public static class UploadItemControllerExtensions
    {
        public static void Upload<TResult, P1>(this UploadItemController<TResult> controller, P1 item)
        {
            controller.UploadParam<P1>(controller.streamingIds[0], item);
        }

        public static void Upload<TResult, P1, P2>(this UploadItemController<TResult> controller, P1 param1, P2 param2)
        {
            controller.UploadParam<P1>(controller.streamingIds[0], param1);
            controller.UploadParam<P2>(controller.streamingIds[1], param2);
        }

        public static void Upload<TResult, P1, P2, P3>(this UploadItemController<TResult> controller, P1 param1, P2 param2, P3 param3)
        {
            controller.UploadParam<P1>(controller.streamingIds[0], param1);
            controller.UploadParam<P2>(controller.streamingIds[1], param2);
            controller.UploadParam<P3>(controller.streamingIds[2], param3);
        }

        public static void Upload<TResult, P1, P2, P3, P4>(this UploadItemController<TResult> controller, P1 param1, P2 param2, P3 param3, P4 param4)
        {
            controller.UploadParam<P1>(controller.streamingIds[0], param1);
            controller.UploadParam<P2>(controller.streamingIds[1], param2);
            controller.UploadParam<P3>(controller.streamingIds[2], param3);
            controller.UploadParam<P4>(controller.streamingIds[3], param4);
        }

        public static void Upload<TResult, P1, P2, P3, P4, P5>(this UploadItemController<TResult> controller, P1 param1, P2 param2, P3 param3, P4 param4, P5 param5)
        {
            controller.UploadParam<P1>(controller.streamingIds[0], param1);
            controller.UploadParam<P2>(controller.streamingIds[1], param2);
            controller.UploadParam<P3>(controller.streamingIds[2], param3);
            controller.UploadParam<P4>(controller.streamingIds[3], param4);
            controller.UploadParam<P5>(controller.streamingIds[4], param5);
        }
    }
}

#endif