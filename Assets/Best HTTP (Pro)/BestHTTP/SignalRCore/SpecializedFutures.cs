#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using BestHTTP.Futures;
using System;

namespace BestHTTP.SignalRCore
{
    //public interface IUploadFeature<T> : IFuture<T>
    //{
    //    HubConnection Hub { get; }
    //    long InvocationId { get; }
    //    int[] StreamIds { get; }
    //
    //    void Upload<P1>(P1 item);
    //    void Upload<P1, P2>(P1 item1, P2 item2);
    //    void Upload<P1, P2, P3>(P1 item1, P2 item2, P3 item3);
    //    void Upload<P1, P2, P3, P4>(P1 item1, P2 item2, P3 item3, P4 item4);
    //    void Finish();
    //    void Cancel();
    //}
    //
    //public sealed class UploadFeature<T> : Future<T>, IUploadFeature<T>
    //{
    //    public HubConnection Hub { get; private set; }
    //    public long InvocationId { get; private set; }
    //    public int[] StreamIds { get; private set; }
    //
    //    public UploadFeature(HubConnection hub, long invocationId, int[] streamIds)
    //        : base()
    //    {
    //        this.Hub = hub;
    //        this.InvocationId = invocationId;
    //        this.StreamIds = streamIds;
    //    }
    //
    //    public void Upload<P1>(P1 item)
    //    {
    //        this.Hub.UploadParam(this.StreamIds[0], item);
    //    }
    //
    //    public void Upload<P1, P2>(P1 item1, P2 item2)
    //    {
    //        this.Hub.UploadParam(this.StreamIds[0], item1);
    //        this.Hub.UploadParam(this.StreamIds[1], item2);
    //    }
    //
    //    public void Upload<P1, P2, P3>(P1 item1, P2 item2, P3 item3)
    //    {
    //        this.Hub.UploadParam(this.StreamIds[0], item1);
    //        this.Hub.UploadParam(this.StreamIds[1], item2);
    //        this.Hub.UploadParam(this.StreamIds[2], item3);
    //    }
    //
    //    public void Upload<P1, P2, P3, P4>(P1 item1, P2 item2, P3 item3, P4 item4)
    //    {
    //        this.Hub.UploadParam(this.StreamIds[0], item1);
    //        this.Hub.UploadParam(this.StreamIds[1], item2);
    //        this.Hub.UploadParam(this.StreamIds[2], item3);
    //        this.Hub.UploadParam(this.StreamIds[3], item4);
    //    }
    //
    //    public void Finish()
    //    {
    //        this.Hub.FinishUpload(this);
    //    }
    //
    //    public void Cancel()
    //    {
    //        this.Hub.CancelUpload(this);
    //    }
    //}
}
#endif