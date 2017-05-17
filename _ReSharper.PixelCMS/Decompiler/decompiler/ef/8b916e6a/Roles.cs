// Type: System.Web.Security.Roles
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\System.Web\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Globalization;
using System.Runtime;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Util;

namespace System.Web.Security
{
  /// <summary>
  /// Manages user membership in roles for authorization checking in an ASP.NET application. This class cannot be inherited.
  /// </summary>
  public static class Roles
  {
    private static Exception s_InitializeException = (Exception) null;
    private static object s_lock = new object();
    private static int s_MaxCachedResults = 25;
    private static RoleProvider s_Provider;
    private static bool s_Enabled;
    private static string s_CookieName;
    private static bool s_CacheRolesInCookie;
    private static int s_CookieTimeout;
    private static string s_CookiePath;
    private static bool s_CookieRequireSSL;
    private static bool s_CookieSlidingExpiration;
    private static CookieProtection s_CookieProtection;
    private static string s_Domain;
    private static bool s_Initialized;
    private static bool s_InitializedDefaultProvider;
    private static bool s_EnabledSet;
    private static RoleProviderCollection s_Providers;
    private static bool s_CreatePersistentCookie;

    /// <summary>
    /// Gets the default role provider for the application.
    /// </summary>
    /// 
    /// <returns>
    /// The default role provider for the application, which is exposed as a class that inherits the <see cref="T:System.Web.Security.RoleProvider"/> abstract class.
    /// </returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static RoleProvider Provider
    {
      get
      {
        Roles.EnsureEnabled();
        if (Roles.s_Provider == null)
          throw new InvalidOperationException(SR.GetString("Def_role_provider_not_found"));
        else
          return Roles.s_Provider;
      }
    }

    /// <summary>
    /// Gets a collection of the role providers for the ASP.NET application.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Web.Security.RoleProviderCollection"/> that contains the role providers configured for the ASP.NET application.
    /// </returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static RoleProviderCollection Providers
    {
      get
      {
        Roles.EnsureEnabled();
        return Roles.s_Providers;
      }
    }

    /// <summary>
    /// Gets the name of the cookie where role names are cached.
    /// </summary>
    /// 
    /// <returns>
    /// The name of the cookie where role names are cached. The default is .ASPXROLES.
    /// </returns>
    public static string CookieName
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookieName;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user's roles are cached in a cookie.
    /// </summary>
    /// 
    /// <returns>
    /// true if the current user's roles are cached in a cookie; otherwise, false. The default is true.
    /// </returns>
    public static bool CacheRolesInCookie
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CacheRolesInCookie;
      }
    }

    /// <summary>
    /// Gets the number of minutes before the roles cookie expires.
    /// </summary>
    /// 
    /// <returns>
    /// An integer specifying the number of minutes before the roles cookie expires. The default is 30 minutes.
    /// </returns>
    public static int CookieTimeout
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookieTimeout;
      }
    }

    /// <summary>
    /// Gets the path for the cached role names cookie.
    /// </summary>
    /// 
    /// <returns>
    /// The path of the cookie where role names are cached. The default is /.
    /// </returns>
    public static string CookiePath
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookiePath;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the role names cookie requires SSL in order to be returned to the server.
    /// </summary>
    /// 
    /// <returns>
    /// true if SSL is required to return the role names cookie to the server; otherwise, false. The default is false.
    /// </returns>
    public static bool CookieRequireSSL
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookieRequireSSL;
      }
    }

    /// <summary>
    /// Indicates whether the role names cookie expiration date and time will be reset periodically.
    /// </summary>
    /// 
    /// <returns>
    /// true if the role names cookie expiration date and time will be reset periodically; otherwise, false. The default is true.
    /// </returns>
    public static bool CookieSlidingExpiration
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookieSlidingExpiration;
      }
    }

    /// <summary>
    /// Gets a value that indicates how role names cached in a cookie are protected.
    /// </summary>
    /// 
    /// <returns>
    /// One of the <see cref="T:System.Web.Security.CookieProtection"/> enumeration values indicating how role names that are cached in a cookie are protected. The default is All.
    /// </returns>
    public static CookieProtection CookieProtectionValue
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CookieProtection;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the role-names cookie is session-based or persistent.
    /// </summary>
    /// 
    /// <returns>
    /// true if the role-names cookie is a persistent cookie; otherwise false. The default is false.
    /// </returns>
    public static bool CreatePersistentCookie
    {
      get
      {
        Roles.Initialize();
        return Roles.s_CreatePersistentCookie;
      }
    }

    /// <summary>
    /// Gets the value of the domain of the role-names cookie.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="P:System.Web.HttpCookie.Domain"/> of the role names cookie.
    /// </returns>
    public static string Domain
    {
      get
      {
        Roles.Initialize();
        return Roles.s_Domain;
      }
    }

    /// <summary>
    /// Gets the maximum number of role names to be cached for a user.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum number of role names to be cached for a user. The default is 25.
    /// </returns>
    public static int MaxCachedResults
    {
      get
      {
        Roles.Initialize();
        return Roles.s_MaxCachedResults;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether role management is enabled for the current Web application.
    /// </summary>
    /// 
    /// <returns>
    /// true if role management is enabled; otherwise, false. The default is false.
    /// </returns>
    public static bool Enabled
    {
      get
      {
        if (HostingEnvironment.IsHosted && !HttpRuntime.HasAspNetHostingPermission(AspNetHostingPermissionLevel.Low))
          return false;
        if (!Roles.s_Initialized && !Roles.s_EnabledSet)
        {
          Roles.s_Enabled = RuntimeConfig.GetAppConfig().RoleManager.Enabled;
          Roles.s_EnabledSet = true;
        }
        return Roles.s_Enabled;
      }
      set
      {
        BuildManager.ThrowIfPreAppStartNotRunning();
        Roles.s_Enabled = value;
        Roles.s_EnabledSet = true;
      }
    }

    /// <summary>
    /// Gets or sets the name of the application to store and retrieve role information for.
    /// </summary>
    /// 
    /// <returns>
    /// The name of the application to store and retrieve role information for.
    /// </returns>
    public static string ApplicationName
    {
      get
      {
        return Roles.Provider.ApplicationName;
      }
      set
      {
        Roles.Provider.ApplicationName = value;
      }
    }

    static Roles()
    {
    }

    /// <summary>
    /// Gets a value indicating whether the specified user is in the specified role.
    /// </summary>
    /// 
    /// <returns>
    /// true if the specified user is in the specified role; otherwise, false.
    /// </returns>
    /// <param name="username">The name of the user to search for. </param><param name="roleName">The name of the role to search in. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-<paramref name="username"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).-or-<paramref name="username"/> contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static bool IsUserInRole(string username, string roleName)
    {
      if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(4, 8))
        EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_BEGIN, HttpContext.Current.WorkerRequest);
      Roles.EnsureEnabled();
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
        SecUtility.CheckParameter(ref username, true, false, true, 0, "username");
        if (username.Length < 1)
          return false;
        IPrincipal currentUser = Roles.GetCurrentUser();
        flag1 = currentUser == null || !(currentUser is RolePrincipal) || (!(((RolePrincipal) currentUser).ProviderName == Roles.Provider.Name) || !StringUtil.EqualsIgnoreCase(username, currentUser.Identity.Name)) ? Roles.Provider.IsUserInRole(username, roleName) : currentUser.IsInRole(roleName);
        return flag1;
      }
      finally
      {
        if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(4, 8))
        {
          if (EtwTrace.IsTraceEnabled(5, 8))
          {
            string @string = SR.Resources.GetString(flag1 ? "Etw_Success" : "Etw_Failure", CultureInfo.InstalledUICulture);
            EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_IS_USER_IN_ROLE, HttpContext.Current.WorkerRequest, flag2 ? "RolePrincipal" : Roles.Provider.GetType().FullName, username, roleName, @string);
          }
          EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_END, HttpContext.Current.WorkerRequest, flag2 ? "RolePrincipal" : Roles.Provider.GetType().FullName, username);
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the currently logged-on user is in the specified role.
    /// </summary>
    /// 
    /// <returns>
    /// true if the currently logged-on user is in the specified role; otherwise, false.
    /// </returns>
    /// <param name="roleName">The name of the role to search in. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-There is no current logged-on user.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static bool IsUserInRole(string roleName)
    {
      return Roles.IsUserInRole(Roles.GetCurrentUserName(), roleName);
    }

    /// <summary>
    /// Gets a list of the roles that a user is in.
    /// </summary>
    /// 
    /// <returns>
    /// A string array containing the names of all the roles that the specified user is in.
    /// </returns>
    /// <param name="username">The user to return a list of roles for. </param><exception cref="T:System.ArgumentNullException"><paramref name="username"/> is null. </exception><exception cref="T:System.ArgumentException"><paramref name="username"/> contains a comma (,). </exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static string[] GetRolesForUser(string username)
    {
      if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(4, 8))
        EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_BEGIN, HttpContext.Current.WorkerRequest);
      Roles.EnsureEnabled();
      string[] strArray = (string[]) null;
      bool flag = false;
      try
      {
        SecUtility.CheckParameter(ref username, true, false, true, 0, "username");
        if (username.Length < 1)
        {
          strArray = new string[0];
          return strArray;
        }
        else
        {
          IPrincipal currentUser = Roles.GetCurrentUser();
          if (currentUser != null && currentUser is RolePrincipal && (((RolePrincipal) currentUser).ProviderName == Roles.Provider.Name && StringUtil.EqualsIgnoreCase(username, currentUser.Identity.Name)))
          {
            strArray = ((RolePrincipal) currentUser).GetRoles();
            flag = true;
          }
          else
            strArray = Roles.Provider.GetRolesForUser(username);
          return strArray;
        }
      }
      finally
      {
        if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(4, 8))
        {
          if (EtwTrace.IsTraceEnabled(5, 8))
          {
            string data3 = (string) null;
            if (strArray != null && strArray.Length > 0)
              data3 = strArray[0];
            for (int index = 1; index < strArray.Length; ++index)
              data3 = data3 + "," + strArray[index];
            EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_GET_USER_ROLES, HttpContext.Current.WorkerRequest, flag ? "RolePrincipal" : Roles.Provider.GetType().FullName, username, data3, (string) null);
          }
          EtwTrace.Trace(EtwTraceType.ETW_TYPE_ROLE_END, HttpContext.Current.WorkerRequest, flag ? "RolePrincipal" : Roles.Provider.GetType().FullName, username);
        }
      }
    }

    /// <summary>
    /// Gets a list of the roles that the currently logged-on user is in.
    /// </summary>
    /// 
    /// <returns>
    /// A string array containing the names of all the roles that the currently logged-on user is in.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">There is no current logged-on user.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static string[] GetRolesForUser()
    {
      return Roles.GetRolesForUser(Roles.GetCurrentUserName());
    }

    /// <summary>
    /// Gets a list of users in the specified role.
    /// </summary>
    /// 
    /// <returns>
    /// A string array containing the names of all the users who are members of the specified role.
    /// </returns>
    /// <param name="roleName">The role to get the list of users for. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static string[] GetUsersInRole(string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      return Roles.Provider.GetUsersInRole(roleName);
    }

    /// <summary>
    /// Adds a new role to the data source.
    /// </summary>
    /// <param name="roleName">The name of the role to create. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string.-or-<paramref name="roleName"/> contains a comma.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void CreateRole(string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      Roles.Provider.CreateRole(roleName);
    }

    /// <summary>
    /// Removes a role from the data source.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="roleName"/> was deleted from the data source; otherwise; false.
    /// </returns>
    /// <param name="roleName">The name of the role to delete.</param><param name="throwOnPopulatedRole">If true, throws an exception if <paramref name="roleName"/> has one or more members.</param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string.</exception><exception cref="T:System.Configuration.Provider.ProviderException"><paramref name="roleName"/> has one or more members and <paramref name="throwOnPopulatedRole"/> is true.-or-Role management is not enabled.</exception>
    public static bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      bool flag = Roles.Provider.DeleteRole(roleName, throwOnPopulatedRole);
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal != null)
        {
          if (rolePrincipal.ProviderName == Roles.Provider.Name)
          {
            if (rolePrincipal.IsRoleListCached)
            {
              if (((ClaimsPrincipal) rolePrincipal).IsInRole(roleName))
                rolePrincipal.SetDirty();
            }
          }
        }
      }
      catch
      {
      }
      return flag;
    }

    /// <summary>
    /// Removes a role from the data source.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="roleName"/> was deleted from the data source; otherwise, false.
    /// </returns>
    /// <param name="roleName">The name of the role to delete. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException"><paramref name="roleName"/> has one or more members.-or-Role management is not enabled.</exception>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static bool DeleteRole(string roleName)
    {
      return Roles.DeleteRole(roleName, true);
    }

    /// <summary>
    /// Gets a value indicating whether the specified role name already exists in the role data source.
    /// </summary>
    /// 
    /// <returns>
    /// true if the role name already exists in the data source; otherwise, false.
    /// </returns>
    /// <param name="roleName">The name of the role to search for in the data source. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null (Nothing in Visual Basic).</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static bool RoleExists(string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      return Roles.Provider.RoleExists(roleName);
    }

    /// <summary>
    /// Adds the specified user to the specified role.
    /// </summary>
    /// <param name="username">The user name to add to the specified role.</param><param name="roleName">The role to add the specified user name to. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-<paramref name="username"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).-or-<paramref name="username"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled. -or-User is already assigned to the specified role.</exception>
    public static void AddUserToRole(string username, string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      Roles.Provider.AddUsersToRoles(new string[1]
      {
        username
      }, new string[1]
      {
        roleName
      });
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || (!rolePrincipal.IsRoleListCached || !StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, username)))
          return;
        rolePrincipal.SetDirty();
      }
      catch
      {
      }
    }

    /// <summary>
    /// Adds the specified user to the specified roles.
    /// </summary>
    /// <param name="username">The user name to add to the specified roles. </param><param name="roleNames">A string array of roles to add the specified user name to. </param><exception cref="T:System.ArgumentNullException">One of the roles in <paramref name="roleNames"/> is null.-or-<paramref name="username"/> is null.</exception><exception cref="T:System.ArgumentException">One of the roles in <paramref name="roleNames"/> is an empty string or contains a comma (,).-or-<paramref name="username"/> is an empty string or contains a comma (,).-or-<paramref name="roleNames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void AddUserToRoles(string username, string[] roleNames)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 0, "roleNames");
      Roles.Provider.AddUsersToRoles(new string[1]
      {
        username
      }, roleNames);
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || (!rolePrincipal.IsRoleListCached || !StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, username)))
          return;
        rolePrincipal.SetDirty();
      }
      catch
      {
      }
    }

    /// <summary>
    /// Adds the specified users to the specified role.
    /// </summary>
    /// <param name="usernames">A string array of user names to add to the specified role. </param><param name="roleName">The role to add the specified user names to.</param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-One of the elements in <paramref name="usernames"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).-or-One of the elements in <paramref name="usernames"/> is an empty string or contains a comma (,).-or-<paramref name="usernames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void AddUsersToRole(string[] usernames, string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      SecUtility.CheckArrayParameter(ref usernames, true, true, true, 0, "usernames");
      Roles.Provider.AddUsersToRoles(usernames, new string[1]
      {
        roleName
      });
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || !rolePrincipal.IsRoleListCached)
          return;
        foreach (string s2 in usernames)
        {
          if (StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, s2))
          {
            rolePrincipal.SetDirty();
            break;
          }
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Adds the specified users to the specified roles.
    /// </summary>
    /// <param name="usernames">A string array of user names to add to the specified roles. </param><param name="roleNames">A string array of role names to add the specified user names to. </param><exception cref="T:System.ArgumentNullException">One of the roles in <paramref name="roleNames"/> is null.-or-One of the users in <paramref name="usernames"/> is null.</exception><exception cref="T:System.ArgumentException">One of the roles in <paramref name="roleNames"/> is an empty string or contains a comma (,).-or-One of the users in <paramref name="usernames"/> is an empty string or contains a comma (,).-or-<paramref name="roleNames"/> contains a duplicate element.-or-<paramref name="usernames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 0, "roleNames");
      SecUtility.CheckArrayParameter(ref usernames, true, true, true, 0, "usernames");
      Roles.Provider.AddUsersToRoles(usernames, roleNames);
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || !rolePrincipal.IsRoleListCached)
          return;
        foreach (string s2 in usernames)
        {
          if (StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, s2))
          {
            rolePrincipal.SetDirty();
            break;
          }
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Removes the specified user from the specified role.
    /// </summary>
    /// <param name="username">The user to remove from the specified role.</param><param name="roleName">The role to remove the specified user from.</param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-<paramref name="username"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,)<paramref name="username"/> is an empty string or contains a comma (,).</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void RemoveUserFromRole(string username, string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      Roles.Provider.RemoveUsersFromRoles(new string[1]
      {
        username
      }, new string[1]
      {
        roleName
      });
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || (!rolePrincipal.IsRoleListCached || !StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, username)))
          return;
        rolePrincipal.SetDirty();
      }
      catch
      {
      }
    }

    /// <summary>
    /// Removes the specified user from the specified roles.
    /// </summary>
    /// <param name="username">The user to remove from the specified roles. </param><param name="roleNames">A string array of role names to remove the specified user from. </param><exception cref="T:System.ArgumentNullException">One of the roles in <paramref name="roleNames"/> is null.-or-<paramref name="username"/> is null.</exception><exception cref="T:System.ArgumentException">One of the roles in <paramref name="roleNames"/> is an empty string or contains a comma (,).-or-<paramref name="username"/> is an empty string or contains a comma (,).-or-<paramref name="roleNames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void RemoveUserFromRoles(string username, string[] roleNames)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 0, "roleNames");
      Roles.Provider.RemoveUsersFromRoles(new string[1]
      {
        username
      }, roleNames);
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || (!rolePrincipal.IsRoleListCached || !StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, username)))
          return;
        rolePrincipal.SetDirty();
      }
      catch
      {
      }
    }

    /// <summary>
    /// Removes the specified users from the specified role.
    /// </summary>
    /// <param name="usernames">A string array of user names to remove from the specified roles. </param><param name="roleName">The name of the role to remove the specified users from. </param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null.-or-One of the user names in <paramref name="usernames"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).-or-One of the user names in <paramref name="usernames"/> is an empty string or contains a comma (,).-or-<paramref name="usernames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void RemoveUsersFromRole(string[] usernames, string roleName)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      SecUtility.CheckArrayParameter(ref usernames, true, true, true, 0, "usernames");
      Roles.Provider.RemoveUsersFromRoles(usernames, new string[1]
      {
        roleName
      });
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || !rolePrincipal.IsRoleListCached)
          return;
        foreach (string s2 in usernames)
        {
          if (StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, s2))
          {
            rolePrincipal.SetDirty();
            break;
          }
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Removes the specified user names from the specified roles.
    /// </summary>
    /// <param name="usernames">A string array of user names to remove from the specified roles. </param><param name="roleNames">A string array of role names to remove the specified users from. </param><exception cref="T:System.ArgumentNullException">One of the roles specified in <paramref name="roleNames"/> is null.-or-One of the users specified in <paramref name="usernames"/> is null.</exception><exception cref="T:System.ArgumentException">One of the roles specified in <paramref name="roleNames"/> is an empty string or contains a comma (,).-or-One of the users specified in <paramref name="usernames"/> is an empty string or contains a comma (,).-or-<paramref name="roleNames"/> contains a duplicate element.-or-<paramref name="usernames"/> contains a duplicate element.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 0, "roleNames");
      SecUtility.CheckArrayParameter(ref usernames, true, true, true, 0, "usernames");
      Roles.Provider.RemoveUsersFromRoles(usernames, roleNames);
      try
      {
        RolePrincipal rolePrincipal = Roles.GetCurrentUser() as RolePrincipal;
        if (rolePrincipal == null || !(rolePrincipal.ProviderName == Roles.Provider.Name) || !rolePrincipal.IsRoleListCached)
          return;
        foreach (string s2 in usernames)
        {
          if (StringUtil.EqualsIgnoreCase(((ClaimsPrincipal) rolePrincipal).get_Identity().Name, s2))
          {
            rolePrincipal.SetDirty();
            break;
          }
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Gets a list of all the roles for the application.
    /// </summary>
    /// 
    /// <returns>
    /// A string array containing the names of all the roles stored in the data source for the application.
    /// </returns>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static string[] GetAllRoles()
    {
      Roles.EnsureEnabled();
      return Roles.Provider.GetAllRoles();
    }

    /// <summary>
    /// Deletes the cookie where role names are cached.
    /// </summary>
    /// <exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static void DeleteCookie()
    {
      Roles.EnsureEnabled();
      if (Roles.CookieName == null || Roles.CookieName.Length < 1)
        return;
      HttpContext current = HttpContext.Current;
      if (current == null || !current.Request.Browser.Cookies)
        return;
      string str = string.Empty;
      if (current.Request.Browser["supportsEmptyStringInCookieValue"] == "false")
        str = "NoCookie";
      HttpCookie cookie = new HttpCookie(Roles.CookieName, str);
      cookie.HttpOnly = true;
      cookie.Path = Roles.CookiePath;
      cookie.Domain = Roles.Domain;
      cookie.Expires = new DateTime(1999, 10, 12);
      cookie.Secure = Roles.CookieRequireSSL;
      current.Response.Cookies.RemoveCookie(Roles.CookieName);
      current.Response.Cookies.Add(cookie);
    }

    /// <summary>
    /// Gets a list of users in a specified role where the user name contains the specified user name to match.
    /// </summary>
    /// 
    /// <returns>
    /// A string array containing the names of all the users whose user name matches <paramref name="usernameToMatch"/> and who are members of the specified role.
    /// </returns>
    /// <param name="roleName">The role to search in.</param><param name="usernameToMatch">The user name to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="roleName"/> is null (Nothing in Visual Basic).-or-<paramref name="usernameToMatch"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="roleName"/> is an empty string or contains a comma (,).-or-<paramref name="usernameToMatch"/> is an empty string.</exception><exception cref="T:System.Configuration.Provider.ProviderException">Role management is not enabled.</exception>
    public static string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      Roles.EnsureEnabled();
      SecUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
      SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 0, "usernameToMatch");
      return Roles.Provider.FindUsersInRole(roleName, usernameToMatch);
    }

    private static void EnsureEnabled()
    {
      Roles.Initialize();
      if (!Roles.s_Enabled)
        throw new ProviderException(SR.GetString("Roles_feature_not_enabled"));
    }

    private static void Initialize()
    {
      if (Roles.s_Initialized)
      {
        if (Roles.s_InitializeException != null)
          throw Roles.s_InitializeException;
        if (Roles.s_InitializedDefaultProvider)
          return;
      }
      lock (Roles.s_lock)
      {
        if (Roles.s_Initialized)
        {
          if (Roles.s_InitializeException != null)
            throw Roles.s_InitializeException;
          if (Roles.s_InitializedDefaultProvider)
            return;
        }
        try
        {
          if (HostingEnvironment.IsHosted)
            HttpRuntime.CheckAspNetHostingPermission(AspNetHostingPermissionLevel.Low, "Feature_not_supported_at_this_level");
          RoleManagerSection local_0 = RuntimeConfig.GetAppConfig().RoleManager;
          if (!Roles.s_EnabledSet)
            Roles.s_Enabled = local_0.Enabled;
          Roles.s_CookieName = local_0.CookieName;
          Roles.s_CacheRolesInCookie = local_0.CacheRolesInCookie;
          Roles.s_CookieTimeout = (int) local_0.CookieTimeout.TotalMinutes;
          Roles.s_CookiePath = local_0.CookiePath;
          Roles.s_CookieRequireSSL = local_0.CookieRequireSSL;
          Roles.s_CookieSlidingExpiration = local_0.CookieSlidingExpiration;
          Roles.s_CookieProtection = local_0.CookieProtection;
          Roles.s_Domain = local_0.Domain;
          Roles.s_CreatePersistentCookie = local_0.CreatePersistentCookie;
          Roles.s_MaxCachedResults = local_0.MaxCachedResults;
          if (Roles.s_Enabled)
          {
            if (Roles.s_MaxCachedResults < 0)
            {
              throw new ProviderException(SR.GetString("Value_must_be_non_negative_integer", new object[1]
              {
                (object) "maxCachedResults"
              }));
            }
            else
            {
              Roles.InitializeSettings(local_0);
              Roles.InitializeDefaultProvider(local_0);
            }
          }
        }
        catch (Exception exception_0)
        {
          Roles.s_InitializeException = exception_0;
        }
        Roles.s_Initialized = true;
      }
      if (Roles.s_InitializeException != null)
        throw Roles.s_InitializeException;
    }

    private static void InitializeSettings(RoleManagerSection settings)
    {
      if (Roles.s_Initialized)
        return;
      Roles.s_Providers = new RoleProviderCollection();
      if (HostingEnvironment.IsHosted)
      {
        ProvidersHelper.InstantiateProviders(settings.Providers, (ProviderCollection) Roles.s_Providers, typeof (RoleProvider));
      }
      else
      {
        foreach (ProviderSettings providerSettings in (ConfigurationElementCollection) settings.Providers)
        {
          Type type = Type.GetType(providerSettings.Type, true, true);
          if (!typeof (RoleProvider).IsAssignableFrom(type))
          {
            throw new ArgumentException(SR.GetString("Provider_must_implement_type", new object[1]
            {
              (object) typeof (RoleProvider).ToString()
            }));
          }
          else
          {
            RoleProvider roleProvider = (RoleProvider) Activator.CreateInstance(type);
            NameValueCollection parameters = providerSettings.Parameters;
            NameValueCollection config = new NameValueCollection(parameters.Count, (IEqualityComparer) StringComparer.Ordinal);
            foreach (string index in (NameObjectCollectionBase) parameters)
              config[index] = parameters[index];
            roleProvider.Initialize(providerSettings.Name, config);
            Roles.s_Providers.Add((ProviderBase) roleProvider);
          }
        }
      }
    }

    private static void InitializeDefaultProvider(RoleManagerSection settings)
    {
      bool flag = !HostingEnvironment.IsHosted || BuildManager.PreStartInitStage == PreStartInitStage.AfterPreStartInit;
      if (Roles.s_InitializedDefaultProvider || !flag)
        return;
      Roles.s_Providers.SetReadOnly();
      if (settings.DefaultProvider == null)
      {
        Roles.s_InitializeException = (Exception) new ProviderException(SR.GetString("Def_role_provider_not_specified"));
      }
      else
      {
        try
        {
          Roles.s_Provider = Roles.s_Providers[settings.DefaultProvider];
        }
        catch
        {
        }
      }
      if (Roles.s_Provider == null)
        Roles.s_InitializeException = (Exception) new ConfigurationErrorsException(SR.GetString("Def_role_provider_not_found"), settings.ElementInformation.Properties["defaultProvider"].Source, settings.ElementInformation.Properties["defaultProvider"].LineNumber);
      Roles.s_InitializedDefaultProvider = true;
    }

    private static string GetCurrentUserName()
    {
      IPrincipal currentUser = Roles.GetCurrentUser();
      if (currentUser == null || currentUser.Identity == null)
        return string.Empty;
      else
        return currentUser.Identity.Name;
    }

    private static IPrincipal GetCurrentUser()
    {
      if (HostingEnvironment.IsHosted)
      {
        HttpContext current = HttpContext.Current;
        if (current != null)
          return current.User;
      }
      return Thread.CurrentPrincipal;
    }
  }
}
