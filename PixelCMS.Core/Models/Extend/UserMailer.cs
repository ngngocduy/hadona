using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Net.Mime;
using PixelCMS.Core.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
// -----------------------------------------
// PIXEL CMS
// Mailer.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Mailer
{
    public class UserMailer
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Mailer/
        //public static int SMTPPort = ;
        //public static string SMTPUser = "mail.saigonpixel@gmail.com";
        //public static string SMTPPassword = "123!pass@word!321";
        //public static string SMTPServer = "smtp.gmail.com";
        public static bool isSSL = true;
        public static bool isAuthen = true;
        //public static string DisplayName = "Saigonpixel";

        public virtual string SendMailContact(string mailTo, string displayName, string email, string subject, string bodyContent, string mailReplyTo, string PhoneNumber, string Address)
        {
            string pathFile = HttpContext.Current.Server.MapPath("~/Content/Email-Template.html");
            StreamReader sr = new StreamReader(pathFile);
            sr = File.OpenText(pathFile);
            string contentRead = sr.ReadToEnd();

            contentRead = contentRead.Replace("[Name]", displayName);
            contentRead = contentRead.Replace("[Email]", email);
            contentRead = contentRead.Replace("[Subject]", subject);
            contentRead = contentRead.Replace("[Content]", bodyContent);
            contentRead = contentRead.Replace("[PhoneNumber]", PhoneNumber);
            contentRead = contentRead.Replace("[Address]", Address);

            int SMTPPort = Convert.ToInt32(db.Options.FirstOrDefault(x=>x.Code == "email-smtpport").Value);
            string SMTPUser = db.Options.Where(x => x.Code == "email-smtpmail").FirstOrDefault().Value;
            string SMTPPassword = db.Options.Where(x => x.Code == "email-smtppass").FirstOrDefault().Value;
            string SMTPServer = db.Options.Where(x => x.Code == "email-smtpserver").FirstOrDefault().Value;

            return SendMail(SMTPUser, displayName, SMTPPassword, SMTPPort, SMTPServer, mailTo, subject, contentRead, isSSL, mailReplyTo);
        }

        public virtual string SendMailorders(string mailTo, string displayName, string email, string subject, string bodyContent, string mailReplyTo, string PhoneNumber, string Address,string Note)
        {
            string pathFile = HttpContext.Current.Server.MapPath("~/Content/Orders-Template.html");
            StreamReader sr = new StreamReader(pathFile);
            sr = File.OpenText(pathFile);
            string contentRead = sr.ReadToEnd();

            contentRead = contentRead.Replace("[Name]", displayName);
            contentRead = contentRead.Replace("[Email]", email);
            contentRead = contentRead.Replace("[Subject]", subject);
            contentRead = contentRead.Replace("[Content]", bodyContent);
            contentRead = contentRead.Replace("[PhoneNumber]", PhoneNumber);
            contentRead = contentRead.Replace("[Address]", Address);
            contentRead = contentRead.Replace("[Note]", Note);

            int SMTPPort = Convert.ToInt32(db.Options.FirstOrDefault(x => x.Code == "email-smtpport").Value);
            string SMTPUser = db.Options.Where(x => x.Code == "email-smtpmail").FirstOrDefault().Value;
            string SMTPPassword = db.Options.Where(x => x.Code == "email-smtppass").FirstOrDefault().Value;
            string SMTPServer = db.Options.Where(x => x.Code == "email-smtpserver").FirstOrDefault().Value;

            return SendMail(SMTPUser, displayName, SMTPPassword, SMTPPort, SMTPServer, mailTo, subject, contentRead, isSSL, mailReplyTo);
        }

        public virtual string SendMailResetPass(string mailTo, string displayName, string subject, string bodyContent)
        {
         

            int SMTPPort = Convert.ToInt32(db.Options.Where(x => x.Code == "email-smtpport").FirstOrDefault().Value);
            string SMTPUser = db.Options.Where(x => x.Code == "email-smtpmail").FirstOrDefault().Value;
            string SMTPPassword = db.Options.Where(x => x.Code == "email-smtppass").FirstOrDefault().Value;
            string SMTPServer = db.Options.Where(x => x.Code == "email-smtpserver").FirstOrDefault().Value;

            return SendMail(SMTPUser, displayName, SMTPPassword, SMTPPort, SMTPServer, mailTo, subject, bodyContent, isSSL, mailTo);
        }

        public virtual string SendMail(string fromMail, string displayname, string passMail, int port, string smtpServer, string toMail, string subject, string body, bool isSSL, string mailReplyTo)
        {
            string msg = "";
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailReplyTo, displayname, System.Text.Encoding.UTF8);
                mail.To.Add(new MailAddress(toMail));
                mail.Subject = subject;
                mail.Body = body;
                mail.ReplyTo = new MailAddress(mailReplyTo);
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential(fromMail, passMail);
                client.EnableSsl = isSSL;
                client.Timeout = 100000;
                client.Port = port;
                client.Host = smtpServer;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                ServicePointManager.ServerCertificateValidationCallback =
                delegate(object sender, X509Certificate certificate, X509Chain chain,
                SslPolicyErrors sslPolicyErrors) { return true; };
                client.Send(mail);
                msg = "Successfull";
            }
            catch (SmtpException s)
            {
                msg = "Fail";
            }

            return msg;
        }

    }
}
