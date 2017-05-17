// Type: Microsoft.AspNet.SignalR.Hub
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.1.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\update_tkm_shop\PixelCMS.Web\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR
{
  /// <summary>
  /// Provides methods that communicate with SignalR connections that connected to a <see cref="T:Microsoft.AspNet.SignalR.Hub"/>.
  /// 
  /// </summary>
  public abstract class Hub : HubBase
  {
    /// <summary/>
    public IHubCallerConnectionContext<object> Clients
    {
      get
      {
        return this.Clients;
      }
      set
      {
        this.Clients = value;
      }
    }
  }
}
