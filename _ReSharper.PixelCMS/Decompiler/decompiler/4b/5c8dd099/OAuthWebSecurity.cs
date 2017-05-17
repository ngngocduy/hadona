// Type: Microsoft.Web.WebPages.OAuth.OAuthWebSecurity
// Assembly: Microsoft.Web.WebPages.OAuth, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\Saigonpixel version1.1\PixelCMS.Web\bin\Microsoft.Web.WebPages.OAuth.dll

using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace Microsoft.Web.WebPages.OAuth
{
  /// <summary>
  /// Manages security that uses OAuth authentication providers like Facebook, Twitter, LinkedIn, Windows Live and OpenID authentication providers like Google and Yahoo.
  /// </summary>
  public static class OAuthWebSecurity
  {
    internal static IOpenAuthDataProvider OAuthDataProvider = (IOpenAuthDataProvider) new WebPagesOAuthDataProvider();
    private static readonly Dictionary<string, AuthenticationClientData> _authenticationClients = new Dictionary<string, AuthenticationClientData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a value that indicates whether the user has been authenticated using OAuth.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user has been authenticated using OAuth; otherwise, false.
    /// </returns>
    public static bool IsAuthenticatedWithOAuth
    {
      get
      {
        if (HttpContext.Current == null)
          throw new InvalidOperationException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.HttpContextNotAvailable);
        else
          return OAuthWebSecurity.GetIsAuthenticatedWithOAuthCore((HttpContextBase) new HttpContextWrapper(HttpContext.Current));
      }
    }

    public static ICollection<AuthenticationClientData> RegisteredClientData
    {
      get
      {
        return (ICollection<AuthenticationClientData>) OAuthWebSecurity._authenticationClients.Values;
      }
    }

    static OAuthWebSecurity()
    {
    }

    public static void RegisterFacebookClient(string appId, string appSecret)
    {
      string displayName = "Facebook";
      OAuthWebSecurity.RegisterFacebookClient(appId, appSecret, displayName);
    }

    public static void RegisterFacebookClient(string appId, string appSecret, string displayName)
    {
      IDictionary<string, object> extraData = (IDictionary<string, object>) new Dictionary<string, object>();
      OAuthWebSecurity.RegisterFacebookClient(appId, appSecret, displayName, extraData);
    }

    public static void RegisterFacebookClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new FacebookClient(appId, appSecret), displayName, extraData);
    }

    public static void RegisterMicrosoftClient(string clientId, string clientSecret)
    {
      string displayName = "Microsoft";
      OAuthWebSecurity.RegisterMicrosoftClient(clientId, clientSecret, displayName);
    }

    public static void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName)
    {
      OAuthWebSecurity.RegisterMicrosoftClient(clientId, clientSecret, displayName, (IDictionary<string, object>) new Dictionary<string, object>());
    }

    public static void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new MicrosoftClient(clientId, clientSecret), displayName, extraData);
    }

    public static void RegisterTwitterClient(string consumerKey, string consumerSecret)
    {
      string displayName = "Twitter";
      OAuthWebSecurity.RegisterTwitterClient(consumerKey, consumerSecret, displayName);
    }

    public static void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName)
    {
      OAuthWebSecurity.RegisterTwitterClient(consumerKey, consumerSecret, displayName, (IDictionary<string, object>) new Dictionary<string, object>());
    }

    public static void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new TwitterClient(consumerKey, consumerSecret), displayName, extraData);
    }

    public static void RegisterLinkedInClient(string consumerKey, string consumerSecret)
    {
      string displayName = "LinkedIn";
      OAuthWebSecurity.RegisterLinkedInClient(consumerKey, consumerSecret, displayName);
    }

    public static void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName)
    {
      OAuthWebSecurity.RegisterLinkedInClient(consumerKey, consumerSecret, displayName, (IDictionary<string, object>) new Dictionary<string, object>());
    }

    public static void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new LinkedInClient(consumerKey, consumerSecret), displayName, extraData);
    }

    public static void RegisterGoogleClient()
    {
      OAuthWebSecurity.RegisterGoogleClient("Google");
    }

    public static void RegisterGoogleClient(string displayName)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new GoogleOpenIdClient(), displayName, (IDictionary<string, object>) new Dictionary<string, object>());
    }

    public static void RegisterGoogleClient(string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new GoogleOpenIdClient(), displayName, extraData);
    }

    public static void RegisterYahooClient()
    {
      OAuthWebSecurity.RegisterYahooClient("Yahoo");
    }

    public static void RegisterYahooClient(string displayName)
    {
      OAuthWebSecurity.RegisterYahooClient(displayName, (IDictionary<string, object>) new Dictionary<string, object>());
    }

    public static void RegisterYahooClient(string displayName, IDictionary<string, object> extraData)
    {
      OAuthWebSecurity.RegisterClient((IAuthenticationClient) new YahooOpenIdClient(), displayName, extraData);
    }

    /// <summary>
    /// Registers an OAuth authentication client.
    /// </summary>
    /// <param name="client">One of the supported OAuth clients.</param>
    [CLSCompliant(false)]
    public static void RegisterClient(IAuthenticationClient client)
    {
      string displayName = (string) null;
      IDictionary<string, object> extraData = (IDictionary<string, object>) new Dictionary<string, object>();
      OAuthWebSecurity.RegisterClient(client, displayName, extraData);
    }

    [CLSCompliant(false)]
    public static void RegisterClient(IAuthenticationClient client, string displayName, IDictionary<string, object> extraData)
    {
      if (client == null)
        throw new ArgumentNullException("client");
      if (string.IsNullOrEmpty(client.ProviderName))
        throw new ArgumentException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.InvalidServiceProviderName, "client");
      if (OAuthWebSecurity._authenticationClients.ContainsKey(client.ProviderName))
        throw new ArgumentException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.ServiceProviderNameExists, "client");
      AuthenticationClientData authenticationClientData = new AuthenticationClientData(client, displayName, extraData);
      OAuthWebSecurity._authenticationClients.Add(client.ProviderName, authenticationClientData);
    }

    /// <summary>
    /// Requests the specified provider to start the authentication by directing users to an external website.
    /// </summary>
    /// <param name="provider">The OAuth provider.</param>
    public static void RequestAuthentication(string provider)
    {
      string returnUrl = (string) null;
      OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
    }

    /// <summary>
    /// Requests the specified provider to start the authentication by directing users to an external website, and directs the provider to redirect the user to the specified URL when authentication is successful.
    /// </summary>
    /// <param name="provider">The OAuth provider.</param><param name="returnUrl">The URL to return to when authentication is successful.</param>
    public static void RequestAuthentication(string provider, string returnUrl)
    {
      if (HttpContext.Current == null)
        throw new InvalidOperationException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.HttpContextNotAvailable);
      OAuthWebSecurity.RequestAuthenticationCore((HttpContextBase) new HttpContextWrapper(HttpContext.Current), provider, returnUrl);
    }

    internal static void RequestAuthenticationCore(HttpContextBase context, string provider, string returnUrl)
    {
      IAuthenticationClient oauthClient = OAuthWebSecurity.GetOAuthClient(provider);
      new OpenAuthSecurityManager(context, oauthClient, OAuthWebSecurity.OAuthDataProvider).RequestAuthentication(returnUrl);
    }

    /// <summary>
    /// Returns a value that indicates whether the user account has been confirmed by the provider.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:DotNetOpenAuth.AspNet.AuthenticationResult"/> instance that you can query to determine whether verification was successful.
    /// </returns>
    [CLSCompliant(false)]
    public static AuthenticationResult VerifyAuthentication()
    {
      return OAuthWebSecurity.VerifyAuthentication((string) null);
    }

    [CLSCompliant(false)]
    public static AuthenticationResult VerifyAuthentication(string returnUrl)
    {
      if (HttpContext.Current == null)
        throw new InvalidOperationException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.HttpContextNotAvailable);
      else
        return OAuthWebSecurity.VerifyAuthenticationCore((HttpContextBase) new HttpContextWrapper(HttpContext.Current), returnUrl);
    }

    internal static AuthenticationResult VerifyAuthenticationCore(HttpContextBase context, string returnUrl)
    {
      string providerName = OpenAuthSecurityManager.GetProviderName(context);
      if (string.IsNullOrEmpty(providerName))
        return AuthenticationResult.Failed;
      IAuthenticationClient client;
      if (OAuthWebSecurity.TryGetOAuthClient(providerName, out client))
        return new OpenAuthSecurityManager(context, client, OAuthWebSecurity.OAuthDataProvider).VerifyAuthentication(returnUrl);
      else
        throw new InvalidOperationException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.InvalidServiceProviderName);
    }

    /// <summary>
    /// Logs the user in.
    /// </summary>
    /// 
    /// <returns>
    /// true if login was successful; otherwise, false.
    /// </returns>
    /// <param name="providerName">The provider name.</param><param name="providerUserId">The user ID for the specified provider.</param><param name="createPersistentCookie">true to create a persistent cookie so that the login information is saved across browser sessions; otherwise, false.</param>
    public static bool Login(string providerName, string providerUserId, bool createPersistentCookie)
    {
      if (HttpContext.Current == null)
        throw new InvalidOperationException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.HttpContextNotAvailable);
      else
        return OAuthWebSecurity.LoginCore((HttpContextBase) new HttpContextWrapper(HttpContext.Current), providerName, providerUserId, createPersistentCookie);
    }

    internal static bool LoginCore(HttpContextBase context, string providerName, string providerUserId, bool createPersistentCookie)
    {
      IAuthenticationClient oauthClient = OAuthWebSecurity.GetOAuthClient(providerName);
      return new OpenAuthSecurityManager(context, oauthClient, OAuthWebSecurity.OAuthDataProvider).Login(providerUserId, createPersistentCookie);
    }

    internal static bool GetIsAuthenticatedWithOAuthCore(HttpContextBase context)
    {
      return new OpenAuthSecurityManager(context).IsAuthenticatedWithOpenAuth;
    }

    /// <summary>
    /// Creates or updates the account using the specified provider and user ID for the provider ID and associate the new account with the specified user name.
    /// </summary>
    /// <param name="providerName">The provider name.</param><param name="providerUserId">The user ID for the specified provider.</param><param name="userName">The name of the user.</param>
    public static void CreateOrUpdateAccount(string providerName, string providerUserId, string userName)
    {
      OAuthWebSecurity.VerifyProvider().CreateOrUpdateOAuthAccount(providerName, providerUserId, userName);
    }

    /// <summary>
    /// Returns the user ID for the specified OAuth or OpenID provider and provider user ID.
    /// </summary>
    /// 
    /// <returns>
    /// The user ID, or null if there is no user ID associated with the OAuth or Open ID provider user ID.
    /// </returns>
    /// <param name="providerName">The provider name.</param><param name="providerUserId">The user ID for the specified provider.</param>
    public static string GetUserName(string providerName, string providerUserId)
    {
      return OAuthWebSecurity.OAuthDataProvider.GetUserNameFromOpenAuth(providerName, providerUserId);
    }

    /// <summary>
    /// Gets the account or accounts that are associated using the specified user name.
    /// </summary>
    /// 
    /// <returns>
    /// The collection of accounts.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public static ICollection<OAuthAccount> GetAccountsFromUserName(string userName)
    {
      if (!string.IsNullOrEmpty(userName))
        return (ICollection<OAuthAccount>) Enumerable.ToList<OAuthAccount>(Enumerable.Select<OAuthAccountData, OAuthAccount>((IEnumerable<OAuthAccountData>) OAuthWebSecurity.VerifyProvider().GetAccountsForUser(userName), (Func<OAuthAccountData, OAuthAccount>) (p => new OAuthAccount(p.Provider, p.ProviderUserId))));
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Web.WebPages.OAuth.Properties.WebResources.Argument_Cannot_Be_Null_Or_Empty, new object[1]
      {
        (object) "userName"
      }), "userName");
    }

    public static bool HasLocalAccount(int userId)
    {
      return OAuthWebSecurity.VerifyProvider().HasLocalAccount(userId);
    }

    /// <summary>
    /// Deletes the specified membership account.
    /// </summary>
    /// 
    /// <returns>
    /// true if the account was deleted, or false if it was not.
    /// </returns>
    /// <param name="providerName">The provider name.</param><param name="providerUserId">The user ID for the specified provider.</param>
    public static bool DeleteAccount(string providerName, string providerUserId)
    {
      ExtendedMembershipProvider membershipProvider = OAuthWebSecurity.VerifyProvider();
      if (string.IsNullOrEmpty(OAuthWebSecurity.GetUserName(providerName, providerUserId)))
        return false;
      membershipProvider.DeleteOAuthAccount(providerName, providerUserId);
      return true;
    }

    public static AuthenticationClientData GetOAuthClientData(string providerName)
    {
      if (providerName == null)
        throw new ArgumentNullException("providerName");
      else
        return OAuthWebSecurity._authenticationClients[providerName];
    }

    public static bool TryGetOAuthClientData(string providerName, out AuthenticationClientData clientData)
    {
      if (providerName == null)
        throw new ArgumentNullException("providerName");
      else
        return OAuthWebSecurity._authenticationClients.TryGetValue(providerName, out clientData);
    }

    internal static IAuthenticationClient GetOAuthClient(string providerName)
    {
      if (!OAuthWebSecurity._authenticationClients.ContainsKey(providerName))
        throw new ArgumentException(Microsoft.Web.WebPages.OAuth.Properties.WebResources.ServiceProviderNotFound, "providerName");
      else
        return OAuthWebSecurity._authenticationClients[providerName].AuthenticationClient;
    }

    internal static bool TryGetOAuthClient(string provider, out IAuthenticationClient client)
    {
      if (OAuthWebSecurity._authenticationClients.ContainsKey(provider))
      {
        client = OAuthWebSecurity._authenticationClients[provider].AuthenticationClient;
        return true;
      }
      else
      {
        client = (IAuthenticationClient) null;
        return false;
      }
    }

    internal static void ClearProviders()
    {
      OAuthWebSecurity._authenticationClients.Clear();
    }

    private static ExtendedMembershipProvider VerifyProvider()
    {
      ExtendedMembershipProvider membershipProvider = Membership.Provider as ExtendedMembershipProvider;
      if (membershipProvider == null)
        throw new InvalidOperationException();
      else
        return membershipProvider;
    }

    public static string SerializeProviderUserId(string providerName, string providerUserId)
    {
      if (providerName == null)
        throw new ArgumentNullException("providerName");
      if (providerUserId == null)
        throw new ArgumentNullException("providerUserId");
      else
        return ProviderUserIdSerializationHelper.ProtectData(providerName, providerUserId);
    }

    public static bool TryDeserializeProviderUserId(string data, out string providerName, out string providerUserId)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      else
        return ProviderUserIdSerializationHelper.UnprotectData(data, out providerName, out providerUserId);
    }
  }
}
