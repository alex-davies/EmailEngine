using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Parser;

namespace EmailEngine.Razor.Generation
{
    class ModelSpan : Span
    {
        /// <summary>
        /// The class name the model is expected to be
        /// </summary>
        public string ModelType { get; set; }

        public ModelSpan(ParserContext context, string modelType)
            : base(context, SpanKind.MetaCode)
        {
            ModelType = modelType;
        }

        
    }
}
