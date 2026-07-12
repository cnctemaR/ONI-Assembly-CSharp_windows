using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class OperandQuery : ValueQuery
	{
		public OperandQuery(object val)
		{
			this.val = val;
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			return this.val;
		}

		public override XPathResultType StaticType
		{
			get
			{
				return base.GetXPathType(this.val);
			}
		}

		public override XPathNodeIterator Clone()
		{
			return this;
		}

		internal object val;
	}
}
