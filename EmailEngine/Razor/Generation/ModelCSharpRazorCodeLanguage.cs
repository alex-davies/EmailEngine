using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;

namespace EmailEngine.Razor.Generation
{
    public class ModelCSharpRazorCodeLanguage : CSharpRazorCodeLanguage
    {
        public Type BaseType { get; private set; }
        
        public ModelCSharpRazorCodeLanguage(Type baseType)
        {
            this.BaseType = baseType;
        }

        public override System.Web.Razor.Generator.RazorCodeGenerator CreateCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
        {
            return new ModelCSharpRazorCodeGenerator(className, rootNamespaceName, BaseType, sourceFileName, host);
        }

        public override System.Web.Razor.Parser.ParserBase CreateCodeParser()
        {
            return new ModelCSharpRazorCodeParser();
        }
    }
}
