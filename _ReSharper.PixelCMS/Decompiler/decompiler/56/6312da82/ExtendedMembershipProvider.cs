// Type: WebMatrix.WebData.ExtendedMembershipProvider
// Assembly: WebMatrix.WebData, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\WebMatrix.WebData\v4.0_2.0.0.0__31bf3856ad364e35\WebMatrix.WebData.dll

using System;
using System.Collections.Generic;
using System.Web.Security;

namespace WebMatrix.WebData
{
  /// <summary>
  /// Represents an abstract class that is used to extend the membership system that is provided by the <see cref="T:System.Web.Security.MembershipProvider"/> class.
  /// </summary>
  public abstract class ExtendedMembershipProvider : MembershipProvider
  {
    private const int OneDayInMinutes = 1440;

    /// <summary>
    /// When overridden in a derived class, deletes the OAuth or OpenID account with the specified provider name and provider user ID.
    /// </summary>
    /// <param name="provider">The name of the OAuth or OpenID provider.</param><param name="providerUserId">The OAuth or OpenID provider user ID. This is not the user ID of the user account, but the user ID on the OAuth or Open ID provider.</param>
    public virtual void DeleteOAuthAccount(string provider, string providerUserId)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// When overridden in a derived class, creates a new OAuth membership account, or updates an existing OAuth Membership account.
    /// </summary>
    /// <param name="provider">The OAuth or OpenID provider.</param><param name="providerUserId">The OAuth or OpenID provider user ID. This is not the user ID of the user account, but the user ID on the OAuth or Open ID provider.</param><param name="userName">The user name.</param>
    public virtual void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// When overridden in a derived class, returns the user ID for the specified OAuth or OpenID provider and provider user ID.
    /// </summary>
    /// <param name="provider">The name of the OAuth or OpenID provider.</param><param name="providerUserId">The OAuth or OpenID provider user ID. This is not the user ID of the user account, but the user ID on the OAuth or Open ID provider.</param>
    public virtual int GetUserIdFromOAuth(string provider, string providerUserId)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the user name that is associated with the specified user ID.
    /// </summary>
    /// 
    /// <returns>
    /// The user name.
    /// </returns>
    /// <param name="userId">The user ID to get the name for.</param>
    public virtual string GetUserNameFromId(int userId)
    {
      throw new NotImplementedException();
    }

    public virtual bool HasLocalAccount(int userId)
    {
      throw new NotImplementedException();
    }

    public virtual string GetOAuthTokenSecret(string token)
    {
      throw new NotImplementedException();
    }

    public virtual void StoreOAuthRequestToken(string requestToken, string requestTokenSecret)
    {
      throw new NotImplementedException();
    }

    public virtual void ReplaceOAuthRequestTokenWithAccessToken(string requestToken, string accessToken, string accessTokenSecret)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteOAuthToken(string token)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// When overridden in a derived class, returns all OAuth membership accounts associated with the specified user name.
    /// </summary>
    /// 
    /// <returns>
    /// A list of all OAuth membership accounts associated with the specified user name.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public abstract ICollection<OAuthAccountData> GetAccountsForUser(string userName);

    /// <summary>
    /// Creates a new user profile and a new membership account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the user account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param>
    public virtual string CreateUserAndAccount(string userName, string password)
    {
      ExtendedMembershipProvider membershipProvider = this;
      bool flag = false;
      IDictionary<string, object> dictionary = (IDictionary<string, object>) null;
      string userName1 = userName;
      string password1 = password;
      int num = flag ? 1 : 0;
      IDictionary<string, object> values = dictionary;
      return membershipProvider.CreateUserAndAccount(userName1, password1, num != 0, values);
    }

    /// <summary>
    /// Creates a new user profile and a new membership account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the user account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="requireConfirmation">(Optional) true to specify that the user account must be confirmed; otherwise, false. The default is false.</param>
    public virtual string CreateUserAndAccount(string userName, string password, bool requireConfirmation)
    {
      ExtendedMembershipProvider membershipProvider = this;
      IDictionary<string, object> dictionary = (IDictionary<string, object>) null;
      string userName1 = userName;
      string password1 = password;
      int num = requireConfirmation ? 1 : 0;
      IDictionary<string, object> values = dictionary;
      return membershipProvider.CreateUserAndAccount(userName1, password1, num != 0, values);
    }

    /// <summary>
    /// When overridden in a derived class, creates a new user profile and a new membership account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the user account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="values">(Optional) A dictionary that contains additional user attributes to store in the user profile. The default is null.</param>
    public virtual string CreateUserAndAccount(string userName, string password, IDictionary<string, object> values)
    {
      ExtendedMembershipProvider membershipProvider = this;
      bool flag = false;
      IDictionary<string, object> dictionary = values;
      string userName1 = userName;
      string password1 = password;
      int num = flag ? 1 : 0;
      IDictionary<string, object> values1 = dictionary;
      return membershipProvider.CreateUserAndAccount(userName1, password1, num != 0, values1);
    }

    /// <summary>
    /// When overridden in a derived class, creates a new user profile and a new membership account.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the user account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="requireConfirmation">(Optional) true to specify that the user account must be confirmed; otherwise, false. The default is false.</param><param name="values">(Optional) A dictionary that contains additional user attributes to store in the user profile. The default is null.</param>
    public abstract string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values);

    /// <summary>
    /// Creates a new user account using the specified user name and password.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param>
    public virtual string CreateAccount(string userName, string password)
    {
      ExtendedMembershipProvider membershipProvider = this;
      bool flag = false;
      string userName1 = userName;
      string password1 = password;
      int num = flag ? 1 : 0;
      return membershipProvider.CreateAccount(userName1, password1, num != 0);
    }

    /// <summary>
    /// When overridden in a derived class, creates a new user account using the specified user name and password, optionally requiring that the new account must be confirmed before the account is available for use.
    /// </summary>
    /// 
    /// <returns>
    /// A token that can be sent to the user to confirm the account.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="password">The password.</param><param name="requireConfirmationToken">(Optional) true to specify that the account must be confirmed; otherwise, false. The default is false.</param>
    public abstract string CreateAccount(string userName, string password, bool requireConfirmationToken);

    /// <summary>
    /// Activates a pending membership account for the specified user.
    /// </summary>
    /// 
    /// <returns>
    /// true if the account is confirmed; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="accountConfirmationToken">A confirmation token to pass to the authentication provider.</param>
    public abstract bool ConfirmAccount(string userName, string accountConfirmationToken);

    /// <summary>
    /// Activates a pending membership account.
    /// </summary>
    /// 
    /// <returns>
    /// true if the account is confirmed; otherwise, false.
    /// </returns>
    /// <param name="accountConfirmationToken">A confirmation token to pass to the authentication provider.</param>
    public abstract bool ConfirmAccount(string accountConfirmationToken);

    /// <summary>
    /// When overridden in a derived class, deletes the specified membership account.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user account was deleted; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public abstract bool DeleteAccount(string userName);

    /// <summary>
    /// Generates a password reset token that can be sent to a user in email.
    /// </summary>
    /// 
    /// <returns>
    /// A token to send to the user.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public virtual string GeneratePasswordResetToken(string userName)
    {
      ExtendedMembershipProvider membershipProvider = this;
      int num = 1440;
      string userName1 = userName;
      int tokenExpirationInMinutesFromNow = num;
      return membershipProvider.GeneratePasswordResetToken(userName1, tokenExpirationInMinutesFromNow);
    }

    /// <summary>
    /// When overridden in a derived class, generates a password reset token that can be sent to a user in email.
    /// </summary>
    /// 
    /// <returns>
    /// A token to send to the user.
    /// </returns>
    /// <param name="userName">The user name.</param><param name="tokenExpirationInMinutesFromNow">(Optional) The time, in minutes, until the password reset token expires. The default is 1440 (24 hours).</param>
    public abstract string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow);

    /// <summary>
    /// When overridden in a derived class, returns an ID for a user based on a password reset token.
    /// </summary>
    /// 
    /// <returns>
    /// The user ID.
    /// </returns>
    /// <param name="token">The password reset token.</param>
    public abstract int GetUserIdFromPasswordResetToken(string token);

    /// <summary>
    /// When overridden in a derived class, returns a value that indicates whether the user account has been confirmed by the provider.
    /// </summary>
    /// 
    /// <returns>
    /// true if the user is confirmed; otherwise, false.
    /// </returns>
    /// <param name="userName">The user name.</param>
    public abstract bool IsConfirmed(string userName);

    /// <summary>
    /// When overridden in a derived class, resets a password after verifying that the specified password reset token is valid.
    /// </summary>
    /// 
    /// <returns>
    /// true if the password was changed; otherwise, false.
    /// </returns>
    /// <param name="token">A password reset token.</param><param name="newPassword">The new password.</param>
    public abstract bool ResetPasswordWithToken(string token, string newPassword);

    /// <summary>
    /// When overridden in a derived class, returns the number of times that the password for the specified user account was incorrectly entered since the most recent successful login or since the user account was created.
    /// </summary>
    /// 
    /// <returns>
    /// The count of failed password attempts for the specified user account.
    /// </returns>
    /// <param name="userName">The user name of the account.</param>
    public abstract int GetPasswordFailuresSinceLastSuccess(string userName);

    /// <summary>
    /// When overridden in a derived class, returns the date and time when the specified user account was created.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time the account was created, or <see cref="F:System.DateTime.MinValue"/> if the account creation date is not available.
    /// </returns>
    /// <param name="userName">The user name of the account.</param>
    public abstract DateTime GetCreateDate(string userName);

    /// <summary>
    /// When overridden in a derived class, returns the date and time when the password was most recently changed for the specified membership account.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time when the password was more recently changed for membership account, or <see cref="F:System.DateTime.MinValue"/> if the password has never been changed for this user account.
    /// </returns>
    /// <param name="userName">The user name of the account.</param>
    public abstract DateTime GetPasswordChangedDate(string userName);

    /// <summary>
    /// When overridden in a derived class, returns the date and time when an incorrect password was most recently entered for the specified user account.
    /// </summary>
    /// 
    /// <returns>
    /// The date and time when an incorrect password was most recently entered for this user account, or <see cref="F:System.DateTime.MinValue"/> if an incorrect password has not been entered for this user account.
    /// </returns>
    /// <param name="userName">The user name of the account.</param>
    public abstract DateTime GetLastPasswordFailureDate(string userName);

    internal virtual void VerifyInitialized()
    {
    }
  }
}
