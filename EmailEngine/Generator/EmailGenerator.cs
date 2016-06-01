using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using RazorEngine;
using System.Net;
using System.Web.Razor;
using System.IO;
using EmailEngine.Razor;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace EmailEngine.Generator
{

   




    /// <summary>
    /// A simple email templator that uses the name as the actual template used to generate the email
    /// </summary>
    public class EmailGenerator : EmailGenerator<string>
    {
        public EmailGenerator(IEnumerable<string> imports = null, IRazorProvider languageProvider = null) :
            base((s)=>s, imports, languageProvider)
        {
        }
    }


    public class EmailGenerator<TKey> : IEmailGenerator<TKey>
    {

        /// <summary>
        /// The imports that are
        /// </summary>
        public static IEnumerable<string> DefaultImports = new string[]{
            "System",
            "System.Linq",
            "System.Collections.Generic"
        };

        /// <summary>
        /// The method used to resolve a named key into a template
        /// </summary>
        public Func<TKey, string> TemplateResolver { get; private set; }


        /// <summary>
        /// The imports to be added when executing the template
        /// </summary>
        public IEnumerable<string> Imports { get; private set; }

        /// <summary>
        /// The compiler that is compiling the razor files
        /// </summary>
        protected RazorCompiler Compiler { get; private set; }

        /// <summary>
        /// Cache to hold already generated templates
        /// </summary>
        protected Dictionary<TKey, Type> TemplateCache = new Dictionary<TKey, Type>();



        public EmailGenerator(Func<TKey, string> templateResolver, IEnumerable<string> imports = null, IRazorProvider languageProvider = null )
        {
            if (templateResolver == null)
                throw new ArgumentNullException("templateResolver");
            imports = imports ?? new string[0];
            Imports = imports.Union(DefaultImports);
            
            TemplateResolver = templateResolver;
            Compiler = new RazorCompiler(languageProvider ?? new ModelCSharpRazorProvider(typeof(EmailTemplate<>)));
        }

       


        public virtual MailMessage Generate(
            TKey key,
            object model,
            IEnumerable<MailAddress> to,
            MailAddress from = null,
            string subject = null,
            IEnumerable<MailAddress> cc = null,
            IEnumerable<MailAddress> bcc = null)
        {
            //Remove null on list to prevent null reference.
            //to address will need to exist but the template might be adding this value so we will let the
            //smtp client throw the exception and just make sure we send empty lists for now
            to = to ?? new MailAddress[0];
            cc = cc ?? new MailAddress[0];
            bcc = bcc ?? new MailAddress[0];




            //Create our mailMessage to be populated by our template
            MailMessage mailMessage = new MailMessage();
            mailMessage.BodyEncoding = Encoding.UTF8;

            Type templateType;
            if (!TemplateCache.TryGetValue(key, out templateType))
            {
                //our cache is missing the template, we need to resolve the template and compile a new type
                string templateString = TemplateResolver(key);
                if (templateString == null)
                    throw new ArgumentException("Template name does not resolve to a template, ensure the template resolver can resolve this name", "name");
                templateType = Compiler.Compile(templateString, imports: Imports);
                TemplateCache[key] = templateType;
            }


            //the actual tempalte creation
            ITemplate template = (ITemplate)Activator.CreateInstance(templateType);


            //Assign a model if the template allows it
            if (IsAssignableToGenericType(template.GetType(), typeof(ITemplate<>)))
            {
                //because anonymous objects contain 'internal' properties they cant be used in models
                //the fix is to convert all anonymous objects into dynamic objects
                model = UnAnonymize(model);
                template.GetType().GetProperty("Model").SetValue(template, model, null);
            }

            //if we are a real email template populate all the email related fields
            if (template is EmailTemplate)
            {
                var emailTemplate = (EmailTemplate)template;
                emailTemplate.TemplateKey = key;
                emailTemplate.MailMessage = mailMessage;
                if(from != null)
                    emailTemplate.From = from;
                emailTemplate.To = to;
                emailTemplate.CC = cc;
                emailTemplate.Bcc = bcc;
                emailTemplate.Subject = subject;
                emailTemplate.IsBodyHtml = true; //will default to true, template can change if needed
            }


            //now we have all the data populated actually run our template
            using (template)
            {
                template.Execute();
            }

            return mailMessage;
        }

        #region ("Helpers")

        protected static object UnAnonymize(object model)
        {
            if (model != null && IsAnonymous(model.GetType()))
                return ToExpando(model);
            return model;
        }

        protected static dynamic ToExpando(object anonymousObject)
        {
            var expando = new ExpandoObject() as IDictionary<string, Object>;
            var attributeFilter = BindingFlags.Public | BindingFlags.Instance;
            foreach (var property in anonymousObject.GetType().GetProperties(attributeFilter))
            {
                if (property.CanRead)
                {
                    expando.Add(property.Name, property.GetValue(anonymousObject, null));
                }
            }
            return expando;
        }



        protected static bool IsAnonymous(Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
        }

        protected static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
                if (it.IsGenericType)
                    if (it.GetGenericTypeDefinition() == genericType) return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == genericType ||
                IsAssignableToGenericType(baseType, genericType);
        }
        
        #endregion

    }


}
