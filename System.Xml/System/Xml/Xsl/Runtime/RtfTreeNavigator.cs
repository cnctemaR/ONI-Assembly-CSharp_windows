using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class RtfTreeNavigator : RtfNavigator
	{
		public RtfTreeNavigator(XmlEventCache events, XmlNameTable nameTable)
		{
			this.events = events;
			this.constr = new NavigatorConstructor();
			this.nameTable = nameTable;
		}

		public RtfTreeNavigator(RtfTreeNavigator that)
		{
			this.events = that.events;
			this.constr = that.constr;
			this.nameTable = that.nameTable;
		}

		public override void CopyToWriter(XmlWriter writer)
		{
			this.events.EventsToWriter(writer);
		}

		public override XPathNavigator ToNavigator()
		{
			return this.constr.GetNavigator(this.events, this.nameTable);
		}

		public override string Value
		{
			get
			{
				return this.events.EventsToString();
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.events.BaseUri;
			}
		}

		public override XPathNavigator Clone()
		{
			return new RtfTreeNavigator(this);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			RtfTreeNavigator rtfTreeNavigator = other as RtfTreeNavigator;
			if (rtfTreeNavigator != null)
			{
				this.events = rtfTreeNavigator.events;
				this.constr = rtfTreeNavigator.constr;
				this.nameTable = rtfTreeNavigator.nameTable;
				return true;
			}
			return false;
		}

		private XmlEventCache events;

		private NavigatorConstructor constr;

		private XmlNameTable nameTable;
	}
}
