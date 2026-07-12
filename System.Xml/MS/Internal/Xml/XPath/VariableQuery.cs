using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class VariableQuery : ExtensionQuery
	{
		public VariableQuery(string name, string prefix)
			: base(prefix, name)
		{
		}

		private VariableQuery(VariableQuery other)
			: base(other)
		{
			this._variable = other._variable;
		}

		public override void SetXsltContext(XsltContext context)
		{
			if (context == null)
			{
				throw XPathException.Create("Namespace Manager or XsltContext needed. This query has a prefix, variable, or user-defined function.");
			}
			if (this.xsltContext != context)
			{
				this.xsltContext = context;
				this._variable = this.xsltContext.ResolveVariable(this.prefix, this.name);
				if (this._variable == null)
				{
					throw XPathException.Create("The variable '{0}' is undefined.", base.QName);
				}
			}
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			if (this.xsltContext == null)
			{
				throw XPathException.Create("Namespace Manager or XsltContext needed. This query has a prefix, variable, or user-defined function.");
			}
			return base.ProcessResult(this._variable.Evaluate(this.xsltContext));
		}

		public override XPathResultType StaticType
		{
			get
			{
				if (this._variable != null)
				{
					return base.GetXPathType(this.Evaluate(null));
				}
				XPathResultType xpathResultType = ((this._variable != null) ? this._variable.VariableType : XPathResultType.Any);
				if (xpathResultType == XPathResultType.Error)
				{
					xpathResultType = XPathResultType.Any;
				}
				return xpathResultType;
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new VariableQuery(this);
		}

		private IXsltContextVariable _variable;
	}
}
