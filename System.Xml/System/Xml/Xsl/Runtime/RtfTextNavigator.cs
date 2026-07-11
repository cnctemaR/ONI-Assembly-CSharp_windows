using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class RtfTextNavigator : RtfNavigator
	{
		public RtfTextNavigator(string text, string baseUri)
		{
			this.text = text;
			this.baseUri = baseUri;
			this.constr = new NavigatorConstructor();
		}

		public RtfTextNavigator(RtfTextNavigator that)
		{
			this.text = that.text;
			this.baseUri = that.baseUri;
			this.constr = that.constr;
		}

		public override void CopyToWriter(XmlWriter writer)
		{
			writer.WriteString(this.Value);
		}

		public override XPathNavigator ToNavigator()
		{
			return this.constr.GetNavigator(this.text, this.baseUri, new NameTable());
		}

		public override string Value
		{
			get
			{
				return this.text;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.baseUri;
			}
		}

		public override XPathNavigator Clone()
		{
			return new RtfTextNavigator(this);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			RtfTextNavigator rtfTextNavigator = other as RtfTextNavigator;
			if (rtfTextNavigator != null)
			{
				this.text = rtfTextNavigator.text;
				this.baseUri = rtfTextNavigator.baseUri;
				this.constr = rtfTextNavigator.constr;
				return true;
			}
			return false;
		}

		private string text;

		private string baseUri;

		private NavigatorConstructor constr;
	}
}
