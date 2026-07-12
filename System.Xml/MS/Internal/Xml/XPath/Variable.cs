using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Variable : AstNode
	{
		public Variable(string name, string prefix)
		{
			this._localname = name;
			this._prefix = prefix;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Variable;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		public string Localname
		{
			get
			{
				return this._localname;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		private string _localname;

		private string _prefix;
	}
}
