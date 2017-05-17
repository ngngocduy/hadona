// Type: Microsoft.AspNet.SignalR.Hubs.HubBase
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.1.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\update_tkm_shop\packages\Microsoft.AspNet.SignalR.Core.2.1.2\lib\net45\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  /// <summary>
  /// Provides methods that communicate with SignalR connections that connected to a <see cref="T:Microsoft.AspNet.SignalR.Hub"/>.
  /// 
  /// </summary>
  public abstract class HubBase : IHub, IDisposable
  {
    IHubCallerConnectionContext<object> IHub.Clients { get; set; }

    /// <summary>
    /// Provides information about the calling client.
    /// 
    /// </summary>
    public HubCallerContext Context { get; set; }

    /// <summary>
    /// The group manager for this hub instance.
    /// 
    /// </summary>
    public IGroupManager Groups { get; set; }

    protected HubBase()
    {
      this.Clients = (IHubCallerConnectionContext<object>) new HubConnectionContext();
    }

    /// <summary>
    /// Called when a connection disconnects from this hub gracefully or due to a timeout.
    /// 
    /// </summary>
    /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
    ///             false, if the connection has been lost for longer than the
    ///             <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout"/>.
    ///             Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.
    ///             </param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task"/>
    /// </returns>
    public virtual Task OnDisconnected(bool stopCalled)
    {
      return TaskAsyncHelper.Empty;
    }

    /// <summary>
    /// Called when the connection connects to this hub instance.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task"/>
    /// </returns>
    public virtual Task OnConnected()
    {
      return TaskAsyncHelper.Empty;
    }

    /// <summary>
    /// Called when the connection reconnects to this hub instance.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task"/>
    /// </returns>
    public virtual Task OnReconnected()
    {
      return TaskAsyncHelper.Empty;
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
    }
  }
}
