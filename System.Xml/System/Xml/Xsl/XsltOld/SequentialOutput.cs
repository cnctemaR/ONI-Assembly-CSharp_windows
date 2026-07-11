using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Xml.Xsl.XsltOld
{
	internal abstract class SequentialOutput : RecordOutput
	{
		private void CacheOuptutProps(XsltOutput output)
		{
			this.output = output;
			this.isXmlOutput = this.output.Method == XsltOutput.OutputMethod.Xml;
			this.isHtmlOutput = this.output.Method == XsltOutput.OutputMethod.Html;
			this.cdataElements = this.output.CDataElements;
			this.indentOutput = this.output.Indent;
			this.outputDoctype = this.output.DoctypeSystem != null || (this.isHtmlOutput && this.output.DoctypePublic != null);
			this.outputXmlDecl = this.isXmlOutput && !this.output.OmitXmlDeclaration && !this.omitXmlDeclCalled;
		}

		internal SequentialOutput(Processor processor)
		{
			this.processor = processor;
			this.CacheOuptutProps(processor.Output);
		}

		public void OmitXmlDecl()
		{
			this.omitXmlDeclCalled = true;
			this.outputXmlDecl = false;
		}

		private void WriteStartElement(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			HtmlElementProps htmlElementProps = null;
			if (this.isHtmlOutput)
			{
				if (mainNode.Prefix.Length == 0)
				{
					htmlElementProps = mainNode.htmlProps;
					if (htmlElementProps == null && mainNode.search)
					{
						htmlElementProps = HtmlElementProps.GetProps(mainNode.LocalName);
					}
					record.Manager.CurrentElementScope.HtmlElementProps = htmlElementProps;
					mainNode.IsEmptyTag = false;
				}
			}
			else if (this.isXmlOutput && mainNode.Depth == 0)
			{
				if (this.secondRoot && (this.output.DoctypeSystem != null || this.output.Standalone))
				{
					throw XsltException.Create("There are multiple root elements in the output XML.", Array.Empty<string>());
				}
				this.secondRoot = true;
			}
			if (this.outputDoctype)
			{
				this.WriteDoctype(mainNode);
				this.outputDoctype = false;
			}
			if (this.cdataElements != null && this.cdataElements.Contains(new XmlQualifiedName(mainNode.LocalName, mainNode.NamespaceURI)) && this.isXmlOutput)
			{
				record.Manager.CurrentElementScope.ToCData = true;
			}
			this.Indent(record);
			this.Write('<');
			this.WriteName(mainNode.Prefix, mainNode.LocalName);
			this.WriteAttributes(record.AttributeList, record.AttributeCount, htmlElementProps);
			if (mainNode.IsEmptyTag)
			{
				this.Write(" />");
			}
			else
			{
				this.Write('>');
			}
			if (htmlElementProps != null && htmlElementProps.Head)
			{
				BuilderInfo builderInfo = mainNode;
				int num = builderInfo.Depth;
				builderInfo.Depth = num + 1;
				this.Indent(record);
				BuilderInfo builderInfo2 = mainNode;
				num = builderInfo2.Depth;
				builderInfo2.Depth = num - 1;
				this.Write("<META http-equiv=\"Content-Type\" content=\"");
				this.Write(this.output.MediaType);
				this.Write("; charset=");
				this.Write(this.encoding.WebName);
				this.Write("\">");
			}
		}

		private void WriteTextNode(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			OutputScope currentElementScope = record.Manager.CurrentElementScope;
			currentElementScope.Mixed = true;
			if (currentElementScope.HtmlElementProps != null && currentElementScope.HtmlElementProps.NoEntities)
			{
				this.Write(mainNode.Value);
				return;
			}
			if (currentElementScope.ToCData)
			{
				this.WriteCDataSection(mainNode.Value);
				return;
			}
			this.WriteTextNode(mainNode);
		}

		private void WriteTextNode(BuilderInfo node)
		{
			for (int i = 0; i < node.TextInfoCount; i++)
			{
				string text = node.TextInfo[i];
				if (text == null)
				{
					i++;
					this.Write(node.TextInfo[i]);
				}
				else
				{
					this.WriteWithReplace(text, SequentialOutput.s_TextValueFind, SequentialOutput.s_TextValueReplace);
				}
			}
		}

		private void WriteCDataSection(string value)
		{
			this.Write("<![CDATA[");
			this.WriteCData(value);
			this.Write("]]>");
		}

		private void WriteDoctype(BuilderInfo mainNode)
		{
			this.Indent(0);
			this.Write("<!DOCTYPE ");
			if (this.isXmlOutput)
			{
				this.WriteName(mainNode.Prefix, mainNode.LocalName);
			}
			else
			{
				this.WriteName(string.Empty, "html");
			}
			this.Write(' ');
			if (this.output.DoctypePublic != null)
			{
				this.Write("PUBLIC ");
				this.Write('"');
				this.Write(this.output.DoctypePublic);
				this.Write("\" ");
			}
			else
			{
				this.Write("SYSTEM ");
			}
			if (this.output.DoctypeSystem != null)
			{
				this.Write('"');
				this.Write(this.output.DoctypeSystem);
				this.Write('"');
			}
			this.Write('>');
		}

		private void WriteXmlDeclaration()
		{
			this.outputXmlDecl = false;
			this.Indent(0);
			this.Write("<?");
			this.WriteName(string.Empty, "xml");
			this.Write(" version=\"1.0\"");
			if (this.encoding != null)
			{
				this.Write(" encoding=\"");
				this.Write(this.encoding.WebName);
				this.Write('"');
			}
			if (this.output.HasStandalone)
			{
				this.Write(" standalone=\"");
				this.Write(this.output.Standalone ? "yes" : "no");
				this.Write('"');
			}
			this.Write("?>");
		}

		private void WriteProcessingInstruction(RecordBuilder record)
		{
			this.Indent(record);
			this.WriteProcessingInstruction(record.MainNode);
		}

		private void WriteProcessingInstruction(BuilderInfo node)
		{
			this.Write("<?");
			this.WriteName(node.Prefix, node.LocalName);
			this.Write(' ');
			this.Write(node.Value);
			if (this.isHtmlOutput)
			{
				this.Write('>');
				return;
			}
			this.Write("?>");
		}

		private void WriteEndElement(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			HtmlElementProps htmlElementProps = record.Manager.CurrentElementScope.HtmlElementProps;
			if (htmlElementProps != null && htmlElementProps.Empty)
			{
				return;
			}
			this.Indent(record);
			this.Write("</");
			this.WriteName(record.MainNode.Prefix, record.MainNode.LocalName);
			this.Write('>');
		}

		public Processor.OutputResult RecordDone(RecordBuilder record)
		{
			if (this.output.Method == XsltOutput.OutputMethod.Unknown)
			{
				if (!this.DecideDefaultOutput(record.MainNode))
				{
					this.CacheRecord(record);
				}
				else
				{
					this.OutputCachedRecords();
					this.OutputRecord(record);
				}
			}
			else
			{
				this.OutputRecord(record);
			}
			record.Reset();
			return Processor.OutputResult.Continue;
		}

		public void TheEnd()
		{
			this.OutputCachedRecords();
			this.Close();
		}

		private bool DecideDefaultOutput(BuilderInfo node)
		{
			XsltOutput.OutputMethod outputMethod = XsltOutput.OutputMethod.Xml;
			XmlNodeType nodeType = node.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Text && nodeType - XmlNodeType.Whitespace > 1)
				{
					return false;
				}
				if (this.xmlCharType.IsOnlyWhitespace(node.Value))
				{
					return false;
				}
				outputMethod = XsltOutput.OutputMethod.Xml;
			}
			else if (node.NamespaceURI.Length == 0 && string.Compare("html", node.LocalName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				outputMethod = XsltOutput.OutputMethod.Html;
			}
			if (this.processor.SetDefaultOutput(outputMethod))
			{
				this.CacheOuptutProps(this.processor.Output);
			}
			return true;
		}

		private void CacheRecord(RecordBuilder record)
		{
			if (this.outputCache == null)
			{
				this.outputCache = new ArrayList();
			}
			this.outputCache.Add(record.MainNode.Clone());
		}

		private void OutputCachedRecords()
		{
			if (this.outputCache == null)
			{
				return;
			}
			for (int i = 0; i < this.outputCache.Count; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)this.outputCache[i];
				this.OutputRecord(builderInfo);
			}
			this.outputCache = null;
		}

		private void OutputRecord(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			if (this.outputXmlDecl)
			{
				this.WriteXmlDeclaration();
			}
			switch (mainNode.NodeType)
			{
			case XmlNodeType.Element:
				this.WriteStartElement(record);
				return;
			case XmlNodeType.Attribute:
			case XmlNodeType.CDATA:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.WriteTextNode(record);
				return;
			case XmlNodeType.EntityReference:
				this.Write('&');
				this.WriteName(mainNode.Prefix, mainNode.LocalName);
				this.Write(';');
				return;
			case XmlNodeType.ProcessingInstruction:
				this.WriteProcessingInstruction(record);
				return;
			case XmlNodeType.Comment:
				this.Indent(record);
				this.Write("<!--");
				this.Write(mainNode.Value);
				this.Write("-->");
				return;
			case XmlNodeType.DocumentType:
				this.Write(mainNode.Value);
				return;
			case XmlNodeType.EndElement:
				this.WriteEndElement(record);
				break;
			default:
				return;
			}
		}

		private void OutputRecord(BuilderInfo node)
		{
			if (this.outputXmlDecl)
			{
				this.WriteXmlDeclaration();
			}
			this.Indent(0);
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Attribute:
			case XmlNodeType.CDATA:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
			case XmlNodeType.EndElement:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.WriteTextNode(node);
				return;
			case XmlNodeType.EntityReference:
				this.Write('&');
				this.WriteName(node.Prefix, node.LocalName);
				this.Write(';');
				return;
			case XmlNodeType.ProcessingInstruction:
				this.WriteProcessingInstruction(node);
				return;
			case XmlNodeType.Comment:
				this.Write("<!--");
				this.Write(node.Value);
				this.Write("-->");
				return;
			case XmlNodeType.DocumentType:
				this.Write(node.Value);
				break;
			default:
				return;
			}
		}

		private void WriteName(string prefix, string name)
		{
			if (prefix != null && prefix.Length > 0)
			{
				this.Write(prefix);
				if (name == null || name.Length <= 0)
				{
					return;
				}
				this.Write(':');
			}
			this.Write(name);
		}

		private void WriteXmlAttributeValue(string value)
		{
			this.WriteWithReplace(value, SequentialOutput.s_XmlAttributeValueFind, SequentialOutput.s_XmlAttributeValueReplace);
		}

		private void WriteHtmlAttributeValue(string value)
		{
			int length = value.Length;
			int i = 0;
			while (i < length)
			{
				char c = value[i];
				i++;
				if (c != '"')
				{
					if (c == '&')
					{
						if (i != length && value[i] == '{')
						{
							this.Write(c);
						}
						else
						{
							this.Write("&amp;");
						}
					}
					else
					{
						this.Write(c);
					}
				}
				else
				{
					this.Write("&quot;");
				}
			}
		}

		private void WriteHtmlUri(string value)
		{
			int length = value.Length;
			int i = 0;
			while (i < length)
			{
				char c = value[i];
				i++;
				if (c <= '\r')
				{
					if (c == '\n')
					{
						this.Write("&#xA;");
						continue;
					}
					if (c == '\r')
					{
						this.Write("&#xD;");
						continue;
					}
				}
				else
				{
					if (c == '"')
					{
						this.Write("&quot;");
						continue;
					}
					if (c == '&')
					{
						if (i != length && value[i] == '{')
						{
							this.Write(c);
							continue;
						}
						this.Write("&amp;");
						continue;
					}
				}
				if ('\u007f' < c)
				{
					if (this.utf8Encoding == null)
					{
						this.utf8Encoding = Encoding.UTF8;
						this.byteBuffer = new byte[this.utf8Encoding.GetMaxByteCount(1)];
					}
					int bytes = this.utf8Encoding.GetBytes(value, i - 1, 1, this.byteBuffer, 0);
					for (int j = 0; j < bytes; j++)
					{
						this.Write("%");
						uint num = (uint)this.byteBuffer[j];
						this.Write(num.ToString("X2", CultureInfo.InvariantCulture));
					}
				}
				else
				{
					this.Write(c);
				}
			}
		}

		private void WriteWithReplace(string value, char[] find, string[] replace)
		{
			int length = value.Length;
			int i;
			for (i = 0; i < length; i++)
			{
				int num = value.IndexOfAny(find, i);
				if (num == -1)
				{
					break;
				}
				while (i < num)
				{
					this.Write(value[i]);
					i++;
				}
				char c = value[i];
				int num2 = find.Length - 1;
				while (0 <= num2)
				{
					if (find[num2] == c)
					{
						this.Write(replace[num2]);
						break;
					}
					num2--;
				}
			}
			if (i == 0)
			{
				this.Write(value);
				return;
			}
			while (i < length)
			{
				this.Write(value[i]);
				i++;
			}
		}

		private void WriteCData(string value)
		{
			this.Write(value.Replace("]]>", "]]]]><![CDATA[>"));
		}

		private void WriteAttributes(ArrayList list, int count, HtmlElementProps htmlElementsProps)
		{
			for (int i = 0; i < count; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)list[i];
				string value = builderInfo.Value;
				bool flag = false;
				bool flag2 = false;
				if (htmlElementsProps != null && builderInfo.Prefix.Length == 0)
				{
					HtmlAttributeProps htmlAttributeProps = builderInfo.htmlAttrProps;
					if (htmlAttributeProps == null && builderInfo.search)
					{
						htmlAttributeProps = HtmlAttributeProps.GetProps(builderInfo.LocalName);
					}
					if (htmlAttributeProps != null)
					{
						flag = htmlElementsProps.AbrParent && htmlAttributeProps.Abr;
						flag2 = htmlElementsProps.UriParent && (htmlAttributeProps.Uri || (htmlElementsProps.NameParent && htmlAttributeProps.Name));
					}
				}
				this.Write(' ');
				this.WriteName(builderInfo.Prefix, builderInfo.LocalName);
				if (!flag || string.Compare(builderInfo.LocalName, value, StringComparison.OrdinalIgnoreCase) != 0)
				{
					this.Write("=\"");
					if (flag2)
					{
						this.WriteHtmlUri(value);
					}
					else if (this.isHtmlOutput)
					{
						this.WriteHtmlAttributeValue(value);
					}
					else
					{
						this.WriteXmlAttributeValue(value);
					}
					this.Write('"');
				}
			}
		}

		private void Indent(RecordBuilder record)
		{
			if (!record.Manager.CurrentElementScope.Mixed)
			{
				this.Indent(record.MainNode.Depth);
			}
		}

		private void Indent(int depth)
		{
			if (this.firstLine)
			{
				if (this.indentOutput)
				{
					this.firstLine = false;
				}
				return;
			}
			this.Write("\r\n");
			int num = 2 * depth;
			while (0 < num)
			{
				this.Write(" ");
				num--;
			}
		}

		internal abstract void Write(char outputChar);

		internal abstract void Write(string outputText);

		internal abstract void Close();

		private const char s_Colon = ':';

		private const char s_GreaterThan = '>';

		private const char s_LessThan = '<';

		private const char s_Space = ' ';

		private const char s_Quote = '"';

		private const char s_Semicolon = ';';

		private const char s_NewLine = '\n';

		private const char s_Return = '\r';

		private const char s_Ampersand = '&';

		private const string s_LessThanQuestion = "<?";

		private const string s_QuestionGreaterThan = "?>";

		private const string s_LessThanSlash = "</";

		private const string s_SlashGreaterThan = " />";

		private const string s_EqualQuote = "=\"";

		private const string s_DocType = "<!DOCTYPE ";

		private const string s_CommentBegin = "<!--";

		private const string s_CommentEnd = "-->";

		private const string s_CDataBegin = "<![CDATA[";

		private const string s_CDataEnd = "]]>";

		private const string s_VersionAll = " version=\"1.0\"";

		private const string s_Standalone = " standalone=\"";

		private const string s_EncodingStart = " encoding=\"";

		private const string s_Public = "PUBLIC ";

		private const string s_System = "SYSTEM ";

		private const string s_Html = "html";

		private const string s_QuoteSpace = "\" ";

		private const string s_CDataSplit = "]]]]><![CDATA[>";

		private const string s_EnLessThan = "&lt;";

		private const string s_EnGreaterThan = "&gt;";

		private const string s_EnAmpersand = "&amp;";

		private const string s_EnQuote = "&quot;";

		private const string s_EnNewLine = "&#xA;";

		private const string s_EnReturn = "&#xD;";

		private const string s_EndOfLine = "\r\n";

		private static char[] s_TextValueFind = new char[] { '&', '>', '<' };

		private static string[] s_TextValueReplace = new string[] { "&amp;", "&gt;", "&lt;" };

		private static char[] s_XmlAttributeValueFind = new char[] { '&', '>', '<', '"', '\n', '\r' };

		private static string[] s_XmlAttributeValueReplace = new string[] { "&amp;", "&gt;", "&lt;", "&quot;", "&#xA;", "&#xD;" };

		private Processor processor;

		protected Encoding encoding;

		private ArrayList outputCache;

		private bool firstLine = true;

		private bool secondRoot;

		private XsltOutput output;

		private bool isHtmlOutput;

		private bool isXmlOutput;

		private Hashtable cdataElements;

		private bool indentOutput;

		private bool outputDoctype;

		private bool outputXmlDecl;

		private bool omitXmlDeclCalled;

		private byte[] byteBuffer;

		private Encoding utf8Encoding;

		private XmlCharType xmlCharType = XmlCharType.Instance;
	}
}
