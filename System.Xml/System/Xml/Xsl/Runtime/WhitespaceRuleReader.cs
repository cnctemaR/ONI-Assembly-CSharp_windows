using System;

namespace System.Xml.Xsl.Runtime
{
	internal class WhitespaceRuleReader : XmlWrappingReader
	{
		public static XmlReader CreateReader(XmlReader baseReader, WhitespaceRuleLookup wsRules)
		{
			if (wsRules == null)
			{
				return baseReader;
			}
			XmlReaderSettings settings = baseReader.Settings;
			if (settings != null)
			{
				if (settings.IgnoreWhitespace)
				{
					return baseReader;
				}
			}
			else
			{
				XmlTextReader xmlTextReader = baseReader as XmlTextReader;
				if (xmlTextReader != null && xmlTextReader.WhitespaceHandling == WhitespaceHandling.None)
				{
					return baseReader;
				}
				XmlTextReaderImpl xmlTextReaderImpl = baseReader as XmlTextReaderImpl;
				if (xmlTextReaderImpl != null && xmlTextReaderImpl.WhitespaceHandling == WhitespaceHandling.None)
				{
					return baseReader;
				}
			}
			return new WhitespaceRuleReader(baseReader, wsRules);
		}

		private WhitespaceRuleReader(XmlReader baseReader, WhitespaceRuleLookup wsRules)
			: base(baseReader)
		{
			this.val = null;
			this.stkStrip = new BitStack();
			this.shouldStrip = false;
			this.preserveAdjacent = false;
			this.wsRules = wsRules;
			this.wsRules.Atomize(baseReader.NameTable);
		}

		public override string Value
		{
			get
			{
				if (this.val != null)
				{
					return this.val;
				}
				return base.Value;
			}
		}

		public override bool Read()
		{
			XmlCharType instance = XmlCharType.Instance;
			string text = null;
			this.val = null;
			while (base.Read())
			{
				XmlNodeType nodeType = base.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType - XmlNodeType.Text > 1)
					{
						switch (nodeType)
						{
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
							break;
						case XmlNodeType.EndElement:
							this.shouldStrip = this.stkStrip.PopBit();
							goto IL_010E;
						case XmlNodeType.EndEntity:
							continue;
						default:
							goto IL_010E;
						}
					}
					else
					{
						if (this.preserveAdjacent)
						{
							return true;
						}
						if (!this.shouldStrip)
						{
							goto IL_010E;
						}
						if (!instance.IsOnlyWhitespace(base.Value))
						{
							if (text != null)
							{
								this.val = text + base.Value;
							}
							this.preserveAdjacent = true;
							return true;
						}
					}
					if (this.preserveAdjacent)
					{
						return true;
					}
					if (this.shouldStrip)
					{
						if (text == null)
						{
							text = base.Value;
							continue;
						}
						text += base.Value;
						continue;
					}
				}
				else if (!base.IsEmptyElement)
				{
					this.stkStrip.PushBit(this.shouldStrip);
					this.shouldStrip = this.wsRules.ShouldStripSpace(base.LocalName, base.NamespaceURI) && base.XmlSpace != XmlSpace.Preserve;
				}
				IL_010E:
				this.preserveAdjacent = false;
				return true;
			}
			return false;
		}

		private WhitespaceRuleLookup wsRules;

		private BitStack stkStrip;

		private bool shouldStrip;

		private bool preserveAdjacent;

		private string val;

		private XmlCharType xmlCharType = XmlCharType.Instance;
	}
}
