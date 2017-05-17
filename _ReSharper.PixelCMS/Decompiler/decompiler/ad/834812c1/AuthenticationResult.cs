// Type: DotNetOpenAuth.AspNet.AuthenticationResult
// Assembly: DotNetOpenAuth.AspNet, Version=4.1.0.0, Culture=neutral, PublicKeyToken=2780ccd10d57b246
// Assembly location: D:\Data\Projects\Saigonpixel version1.1\PixelCMS.Web\bin\DotNetOpenAuth.AspNet.dll

using DotNetOpenAuth.Messaging;
using System;
using System.Collections.Generic;

namespace DotNetOpenAuth.AspNet
{
  /// <summary>
  /// Represents the result of OAuth or OpenID authentication.
  /// 
  /// </summary>
  public class AuthenticationResult
  {
    /// <summary>
    /// Returns an instance which indicates failed authentication.
    /// 
    /// </summary>
    public static readonly AuthenticationResult Failed = new AuthenticationResult(false);

    /// <summary>
    /// Gets the error that may have occured during the authentication process
    /// 
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// Gets the optional extra data that may be returned from the provider
    /// 
    /// </summary>
    public IDictionary<string, string> ExtraData { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the authentication step is successful.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// <c>true</c> if authentication is successful; otherwise, <c>false</c> .
    /// </value>
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// Gets the provider's name.
    /// 
    /// </summary>
    public string Provider { get; private set; }

    /// <summary>
    /// Gets the user id that is returned from the provider.  It is unique only within the Provider's namespace.
    /// 
    /// </summary>
    public string ProviderUserId { get; private set; }

    /// <summary>
    /// Gets an (insecure, non-unique) alias for the user that the user should recognize as himself/herself.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// This may take the form of an email address, a URL, or any other value that the user may recognize.
    /// </value>
    /// 
    /// <remarks>
    /// This alias may come from the Provider or may be derived by the relying party if the Provider does not supply one.
    ///             It is not guaranteed to be unique and certainly does not merit any trust in any suggested authenticity.
    /// 
    /// </remarks>
    public string UserName { get; private set; }

    static AuthenticationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DotNetOpenAuth.AspNet.AuthenticationResult"/> class.
    /// 
    /// </summary>
    /// <param name="isSuccessful">if set to <c>true</c> [is successful].
    ///             </param>
    public AuthenticationResult(bool isSuccessful)
      : this(isSuccessful, (string) null, (string) null, (string) null, (IDictionary<string, string>) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DotNetOpenAuth.AspNet.AuthenticationResult"/> class.
    /// 
    /// </summary>
    /// <param name="exception">The exception.
    ///             </param>
    public AuthenticationResult(Exception exception)
      : this(exception, (string) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DotNetOpenAuth.AspNet.AuthenticationResult"/> class.
    /// 
    /// </summary>
    /// <param name="exception">The exception.</param><param name="provider">The provider name.</param>
    public AuthenticationResult(Exception exception, string provider)
      : this(false)
    {
      if (exception == null)
        throw new ArgumentNullException("exception");
      this.Error = exception;
      this.Provider = provider;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DotNetOpenAuth.AspNet.AuthenticationResult"/> class.
    /// 
    /// </summary>
    /// <param name="isSuccessful">if set to <c>true</c> [is successful].
    ///             </param><param name="provider">The provider.
    ///             </param><param name="providerUserId">The provider user id.
    ///             </param><param name="userName">Name of the user.
    ///             </param><param name="extraData">The extra data.
    ///             </param>
    public AuthenticationResult(bool isSuccessful, string provider, string providerUserId, string userName, IDictionary<string, string> extraData)
    {
      this.IsSuccessful = isSuccessful;
      this.Provider = provider;
      this.ProviderUserId = providerUserId;
      this.UserName = userName;
      if (extraData == null)
        return;
      this.ExtraData = (IDictionary<string, string>) new ReadOnlyDictionary<string, string>(extraData);
    }
  }
}
