// Type: System.Net.Mail.SmtpClient
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Runtime;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail
{
  /// <summary>
  /// Allows applications to send e-mail by using the Simple Mail Transfer Protocol (SMTP).
  /// </summary>
  public class SmtpClient : IDisposable
  {
    private static AsyncCallback _ContextSafeCompleteCallback = new AsyncCallback(SmtpClient.ContextSafeCompleteCallback);
    private static int defaultPort = 25;
    private string host;
    private int port;
    private bool inCall;
    private bool cancelled;
    private bool timedOut;
    private string targetName;
    private SmtpDeliveryMethod deliveryMethod;
    private SmtpDeliveryFormat deliveryFormat;
    private string pickupDirectoryLocation;
    private SmtpTransport transport;
    private MailMessage message;
    private MailWriter writer;
    private MailAddressCollection recipients;
    private SendOrPostCallback onSendCompletedDelegate;
    private System.Threading.Timer timer;
    private static volatile MailSettingsSectionGroupInternal mailConfiguration;
    private ContextAwareResult operationCompletedResult;
    private AsyncOperation asyncOp;
    internal string clientDomain;
    private bool disposed;
    private bool servicePointChanged;
    private ServicePoint servicePoint;
    private SmtpFailedRecipientException failedRecipientException;
    private SendCompletedEventHandler SendCompleted;
    private const int maxPortValue = 65535;

    /// <summary>
    /// Gets or sets the name or IP address of the host used for SMTP transactions.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the name or IP address of the computer to use for SMTP transactions.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The value specified for a set operation is null.</exception><exception cref="T:System.ArgumentException">The value specified for a set operation is equal to <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
    public string Host
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.host;
      }
      set
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("SmtpInvalidOperationDuringSend"));
        if (value == null)
          throw new ArgumentNullException("value");
        if (value == string.Empty)
          throw new ArgumentException(SR.GetString("net_emptystringset"), "value");
        value = value.Trim();
        if (!(value != this.host))
          return;
        this.host = value;
        this.servicePointChanged = true;
      }
    }

    /// <summary>
    /// Gets or sets the port used for SMTP transactions.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Int32"/> that contains the port number on the SMTP host. The default value is 25.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than or equal to zero.</exception><exception cref="T:System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
    public int Port
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.port;
      }
      set
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("SmtpInvalidOperationDuringSend"));
        if (value <= 0)
          throw new ArgumentOutOfRangeException("value");
        if (value != SmtpClient.defaultPort)
          new SmtpPermission(SmtpAccess.ConnectToUnrestrictedPort).Demand();
        if (value == this.port)
          return;
        this.port = value;
        this.servicePointChanged = true;
      }
    }

    /// <summary>
    /// Gets or sets a <see cref="T:System.Boolean"/> value that controls whether the <see cref="P:System.Net.CredentialCache.DefaultCredentials"/> are sent with requests.
    /// </summary>
    /// 
    /// <returns>
    /// true if the default credentials are used; otherwise false. The default value is false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">You cannot change the value of this property when an e-mail is being sent.</exception>
    public bool UseDefaultCredentials
    {
      get
      {
        return this.transport.Credentials is SystemNetworkCredential;
      }
      set
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("SmtpInvalidOperationDuringSend"));
        this.transport.Credentials = value ? (ICredentialsByHost) CredentialCache.DefaultNetworkCredentials : (ICredentialsByHost) null;
      }
    }

    /// <summary>
    /// Gets or sets the credentials used to authenticate the sender.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Net.ICredentialsByHost"/> that represents the credentials to use for authentication; or null if no credentials have been specified.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
    public ICredentialsByHost Credentials
    {
      get
      {
        return this.transport.Credentials;
      }
      set
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("SmtpInvalidOperationDuringSend"));
        this.transport.Credentials = value;
      }
    }

    /// <summary>
    /// Gets or sets a value that specifies the amount of time after which a synchronous <see cref="Overload:System.Net.Mail.SmtpClient.Send"/> call times out.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Int32"/> that specifies the time-out value in milliseconds. The default value is 100,000 (100 seconds).
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation was less than zero.</exception><exception cref="T:System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
    public int Timeout
    {
      get
      {
        return this.transport.Timeout;
      }
      set
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("SmtpInvalidOperationDuringSend"));
        if (value < 0)
          throw new ArgumentOutOfRangeException("value");
        this.transport.Timeout = value;
      }
    }

    /// <summary>
    /// Gets the network connection used to transmit the e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Net.ServicePoint"/> that connects to the <see cref="P:System.Net.Mail.SmtpClient.Host"/> property used for SMTP.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException"><see cref="P:System.Net.Mail.SmtpClient.Host"/> is null or the empty string ("").-or-<see cref="P:System.Net.Mail.SmtpClient.Port"/> is zero.</exception>
    public ServicePoint ServicePoint
    {
      get
      {
        this.CheckHostAndPort();
        if (this.servicePoint == null || this.servicePointChanged)
        {
          this.servicePoint = ServicePointManager.FindServicePoint(this.host, this.port);
          this.servicePointChanged = false;
        }
        return this.servicePoint;
      }
    }

    /// <summary>
    /// Specifies how outgoing email messages will be handled.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Net.Mail.SmtpDeliveryMethod"/> that indicates how email messages are delivered.
    /// </returns>
    public SmtpDeliveryMethod DeliveryMethod
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.deliveryMethod;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.deliveryMethod = value;
      }
    }

    /// <summary>
    /// Gets or sets the delivery format used by <see cref="T:System.Net.Mail.SmtpClient"/> to send e-mail.
    /// </summary>
    /// 
    /// <returns>
    /// Returns <see cref="T:System.Net.Mail.SmtpDeliveryFormat"/>.The delivery format used by <see cref="T:System.Net.Mail.SmtpClient"/>.
    /// </returns>
    public SmtpDeliveryFormat DeliveryFormat
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.deliveryFormat;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.deliveryFormat = value;
      }
    }

    /// <summary>
    /// Gets or sets the folder where applications save mail messages to be processed by the local SMTP server.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that specifies the pickup directory for mail messages.
    /// </returns>
    public string PickupDirectoryLocation
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.pickupDirectoryLocation;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.pickupDirectoryLocation = value;
      }
    }

    /// <summary>
    /// Specify whether the <see cref="T:System.Net.Mail.SmtpClient"/> uses Secure Sockets Layer (SSL) to encrypt the connection.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.Net.Mail.SmtpClient"/> uses SSL; otherwise, false. The default is false.
    /// </returns>
    public bool EnableSsl
    {
      get
      {
        return this.transport.EnableSsl;
      }
      set
      {
        this.transport.EnableSsl = value;
      }
    }

    /// <summary>
    /// Specify which certificates should be used to establish the Secure Sockets Layer (SSL) connection.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Security.Cryptography.X509Certificates.X509CertificateCollection"/>, holding one or more client certificates. The default value is derived from the mail configuration attributes in a configuration file.
    /// </returns>
    public X509CertificateCollection ClientCertificates
    {
      get
      {
        return this.transport.ClientCertificates;
      }
    }

    /// <summary>
    /// Gets or sets the Service Provider Name (SPN) to use for authentication when using extended protection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that specifies the SPN to use for extended protection. The default value for this SPN is of the form "SMTPSVC/&lt;host&gt;" where &lt;host&gt; is the hostname of the SMTP mail server.
    /// </returns>
    public string TargetName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.targetName;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.targetName = value;
      }
    }

    bool ServerSupportsEai
    {
      private get
      {
        return this.transport.ServerSupportsEai;
      }
    }

    internal bool InCall
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.inCall;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.inCall = value;
      }
    }

    internal static MailSettingsSectionGroupInternal MailConfiguration
    {
      get
      {
        if (SmtpClient.mailConfiguration == null)
          SmtpClient.mailConfiguration = MailSettingsSectionGroupInternal.GetSection();
        return SmtpClient.mailConfiguration;
      }
    }

    /// <summary>
    /// Occurs when an asynchronous e-mail send operation completes.
    /// </summary>
    public event SendCompletedEventHandler SendCompleted
    {
      add
      {
        SendCompletedEventHandler completedEventHandler = this.SendCompleted;
        SendCompletedEventHandler comparand;
        do
        {
          comparand = completedEventHandler;
          completedEventHandler = Interlocked.CompareExchange<SendCompletedEventHandler>(ref this.SendCompleted, comparand + value, comparand);
        }
        while (completedEventHandler != comparand);
      }
      remove
      {
        SendCompletedEventHandler completedEventHandler = this.SendCompleted;
        SendCompletedEventHandler comparand;
        do
        {
          comparand = completedEventHandler;
          completedEventHandler = Interlocked.CompareExchange<SendCompletedEventHandler>(ref this.SendCompleted, comparand - value, comparand);
        }
        while (completedEventHandler != comparand);
      }
    }

    static SmtpClient()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class by using configuration file settings.
    /// </summary>
    public SmtpClient()
    {
      if (Logging.On)
        Logging.Enter(Logging.Web, "SmtpClient", ".ctor", "");
      try
      {
        this.Initialize();
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, "SmtpClient", ".ctor", (object) this);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class that sends e-mail by using the specified SMTP server.
    /// </summary>
    /// <param name="host">A <see cref="T:System.String"/> that contains the name or IP address of the host computer used for SMTP transactions.</param>
    public SmtpClient(string host)
    {
      if (Logging.On)
        Logging.Enter(Logging.Web, "SmtpClient", ".ctor", "host=" + host);
      try
      {
        this.host = host;
        this.Initialize();
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, "SmtpClient", ".ctor", (object) this);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class that sends e-mail by using the specified SMTP server and port.
    /// </summary>
    /// <param name="host">A <see cref="T:System.String"/> that contains the name or IP address of the host used for SMTP transactions.</param><param name="port">An <see cref="T:System.Int32"/> greater than zero that contains the port to be used on <paramref name="host"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="port"/> cannot be less than zero.</exception>
    public SmtpClient(string host, int port)
    {
      if (Logging.On)
        Logging.Enter(Logging.Web, "SmtpClient", ".ctor", string.Concat(new object[4]
        {
          (object) "host=",
          (object) host,
          (object) ", port=",
          (object) port
        }));
      try
      {
        if (port < 0)
          throw new ArgumentOutOfRangeException("port");
        this.host = host;
        this.port = port;
        this.Initialize();
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, "SmtpClient", ".ctor", (object) this);
      }
    }

    internal MailWriter GetFileMailWriter(string pickupDirectory)
    {
      if (Logging.On)
        Logging.PrintInfo(Logging.Web, "SmtpClient.Send() pickupDirectory=" + pickupDirectory);
      if (!Path.IsPathRooted(pickupDirectory))
        throw new SmtpException(SR.GetString("SmtpNeedAbsolutePickupDirectory"));
      string path;
      do
      {
        string path2 = Guid.NewGuid().ToString() + ".eml";
        path = Path.Combine(pickupDirectory, path2);
      }
      while (System.IO.File.Exists(path));
      return new MailWriter((Stream) new FileStream(path, FileMode.CreateNew));
    }

    /// <summary>
    /// Raises the <see cref="E:System.Net.Mail.SmtpClient.SendCompleted"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.ComponentModel.AsyncCompletedEventArgs"/> that contains event data.</param>
    protected void OnSendCompleted(AsyncCompletedEventArgs e)
    {
      if (this.SendCompleted == null)
        return;
      this.SendCompleted((object) this, e);
    }

    /// <summary>
    /// Sends the specified e-mail message to an SMTP server for delivery. The message sender, recipients, subject, and message body are specified using <see cref="T:System.String"/> objects.
    /// </summary>
    /// <param name="from">A <see cref="T:System.String"/> that contains the address information of the message sender.</param><param name="recipients">A <see cref="T:System.String"/> that contains the addresses that the message is sent to.</param><param name="subject">A <see cref="T:System.String"/> that contains the subject line for the message.</param><param name="body">A <see cref="T:System.String"/> that contains the message body.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="recipients"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="from"/> is <see cref="F:System.String.Empty"/>.-or-<paramref name="recipients"/> is <see cref="F:System.String.Empty"/>.</exception><exception cref="T:System.InvalidOperationException">This <see cref="T:System.Net.Mail.SmtpClient"/> has a <see cref="Overload:System.Net.Mail.SmtpClient.SendAsync"/> call in progress.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is null.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is equal to the empty string ("").-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Port"/> is zero, a negative number, or greater than 65,535.</exception><exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception><exception cref="T:System.Net.Mail.SmtpException">The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.-or- <see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true but the <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory"/> or <see cref="F:System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis"/>.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.</exception><exception cref="T:System.Net.Mail.SmtpFailedRecipientsException">The message could not be delivered to one or more of the recipients in <paramref name="recipients"/>. </exception>
    public void Send(string from, string recipients, string subject, string body)
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
      this.Send(new MailMessage(from, recipients, subject, body));
    }

    /// <summary>
    /// Sends the specified message to an SMTP server for delivery.
    /// </summary>
    /// <param name="message">A <see cref="T:System.Net.Mail.MailMessage"/> that contains the message to send.</param><exception cref="T:System.ArgumentNullException"><paramref name="message"/> is null.</exception><exception cref="T:System.InvalidOperationException">This <see cref="T:System.Net.Mail.SmtpClient"/> has a <see cref="Overload:System.Net.Mail.SmtpClient.SendAsync"/> call in progress.-or- <see cref="P:System.Net.Mail.MailMessage.From"/> is null.-or- There are no recipients specified in <see cref="P:System.Net.Mail.MailMessage.To"/>, <see cref="P:System.Net.Mail.MailMessage.CC"/>, and <see cref="P:System.Net.Mail.MailMessage.Bcc"/> properties.-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is null.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is equal to the empty string ("").-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Port"/> is zero, a negative number, or greater than 65,535.</exception><exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception><exception cref="T:System.Net.Mail.SmtpException">The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true but the <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory"/> or <see cref="F:System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis"/>.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.</exception><exception cref="T:System.Net.Mail.SmtpFailedRecipientsException">The <paramref name="message"/> could not be delivered to one or more of the recipients in <see cref="P:System.Net.Mail.MailMessage.To"/>, <see cref="P:System.Net.Mail.MailMessage.CC"/>, or <see cref="P:System.Net.Mail.MailMessage.Bcc"/>.</exception>
    public void Send(MailMessage message)
    {
      if (Logging.On)
        Logging.Enter(Logging.Web, (object) this, "Send", (object) message);
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
      try
      {
        if (Logging.On)
          Logging.PrintInfo(Logging.Web, (object) this, "Send", "DeliveryMethod=" + ((object) this.DeliveryMethod).ToString());
        if (Logging.On)
          Logging.Associate(Logging.Web, (object) this, (object) message);
        SmtpFailedRecipientException exception = (SmtpFailedRecipientException) null;
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("net_inasync"));
        if (message == null)
          throw new ArgumentNullException("message");
        if (this.DeliveryMethod == SmtpDeliveryMethod.Network)
          this.CheckHostAndPort();
        MailAddressCollection recipients = new MailAddressCollection();
        if (message.From == null)
          throw new InvalidOperationException(SR.GetString("SmtpFromRequired"));
        if (message.To != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.To)
            ((Collection<MailAddress>) recipients).Add(mailAddress);
        }
        if (message.Bcc != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.Bcc)
            ((Collection<MailAddress>) recipients).Add(mailAddress);
        }
        if (message.CC != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.CC)
            ((Collection<MailAddress>) recipients).Add(mailAddress);
        }
        if (recipients.Count == 0)
          throw new InvalidOperationException(SR.GetString("SmtpRecipientRequired"));
        this.transport.IdentityRequired = false;
        try
        {
          this.InCall = true;
          this.timedOut = false;
          this.timer = new System.Threading.Timer(new TimerCallback(this.TimeOutCallback), (object) null, this.Timeout, this.Timeout);
          string pickupDirectory = this.PickupDirectoryLocation;
          bool allowUnicode;
          MailWriter mailWriter;
          switch (this.DeliveryMethod)
          {
            case SmtpDeliveryMethod.SpecifiedPickupDirectory:
              if (this.EnableSsl)
                throw new SmtpException(SR.GetString("SmtpPickupDirectoryDoesnotSupportSsl"));
              allowUnicode = this.IsUnicodeSupported();
              this.ValidateUnicodeRequirement(message, recipients, allowUnicode);
              mailWriter = this.GetFileMailWriter(pickupDirectory);
              break;
            case SmtpDeliveryMethod.PickupDirectoryFromIis:
              pickupDirectory = IisPickupDirectory.GetPickupDirectory();
              goto case 1;
            default:
              this.GetConnection();
              allowUnicode = this.IsUnicodeSupported();
              this.ValidateUnicodeRequirement(message, recipients, allowUnicode);
              mailWriter = this.transport.SendMail(message.Sender ?? message.From, recipients, message.BuildDeliveryStatusNotificationString(), allowUnicode, out exception);
              break;
          }
          this.message = message;
          message.Send((BaseWriter) mailWriter, this.DeliveryMethod != SmtpDeliveryMethod.Network, allowUnicode);
          mailWriter.Close();
          this.transport.ReleaseConnection();
          if (this.DeliveryMethod == SmtpDeliveryMethod.Network && exception != null)
            throw exception;
        }
        catch (Exception ex)
        {
          if (Logging.On)
            Logging.Exception(Logging.Web, (object) this, "Send", ex);
          if (ex is SmtpFailedRecipientException && !((SmtpFailedRecipientException) ex).fatal)
          {
            throw;
          }
          else
          {
            this.Abort();
            if (this.timedOut)
              throw new SmtpException(SR.GetString("net_timeout"));
            if (!(ex is SecurityException) && !(ex is AuthenticationException) && !(ex is SmtpException))
              throw new SmtpException(SR.GetString("SmtpSendMailFailure"), ex);
            throw;
          }
        }
        finally
        {
          this.InCall = false;
          if (this.timer != null)
            this.timer.Dispose();
        }
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, (object) this, "Send", (string) null);
      }
    }

    /// <summary>
    /// Sends an e-mail message to an SMTP server for delivery. The message sender, recipients, subject, and message body are specified using <see cref="T:System.String"/> objects. This method does not block the calling thread and allows the caller to pass an object to the method that is invoked when the operation completes.
    /// </summary>
    /// <param name="from">A <see cref="T:System.String"/> that contains the address information of the message sender.</param><param name="recipients">A <see cref="T:System.String"/> that contains the address that the message is sent to.</param><param name="subject">A <see cref="T:System.String"/> that contains the subject line for the message.</param><param name="body">A <see cref="T:System.String"/> that contains the message body.</param><param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="recipient"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="from"/> is <see cref="F:System.String.Empty"/>.-or-<paramref name="recipient"/> is <see cref="F:System.String.Empty"/>.</exception><exception cref="T:System.InvalidOperationException">This <see cref="T:System.Net.Mail.SmtpClient"/> has a <see cref="Overload:System.Net.Mail.SmtpClient.SendAsync"/> call in progress.-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is null.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is equal to the empty string ("").-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Port"/> is zero, a negative number, or greater than 65,535.</exception><exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception><exception cref="T:System.Net.Mail.SmtpException">The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.-or- <see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true but the <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory"/> or <see cref="F:System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis"/>.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.-or-The message could not be delivered to one or more of the recipients in <paramref name="recipients"/>.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public void SendAsync(string from, string recipients, string subject, string body, object userToken)
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
      this.SendAsync(new MailMessage(from, recipients, subject, body), userToken);
    }

    /// <summary>
    /// Sends the specified e-mail message to an SMTP server for delivery. This method does not block the calling thread and allows the caller to pass an object to the method that is invoked when the operation completes.
    /// </summary>
    /// <param name="message">A <see cref="T:System.Net.Mail.MailMessage"/> that contains the message to send.</param><param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param><exception cref="T:System.ArgumentNullException"><paramref name="message"/> is null.-or-<see cref="P:System.Net.Mail.MailMessage.From"/> is null.</exception><exception cref="T:System.InvalidOperationException">This <see cref="T:System.Net.Mail.SmtpClient"/> has a <see cref="Overload:System.Net.Mail.SmtpClient.SendAsync"/> call in progress.-or- There are no recipients specified in <see cref="P:System.Net.Mail.MailMessage.To"/>, <see cref="P:System.Net.Mail.MailMessage.CC"/>, and <see cref="P:System.Net.Mail.MailMessage.Bcc"/> properties.-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is null.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Host"/> is equal to the empty string ("").-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network"/> and <see cref="P:System.Net.Mail.SmtpClient.Port"/> is zero, a negative number, or greater than 65,535.</exception><exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception><exception cref="T:System.Net.Mail.SmtpException">The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.-or- <see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true but the <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod"/> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory"/> or <see cref="F:System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis"/>.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl"/> is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.-or-The <paramref name="message"/> could not be delivered to one or more of the recipients in <see cref="P:System.Net.Mail.MailMessage.To"/>, <see cref="P:System.Net.Mail.MailMessage.CC"/>, or <see cref="P:System.Net.Mail.MailMessage.Bcc"/>.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public void SendAsync(MailMessage message, object userToken)
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
      if (Logging.On)
        Logging.Enter(Logging.Web, (object) this, "SendAsync", "DeliveryMethod=" + ((object) this.DeliveryMethod).ToString());
      try
      {
        if (this.InCall)
          throw new InvalidOperationException(SR.GetString("net_inasync"));
        if (message == null)
          throw new ArgumentNullException("message");
        if (this.DeliveryMethod == SmtpDeliveryMethod.Network)
          this.CheckHostAndPort();
        this.recipients = new MailAddressCollection();
        if (message.From == null)
          throw new InvalidOperationException(SR.GetString("SmtpFromRequired"));
        if (message.To != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.To)
            ((Collection<MailAddress>) this.recipients).Add(mailAddress);
        }
        if (message.Bcc != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.Bcc)
            ((Collection<MailAddress>) this.recipients).Add(mailAddress);
        }
        if (message.CC != null)
        {
          foreach (MailAddress mailAddress in (Collection<MailAddress>) message.CC)
            ((Collection<MailAddress>) this.recipients).Add(mailAddress);
        }
        if (this.recipients.Count == 0)
          throw new InvalidOperationException(SR.GetString("SmtpRecipientRequired"));
        try
        {
          this.InCall = true;
          this.cancelled = false;
          this.message = message;
          string pickupDirectory = this.PickupDirectoryLocation;
          CredentialCache credentialCache;
          this.transport.IdentityRequired = this.Credentials != null && (this.Credentials is SystemNetworkCredential || (credentialCache = this.Credentials as CredentialCache) == null || credentialCache.IsDefaultInCache);
          this.asyncOp = AsyncOperationManager.CreateOperation(userToken);
          switch (this.DeliveryMethod)
          {
            case SmtpDeliveryMethod.SpecifiedPickupDirectory:
              if (this.EnableSsl)
                throw new SmtpException(SR.GetString("SmtpPickupDirectoryDoesnotSupportSsl"));
              this.writer = this.GetFileMailWriter(pickupDirectory);
              bool allowUnicode = this.IsUnicodeSupported();
              this.ValidateUnicodeRequirement(message, this.recipients, allowUnicode);
              message.Send((BaseWriter) this.writer, true, allowUnicode);
              if (this.writer != null)
                this.writer.Close();
              this.transport.ReleaseConnection();
              AsyncCompletedEventArgs completedEventArgs = new AsyncCompletedEventArgs((Exception) null, false, this.asyncOp.UserSuppliedState);
              this.InCall = false;
              this.asyncOp.PostOperationCompleted(this.onSendCompletedDelegate, (object) completedEventArgs);
              break;
            case SmtpDeliveryMethod.PickupDirectoryFromIis:
              pickupDirectory = IisPickupDirectory.GetPickupDirectory();
              goto case 1;
            default:
              this.operationCompletedResult = new ContextAwareResult(this.transport.IdentityRequired, true, (object) null, (object) this, SmtpClient._ContextSafeCompleteCallback);
              lock (this.operationCompletedResult.StartPostingAsyncOp())
              {
                this.transport.BeginGetConnection(this.ServicePoint, this.operationCompletedResult, new AsyncCallback(this.ConnectCallback), (object) this.operationCompletedResult);
                this.operationCompletedResult.FinishPostingAsyncOp();
                break;
              }
          }
        }
        catch (Exception ex)
        {
          this.InCall = false;
          if (Logging.On)
            Logging.Exception(Logging.Web, (object) this, "Send", ex);
          if (ex is SmtpFailedRecipientException && !((SmtpFailedRecipientException) ex).fatal)
          {
            throw;
          }
          else
          {
            this.Abort();
            if (this.timedOut)
              throw new SmtpException(SR.GetString("net_timeout"));
            if (!(ex is SecurityException) && !(ex is AuthenticationException) && !(ex is SmtpException))
              throw new SmtpException(SR.GetString("SmtpSendMailFailure"), ex);
            throw;
          }
        }
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, (object) this, "SendAsync", (string) null);
      }
    }

    /// <summary>
    /// Cancels an asynchronous operation to send an e-mail message.
    /// </summary>
    /// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
    public void SendAsyncCancel()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
      if (Logging.On)
        Logging.Enter(Logging.Web, (object) this, "SendAsyncCancel", (string) null);
      try
      {
        if (!this.InCall || this.cancelled)
          return;
        this.cancelled = true;
        this.Abort();
      }
      finally
      {
        if (Logging.On)
          Logging.Exit(Logging.Web, (object) this, "SendAsyncCancel", (string) null);
      }
    }

    /// <summary>
    /// Sends the specified message to an SMTP server for delivery as an asynchronous operation. . The message sender, recipients, subject, and message body are specified using <see cref="T:System.String"/> objects.
    /// </summary>
    /// 
    /// <returns>
    /// Returns <see cref="T:System.Threading.Tasks.Task"/>.The task object representing the asynchronous operation.
    /// </returns>
    /// <param name="from">A <see cref="T:System.String"/> that contains the address information of the message sender.</param><param name="recipients">A <see cref="T:System.String"/> that contains the addresses that the message is sent to.</param><param name="subject">A <see cref="T:System.String"/> that contains the subject line for the message.</param><param name="body">A <see cref="T:System.String"/> that contains the message body.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="recipients"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="from"/> is <see cref="F:System.String.Empty"/>.-or-<paramref name="recipients"/> is <see cref="F:System.String.Empty"/>.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public Task SendMailAsync(string from, string recipients, string subject, string body)
    {
      return this.SendMailAsync(new MailMessage(from, recipients, subject, body));
    }

    /// <summary>
    /// Sends the specified message to an SMTP server for delivery as an asynchronous operation.
    /// </summary>
    /// 
    /// <returns>
    /// Returns <see cref="T:System.Threading.Tasks.Task"/>.The task object representing the asynchronous operation.
    /// </returns>
    /// <param name="message">A <see cref="T:System.Net.Mail.MailMessage"/> that contains the message to send.</param><exception cref="T:System.ArgumentNullException"><paramref name="message"/> is null.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public Task SendMailAsync(MailMessage message)
    {
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      SendCompletedEventHandler handler = (SendCompletedEventHandler) null;
      handler = (SendCompletedEventHandler) ((sender, e) => this.HandleCompletion(tcs, e, handler));
      this.SendCompleted += handler;
      try
      {
        this.SendAsync(message, (object) tcs);
      }
      catch
      {
        this.SendCompleted -= handler;
        throw;
      }
      return (Task) tcs.Task;
    }

    /// <summary>
    /// Sends a QUIT message to the SMTP server, gracefully ends the TCP connection, and releases all resources used by the current instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Sends a QUIT message to the SMTP server, gracefully ends the TCP connection, releases all resources used by the current instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class, and optionally disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      if (this.InCall && !this.cancelled)
      {
        this.cancelled = true;
        this.Abort();
      }
      if (this.transport != null && this.servicePoint != null)
        this.transport.CloseIdleConnections(this.servicePoint);
      if (this.timer != null)
        this.timer.Dispose();
      this.disposed = true;
    }

    private void Initialize()
    {
      if (this.port == SmtpClient.defaultPort || this.port == 0)
        new SmtpPermission(SmtpAccess.Connect).Demand();
      else
        new SmtpPermission(SmtpAccess.ConnectToUnrestrictedPort).Demand();
      this.transport = new SmtpTransport(this);
      if (Logging.On)
        Logging.Associate(Logging.Web, (object) this, (object) this.transport);
      this.onSendCompletedDelegate = new SendOrPostCallback(this.SendCompletedWaitCallback);
      if (SmtpClient.MailConfiguration.Smtp != null)
      {
        if (SmtpClient.MailConfiguration.Smtp.Network != null)
        {
          if (this.host == null || this.host.Length == 0)
            this.host = SmtpClient.MailConfiguration.Smtp.Network.Host;
          if (this.port == 0)
            this.port = SmtpClient.MailConfiguration.Smtp.Network.Port;
          this.transport.Credentials = (ICredentialsByHost) SmtpClient.MailConfiguration.Smtp.Network.Credential;
          this.transport.EnableSsl = SmtpClient.MailConfiguration.Smtp.Network.EnableSsl;
          if (SmtpClient.MailConfiguration.Smtp.Network.TargetName != null)
            this.targetName = SmtpClient.MailConfiguration.Smtp.Network.TargetName;
          this.clientDomain = SmtpClient.MailConfiguration.Smtp.Network.ClientDomain;
        }
        this.deliveryFormat = SmtpClient.MailConfiguration.Smtp.DeliveryFormat;
        this.deliveryMethod = SmtpClient.MailConfiguration.Smtp.DeliveryMethod;
        if (SmtpClient.MailConfiguration.Smtp.SpecifiedPickupDirectory != null)
          this.pickupDirectoryLocation = SmtpClient.MailConfiguration.Smtp.SpecifiedPickupDirectory.PickupDirectoryLocation;
      }
      if (this.host != null && this.host.Length != 0)
        this.host = this.host.Trim();
      if (this.port == 0)
        this.port = SmtpClient.defaultPort;
      if (this.targetName == null)
        this.targetName = "SMTPSVC/" + this.host;
      if (this.clientDomain != null)
        return;
      string unicode = IPGlobalProperties.InternalGetIPGlobalProperties().HostName;
      IdnMapping idnMapping = new IdnMapping();
      try
      {
        unicode = idnMapping.GetAscii(unicode);
      }
      catch (ArgumentException ex)
      {
      }
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < unicode.Length; ++index)
      {
        char ch = unicode[index];
        if ((int) ch <= (int) sbyte.MaxValue)
          stringBuilder.Append(ch);
      }
      if (stringBuilder.Length > 0)
        this.clientDomain = ((object) stringBuilder).ToString();
      else
        this.clientDomain = "LocalHost";
    }

    private bool IsUnicodeSupported()
    {
      if (this.DeliveryMethod == SmtpDeliveryMethod.Network && !this.ServerSupportsEai)
        return false;
      else
        return this.DeliveryFormat == SmtpDeliveryFormat.International;
    }

    private void SendCompletedWaitCallback(object operationState)
    {
      this.OnSendCompleted((AsyncCompletedEventArgs) operationState);
    }

    private void HandleCompletion(TaskCompletionSource<object> tcs, AsyncCompletedEventArgs e, SendCompletedEventHandler handler)
    {
      if (e.UserState != tcs)
        return;
      try
      {
        this.SendCompleted -= handler;
      }
      finally
      {
        if (e.Error != null)
          tcs.TrySetException(e.Error);
        else if (e.Cancelled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult((object) null);
      }
    }

    private void CheckHostAndPort()
    {
      if (this.host == null || this.host.Length == 0)
        throw new InvalidOperationException(SR.GetString("UnspecifiedHost"));
      if (this.port <= 0 || this.port > (int) ushort.MaxValue)
        throw new InvalidOperationException(SR.GetString("InvalidPort"));
    }

    private void TimeOutCallback(object state)
    {
      if (this.timedOut)
        return;
      this.timedOut = true;
      this.Abort();
    }

    private void Complete(Exception exception, IAsyncResult result)
    {
      ContextAwareResult contextAwareResult = (ContextAwareResult) result.AsyncState;
      try
      {
        if (this.cancelled)
        {
          exception = (Exception) null;
          this.Abort();
        }
        else if (exception != null && (!(exception is SmtpFailedRecipientException) || ((SmtpFailedRecipientException) exception).fatal))
        {
          this.Abort();
          if (exception is SmtpException)
            return;
          exception = (Exception) new SmtpException(SR.GetString("SmtpSendMailFailure"), exception);
        }
        else
        {
          if (this.writer != null)
          {
            try
            {
              this.writer.Close();
            }
            catch (SmtpException ex)
            {
              exception = (Exception) ex;
            }
          }
          this.transport.ReleaseConnection();
        }
      }
      finally
      {
        contextAwareResult.InvokeCallback((object) exception);
      }
    }

    private static void ContextSafeCompleteCallback(IAsyncResult ar)
    {
      ContextAwareResult contextAwareResult = (ContextAwareResult) ar;
      SmtpClient smtpClient = (SmtpClient) ar.AsyncState;
      Exception error = contextAwareResult.Result as Exception;
      AsyncOperation asyncOperation = smtpClient.asyncOp;
      AsyncCompletedEventArgs completedEventArgs = new AsyncCompletedEventArgs(error, smtpClient.cancelled, asyncOperation.UserSuppliedState);
      smtpClient.InCall = false;
      smtpClient.failedRecipientException = (SmtpFailedRecipientException) null;
      asyncOperation.PostOperationCompleted(smtpClient.onSendCompletedDelegate, (object) completedEventArgs);
    }

    private void SendMessageCallback(IAsyncResult result)
    {
      try
      {
        this.message.EndSend(result);
        this.Complete((Exception) this.failedRecipientException, result);
      }
      catch (Exception ex)
      {
        this.Complete(ex, result);
      }
    }

    private void SendMailCallback(IAsyncResult result)
    {
      try
      {
        this.writer = this.transport.EndSendMail(result);
        this.failedRecipientException = ((SendMailAsyncResult) result).GetFailedRecipientException();
      }
      catch (Exception ex)
      {
        this.Complete(ex, result);
        return;
      }
      try
      {
        if (this.cancelled)
          this.Complete((Exception) null, result);
        else
          this.message.BeginSend((BaseWriter) this.writer, this.DeliveryMethod != SmtpDeliveryMethod.Network, this.ServerSupportsEai, new AsyncCallback(this.SendMessageCallback), result.AsyncState);
      }
      catch (Exception ex)
      {
        this.Complete(ex, result);
      }
    }

    private void ConnectCallback(IAsyncResult result)
    {
      try
      {
        this.transport.EndGetConnection(result);
        if (this.cancelled)
        {
          this.Complete((Exception) null, result);
        }
        else
        {
          bool allowUnicode = this.IsUnicodeSupported();
          this.ValidateUnicodeRequirement(this.message, this.recipients, allowUnicode);
          this.transport.BeginSendMail(this.message.Sender ?? this.message.From, this.recipients, this.message.BuildDeliveryStatusNotificationString(), allowUnicode, new AsyncCallback(this.SendMailCallback), result.AsyncState);
        }
      }
      catch (Exception ex)
      {
        this.Complete(ex, result);
      }
    }

    private void ValidateUnicodeRequirement(MailMessage message, MailAddressCollection recipients, bool allowUnicode)
    {
      foreach (MailAddress mailAddress in (Collection<MailAddress>) recipients)
        mailAddress.GetSmtpAddress(allowUnicode);
      if (message.Sender != null)
        message.Sender.GetSmtpAddress(allowUnicode);
      message.From.GetSmtpAddress(allowUnicode);
    }

    private void GetConnection()
    {
      if (this.transport.IsConnected)
        return;
      this.transport.GetConnection(this.ServicePoint);
    }

    private void Abort()
    {
      try
      {
        this.transport.Abort();
      }
      catch
      {
      }
    }
  }
}
