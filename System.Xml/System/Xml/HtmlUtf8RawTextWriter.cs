using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Xml
{
	internal class HtmlUtf8RawTextWriter : XmlUtf8RawTextWriter
	{
		public HtmlUtf8RawTextWriter(Stream stream, XmlWriterSettings settings)
			: base(stream, settings)
		{
			this.Init(settings);
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			base.RawText("<!DOCTYPE ");
			if (name == "HTML")
			{
				base.RawText("HTML");
			}
			else
			{
				base.RawText("html");
			}
			int num;
			if (pubid != null)
			{
				base.RawText(" PUBLIC \"");
				base.RawText(pubid);
				if (sysid != null)
				{
					base.RawText("\" \"");
					base.RawText(sysid);
				}
				byte[] bufBytes = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes[num] = 34;
			}
			else if (sysid != null)
			{
				base.RawText(" SYSTEM \"");
				base.RawText(sysid);
				byte[] bufBytes2 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes2[num] = 34;
			}
			else
			{
				byte[] bufBytes3 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes3[num] = 32;
			}
			if (subset != null)
			{
				byte[] bufBytes4 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes4[num] = 91;
				base.RawText(subset);
				byte[] bufBytes5 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes5[num] = 93;
			}
			byte[] bufBytes6 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes6[num] = 62;
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.elementScope.Push((byte)this.currentElementProperties);
			if (ns.Length == 0)
			{
				this.currentElementProperties = (ElementProperties)HtmlUtf8RawTextWriter.elementPropertySearch.FindCaseInsensitiveString(localName);
				byte[] bufBytes = this.bufBytes;
				int bufPos = this.bufPos;
				this.bufPos = bufPos + 1;
				bufBytes[bufPos] = 60;
				base.RawText(localName);
				this.attrEndPos = this.bufPos;
				return;
			}
			this.currentElementProperties = ElementProperties.HAS_NS;
			base.WriteStartElement(prefix, localName, ns);
		}

		internal override void StartElementContent()
		{
			byte[] bufBytes = this.bufBytes;
			int bufPos = this.bufPos;
			this.bufPos = bufPos + 1;
			bufBytes[bufPos] = 62;
			this.contentPos = this.bufPos;
			if ((this.currentElementProperties & ElementProperties.HEAD) != ElementProperties.DEFAULT)
			{
				this.WriteMetaElement();
			}
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			if (ns.Length == 0)
			{
				if ((this.currentElementProperties & ElementProperties.EMPTY) == ElementProperties.DEFAULT)
				{
					byte[] bufBytes = this.bufBytes;
					int num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes[num] = 60;
					byte[] bufBytes2 = this.bufBytes;
					num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes2[num] = 47;
					base.RawText(localName);
					byte[] bufBytes3 = this.bufBytes;
					num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes3[num] = 62;
				}
			}
			else
			{
				base.WriteEndElement(prefix, localName, ns);
			}
			this.currentElementProperties = (ElementProperties)this.elementScope.Pop();
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			if (ns.Length == 0)
			{
				if ((this.currentElementProperties & ElementProperties.EMPTY) == ElementProperties.DEFAULT)
				{
					byte[] bufBytes = this.bufBytes;
					int num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes[num] = 60;
					byte[] bufBytes2 = this.bufBytes;
					num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes2[num] = 47;
					base.RawText(localName);
					byte[] bufBytes3 = this.bufBytes;
					num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes3[num] = 62;
				}
			}
			else
			{
				base.WriteFullEndElement(prefix, localName, ns);
			}
			this.currentElementProperties = (ElementProperties)this.elementScope.Pop();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (ns.Length == 0)
			{
				int num;
				if (this.attrEndPos == this.bufPos)
				{
					byte[] bufBytes = this.bufBytes;
					num = this.bufPos;
					this.bufPos = num + 1;
					bufBytes[num] = 32;
				}
				base.RawText(localName);
				if ((this.currentElementProperties & (ElementProperties)7U) != ElementProperties.DEFAULT)
				{
					this.currentAttributeProperties = (AttributeProperties)((ElementProperties)HtmlUtf8RawTextWriter.attributePropertySearch.FindCaseInsensitiveString(localName) & this.currentElementProperties);
					if ((this.currentAttributeProperties & AttributeProperties.BOOLEAN) != AttributeProperties.DEFAULT)
					{
						this.inAttributeValue = true;
						return;
					}
				}
				else
				{
					this.currentAttributeProperties = AttributeProperties.DEFAULT;
				}
				byte[] bufBytes2 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes2[num] = 61;
				byte[] bufBytes3 = this.bufBytes;
				num = this.bufPos;
				this.bufPos = num + 1;
				bufBytes3[num] = 34;
			}
			else
			{
				base.WriteStartAttribute(prefix, localName, ns);
				this.currentAttributeProperties = AttributeProperties.DEFAULT;
			}
			this.inAttributeValue = true;
		}

		public override void WriteEndAttribute()
		{
			if ((this.currentAttributeProperties & AttributeProperties.BOOLEAN) != AttributeProperties.DEFAULT)
			{
				this.attrEndPos = this.bufPos;
			}
			else
			{
				if (this.endsWithAmpersand)
				{
					this.OutputRestAmps();
					this.endsWithAmpersand = false;
				}
				byte[] bufBytes = this.bufBytes;
				int bufPos = this.bufPos;
				this.bufPos = bufPos + 1;
				bufBytes[bufPos] = 34;
			}
			this.inAttributeValue = false;
			this.attrEndPos = this.bufPos;
		}

		public override void WriteProcessingInstruction(string target, string text)
		{
			byte[] bufBytes = this.bufBytes;
			int num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes[num] = 60;
			byte[] bufBytes2 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes2[num] = 63;
			base.RawText(target);
			byte[] bufBytes3 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes3[num] = 32;
			base.WriteCommentOrPi(text, 63);
			byte[] bufBytes4 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes4[num] = 62;
			if (this.bufPos > this.bufLen)
			{
				this.FlushBuffer();
			}
		}

		public unsafe override void WriteString(string text)
		{
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + text.Length;
				if (this.inAttributeValue)
				{
					this.WriteHtmlAttributeTextBlock(ptr, ptr2);
				}
				else
				{
					this.WriteHtmlElementTextBlock(ptr, ptr2);
				}
			}
		}

		public override void WriteEntityRef(string name)
		{
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
		}

		public override void WriteCharEntity(char ch)
		{
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
		}

		public unsafe override void WriteChars(char[] buffer, int index, int count)
		{
			fixed (char* ptr = &buffer[index])
			{
				char* ptr2 = ptr;
				if (this.inAttributeValue)
				{
					base.WriteAttributeTextBlock(ptr2, ptr2 + count);
				}
				else
				{
					base.WriteElementTextBlock(ptr2, ptr2 + count);
				}
			}
		}

		private void Init(XmlWriterSettings settings)
		{
			if (HtmlUtf8RawTextWriter.elementPropertySearch == null)
			{
				HtmlUtf8RawTextWriter.attributePropertySearch = new TernaryTreeReadOnly(HtmlTernaryTree.htmlAttributes);
				HtmlUtf8RawTextWriter.elementPropertySearch = new TernaryTreeReadOnly(HtmlTernaryTree.htmlElements);
			}
			this.elementScope = new ByteStack(10);
			this.uriEscapingBuffer = new byte[5];
			this.currentElementProperties = ElementProperties.DEFAULT;
			this.mediaType = settings.MediaType;
			this.doNotEscapeUriAttributes = settings.DoNotEscapeUriAttributes;
		}

		protected void WriteMetaElement()
		{
			base.RawText("<META http-equiv=\"Content-Type\"");
			if (this.mediaType == null)
			{
				this.mediaType = "text/html";
			}
			base.RawText(" content=\"");
			base.RawText(this.mediaType);
			base.RawText("; charset=");
			base.RawText(this.encoding.WebName);
			base.RawText("\">");
		}

		protected unsafe void WriteHtmlElementTextBlock(char* pSrc, char* pSrcEnd)
		{
			if ((this.currentElementProperties & ElementProperties.NO_ENTITIES) != ElementProperties.DEFAULT)
			{
				base.RawText(pSrc, pSrcEnd);
				return;
			}
			base.WriteElementTextBlock(pSrc, pSrcEnd);
		}

		protected unsafe void WriteHtmlAttributeTextBlock(char* pSrc, char* pSrcEnd)
		{
			if ((this.currentAttributeProperties & (AttributeProperties)7U) != AttributeProperties.DEFAULT)
			{
				if ((this.currentAttributeProperties & AttributeProperties.BOOLEAN) != AttributeProperties.DEFAULT)
				{
					return;
				}
				if ((this.currentAttributeProperties & (AttributeProperties)5U) != AttributeProperties.DEFAULT && !this.doNotEscapeUriAttributes)
				{
					this.WriteUriAttributeText(pSrc, pSrcEnd);
					return;
				}
				this.WriteHtmlAttributeText(pSrc, pSrcEnd);
				return;
			}
			else
			{
				if ((this.currentElementProperties & ElementProperties.HAS_NS) != ElementProperties.DEFAULT)
				{
					base.WriteAttributeTextBlock(pSrc, pSrcEnd);
					return;
				}
				this.WriteHtmlAttributeText(pSrc, pSrcEnd);
				return;
			}
		}

		private unsafe void WriteHtmlAttributeText(char* pSrc, char* pSrcEnd)
		{
			if (this.endsWithAmpersand)
			{
				if ((long)(pSrcEnd - pSrc) > 0L && *pSrc != '{')
				{
					this.OutputRestAmps();
				}
				this.endsWithAmpersand = false;
			}
			byte[] array;
			byte* ptr;
			if ((array = this.bufBytes) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte* ptr2 = ptr + this.bufPos;
			char c = '\0';
			for (;;)
			{
				byte* ptr3 = ptr2 + (long)(pSrcEnd - pSrc);
				if (ptr3 != ptr + this.bufLen)
				{
					ptr3 = ptr + this.bufLen;
				}
				while (ptr2 < ptr3 && (this.xmlCharType.charProperties[(int)(c = *pSrc)] & 128) != 0 && c <= '\u007f')
				{
					*(ptr2++) = (byte)c;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					break;
				}
				if (ptr2 < ptr3)
				{
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							goto IL_0136;
						case '\n':
							ptr2 = XmlUtf8RawTextWriter.LineFeedEntity(ptr2);
							goto IL_0162;
						case '\v':
						case '\f':
							break;
						case '\r':
							ptr2 = XmlUtf8RawTextWriter.CarriageReturnEntity(ptr2);
							goto IL_0162;
						default:
							if (c == '"')
							{
								ptr2 = XmlUtf8RawTextWriter.QuoteEntity(ptr2);
								goto IL_0162;
							}
							if (c == '&')
							{
								if (pSrc + 1 == pSrcEnd)
								{
									this.endsWithAmpersand = true;
								}
								else if (pSrc[1] != '{')
								{
									ptr2 = XmlUtf8RawTextWriter.AmpEntity(ptr2);
									goto IL_0162;
								}
								*(ptr2++) = (byte)c;
								goto IL_0162;
							}
							break;
						}
					}
					else if (c == '\'' || c == '<' || c == '>')
					{
						goto IL_0136;
					}
					base.EncodeChar(ref pSrc, pSrcEnd, ref ptr2);
					continue;
					IL_0162:
					pSrc++;
					continue;
					IL_0136:
					*(ptr2++) = (byte)c;
					goto IL_0162;
				}
				this.bufPos = (int)((long)(ptr2 - ptr));
				this.FlushBuffer();
				ptr2 = ptr + 1;
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			array = null;
		}

		private unsafe void WriteUriAttributeText(char* pSrc, char* pSrcEnd)
		{
			if (this.endsWithAmpersand)
			{
				if ((long)(pSrcEnd - pSrc) > 0L && *pSrc != '{')
				{
					this.OutputRestAmps();
				}
				this.endsWithAmpersand = false;
			}
			byte[] array;
			byte* ptr;
			if ((array = this.bufBytes) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte* ptr2 = ptr + this.bufPos;
			char c = '\0';
			for (;;)
			{
				byte* ptr3 = ptr2 + (long)(pSrcEnd - pSrc);
				if (ptr3 != ptr + this.bufLen)
				{
					ptr3 = ptr + this.bufLen;
				}
				while (ptr2 < ptr3 && (this.xmlCharType.charProperties[(int)(c = *pSrc)] & 128) != 0 && c < '\u0080')
				{
					*(ptr2++) = (byte)c;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					break;
				}
				if (ptr2 < ptr3)
				{
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							goto IL_0142;
						case '\n':
							ptr2 = XmlUtf8RawTextWriter.LineFeedEntity(ptr2);
							goto IL_01E3;
						case '\v':
						case '\f':
							break;
						case '\r':
							ptr2 = XmlUtf8RawTextWriter.CarriageReturnEntity(ptr2);
							goto IL_01E3;
						default:
							if (c == '"')
							{
								ptr2 = XmlUtf8RawTextWriter.QuoteEntity(ptr2);
								goto IL_01E3;
							}
							if (c == '&')
							{
								if (pSrc + 1 == pSrcEnd)
								{
									this.endsWithAmpersand = true;
								}
								else if (pSrc[1] != '{')
								{
									ptr2 = XmlUtf8RawTextWriter.AmpEntity(ptr2);
									goto IL_01E3;
								}
								*(ptr2++) = (byte)c;
								goto IL_01E3;
							}
							break;
						}
					}
					else if (c == '\'' || c == '<' || c == '>')
					{
						goto IL_0142;
					}
					byte[] array2;
					byte* ptr4;
					if ((array2 = this.uriEscapingBuffer) == null || array2.Length == 0)
					{
						ptr4 = null;
					}
					else
					{
						ptr4 = &array2[0];
					}
					byte* ptr5 = ptr4;
					byte* ptr6 = ptr5;
					XmlUtf8RawTextWriter.CharToUTF8(ref pSrc, pSrcEnd, ref ptr6);
					while (ptr5 < ptr6)
					{
						*(ptr2++) = 37;
						*(ptr2++) = (byte)"0123456789ABCDEF"[*ptr5 >> 4];
						*(ptr2++) = (byte)"0123456789ABCDEF"[(int)(*ptr5 & 15)];
						ptr5++;
					}
					array2 = null;
					continue;
					IL_01E3:
					pSrc++;
					continue;
					IL_0142:
					*(ptr2++) = (byte)c;
					goto IL_01E3;
				}
				this.bufPos = (int)((long)(ptr2 - ptr));
				this.FlushBuffer();
				ptr2 = ptr + 1;
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			array = null;
		}

		private void OutputRestAmps()
		{
			byte[] bufBytes = this.bufBytes;
			int num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes[num] = 97;
			byte[] bufBytes2 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes2[num] = 109;
			byte[] bufBytes3 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes3[num] = 112;
			byte[] bufBytes4 = this.bufBytes;
			num = this.bufPos;
			this.bufPos = num + 1;
			bufBytes4[num] = 59;
		}

		protected ByteStack elementScope;

		protected ElementProperties currentElementProperties;

		private AttributeProperties currentAttributeProperties;

		private bool endsWithAmpersand;

		private byte[] uriEscapingBuffer;

		private string mediaType;

		private bool doNotEscapeUriAttributes;

		protected static TernaryTreeReadOnly elementPropertySearch;

		protected static TernaryTreeReadOnly attributePropertySearch;

		private const int StackIncrement = 10;
	}
}
