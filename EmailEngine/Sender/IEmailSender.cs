using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EmailEngine.Generator;

namespace EmailEngine.Sender
{
     public interface IEmailSender<TKey>
    {
         /// <summary>
         /// Generates the email
         /// </summary>
         IEmailGenerator<TKey> EmailGenerator {get; }

        /// <summary>
        /// Sends mail after using a template to generate the body
        /// </summary>
        /// <param name="key">name to be resolved into a template</param>
        /// <param name="model">Data that can be accessed in the template</param>
        /// <param name="to">address to send the email, the template may modify this value</param>
        /// <param name="from">address the mail is from, the template may modify this value</param>
        /// <param name="subject">the subject of the email, the template may modify this value</param>
        /// <param name="cc">the carbon copy for this email, the template may modify this value</param>
        /// <param name="bcc">the blind carbon copy for this email, the template may modify this value</param>
        MailMessage Send(
           TKey key,
           object model,
           IEnumerable<MailAddress> to,
           MailAddress from = null,
           string subject = null,
           IEnumerable<MailAddress> cc = null,
           IEnumerable<MailAddress> bcc = null);

         /// <summary>
         /// Sends mail after using a template to generate the body asynchronously
         /// </summary>
         /// <param name="key"></param>
         /// <param name="model"></param>
         /// <param name="to"></param>
         /// <param name="from"></param>
         /// <param name="subject"></param>
         /// <param name="cc"></param>
         /// <param name="bcc"></param>
         /// <returns></returns>
         Task<MailMessage> SendAsync(
           TKey key,
           object model,
           IEnumerable<MailAddress> to,
           MailAddress from = null,
           string subject = null,
           IEnumerable<MailAddress> cc = null,
           IEnumerable<MailAddress> bcc = null);

         /// <summary>
        /// Sends a mail message without using template generation
        /// </summary>
        /// <param name="message"></param>
        void Send(System.Net.Mail.MailMessage message);
    }

    /// <summary>
    /// Extension methods for the IEmailGenerator interface
    /// </summary>
     public static class EmailSenderExtensions
     {
         /// <summary>
         /// Sends mail after using a template to generate the body
         /// </summary>
         /// <param name="key">name to be resolved into a template</param>
         /// <param name="model">Data that can be accessed in the template</param>
         /// <param name="to">address to send the email, the template may modify this value</param>
         /// <param name="from">address the mail is from, the template may modify this value</param>
         /// <param name="subject">the subject of the email, the template may modify this value</param>
         /// <param name="cc">the carbon copy for this email, the template may modify this value</param>
         /// <param name="bcc">the blind carbon copy for this email, the template may modify this value</param>
         public static MailMessage Send<TKey>(
            this IEmailSender<TKey> emailSender,
            TKey key,
            object model,
            IEnumerable<string> to,
            string from = null,
            string subject = null,
            IEnumerable<string> cc = null,
            IEnumerable<string> bcc = null)
         {
             to = to ?? new string[0];
             cc = cc ?? new string[0];
             bcc = bcc ?? new string[0];

             //if our inputs are null we will keep them null and let the overload Send deal with them
             //if they are not null then we will convert them to real mail addresses so we can use them
             var fromMailAddress = from == null ? null : new MailAddress(from);
             var toMailAddresses = to == null ? null : to.Select(a => new MailAddress(a));
             var ccMailAddresses = cc == null ? null : cc.Select(a => new MailAddress(a));
             var bccMailAddresses = bcc == null ? null : bcc.Select(a => new MailAddress(a));

             return emailSender.Send(key,
                 model,
                 toMailAddresses,
                 fromMailAddress,
                 subject,
                 ccMailAddresses,
                 bccMailAddresses);
         }


         /// <summary>
         /// Sends mail after using a template to generate the body asynchronously
         /// </summary>
         /// <param name="key">name to be resolved into a template</param>
         /// <param name="model">Data that can be accessed in the template</param>
         /// <param name="to">address to send the email, the template may modify this value</param>
         /// <param name="from">address the mail is from, the template may modify this value</param>
         /// <param name="subject">the subject of the email, the template may modify this value</param>
         /// <param name="cc">the carbon copy for this email, the template may modify this value</param>
         /// <param name="bcc">the blind carbon copy for this email, the template may modify this value</param>
         public static Task<MailMessage> SendAsync<TKey>(
            this IEmailSender<TKey> emailSender,
            TKey key,
            object model,
            IEnumerable<string> to,
            string from = null,
            string subject = null,
            IEnumerable<string> cc = null,
            IEnumerable<string> bcc = null)
         {
             to = to ?? new string[0];
             cc = cc ?? new string[0];
             bcc = bcc ?? new string[0];

             //if our inputs are null we will keep them null and let the overload Send deal with them
             //if they are not null then we will convert them to real mail addresses so we can use them
             var fromMailAddress = from == null ? null : new MailAddress(from);
             var toMailAddresses = to == null ? null : to.Select(a => new MailAddress(a));
             var ccMailAddresses = cc == null ? null : cc.Select(a => new MailAddress(a));
             var bccMailAddresses = bcc == null ? null : bcc.Select(a => new MailAddress(a));

             return emailSender.SendAsync(key,
                 model,
                 toMailAddresses,
                 fromMailAddress,
                 subject,
                 ccMailAddresses,
                 bccMailAddresses);
         }
     }
}
