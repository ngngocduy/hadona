// Type: System.Net.Mail.MailMessage
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mime;
using System.Runtime;
using System.Text;

namespace System.Net.Mail
{
  /// <summary>
  /// Represents an e-mail message that can be sent using the <see cref="T:System.Net.Mail.SmtpClient"/> class.
  /// </summary>
  public class MailMessage : IDisposable
  {
    private string body = string.Empty;
    private TransferEncoding bodyTransferEncoding = TransferEncoding.Unknown;
    private AlternateViewCollection views;
    private AttachmentCollection attachments;
    private AlternateView bodyView;
    private Encoding bodyEncoding;
    private bool isBodyHtml;
    private bool disposed;
    private Message message;
    private DeliveryNotificationOptions deliveryStatusNotification;

    /// <summary>
    /// Gets or sets the from address for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Net.Mail.MailAddress"/> that contains the from address information.
    /// </returns>
    public MailAddress From
    {
      get
      {
        return this.message.From;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.message.From = value;
      }
    }

    /// <summary>
    /// Gets or sets the sender's address for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Net.Mail.MailAddress"/> that contains the sender's address information.
    /// </returns>
    public MailAddress Sender
    {
      get
      {
        return this.message.Sender;
      }
      set
      {
        this.message.Sender = value;
      }
    }

    /// <summary>
    /// Gets or sets the ReplyTo address for the mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A MailAddress that indicates the value of the <see cref="P:System.Net.Mail.MailMessage.ReplyTo"/> field.
    /// </returns>
    [Obsolete("ReplyTo is obsoleted for this type.  Please use ReplyToList instead which can accept multiple addresses. http://go.microsoft.com/fwlink/?linkid=14202")]
    public MailAddress ReplyTo
    {
      get
      {
        return this.message.ReplyTo;
      }
      set
      {
        this.message.ReplyTo = value;
      }
    }

    /// <summary>
    /// Gets or sets the list of addresses to reply to for the mail message.
    /// </summary>
    /// 
    /// <returns>
    /// The list of the addresses to reply to for the mail message.
    /// </returns>
    public MailAddressCollection ReplyToList
    {
      get
      {
        return this.message.ReplyToList;
      }
    }

    /// <summary>
    /// Gets the address collection that contains the recipients of this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A writable <see cref="T:System.Net.Mail.MailAddressCollection"/> object.
    /// </returns>
    public MailAddressCollection To
    {
      get
      {
        return this.message.To;
      }
    }

    /// <summary>
    /// Gets the address collection that contains the blind carbon copy (BCC) recipients for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A writable <see cref="T:System.Net.Mail.MailAddressCollection"/> object.
    /// </returns>
    public MailAddressCollection Bcc
    {
      get
      {
        return this.message.Bcc;
      }
    }

    /// <summary>
    /// Gets the address collection that contains the carbon copy (CC) recipients for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A writable <see cref="T:System.Net.Mail.MailAddressCollection"/> object.
    /// </returns>
    public MailAddressCollection CC
    {
      get
      {
        return this.message.CC;
      }
    }

    /// <summary>
    /// Gets or sets the priority of this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Net.Mail.MailPriority"/> that contains the priority of this message.
    /// </returns>
    public MailPriority Priority
    {
      get
      {
        return this.message.Priority;
      }
      set
      {
        this.message.Priority = value;
      }
    }

    /// <summary>
    /// Gets or sets the delivery notifications for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Net.Mail.DeliveryNotificationOptions"/> value that contains the delivery notifications for this message.
    /// </returns>
    public DeliveryNotificationOptions DeliveryNotificationOptions
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.deliveryStatusNotification;
      }
      set
      {
        if (7U < (uint) value && value != DeliveryNotificationOptions.Never)
          throw new ArgumentOutOfRangeException("value");
        this.deliveryStatusNotification = value;
      }
    }

    /// <summary>
    /// Gets or sets the subject line for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> that contains the subject content.
    /// </returns>
    public string Subject
    {
      get
      {
        if (this.message.Subject == null)
          return string.Empty;
        else
          return this.message.Subject;
      }
      set
      {
        this.message.Subject = value;
      }
    }

    /// <summary>
    /// Gets or sets the encoding used for the subject content for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Text.Encoding"/> that was used to encode the <see cref="P:System.Net.Mail.MailMessage.Subject"/> property.
    /// </returns>
    public Encoding SubjectEncoding
    {
      get
      {
        return this.message.SubjectEncoding;
      }
      set
      {
        this.message.SubjectEncoding = value;
      }
    }

    /// <summary>
    /// Gets the e-mail headers that are transmitted with this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection"/> that contains the e-mail headers.
    /// </returns>
    public NameValueCollection Headers
    {
      get
      {
        return (NameValueCollection) this.message.Headers;
      }
    }

    /// <summary>
    /// Gets or sets the encoding used for the user-defined custom headers for this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// The encoding used for user-defined custom headers for this e-mail message.
    /// </returns>
    public Encoding HeadersEncoding
    {
      get
      {
        return this.message.HeadersEncoding;
      }
      set
      {
        this.message.HeadersEncoding = value;
      }
    }

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.String"/> value that contains the body text.
    /// </returns>
    public string Body
    {
      get
      {
        if (this.body == null)
          return string.Empty;
        else
          return this.body;
      }
      set
      {
        this.body = value;
        if (this.bodyEncoding != null || this.body == null)
          return;
        if (MimeBasePart.IsAscii(this.body, true))
          this.bodyEncoding = Encoding.ASCII;
        else
          this.bodyEncoding = Encoding.GetEncoding("utf-8");
      }
    }

    /// <summary>
    /// Gets or sets the encoding used to encode the message body.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Text.Encoding"/> applied to the contents of the <see cref="P:System.Net.Mail.MailMessage.Body"/>.
    /// </returns>
    public Encoding BodyEncoding
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.bodyEncoding;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.bodyEncoding = value;
      }
    }

    /// <summary>
    /// Gets or sets the transfer encoding used to encode the message body.
    /// </summary>
    /// 
    /// <returns>
    /// Returns <see cref="T:System.Net.Mime.TransferEncoding"/>.A <see cref="T:System.Net.Mime.TransferEncoding"/> applied to the contents of the <see cref="P:System.Net.Mail.MailMessage.Body"/>.
    /// </returns>
    public TransferEncoding BodyTransferEncoding
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.bodyTransferEncoding;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.bodyTransferEncoding = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the mail message body is in Html.
    /// </summary>
    /// 
    /// <returns>
    /// true if the message body is in Html; else false. The default is false.
    /// </returns>
    public bool IsBodyHtml
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.isBodyHtml;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.isBodyHtml = value;
      }
    }

    /// <summary>
    /// Gets the attachment collection used to store data attached to this e-mail message.
    /// </summary>
    /// 
    /// <returns>
    /// A writable <see cref="T:System.Net.Mail.AttachmentCollection"/>.
    /// </returns>
    public AttachmentCollection Attachments
    {
      get
      {
        if (this.disposed)
          throw new ObjectDisposedException(this.GetType().FullName);
        if (this.attachments == null)
          this.attachments = new AttachmentCollection();
        return this.attachments;
      }
    }

    /// <summary>
    /// Gets the attachment collection used to store alternate forms of the message body.
    /// </summary>
    /// 
    /// <returns>
    /// A writable <see cref="T:System.Net.Mail.AlternateViewCollection"/>.
    /// </returns>
    public AlternateViewCollection AlternateViews
    {
      get
      {
        if (this.disposed)
          throw new ObjectDisposedException(this.GetType().FullName);
        if (this.views == null)
          this.views = new AlternateViewCollection();
        return this.views;
      }
    }

    /// <summary>
    /// Initializes an empty instance of the <see cref="T:System.Net.Mail.MailMessage"/> class.
    /// </summary>
    public MailMessage()
    {
      this.message = new Message();
      if (Logging.On)
        Logging.Associate(Logging.Web, (object) this, (object) this.message);
      string from = SmtpClient.MailConfiguration.Smtp.From;
      if (from == null || from.Length <= 0)
        return;
      this.message.From = new MailAddress(from);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage"/> class by using the specified <see cref="T:System.String"/> class objects.
    /// </summary>
    /// <param name="from">A <see cref="T:System.String"/> that contains the address of the sender of the e-mail message.</param><param name="to">A <see cref="T:System.String"/> that contains the addresses of the recipients of the e-mail message.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="to"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="from"/> is <see cref="F:System.String.Empty"/> ("").-or-<paramref name="to"/> is <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.FormatException"><paramref name="from"/> or <paramref name="to"/> is malformed.</exception>
    public MailMessage(string from, string to)
    {
      if (from == null)
        throw new ArgumentNullException("from");
      if (to == null)
        throw new ArgumentNullException("to");
      if (from == string.Empty)
        throw new ArgumentException(SR.GetString("net_emptystringcall", new object[1]
        {
          (object) "from"
        }), "from");
      else if (to == string.Empty)
      {
        throw new ArgumentException(SR.GetString("net_emptystringcall", new object[1]
        {
          (object) "to"
        }), "to");
      }
      else
      {
        this.message = new Message(from, to);
        if (!Logging.On)
          return;
        Logging.Associate(Logging.Web, (object) this, (object) this.message);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage"/> class.
    /// </summary>
    /// <param name="from">A <see cref="T:System.String"/> that contains the address of the sender of the e-mail message.</param><param name="to">A <see cref="T:System.String"/> that contains the address of the recipient of the e-mail message.</param><param name="subject">A <see cref="T:System.String"/> that contains the subject text.</param><param name="body">A <see cref="T:System.String"/> that contains the message body.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="to"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="from"/> is <see cref="F:System.String.Empty"/> ("").-or-<paramref name="to"/> is <see cref="F:System.String.Empty"/> ("").</exception><exception cref="T:System.FormatException"><paramref name="from"/> or <paramref name="to"/> is malformed.</exception>
    public MailMessage(string from, string to, string subject, string body)
      : this(from, to)
    {
      this.Subject = subject;
      this.Body = body;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage"/> class by using the specified <see cref="T:System.Net.Mail.MailAddress"/> class objects.
    /// </summary>
    /// <param name="from">A <see cref="T:System.Net.Mail.MailAddress"/> that contains the address of the sender of the e-mail message.</param><param name="to">A <see cref="T:System.Net.Mail.MailAddress"/> that contains the address of the recipient of the e-mail message.</param><exception cref="T:System.ArgumentNullException"><paramref name="from"/> is null.-or-<paramref name="to"/> is null.</exception><exception cref="T:System.FormatException"><paramref name="from"/> or <paramref name="to"/> is malformed.</exception>
    public MailMessage(MailAddress from, MailAddress to)
    {
      if (from == null)
        throw new ArgumentNullException("from");
      if (to == null)
        throw new ArgumentNullException("to");
      this.message = new Message(from, to);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="T:System.Net.Mail.MailMessage"/>.
    /// </summary>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public void Dispose()
    {
      this.Dispose(true);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.Net.Mail.MailMessage"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (this.views != null)
        this.views.Dispose();
      if (this.attachments != null)
        this.attachments.Dispose();
      if (this.bodyView == null)
        return;
      this.bodyView.Dispose();
    }

    internal void Send(BaseWriter writer, bool sendEnvelope, bool allowUnicode)
    {
      this.SetContent(allowUnicode);
      this.message.Send(writer, sendEnvelope, allowUnicode);
    }

    internal IAsyncResult BeginSend(BaseWriter writer, bool sendEnvelope, bool allowUnicode, AsyncCallback callback, object state)
    {
      this.SetContent(allowUnicode);
      return this.message.BeginSend(writer, sendEnvelope, allowUnicode, callback, state);
    }

    internal void EndSend(IAsyncResult asyncResult)
    {
      this.message.EndSend(asyncResult);
    }

    internal string BuildDeliveryStatusNotificationString()
    {
      if (this.deliveryStatusNotification == DeliveryNotificationOptions.None)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(" NOTIFY=");
      bool flag = false;
      if (this.deliveryStatusNotification == DeliveryNotificationOptions.Never)
      {
        stringBuilder.Append("NEVER");
        return ((object) stringBuilder).ToString();
      }
      else
      {
        if ((this.deliveryStatusNotification & DeliveryNotificationOptions.OnSuccess) > DeliveryNotificationOptions.None)
        {
          stringBuilder.Append("SUCCESS");
          flag = true;
        }
        if ((this.deliveryStatusNotification & DeliveryNotificationOptions.OnFailure) > DeliveryNotificationOptions.None)
        {
          if (flag)
            stringBuilder.Append(",");
          stringBuilder.Append("FAILURE");
          flag = true;
        }
        if ((this.deliveryStatusNotification & DeliveryNotificationOptions.Delay) > DeliveryNotificationOptions.None)
        {
          if (flag)
            stringBuilder.Append(",");
          stringBuilder.Append("DELAY");
        }
        return ((object) stringBuilder).ToString();
      }
    }

    private void SetContent(bool allowUnicode)
    {
      if (this.bodyView != null)
      {
        this.bodyView.Dispose();
        this.bodyView = (AlternateView) null;
      }
      if (this.AlternateViews.Count == 0 && this.Attachments.Count == 0)
      {
        if (!string.IsNullOrEmpty(this.body))
        {
          this.bodyView = AlternateView.CreateAlternateViewFromString(this.body, this.bodyEncoding, this.isBodyHtml ? "text/html" : (string) null);
          this.message.Content = (MimeBasePart) this.bodyView.MimePart;
        }
      }
      else if (this.AlternateViews.Count == 0 && this.Attachments.Count > 0)
      {
        MimeMultiPart mimeMultiPart = new MimeMultiPart(MimeMultiPartType.Mixed);
        this.bodyView = string.IsNullOrEmpty(this.body) ? AlternateView.CreateAlternateViewFromString(string.Empty) : AlternateView.CreateAlternateViewFromString(this.body, this.bodyEncoding, this.isBodyHtml ? "text/html" : (string) null);
        mimeMultiPart.Parts.Add((MimeBasePart) this.bodyView.MimePart);
        foreach (Attachment attachment in (Collection<Attachment>) this.Attachments)
        {
          if (attachment != null)
          {
            attachment.PrepareForSending(allowUnicode);
            mimeMultiPart.Parts.Add((MimeBasePart) attachment.MimePart);
          }
        }
        this.message.Content = (MimeBasePart) mimeMultiPart;
      }
      else
      {
        MimeMultiPart mimeMultiPart1 = new MimeMultiPart(MimeMultiPartType.Alternative);
        if (!string.IsNullOrEmpty(this.body))
        {
          this.bodyView = AlternateView.CreateAlternateViewFromString(this.body, this.bodyEncoding, (string) null);
          mimeMultiPart1.Parts.Add((MimeBasePart) this.bodyView.MimePart);
        }
        foreach (AlternateView alternateView in (Collection<AlternateView>) this.AlternateViews)
        {
          if (alternateView != null)
          {
            alternateView.PrepareForSending(allowUnicode);
            if (alternateView.LinkedResources.Count > 0)
            {
              MimeMultiPart mimeMultiPart2 = new MimeMultiPart(MimeMultiPartType.Related);
              mimeMultiPart2.ContentType.Parameters["type"] = alternateView.ContentType.MediaType;
              mimeMultiPart2.ContentLocation = alternateView.MimePart.ContentLocation;
              mimeMultiPart2.Parts.Add((MimeBasePart) alternateView.MimePart);
              foreach (LinkedResource linkedResource in (Collection<LinkedResource>) alternateView.LinkedResources)
              {
                linkedResource.PrepareForSending(allowUnicode);
                mimeMultiPart2.Parts.Add((MimeBasePart) linkedResource.MimePart);
              }
              mimeMultiPart1.Parts.Add((MimeBasePart) mimeMultiPart2);
            }
            else
              mimeMultiPart1.Parts.Add((MimeBasePart) alternateView.MimePart);
          }
        }
        if (this.Attachments.Count > 0)
        {
          MimeMultiPart mimeMultiPart2 = new MimeMultiPart(MimeMultiPartType.Mixed);
          mimeMultiPart2.Parts.Add((MimeBasePart) mimeMultiPart1);
          MimeMultiPart mimeMultiPart3 = new MimeMultiPart(MimeMultiPartType.Mixed);
          foreach (Attachment attachment in (Collection<Attachment>) this.Attachments)
          {
            if (attachment != null)
            {
              attachment.PrepareForSending(allowUnicode);
              mimeMultiPart3.Parts.Add((MimeBasePart) attachment.MimePart);
            }
          }
          mimeMultiPart2.Parts.Add((MimeBasePart) mimeMultiPart3);
          this.message.Content = (MimeBasePart) mimeMultiPart2;
        }
        else
          this.message.Content = mimeMultiPart1.Parts.Count != 1 || !string.IsNullOrEmpty(this.body) ? (MimeBasePart) mimeMultiPart1 : mimeMultiPart1.Parts[0];
      }
      if (this.bodyView == null || this.bodyTransferEncoding == TransferEncoding.Unknown)
        return;
      this.bodyView.TransferEncoding = this.bodyTransferEncoding;
    }
  }
}
