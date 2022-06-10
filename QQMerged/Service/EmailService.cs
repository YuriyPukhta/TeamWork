using QuiQue.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using System.Threading;
using MailKit.Net.Pop3;

namespace QuiQue.Service
{
    public interface IEmailSender
    {
        Task<string> SendEmailAsync(string recipientEmail, string Text, string Subject = "QuickQueue");
    }
    public class EmailSenderService : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private bool IsSend = false;
        public EmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        private void OnMessageSent(object sender, MessageSentEventArgs e)
        {
            IsSend = true;
            Console.WriteLine("The message was sent!");
        }
        public async Task<string> SendEmailAsync(string recipientEmail, string Text, string Subject = "QuickQueue")
        {
            if (!recipientEmail.Contains("@") && !recipientEmail.Contains(".") && recipientEmail.Length < 7)
            {
                return "bad Enail";
            }

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = "QuickQueue";
            message.Body = new TextPart("plain")
            {
                Text = Text
            };



            var client = new SmtpClient();
            IsSend = false;
            try
            {

                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                await client.AuthenticateAsync(new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password));
                await client.SendAsync(message);



                await client.DisconnectAsync(true);

                //return "send email: " + IsSend.ToString();
            }
            catch (Exception ex)
            {
                return "Errore : " + ex.Message.ToString();
            }
            

            return "Send email";//CheckMessage(recipientEmail).ToString();
        }
        private bool CheckMessage(string Email)
        {

            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(_smtpSettings.PopServer, _smtpSettings.PopPort, true);
                //emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_smtpSettings.PopUsername, _smtpSettings.PopPassword);

                for (int i = emailClient.Count - 1; i > 0 && i > emailClient.Count - 10; i--)
                {
                    Console.WriteLine($"looking for {Email}");
                    var message = emailClient.GetMessage(i);
                    if(ProcessDeliveryStatusNotification(message, Email) == ":)))")
                    {
                        Console.WriteLine("found");
                        return true;
                    }

                        
                }
            }
            return false;
        }
        private string ProcessDeliveryStatusNotification(MimeMessage message, string Email)
        {
            var report = message.Body as MultipartReport;

            if (report == null || report.ReportType == null || !report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("not deliver message");
                // this is not a delivery status notification message...
                return null;
            }

            // process the report
            foreach (var mds in report.OfType<MessageDeliveryStatus>())
            {
                // process the status groups - each status group represents a different recipient

                // The first status group contains information about the message
                var envelopeId = mds.StatusGroups[0]["Original-Envelope-Id"];

                // all of the other status groups contain per-recipient information
                for (int i = 1; i < mds.StatusGroups.Count(); i++)
                {
                    var recipient = mds.StatusGroups[i]["Original-Recipient"];
                    var action = mds.StatusGroups[i]["Action"];

                    if (recipient == null)
                        recipient = mds.StatusGroups[i]["Final-Recipient"];

                    // the recipient string should be in the form: "rfc822;user@domain.com"
                    var index = recipient.IndexOf(';');
                    var address = recipient.Substring(index + 1);
                    Console.WriteLine($"this address{address} and this {Email}");
                    int t = address.CompareTo(Email);
                    if (t == 0)
                    {
                        Console.WriteLine(t.ToString());
                        return ":)))";
                    }
                    else
                    {
                        Console.WriteLine(t.ToString());
                        return ":(((";
                    }

                    /*
                    switch (action)
                    {
                        case "failed":
                            Console.WriteLine("Delivery of message {0} failed for {1}", envelopeId, address);
                            return ":)))";
                            break;
                        case "delayed":
                            Console.WriteLine("Delivery of message {0} has been delayed for {1}", envelopeId, address);
                            break;
                        case "delivered":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1}", envelopeId, address);
                            break;
                        case "relayed":
                            Console.WriteLine("Delivery of message {0} has been relayed for {1}", envelopeId, address);
                            break;
                        case "expanded":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1} and relayed to the the expanded recipients", envelopeId, address);
                            break;
                    }*/
                }
            }
            Console.WriteLine(":///");
            return ":///";
        }
    }
}

/*
public class DSNSmtpClient : SmtpClient
{
    public DSNSmtpClient()
    {
    }

    /// <summary>
    /// Get the envelope identifier to be used with delivery status notifications.
    /// </summary>
    /// <remarks>
    /// <para>The envelope identifier, if non-empty, is useful in determining which message
    /// a delivery status notification was issued for.</para>
    /// <para>The envelope identifier should be unique and may be up to 100 characters in
    /// length, but must consist only of printable ASCII characters and no white space.</para>
    /// <para>For more information, see rfc3461, section 4.4.</para>
    /// </remarks>
    /// <returns>The envelope identifier.</returns>
    /// <param name="message">The message.</param>
    protected override string GetEnvelopeId(MimeMessage message)
    {
        // Since you will want to be able to map whatever identifier you return here to the
        // message, the obvious identifier to use is probably the Message-Id value.
        return message.MessageId;
    }

    /// <summary>
    /// Get the types of delivery status notification desired for the specified recipient mailbox.
    /// </summary>
    /// <remarks>
    /// Gets the types of delivery status notification desired for the specified recipient mailbox.
    /// </remarks>
    /// <returns>The desired delivery status notification type.</returns>
    /// <param name="message">The message being sent.</param>
    /// <param name="mailbox">The mailbox.</param>
    protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
    {
        // In this example, we only want to be notified of failures to deliver to a mailbox.
        // If you also want to be notified of delays or successful deliveries, simply bitwise-or
        // whatever combination of flags you want to be notified about.
        return DeliveryStatusNotification.Failure;
    }
}
    /
}  
    public class CustomSmtpClient : SmtpClient
    {
        protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
        {
            if (!(message.Body is MultipartReport report) || report.ReportType == null || !report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
                return default;
            report.OfType<MessageDeliveryStatus>().ToList().ForEach(x => {
                x.StatusGroups.Where(y => y.Contains("Action") && y.Contains("Final-Recipient")).ToList().ForEach(z => {
                    switch (z["Action"])
                    {
                        case "failed":
                            Console.WriteLine("Delivery of message {0} failed for {1}", z["Action"], z["Final-Recipient"]);
                            break;
                        case "delayed":
                            Console.WriteLine("Delivery of message {0} has been delayed for {1}", z["Action"], z["Final-Recipient"]);
                            break;
                        case "delivered":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1}", z["Action"], z["Final-Recipient"]);
                            break;
                        case "relayed":
                            Console.WriteLine("Delivery of message {0} has been relayed for {1}", z["Action"], z["Final-Recipient"]);
                            break;
                        case "expanded":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1} and relayed to the the expanded recipients", z["Action"], z["Final-Recipient"]);
                            break;
                    }
                });
            });
            return default;
        }
    }
*/