using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EmailEngine.Generator;
using EmailEngine.Razor;

namespace EmailEngine.Sender
{
    public class EmailSender : EmailSender<string>
    {
        public EmailSender(SmtpClient client, IEnumerable<string> imports = null, IRazorProvider languageProvider = null)
            : base(client, new EmailGenerator(imports, languageProvider))
        {
        }
    }

    public class EmailSender<TKey> : IEmailSender<TKey>
    {
        /// <summary>
        /// Generates the emails
        /// </summary>
        public IEmailGenerator<TKey> EmailGenerator { get; set; }

        /// <summary>
        /// SmtpClient to send the emails
        /// </summary>
        public SmtpClient SmtpClient {get; set;}

        public EmailSender(Func<TKey, string> templateResolver, SmtpClient client, IEnumerable<string> imports = null, IRazorProvider languageProvider = null)
            : this(client, new EmailGenerator<TKey>(templateResolver, imports, languageProvider))
        {
        }

        public EmailSender(SmtpClient smtpClient, IEmailGenerator<TKey> emailGenerator)
        {
            SmtpClient = smtpClient;
            EmailGenerator = emailGenerator;
        }
        
       
        public MailMessage Send(
            TKey key,
            object model,
            IEnumerable<MailAddress> to,
            MailAddress from = null,
            string subject = null,
            IEnumerable<MailAddress> cc = null,
            IEnumerable<MailAddress> bcc = null)
        {
            var result = EmailGenerator.Generate(key, model, to, from, subject, cc, bcc);
            Send(result);
            return result;
        }

        public Task<MailMessage> SendAsync(
            TKey key,
            object model,
            IEnumerable<MailAddress> to,
            MailAddress from = null,
            string subject = null,
            IEnumerable<MailAddress> cc = null,
            IEnumerable<MailAddress> bcc = null)
        {
            
            return Task.Factory.StartNew<MailMessage>(() =>
            {
                return Send(key, model, to, from, subject, cc, bcc);
            });
        }

        public void Send(System.Net.Mail.MailMessage message)
        {
            SmtpClient.Send(message);
        }


        
    }
}
