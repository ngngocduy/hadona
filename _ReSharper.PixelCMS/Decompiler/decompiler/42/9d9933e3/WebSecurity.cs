﻿// Type: WebMatrix.WebData.WebSecurity
// Assembly: WebMatrix.WebData, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\TheKyMoi_Shop\packages\Microsoft.AspNet.WebPages.WebData.2.0.20710.0\lib\net40\WebMatrix.WebData.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.WebPages;
using WebMatrix.WebData.Resources;

namespace WebMatrix.WebData
{
  /// <summary>
  /// Provides security and authentication features for ASP.NET Web Pages applications, including the ability to create user accounts, log users in and out, reset or change passwords, and perform related tasks.
  /// </summary>
  public static class WebSecurity
  {
    /// <summary>
    /// Represents the key to the enableSimpleMembership value in the <see cref="P:System.Configuration.ConfigurationManager.AppSettings"/> property.
    /// </summary>
    public static readonly string EnableSimpleMembershipKey = "enableSimpleMembership";

    /// <summary>
    /// Gets a value that indicates whether the <see cref="M:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection(System.String,System.String,System.String,System.String,System.Boolean)"/> method has been called.
    /// </summary>
    /// 
    /// <returns>
    /// true if the initialization method has been called; otherwise, false.
    /// </returns>
    public static bool Initialized { get; private set; }

    /// <summary>
    /// Gets the ID for the current user.
    /// </summary>
    /// 
    /// <returns>
    /// The ID for the current user.
    /// </returns>
    public static int CurrentUserId
    {
      get
      {
        return WebSecurity.GetUserId(WebSecurity.CurrentUserName);
      }
    }

    /// <summary>
    /// Gets the user name for the current user.
    /// </summary>
    /// 
    /// <returns>
    /// The name of the current user.
    /// </returns>
    public static string CurrentUserName
    {
      get
      {
        return WebSecurity.Context.User.Identity.Name;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the current user has a user ID.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user has a user ID; otherwise, false.
    /// </returns>
    public static bool HasUserId
    {
      get
      {
        return WebSecurity.CurrentUserId != -1;
      }
    }

    /// <summary>
    /// Gets the authentication status of the current user.
    /// </summary>
    /// 
    /// <returns>
    /// true if the current user is authenticated; otherwise, false. The default is false.
    /// </returns>
    public static bool IsAuthenticated
    {
      get
      {
        return WebSecurity.Request.IsAuthenticated;
      }
    }

    internal static HttpContextBase Context
    {
      get
      {
        return (HttpContextBase) new HttpContextWrapper(HttpContext.Current);
      }
    }

    internal static HttpRequestBase Request
    {
      get
      {
        return WebSecurity.Context.Request;
      }
    }

    internal static HttpResponseBase Response
    {
      get
      {
        return WebSecurity.Context.Response;
      }
    }

    static WebSecurity()
    {
    }

    internal static void PreAppStartInit()
    {
      if (!ConfigUtil.SimpleMembershipEnabled)
        return;
      MembershipProvider currentDefault1 = Membership.Providers["AspNetSqlMembershipProvider"];
      if (currentDefault1 != null)
      {
        SimpleMembershipProvider membershipProvider = WebSecurity.CreateDefaultSimpleMembershipProvider("AspNetSqlMembershipProvider", currentDefault1);
        Membership.Providers.Remove("AspNetSqlMembershipProvider");
        Membership.Providers.Add((ProviderBase) membershipProvider);
      }
      Roles.Enabled = true;
      RoleProvider currentDefault2 = Roles.Providers["AspNetSqlRoleProvider"];
      if (currentDefault2 == null)
        return;
      SimpleRoleProvider simpleRoleProvider = WebSecurity.CreateDefaultSimpleRoleProvider("AspNetSqlRoleProvider", currentDefault2);
      Roles.Providers.Remove("AspNetSqlRoleProvider");
      Roles.Providers.Add((ProviderBase) simpleRoleProvider);
    }

    private static ExtendedMembershipProvider VerifyProvider()
    {
      ExtendedMembershipProvider membershipProvider = Membership.Provider as ExtendedMembershipProvider;
      if (membershipProvider == null)
        throw new InvalidOperationException(WebDataResources.Security_NoExtendedMembershipProvider);
      membershipProvider.VerifyInitialized();
      return membershipProvider;
    }

    /// <summary>
    /// Initializes the membership system by connecting to a database that contains user information and optionally creates membership tables if they do not already exist.
    /// </summary>
    /// <param name="connectionStringName">The name of the connection string for the database that contains user information. If you are using SQL Server Compact, this can be the name of the database file (.sdf file) without the .sdf file name extension.</param><param name="userTableName">The name of the database table that contains the user profile information.</param><param name="userIdColumn">The name of the database column that contains user IDs. This column must be typed as an integer (int).</param><param name="userNameColumn">The name of the database column that contains user names. This column is used to match user profile data to membership account data.</param><param name="autoCreateTables">true to indicate that user profile and membership tables should be created if they do not exist; false to indicate that tables should not be created automatically. Although the membership tables can be created automatically, the database itself must already exist.</param>
    public static void InitializeDatabaseConnection(string connectionStringName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {
      WebSecurity.InitializeProviders(new DatabaseConnectionInfo()
      {
        ConnectionStringName = connectionStringName
      }, userTableName, userIdColumn, userNameColumn, autoCreateTables);
    }

    /// <summary>
    /// Initializes the membership system by connecting to a database that contains user information by using the specified membership or role provider, and optionally creates membership tables if they do not already exist.
    /// </summary>
    /// <param name="connectionString">The name of the connection string for the database that contains user information. If you are using SQL Server Compact, this can be the name of the database file (.sdf file) without the .sdf file name extension.</param><param name="providerName">The name of the ADO.NET data provider. If you want to use Microsoft SQL Server, the <see cref="M:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection(System.String,System.String,System.String,System.String,System.Boolean)"/> overload is recommended.</param><param name="userTableName">The name of the database table that contains the user profile information.</param><param name="userIdColumn">The name of the database column that contains user IDs. This column must be typed as an integer (int).</param><param name="userNameColumn">The name of the database column that contains user names. This column is used to match user profile data to membership account data.</param><param name="autoCreateTables">true to indicate that user profile and membership tables should be created automatically; false to indicate that tables should not be created automatically. Although the membership tables can be created automatically, the database itself must already exist.</param>
    public static void InitializeDatabaseConnection(string connectionString, string providerName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {
      WebSecurity.InitializeProviders(new DatabaseConnectionInfo()
      {
        ConnectionString = connectionString,
        ProviderName = providerName
      }, userTableName, userIdColumn, userNameColumn, autoCreateTables);
    }

    private static void InitializeProviders(DatabaseConnectionInfo connect, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {
      SimpleMembershipProvider simpleMembership = Membership.Provider as SimpleMembershipProvider;
      if (simpleMembership != null)
        WebSecurity.InitializeMembershipProvider(simpleMembership, connect, userTableName, userIdColumn, userNameColumn, autoCreateTables);
      SimpleRoleProvider simpleRoles = Roles.Provider as SimpleRoleProvider;
      if (simpleRoles != null)
        WebSecurity.InitializeRoleProvider(simpleRoles, connect, userTableName, userIdColumn, userNameColumn, autoCreateTables);
      WebSecurity.Initialized = true;
    }

    internal static void InitializeMembershipProvider(SimpleMembershipProvider simpleMembership, DatabaseConnectionInfo connect, string userTableName, string userIdColumn, string userNameColumn, bool createTables)
    {
      if (simpleMembership.InitializeCalled)
        throw new InvalidOperationException(WebDataResources.Security_InitializeAlreadyCalled);
      simpleMembership.ConnectionInfo = connect;
      simpleMembership.UserIdColumn = userIdColumn;
      simpleMembership.UserNameColumn = userNameColumn;
      simpleMembership.UserTableName = userTableName;
      if (createTables)
        simpleMembership.CreateTablesIfNeeded();
      else
        simpleMembership.ValidateUserTable();
      simpleMembership.InitializeCalled = true;
    }

    internal static void InitializeRoleProvider(SimpleRoleProvider simpleRoles, DatabaseConnectionInfo connect, string userTableName, string userIdColumn, string userNameColumn, bool createTables)
    {
      if (simpleRoles.InitializeCalled)
        throw new InvalidOperationException(WebDataResources.Security_InitializeAlreadyCalled);
      simpleRoles.ConnectionInfo = connect;
      simpleRoles.UserTableName = userTableName;
      simpleRoles.UserIdColumn = userIdColumn;
      simpleRoles.UserNameColumn = userNameColumn;
      if (createTables)
        simpleRoles.CreateTablesIfNeeded();
      simpleRoles.InitializeCalled = true;
    }

    private static SimpleMembershipProvider CreateDefaultSimpleMembershipProvider(string name, MembershipProvider currentDefault)
    {
      SimpleMembershipProvider membershipProvider = new SimpleMembershipProvider(currentDefault);
      NameValueCollection config = new NameValueCollection();
      membershipProvider.Initialize(name, config);
      return membershipProvider;
    }

    private static SimpleRoleProvider CreateDefaultSimpleRoleProvider(string name, RoleProvider currentDefault)
    {
      SimpleRoleProvider simpleRoleProvider = new SimpleRoleProvider(currentDefault);
      NameValueCollection config = new NameValueCollection();
      simpleRoleProvider.Initialize(name, config);
      return simpleRoleProvider;
    }

    /// <summary>
    /// Logs the user in.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user was logged in; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="persistCookie">(Optional) true to specify that the authentication token in the cookie should be persisted beyond the current session; otherwise false. The default is false.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool Login(string userName, string password, bool persistCookie = false)
    {
      WebSecurity.VerifyProvider();
      bool flag = Membership.ValidateUser(userName, password);
      if (flag)
        FormsAuthentication.SetAuthCookie(userName, persistCookie);
      return flag;
    }

    /// <summary>
    /// Logs the user out.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static void Logout()
    {
      WebSecurity.VerifyProvider();
      FormsAuthentication.SignOut();
    }

    /// <summary>
    /// Changes the password for the specified user.
    /// </summary>
    /// 
    /// <returns>
    /// true if the password is successfully changed; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="currentPassword">The current password for the user.</param><param name="newPassword">The new password.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool ChangePassword(string userName, string currentPassword, string newPassword)
    {
      WebSecurity.VerifyProvider();
      bool flag = false;
      try
      {
        flag = Membership.GetUser(userName, true).ChangePassword(currentPassword, newPassword);
      }
      catch (ArgumentException ex)
      {
      }
      return flag;
    }

    /// <summary>
    /// Confirms that an account is valid and activates the account.
    /// </summary>
    /// 
    /// <returns>
    /// true if the account is confirmed; otherwise, false.
    /// </returns>
    /// <param name="accountConfirmationToken">A confirmation token to pass to the authentication provider.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool ConfirmAccount(string accountConfirmationToken)
    {
      return WebSecurity.VerifyProvider().ConfirmAccount(accountConfirmationToken);
    }

    /// <summary>
    /// Confirms that an account for the specified user name is valid and activates the account.
    /// </summary>
    /// 
    /// <returns>
    /// true if the account is confirmed; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="accountConfirmationToken">A confirmation token to pass to the authentication provider.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool ConfirmAccount(string userName, string accountConfirmationToken)
    {
      return WebSecurity.VerifyProvider().ConfirmAccount(userName, accountConfirmationToken);
    }

    /// <summary>
    /// Creates a new membership account using the specified user name and password and optionally lets you specify that the user must explicitly confirm the account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="requireConfirmationToken">(Optional) true to specify that the account must be confirmed by using the token return value; otherwise, false. The default is false. </param><exception cref="T:System.Web.Security.MembershipCreateUserException"><paramref name="username"/> is empty.-or-<paramref name="username"/> already has a membership account.-or-<paramref name="password"/> is empty.-or-<paramref name="password"/> is too long.-or-The database operation failed.</exception><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
    {
      return WebSecurity.VerifyProvider().CreateAccount(userName, password, requireConfirmationToken);
    }

    /// <summary>
    /// Creates a new user profile entry and a new membership account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the user account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password for the user.</param><param name="propertyValues">(Optional) A dictionary that contains additional user attributes. The default is null.</param><param name="requireConfirmationToken">(Optional) true to specify that the user account must be confirmed; otherwise, false. The default is false.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
    {
      ExtendedMembershipProvider membershipProvider = WebSecurity.VerifyProvider();
      IDictionary<string, object> values = (IDictionary<string, object>) null;
      if (propertyValues != null)
        values = (IDictionary<string, object>) new RouteValueDictionary(propertyValues);
      return membershipProvider.CreateUserAndAccount(userName, password, requireConfirmationToken, values);
    }

    /// <summary>
    /// Generates a password reset token that can be sent to a user in email.
    /// </summary>
    /// 
    /// <returns>
    /// A token to send to the user.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="tokenExpirationInMinutesFromNow">(Optional) The time in minutes until the password reset token expires. The default is 1440 (24 hours).</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
    {
      return WebSecurity.VerifyProvider().GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
    }

    /// <summary>
    /// Returns a value that indicates whether the specified user exists in the membership database.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <paramref name="username"/> exists in the user profile table; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool UserExists(string userName)
    {
      WebSecurity.VerifyProvider();
      return Membership.GetUser(userName) != null;
    }

    /// <summary>
    /// Returns the ID for a user based on the specified user name.
    /// </summary>
    /// 
    /// <returns>
    /// The user ID.
    /// </returns>
    /// <param name="userName">The user name.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static int GetUserId(string userName)
    {
      WebSecurity.VerifyProvider();
      MembershipUser user = Membership.GetUser(userName);
      if (user == null)
        return -1;
      else
        return (int) user.ProviderUserKey;
    }

    /// <summary>
    /// Returns a user ID from a password reset token.
    /// </summary>
    /// 
    /// <returns>
    /// The user ID.
    /// </returns>
    /// <param name="token">The password reset token.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static int GetUserIdFromPasswordResetToken(string token)
    {
      return WebSecurity.VerifyProvider().GetUserIdFromPasswordResetToken(token);
    }

    /// <summary>
    /// Returns a value that indicates whether the user name of the logged-in user matches the specified user name.
    /// </summary>
    /// 
    /// <returns>
    /// true if the logged-in user name matches <paramref name="userName"/>; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name to compare the logged-in user name to.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool IsCurrentUser(string userName)
    {
      WebSecurity.VerifyProvider();
      return string.Equals(WebSecurity.CurrentUserName, userName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a value that indicates whether the user has been confirmed.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user is confirmed; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public static bool IsConfirmed(string userName)
    {
      return WebSecurity.VerifyProvider().IsConfirmed(userName);
    }

    private static bool IsUserLoggedOn(int userId)
    {
      WebSecurity.VerifyProvider();
      return WebSecurity.CurrentUserId == userId;
    }

    /// <summary>
    /// If the user is not authenticated, sets the HTTP status to 401 (Unauthorized).
    /// </summary>
    public static void RequireAuthenticatedUser()
    {
      WebSecurity.VerifyProvider();
      IPrincipal user = WebSecurity.Context.User;
      if (user != null && user.Identity.IsAuthenticated)
        return;
      ResponseExtensions.SetStatus(WebSecurity.Response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// If the specified user is not logged on, sets the HTTP status to 401 (Unauthorized).
    /// </summary>
    /// <param name="userId">The ID of the user to compare.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static void RequireUser(int userId)
    {
      WebSecurity.VerifyProvider();
      if (WebSecurity.IsUserLoggedOn(userId))
        return;
      ResponseExtensions.SetStatus(WebSecurity.Response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// If the current user does not match the specified user name, sets the HTTP status to 401 (Unauthorized).
    /// </summary>
    /// <param name="userName">The name of the user to compare.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static void RequireUser(string userName)
    {
      WebSecurity.VerifyProvider();
      if (string.Equals(WebSecurity.CurrentUserName, userName, StringComparison.OrdinalIgnoreCase))
        return;
      ResponseExtensions.SetStatus(WebSecurity.Response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// If the current user is not in all of the specified roles, sets the HTTP status code to 401 (Unauthorized).
    /// </summary>
    /// <param name="roles">The roles to check. The current user must be in all of the roles that are passed in this parameter.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static void RequireRoles(params string[] roles)
    {
      WebSecurity.VerifyProvider();
      foreach (string roleName in roles)
      {
        if (!Roles.IsUserInRole(WebSecurity.CurrentUserName, roleName))
        {
          ResponseExtensions.SetStatus(WebSecurity.Response, HttpStatusCode.Unauthorized);
          break;
        }
      }
    }

    /// <summary>
    /// Resets a password by using a password reset token.
    /// </summary>
    /// 
    /// <returns>
    /// true if the password was changed; otherwise, false.
    /// </returns>
    /// <param name="passwordResetToken">A password reset token.</param><param name="newPassword">The new password.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool ResetPassword(string passwordResetToken, string newPassword)
    {
      return WebSecurity.VerifyProvider().ResetPasswordWithToken(passwordResetToken, newPassword);
    }

    /// <summary>
    /// Returns a value that indicates whether the specified membership account is temporarily locked because of too many failed password attempts in the specified number of seconds.
    /// </summary>
    /// 
    /// <returns>
    /// true if the membership account is locked; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name of the membership account.</param><param name="allowedPasswordAttempts">The number of password attempts the user is permitted before the membership account is locked.</param><param name="intervalInSeconds">The number of seconds to lock a user account after the number of password attempts exceeds the value in the <paramref name="allowedPasswordAttempts"/> parameter.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds)
    {
      WebSecurity.VerifyProvider();
      return WebSecurity.IsAccountLockedOut(userName, allowedPasswordAttempts, TimeSpan.FromSeconds((double) intervalInSeconds));
    }

    /// <summary>
    /// Returns a value that indicates whether the specified membership account is temporarily locked because of too many failed password attempts in the specified time span.
    /// </summary>
    /// 
    /// <returns>
    /// true if the membership account is locked; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name of the membership account.</param><param name="allowedPasswordAttempts">The number of password attempts the user is permitted before the membership account is locked.</param><param name="interval">The number of seconds to lock out a user account after the number of password attempts exceeds the value in the <paramref name="allowedPasswordAttempts"/> parameter.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval)
    {
      return WebSecurity.IsAccountLockedOutInternal(WebSecurity.VerifyProvider(), userName, allowedPasswordAttempts, interval);
    }

    internal static bool IsAccountLockedOutInternal(ExtendedMembershipProvider provider, string userName, int allowedPasswordAttempts, TimeSpan interval)
    {
      if (provider.GetUser(userName, false) != null && provider.GetPasswordFailuresSinceLastSuccess(userName) > allowedPasswordAttempts)
        return provider.GetLastPasswordFailureDate(userName).Add(interval) > DateTime.UtcNow;
      else
        return false;
    }

    /// <summary>
    /// Returns the number of times that the password for the specified account was incorrectly entered since the last successful login or since the membership account was created.
    /// </summary>
    /// 
    /// <returns>
    /// The count of failed password attempts for the specified account.
    /// </returns>
    /// <param name="userName">The user name of the account.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      return WebSecurity.VerifyProvider().GetPasswordFailuresSinceLastSuccess(userName);
    }

    /// <summary>
    /// Returns the date and time when the specified membership account was created.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time that the membership account was created, or <see cref="F:System.DateTime.MinValue"/> if the account creation date is not available.
    /// </returns>
    /// <param name="userName">The user name for the membership account.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static DateTime GetCreateDate(string userName)
    {
      return WebSecurity.VerifyProvider().GetCreateDate(userName);
    }

    /// <summary>
    /// Returns the date and time when the password was most recently changed for the specified membership account.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time when the password was most recently changed, or <see cref="F:System.DateTime.MinValue"/> if the password has not been changed for this account.
    /// </returns>
    /// <param name="userName">The user name of the account.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static DateTime GetPasswordChangedDate(string userName)
    {
      return WebSecurity.VerifyProvider().GetPasswordChangedDate(userName);
    }

    /// <summary>
    /// Returns the date and time when an incorrect password was most recently entered for the specified account.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time when an incorrect password was most recently entered for this account, or <see cref="F:System.DateTime.MinValue"/> if an incorrect password has not been entered for this account.
    /// </returns>
    /// <param name="userName">The user name of the membership account.</param><exception cref="T:System.InvalidOperationException">The <see cref="M:WebMatrix.WebData.SimpleMembershipProvider.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> method was not called.-or-The <see cref="Overload:WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection"/> method was not called.-or-The <see cref="T:WebMatrix.WebData.SimpleMembershipProvider"/> membership provider is not registered in the configuration of your site. For more information, contact your site's system administrator.</exception>
    public static DateTime GetLastPasswordFailureDate(string userName)
    {
      return WebSecurity.VerifyProvider().GetLastPasswordFailureDate(userName);
    }
  }
}
