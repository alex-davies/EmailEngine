// Type: System.Web.Mvc.Razor.MvcCSharpRazorCodeParser
// Assembly: System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: c:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies\System.Web.Mvc.dll

using System;
using System.Globalization;

using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace EmailEngine.Razor.Generation
{
    public class ModelCSharpRazorCodeParser : CSharpCodeParser
    {
        private const string ModelKeyword = "model";

        public ModelCSharpRazorCodeParser()
        {
            this.RazorKeywords.Add(ModelKeyword, this.WrapSimpleBlockParser(BlockType.Directive, new CodeParser.BlockParser(block =>
            {
                ParserContextExtensions.AcceptWhiteSpace(this.Context, false);
                string modelTypeName = null;
                if (ParserHelpers.IsIdentifierStart(this.CurrentCharacter))
                {
                    using (this.Context.StartTemporaryBuffer())
                    {
                        ParserContextExtensions.AcceptUntil(this.Context, (Predicate<char>)(c => ParserHelpers.IsNewLine(c)));
                        modelTypeName = this.Context.ContentBuffer.ToString();
                        this.Context.AcceptTemporaryBuffer();
                    }
                    ParserContextExtensions.AcceptNewLine(this.Context);
                }

                this.End(new ModelSpan(this.Context, modelTypeName));
                return false;
            })));
        }

      //  protected override bool ParseInheritsStatement(CodeBlockInfo block)
      //  {
      //      this._endInheritsLocation = new SourceLocation?(this.CurrentLocation);
      //      bool flag = base.ParseInheritsStatement(block);
      //      this.CheckForInheritsAndModelStatements();
      //      return flag;
      //  }

      //  private void CheckForInheritsAndModelStatements()
      //  {
      //      if (!this._modelStatementFound || !this._endInheritsLocation.HasValue)
      //          return;
      //      this.OnError(this._endInheritsLocation.Value, string.Format((IFormatProvider)CultureInfo.CurrentCulture, MvcResources.MvcRazorCodeParser_CannotHaveModelAndInheritsKeyword, new object[1]
      //{
      //  (object) "model"
      //}));
      //  }

        //private bool ParseModelStatement(CodeBlockInfo block)
        //{
        //    SourceLocation currentLocation = this.CurrentLocation;
        //    this.End((Span)MetaCodeSpan.Create(this.Context, false, this.RequireSingleWhiteSpace() ? AcceptedCharacters.None : AcceptedCharacters.NewLine | AcceptedCharacters.WhiteSpace | AcceptedCharacters.NonWhiteSpace));
        //    if (this._modelStatementFound)
        //        this.OnError(currentLocation, string.Format((IFormatProvider)CultureInfo.CurrentCulture, MvcResources.MvcRazorCodeParser_OnlyOneModelStatementIsAllowed, new object[1]
        //{
        //  (object) "model"
        //}));
        //    this._modelStatementFound = true;
        //    ParserContextExtensions.AcceptWhiteSpace(this.Context, false);
        //    string modelTypeName = (string)null;
        //    if (ParserHelpers.IsIdentifierStart(this.CurrentCharacter))
        //    {
        //        using (this.Context.StartTemporaryBuffer())
        //        {
        //            ParserContextExtensions.AcceptUntil(this.Context, (Predicate<char>)(c => ParserHelpers.IsNewLine(c)));
        //            modelTypeName = ((object)this.Context.ContentBuffer).ToString();
        //            this.Context.AcceptTemporaryBuffer();
        //        }
        //        ParserContextExtensions.AcceptNewLine(this.Context);
        //    }
        //    else
        //        this.OnError(currentLocation, string.Format((IFormatProvider)CultureInfo.CurrentCulture, MvcResources.MvcRazorCodeParser_ModelKeywordMustBeFollowedByTypeName, new object[1]
        //{
        //  (object) "model"
        //}));
        //    this.CheckForInheritsAndModelStatements();
        //    this.End((Span)new ModelSpan(this.Context, modelTypeName));
        //    return false;
        //}
    }
}
