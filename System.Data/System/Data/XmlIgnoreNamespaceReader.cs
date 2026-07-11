using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Data
{
	internal sealed class XmlIgnoreNamespaceReader : XmlNodeReader
	{
		internal XmlIgnoreNamespaceReader(XmlDocument xdoc, string[] namespacesToIgnore)
			: base(xdoc)
		{
			this._namespacesToIgnore = new List<string>(namespacesToIgnore);
		}

		public override bool MoveToFirstAttribute()
		{
			return base.MoveToFirstAttribute() && ((!this._namespacesToIgnore.Contains(this.NamespaceURI) && (!(this.NamespaceURI == "http://www.w3.org/XML/1998/namespace") || !(this.LocalName != "lang"))) || this.MoveToNextAttribute());
		}

		public override bool MoveToNextAttribute()
		{
			bool flag;
			bool flag2;
			do
			{
				flag = false;
				flag2 = false;
				if (base.MoveToNextAttribute())
				{
					flag = true;
					if (this._namespacesToIgnore.Contains(this.NamespaceURI) || (this.NamespaceURI == "http://www.w3.org/XML/1998/namespace" && this.LocalName != "lang"))
					{
						flag2 = true;
					}
				}
			}
			while (flag2);
			return flag;
		}

		private List<string> _namespacesToIgnore;
	}
}
