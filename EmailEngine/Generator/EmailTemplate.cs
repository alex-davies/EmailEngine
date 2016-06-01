using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmailEngine.Razor;
using System.Net.Mail;

namespace EmailEngine.Generator
{
    public abstract class EmailTemplate : ITemplate
    {
        protected StringBuilder Result = new StringBuilder();

        /// <summary>
        /// The name of this template
        /// </summary>
        public object TemplateKey { get; set; }

        /// <summary>
        /// The SmtpClient that will be used to send this email
        /// </summary>
        public SmtpClient SmtpClient { get; set; }

        /// <summary>
        /// The mail message this will be sent out in. The body will be populated after the template has run
        /// </summary>
        public MailMessage MailMessage { get; set; }

        #region ("ITemplate methods")

        /// <summary>
        /// Razor Parser overrides this method
        /// </summary>
        public virtual void Execute() { }

        public virtual void WriteLiteral(object value)
        {
            Result.Append(value.ToString());
        }

        public virtual void Write(object value)
        {
            Result.Append(value);
        }

        /// <summary>
        /// When the template is finished we will write the body of our email
        /// </summary>
        public void Dispose()
        {
            MailMessage.Body = Result.ToString();
        }

        #endregion

        #region ("Helper Shortcuts")

        /// <summary>
        /// If the body of the email is HTML
        /// </summary>
        public bool IsBodyHtml
        {
            get
            {
                return MailMessage.IsBodyHtml;
            }
            set
            {
                MailMessage.IsBodyHtml = value;
            }
        }

        /// <summary>
        /// The Address this mail will be sent from
        /// </summary>
        public MailAddress From { 
            get 
            {
                return MailMessage.From;
            } 
            set
            {
                MailMessage.From = value;
            }
        }
        
        /// <summary>
        /// The addresses this mail will send to
        /// </summary>
        public IEnumerable<MailAddress> To
        {
            get
            {
                return MailMessage.To;
            }
            set
            {
                MailMessage.To.Clear();
                foreach (var toAddress in value)
                {
                    MailMessage.To.Add(toAddress);
                }
            }
        }
        
        /// <summary>
        /// The address this mail will be carbon copyed to
        /// </summary>
        public IEnumerable<MailAddress> CC
        {
            get
            {
                return MailMessage.CC;
            }
            set
            {
                MailMessage.CC.Clear();
                foreach (var toAddress in value)
                {
                    MailMessage.CC.Add(toAddress);
                }
            }
        }
        
        /// <summary>
        /// The address this mail will be blind carbon copyed to
        /// </summary>
        public IEnumerable<MailAddress> Bcc
        {
            get
            {
                return MailMessage.Bcc;
            }
            set
            {
                MailMessage.Bcc.Clear();
                foreach (var toAddress in value)
                {
                    MailMessage.Bcc.Add(toAddress);
                }
            }
        }

        /// <summary>
        /// The subject of the email
        /// </summary>
        public string Subject
        {
            get
            {
                return MailMessage.Subject;
            }
            set
            {
                MailMessage.Subject = value;
            }
        }

        #endregion
    }

    public abstract class EmailTemplate<TModel> : EmailTemplate, ITemplate<TModel>
    {
        public TModel Model { get; set; }
    }


}
