using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailEngine.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Razor;
    using System.Web.Razor.Parser;
    using EmailEngine.Razor.Generation;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// Compiles razor templates.
    /// </summary>
    public class RazorCompiler
    {
        /// <summary>
        /// Namespace the dynamically created  class is put into
        /// </summary>
        protected static string DynamicNamespace = "EmailEngine.Razor.Dynamic";

        /// <summary>
        /// Influences the generated code, allows different languages and special handling of tags
        /// </summary>
        protected readonly IRazorProvider Provider;

        
        
        /// <summary>
        /// Initialises a new instance of <see cref="RazorCompiler"/>.
        /// </summary>
        /// <param name="provider">The provider used to compile templates.</param>
        public RazorCompiler(IRazorProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.Provider = provider;
        }


  
        /// <summary>
        /// Compiles a template into an actual class
        /// </summary>
        /// <param name="template">The razor template to compile</param>
        /// <param name="className">The name of the generated class</param>
        /// <param name="baseType">The base type of the generated class</param>
        /// <param name="imports">List of imports statement the template expects</param>
        /// <returns></returns>
        public Type Compile(string template, string className=null, Type baseType=null, IEnumerable<string> imports = null)
        {
            //if we dont have a class make a random one
            //if we dont have a base type use a default TemplateBase
            //if we dont have imports then use an empty list
            className = className ?? Regex.Replace(Guid.NewGuid().ToString("N"), @"[^A-Za-z]*", "");
            baseType = baseType ?? typeof(TemplateBase);
            imports = imports ?? new string[0];

            var languageService = Provider.CreateLanguageService();
            var codeDom = Provider.CreateCodeDomProvider();
            var host = new RazorEngineHost(languageService);

            var generator = languageService.CreateCodeGenerator(className, DynamicNamespace, null, host);
            var parser = new RazorParser(languageService.CreateCodeParser(), new HtmlMarkupParser());

            //add the imports
            foreach (var import in imports)
            {
                generator.GeneratedNamespace.Imports.Add(new CodeNamespaceImport(import));
            }

            using (var reader = new StringReader(template))
            {
                parser.Parse(reader, generator);
            }
            
            //set the base type if the parsing didnt set it for us
            if(generator.GeneratedClass.BaseTypes.Count == 0)
                generator.GeneratedClass.BaseTypes.Add(baseType);

#if DEBUG
            //just a useful output of the class to a string to make sure its all working fine
            //output is not directly used, but useful to putting in a breakpoint and have a look
            var classPreviewBuilder = new StringBuilder();
            using (var writer = new StringWriter(classPreviewBuilder))
                codeDom.GenerateCodeFromCompileUnit(generator.GeneratedCode, writer, new CodeGeneratorOptions());
            var preview = classPreviewBuilder.ToString();
#endif

            //Build up the compiler parameters. We want it a non debug in memory library, and
            //we want to reference all the libraries our current app is referencing
            var compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = true;
            compilerParameters.IncludeDebugInformation = false;
            compilerParameters.GenerateExecutable = false;
            compilerParameters.CompilerOptions = "/target:library /optimize";
            compilerParameters.TreatWarningsAsErrors = false;

            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(!assembly.IsDynamic)
                    compilerParameters.ReferencedAssemblies.Add(assembly.Location);
            }

            var result = codeDom.CompileAssemblyFromDom(compilerParameters, generator.GeneratedCode);
            
            //if something went wrong throw an error
            if (result.Errors != null && result.Errors.Count > 0)
                throw new CompilationException(result.Errors);
            
            return result.CompiledAssembly.GetType(DynamicNamespace + "." + className);

        }

    }

    public class CompilationException : Exception
    {
        public IEnumerable<CompilerError> CompileErrors { get; private set; }

        public CompilationException(string message, params object[] messageParams)
            : base(String.Format(message, messageParams))
        {
            CompileErrors = new List<CompilerError>();
        }

        public CompilationException(IEnumerable<CompilerError> compileErrors)
            : base(string.Join(Environment.NewLine, compileErrors.Select(e => e.ToString())))
        {
            this.CompileErrors = compileErrors;
        }

        public CompilationException(CompilerErrorCollection compileErrors)
            : this(compileErrors.OfType<CompilerError>())
        {

        }
    }
}