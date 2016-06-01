using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using EmailEngine.Razor.Generation;
using RazorEngine.Templating;

namespace EmailEngine.Razor
{
    public class ModelCSharpRazorProvider : IRazorProvider
    {
        public Type BaseType { get; private set; }

        public ModelCSharpRazorProvider()
        {
            this.BaseType = typeof(TemplateBase<>);
        }

        public ModelCSharpRazorProvider(Type baseType)
        {
            //We need this assembly to be loaded, otherwise we get strange errors 
            //when compiling our template, we will load it here as it looks C# specific
            Type t = typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException);

            this.BaseType = baseType;
        }

        /// <summary>
        /// Creates a code language service.
        /// </summary>
        /// <returns>Creates a language service.</returns>
        public RazorCodeLanguage CreateLanguageService()
        {
            return new ModelCSharpRazorCodeLanguage(BaseType);
            
        }

        /// <summary>
        /// Creates a <see cref="CodeDomProvider"/>.
        /// </summary>
        /// <returns>The a code dom provider.</returns>
        public CodeDomProvider CreateCodeDomProvider()
        {
            return new CSharpCodeProvider();
        }

    }


}
