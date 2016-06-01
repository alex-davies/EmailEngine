using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RazorEngine.Compilation.CSharp;
using System.Web.Razor.Generator;
using EmailEngine.Razor.Generation;
using System.CodeDom;
using System.Web.Razor;

namespace EmailEngine.Razor.Generation
{
    public class ModelCSharpRazorCodeGenerator : System.Web.Razor.Generator.CSharpRazorCodeGenerator
    {
        public readonly Type GenericBaseType;

        public ModelCSharpRazorCodeGenerator(string className, string rootNamespaceName, Type genericBaseType, string sourceFileName, RazorEngineHost host) 
            : base(className, rootNamespaceName, sourceFileName, host)
        {
            if (genericBaseType == null)
                throw new ArgumentNullException("genericBaseType");
            if (!ValidateBaseType(genericBaseType))
                throw new ArgumentException("genericBaseType must be a generic type definition and extend EmailEngine.Razor.TemplateBase e.g. typeof(EmailEngine.Razor.TemplateBase<>)", "genericBaseType");

            this.GenericBaseType = genericBaseType;
            SetBaseType();
            
        }

        protected bool ValidateBaseType(Type genericBaseType)
        {
            return genericBaseType.IsGenericTypeDefinition
                && typeof(ITemplate).IsAssignableFrom(genericBaseType);
        }

        protected void SetBaseType(string modelType = "dynamic")
        {
            string BaseTypeFullName = GenericBaseType.FullName;
            BaseTypeFullName = BaseTypeFullName.Substring(0, BaseTypeFullName.Length - 2);

            CodeTypeReference codeTypeReference = new CodeTypeReference(BaseTypeFullName + "<" + modelType + ">");
            this.GeneratedClass.BaseTypes.Clear();
            this.GeneratedClass.BaseTypes.Add(codeTypeReference);
        }

        protected override bool TryVisitSpecialSpan(System.Web.Razor.Parser.SyntaxTree.Span span)
        {
            
            return RazorCodeGenerator.TryVisit<ModelSpan>(span, (s) =>
            {
                SetBaseType(s.ModelType);
            });

        }
    }
}
