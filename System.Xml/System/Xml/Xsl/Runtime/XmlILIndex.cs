using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlILIndex
	{
		internal XmlILIndex()
		{
			this.table = new Dictionary<string, XmlQueryNodeSequence>();
		}

		public void Add(string key, XPathNavigator navigator)
		{
			XmlQueryNodeSequence xmlQueryNodeSequence;
			if (!this.table.TryGetValue(key, out xmlQueryNodeSequence))
			{
				xmlQueryNodeSequence = new XmlQueryNodeSequence();
				xmlQueryNodeSequence.AddClone(navigator);
				this.table.Add(key, xmlQueryNodeSequence);
				return;
			}
			if (!navigator.IsSamePosition(xmlQueryNodeSequence[xmlQueryNodeSequence.Count - 1]))
			{
				xmlQueryNodeSequence.AddClone(navigator);
			}
		}

		public XmlQueryNodeSequence Lookup(string key)
		{
			XmlQueryNodeSequence xmlQueryNodeSequence;
			if (!this.table.TryGetValue(key, out xmlQueryNodeSequence))
			{
				xmlQueryNodeSequence = new XmlQueryNodeSequence();
			}
			return xmlQueryNodeSequence;
		}

		private Dictionary<string, XmlQueryNodeSequence> table;
	}
}
