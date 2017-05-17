// Type: System.Net.Mail.MailAddress
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Globalization;
using System.Net.Mime;
using System.Runtime;
using System.Text;

namespace System.Net.Mail
{
  /// <summary>
  /// Represents the address of an electronic mail sender or recipient.
  /// </summary>
  public class MailAddress
  {
    private static EncodedStreamFactory encoderFactory = new EncodedStreamFactory();
    private readonly Encoding displayNameEncoding;
    private readonly string displayName;
    private readonly string userName;
    private readonly string host;

    /// <summary>
    /// Gets the display name composed from the display name and address information specified when this instance was created.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the display name; otherwise, <see cref="F:System.String.Empty"/> ("") if no display name information was specified when this instance was created.
    /// </returns>
    public string DisplayName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.displayName;
      }
    }

    /// <summary>
    /// Gets the user information from the address specified when this instance was created.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the user name portion of the <see cref="P:System.Net.Mail.MailAddress.Address"/>.
    /// </returns>
    public string User
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.userName;
      }
    }

    /// <summary>
    /// Gets the host portion of the address specified when this instance was created.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the name of the host computer that accepts e-mail for the <see cref="P:System.Net.Mail.MailAddress.User"/> property.
    /// </returns>
    public string Host
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.host;
      }
    }

    /// <summary>
    /// Gets the e-mail address specified when this instance was created.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the e-mail address.
    /// </returns>
    public string Address
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", new object[2]
        {
          (object) this.userName,
          (object) this.host
        });
      }
    }

    string SmtpAddress
    {
      private get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0}>", new object[1]
        {
          (object) this.Address
        });
      }
    }

    static MailAddress()
    {
    }

    internal MailAddress(string displayName, string userName, string domain)
    {
      this.host = domain;
      this.userName = userName;
      this.displayName = displayName;
      this.displayNameEncoding = Encoding.GetEncoding("utf-8");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailAddress"/> class using the specified address.
    /// </summary>
    /// <param name="address">A <see cref="T:System.String"/> that contains an e-mail address.</param><exception cref="T:System.ArgumentNullException"><paramref name="address"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="address"/> is <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.FormatException"><paramref name="address"/> is not in a recognized format.</exception>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public MailAddress(string address)
      : this(address, (string) null, (Encoding) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailAddress"/> class using the specified address and display name.
    /// </summary>
    /// <param name="address">A <see cref="T:System.String"/> that contains an e-mail address.</param><param name="displayName">A <see cref="T:System.String"/> that contains the display name associated with <paramref name="address"/>. This parameter can be null.</param><exception cref="T:System.ArgumentNullException"><paramref name="address"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="address"/> is <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.FormatException"><paramref name="address"/> is not in a recognized format.-or-<paramref name="address"/> contains non-ASCII characters.</exception>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public MailAddress(string address, string displayName)
      : this(address, displayName, (Encoding) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailAddress"/> class using the specified address, display name, and encoding.
    /// </summary>
    /// <param name="address">A <see cref="T:System.String"/> that contains an e-mail address.</param><param name="displayName">A <see cref="T:System.String"/> that contains the display name associated with <paramref name="address"/>.</param><param name="displayNameEncoding">The <see cref="T:System.Text.Encoding"/> that defines the character set used for <paramref name="displayName"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="address"/> is null.-or-<paramref name="displayName"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="address"/> is <see cref="F:System.String.Empty"/> ("").-or-<paramref name="displayName"/> is <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.FormatException"><paramref name="address"/> is not in a recognized format.-or-<paramref name="address"/> contains non-ASCII characters.</exception>
    public MailAddress(string address, string displayName, Encoding displayNameEncoding)
    {
      if (address == null)
        throw new ArgumentNullException("address");
      if (address == string.Empty)
      {
        throw new ArgumentException(SR.GetString("net_emptystringcall", new object[1]
        {
          (object) "address"
        }), "address");
      }
      else
      {
        this.displayNameEncoding = displayNameEncoding ?? Encoding.GetEncoding("utf-8");
        this.displayName = displayName ?? string.Empty;
        if (!string.IsNullOrEmpty(this.displayName))
        {
          this.displayName = MailAddressParser.NormalizeOrThrow(this.displayName);
          if (this.displayName.Length >= 2 && (int) this.displayName[0] == 34 && (int) this.displayName[this.displayName.Length - 1] == 34)
            this.displayName = this.displayName.Substring(1, this.displayName.Length - 2);
        }
        MailAddress mailAddress = MailAddressParser.ParseAddress(address);
        this.host = mailAddress.host;
        this.userName = mailAddress.userName;
        if (!string.IsNullOrEmpty(this.displayName))
          return;
        this.displayName = mailAddress.displayName;
      }
    }

    internal string GetSmtpAddress(bool allowUnicode)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0}>", new object[1]
      {
        (object) this.GetAddress(allowUnicode)
      });
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the contents of this <see cref="T:System.Net.Mail.MailAddress"/>.
    /// </returns>
    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.DisplayName))
        return this.Address;
      else
        return string.Format("\"{0}\" {1}", (object) this.DisplayName, (object) this.SmtpAddress);
    }

    /// <summary>
    /// Compares two mail addresses.
    /// </summary>
    /// 
    /// <returns>
    /// true if the two mail addresses are equal; otherwise, false.
    /// </returns>
    /// <param name="value">A <see cref="T:System.Net.Mail.MailAddress"/> instance to compare to the current instance.</param>
    public override bool Equals(object value)
    {
      if (value == null)
        return false;
      else
        return this.ToString().Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Returns a hash value for a mail address.
    /// </summary>
    /// 
    /// <returns>
    /// An integer hash value.
    /// </returns>
    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
    }

    internal string Encode(int charsConsumed, bool allowUnicode)
    {
      string str1 = string.Empty;
      string str2;
      if (!string.IsNullOrEmpty(this.displayName))
      {
        string str3;
        if (MimeBasePart.IsAscii(this.displayName, false) || allowUnicode)
        {
          str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
          {
            (object) this.displayName
          });
        }
        else
        {
          IEncodableStream encoderForHeader = MailAddress.encoderFactory.GetEncoderForHeader(this.displayNameEncoding, false, charsConsumed);
          byte[] bytes = this.displayNameEncoding.GetBytes(this.displayName);
          encoderForHeader.EncodeBytes(bytes, 0, bytes.Length);
          str3 = encoderForHeader.GetEncodedString();
        }
        str2 = str3 + " " + this.GetSmtpAddress(allowUnicode);
      }
      else
        str2 = this.GetAddress(allowUnicode);
      return str2;
    }

    private string GetUser(bool allowUnicode)
    {
      if (allowUnicode || MimeBasePart.IsAscii(this.userName, true))
        return this.userName;
      throw new SmtpException(SR.GetString("SmtpNonAsciiUserNotSupported", new object[1]
      {
        (object) this.Address
      }));
    }

    private string GetHost(bool allowUnicode)
    {
      string unicode = this.host;
      if (!allowUnicode && !MimeBasePart.IsAscii(unicode, true))
      {
        IdnMapping idnMapping = new IdnMapping();
        try
        {
          unicode = idnMapping.GetAscii(unicode);
        }
        catch (ArgumentException ex)
        {
          throw new SmtpException(SR.GetString("SmtpInvalidHostName", new object[1]
          {
            (object) this.Address
          }), (Exception) ex);
        }
      }
      return unicode;
    }

    private string GetAddress(bool allowUnicode)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", new object[2]
      {
        (object) this.GetUser(allowUnicode),
        (object) this.GetHost(allowUnicode)
      });
    }
  }
}
