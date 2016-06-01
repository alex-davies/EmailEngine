using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace EmailEngine.Generator
{
    public interface IEmailGenerator<TKey>
    {
        /// <summary>
        /// Generates mail after by using a template to write the body
        /// </summary>
        /// <param name="key">name to be resolved into a template</param>
        /// <param name="model">Data that can be accessed in the template</param>
        /// <param name="to">address to send the email, the template may modify this value</param>
        /// <param name="from">address the mail is from, the template may modify this value</param>
        /// <param name="subject">the subject of the email, the template may modify this value</param>
        /// <param name="cc">the carbon copy for this email, the template may modify this value</param>
        /// <param name="bcc">the blind carbon copy for this email, the template may modify this value</param>
        MailMessage Generate(
           TKey key,
           object model,
           IEnumerable<MailAddress> to,
           MailAddress from = null,
           string subject = null,
           IEnumerable<MailAddress> cc = null,
           IEnumerable<MailAddress> bcc = null);

    }

    /// <summary>
    /// Extension methods for the IEmailGenerator interface
    /// </summary>
    public static class EmailGeneratorExtensions
    {
        /// <summary>
        /// Generates mail after by using a template to write the body
        /// </summary>
        /// <param name="key">name to be resolved into a template</param>
        /// <param name="model">Data that can be accessed in the template</param>
        /// <param name="to">address to send the email, the template may modify this value</param>
        /// <param name="from">address the mail is from, the template may modify this value</param>
        /// <param name="subject">the subject of the email, the template may modify this value</param>
        /// <param name="cc">the carbon copy for this email, the template may modify this value</param>
        /// <param name="bcc">the blind carbon copy for this email, the template may modify this value</param>
        public static MailMessage Generate<TKey>(
            this IEmailGenerator<TKey> emailGenerator,
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

            return emailGenerator.Generate(key,
                model,
                toMailAddresses,
                fromMailAddress,
                subject,
                ccMailAddresses,
                bccMailAddresses);
        }
    }
}
