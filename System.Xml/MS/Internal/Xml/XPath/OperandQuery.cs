using System;
using System.Globalization;
using System.Xml;
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("value", Convert.ToString(this.val, CultureInfo.InvariantCulture));
			w.WriteEndElement();
		}

		internal object val;
	}
}
