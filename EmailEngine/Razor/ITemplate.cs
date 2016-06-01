using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EmailEngine.Razor
{
    public interface ITemplate : IDisposable
    {
        void Execute();

        void Write(object value);

        void WriteLiteral(object value);
    }

    public interface ITemplate<TModel> : ITemplate
    {
        TModel Model { get; set; }
    }

    /// <summary>
    /// A base implementation for a template, used as a final resort for a base class on our generated code
    /// </summary>
    public abstract class TemplateBase : ITemplate
    {
        public StringBuilder Result = new StringBuilder();

        #region ("ITemplate methods")

        public virtual void Execute() { }

        public virtual void WriteLiteral(object value)
        {
            Result.Append(value.ToString());
        }

        public virtual void Write(object value)
        {
            Result.Append(value);
        }

        public virtual void Dispose()
        {
           
        }

        #endregion
    }

    /// <summary>
    /// A base implementation for a template with a model
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class TemplateBase<TModel> : TemplateBase, ITemplate<TModel>
    {
        public TModel Model { get; set; }

        public override void WriteLiteral(object value)
        {
            Result.Append(value);
        }
    }
}
