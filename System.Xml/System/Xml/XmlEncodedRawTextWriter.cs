using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlEncodedRawTextWriter : XmlRawWriter
	{
		protected XmlEncodedRawTextWriter(XmlWriterSettings settings)
		{
			this.useAsync = settings.Async;
			this.newLineHandling = settings.NewLineHandling;
			this.omitXmlDeclaration = settings.OmitXmlDeclaration;
			this.newLineChars = settings.NewLineChars;
			this.checkCharacters = settings.CheckCharacters;
			this.closeOutput = settings.CloseOutput;
			this.standalone = settings.Standalone;
			this.outputMethod = settings.OutputMethod;
			this.mergeCDataSections = settings.MergeCDataSections;
			if (this.checkCharacters && this.newLineHandling == NewLineHandling.Replace)
			{
				this.ValidateContentChars(this.newLineChars, "NewLineChars", false);
			}
		}

		public XmlEncodedRawTextWriter(TextWriter writer, XmlWriterSettings settings)
			: this(settings)
		{
			this.writer = writer;
			this.encoding = writer.Encoding;
			if (settings.Async)
			{
				this.bufLen = 65536;
			}
			this.bufChars = new char[this.bufLen + 32];
			if (settings.AutoXmlDeclaration)
			{
				this.WriteXmlDeclaration(this.standalone);
				this.autoXmlDeclaration = true;
			}
		}

		public XmlEncodedRawTextWriter(Stream stream, XmlWriterSettings settings)
			: this(settings)
		{
			this.stream = stream;
			this.encoding = settings.Encoding;
			if (settings.Async)
			{
				this.bufLen = 65536;
			}
			this.bufChars = new char[this.bufLen + 32];
			this.bufBytes = new byte[this.bufChars.Length];
			this.bufBytesUsed = 0;
			this.trackTextContent = true;
			this.inTextContent = false;
			this.lastMarkPos = 0;
			this.textContentMarks = new int[64];
			this.textContentMarks[0] = 1;
			this.charEntityFallback = new CharEntityEncoderFallback();
			this.encoding = (Encoding)settings.Encoding.Clone();
			this.encoding.EncoderFallback = this.charEntityFallback;
			this.encoder = this.encoding.GetEncoder();
			if (!stream.CanSeek || stream.Position == 0L)
			{
				byte[] preamble = this.encoding.GetPreamble();
				if (preamble.Length != 0)
				{
					this.stream.Write(preamble, 0, preamble.Length);
				}
			}
			if (settings.AutoXmlDeclaration)
			{
				this.WriteXmlDeclaration(this.standalone);
				this.autoXmlDeclaration = true;
			}
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return new XmlWriterSettings
				{
					Encoding = this.encoding,
					OmitXmlDeclaration = this.omitXmlDeclaration,
					NewLineHandling = this.newLineHandling,
					NewLineChars = this.newLineChars,
					CloseOutput = this.closeOutput,
					ConformanceLevel = ConformanceLevel.Auto,
					CheckCharacters = this.checkCharacters,
					AutoXmlDeclaration = this.autoXmlDeclaration,
					Standalone = this.standalone,
					OutputMethod = this.outputMethod,
					ReadOnly = true
				};
			}
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
			if (!this.omitXmlDeclaration && !this.autoXmlDeclaration)
			{
				if (this.trackTextContent && this.inTextContent)
				{
					this.ChangeTextContentMark(false);
				}
				this.RawText("<?xml version=\"");
				this.RawText("1.0");
				if (this.encoding != null)
				{
					this.RawText("\" encoding=\"");
					this.RawText(this.encoding.WebName);
				}
				if (standalone != XmlStandalone.Omit)
				{
					this.RawText("\" standalone=\"");
					this.RawText((standalone == XmlStandalone.Yes) ? "yes" : "no");
				}
				this.RawText("\"?>");
			}
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
			if (!this.omitXmlDeclaration && !this.autoXmlDeclaration)
			{
				this.WriteProcessingInstruction("xml", xmldecl);
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			this.RawText("<!DOCTYPE ");
			this.RawText(name);
			int num;
			if (pubid != null)
			{
				this.RawText(" PUBLIC \"");
				this.RawText(pubid);
				this.RawText("\" \"");
				if (sysid != null)
				{
					this.RawText(sysid);
				}
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 34;
			}
			else if (sysid != null)
			{
				this.RawText(" SYSTEM \"");
				this.RawText(sysid);
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 34;
			}
			else
			{
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 32;
			}
			if (subset != null)
			{
				char[] array4 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array4[num] = 91;
				this.RawText(subset);
				char[] array5 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array5[num] = 93;
			}
			char[] array6 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array6[num] = 62;
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			if (prefix != null && prefix.Length != 0)
			{
				this.RawText(prefix);
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 58;
			}
			this.RawText(localName);
			this.attrEndPos = this.bufPos;
		}

		internal override void StartElementContent()
		{
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 62;
			this.contentPos = this.bufPos;
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num;
			if (this.contentPos != this.bufPos)
			{
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 60;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 47;
				if (prefix != null && prefix.Length != 0)
				{
					this.RawText(prefix);
					char[] array3 = this.bufChars;
					num = this.bufPos;
					this.bufPos = num + 1;
					array3[num] = 58;
				}
				this.RawText(localName);
				char[] array4 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array4[num] = 62;
				return;
			}
			this.bufPos--;
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 32;
			char[] array6 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array6[num] = 47;
			char[] array7 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array7[num] = 62;
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 47;
			if (prefix != null && prefix.Length != 0)
			{
				this.RawText(prefix);
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 58;
			}
			this.RawText(localName);
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 62;
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num;
			if (this.attrEndPos == this.bufPos)
			{
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 32;
			}
			if (prefix != null && prefix.Length > 0)
			{
				this.RawText(prefix);
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 58;
			}
			this.RawText(localName);
			char[] array3 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array3[num] = 61;
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 34;
			this.inAttributeValue = true;
		}

		public override void WriteEndAttribute()
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 34;
			this.inAttributeValue = false;
			this.attrEndPos = this.bufPos;
		}

		internal override void WriteNamespaceDeclaration(string prefix, string namespaceName)
		{
			this.WriteStartNamespaceDeclaration(prefix);
			this.WriteString(namespaceName);
			this.WriteEndNamespaceDeclaration();
		}

		internal override bool SupportsNamespaceDeclarationInChunks
		{
			get
			{
				return true;
			}
		}

		internal override void WriteStartNamespaceDeclaration(string prefix)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			if (prefix.Length == 0)
			{
				this.RawText(" xmlns=\"");
			}
			else
			{
				this.RawText(" xmlns:");
				this.RawText(prefix);
				char[] array = this.bufChars;
				int num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 61;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 34;
			}
			this.inAttributeValue = true;
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
		}

		internal override void WriteEndNamespaceDeclaration()
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			this.inAttributeValue = false;
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 34;
			this.attrEndPos = this.bufPos;
		}

		public override void WriteCData(string text)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num;
			if (this.mergeCDataSections && this.bufPos == this.cdataPos)
			{
				this.bufPos -= 3;
			}
			else
			{
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 60;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 33;
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 91;
				char[] array4 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array4[num] = 67;
				char[] array5 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array5[num] = 68;
				char[] array6 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array6[num] = 65;
				char[] array7 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array7[num] = 84;
				char[] array8 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array8[num] = 65;
				char[] array9 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array9[num] = 91;
			}
			this.WriteCDataSection(text);
			char[] array10 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array10[num] = 93;
			char[] array11 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array11[num] = 93;
			char[] array12 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array12[num] = 62;
			this.textPos = this.bufPos;
			this.cdataPos = this.bufPos;
		}

		public override void WriteComment(string text)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 33;
			char[] array3 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array3[num] = 45;
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 45;
			this.WriteCommentOrPi(text, 45);
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 45;
			char[] array6 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array6[num] = 45;
			char[] array7 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array7[num] = 62;
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 63;
			this.RawText(name);
			if (text.Length > 0)
			{
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 32;
				this.WriteCommentOrPi(text, 63);
			}
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 63;
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 62;
		}

		public override void WriteEntityRef(string name)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 38;
			this.RawText(name);
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 59;
			if (this.bufPos > this.bufLen)
			{
				this.FlushBuffer();
			}
			this.textPos = this.bufPos;
		}

		public override void WriteCharEntity(char ch)
		{
			int num = (int)ch;
			string text = num.ToString("X", NumberFormatInfo.InvariantInfo);
			if (this.checkCharacters && !this.xmlCharType.IsCharData(ch))
			{
				throw XmlConvert.CreateInvalidCharException(ch, '\0');
			}
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 38;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 35;
			char[] array3 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array3[num] = 120;
			this.RawText(text);
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 59;
			if (this.bufPos > this.bufLen)
			{
				this.FlushBuffer();
			}
			this.textPos = this.bufPos;
		}

		public unsafe override void WriteWhitespace(string ws)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			fixed (string text = ws)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + ws.Length;
				if (this.inAttributeValue)
				{
					this.WriteAttributeTextBlock(ptr, ptr2);
				}
				else
				{
					this.WriteElementTextBlock(ptr, ptr2);
				}
			}
		}

		public unsafe override void WriteString(string text)
		{
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
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
					this.WriteAttributeTextBlock(ptr, ptr2);
				}
				else
				{
					this.WriteElementTextBlock(ptr, ptr2);
				}
			}
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num = XmlCharType.CombineSurrogateChar((int)lowChar, (int)highChar);
			char[] array = this.bufChars;
			int num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array[num2] = 38;
			char[] array2 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array2[num2] = 35;
			char[] array3 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array3[num2] = 120;
			this.RawText(num.ToString("X", NumberFormatInfo.InvariantInfo));
			char[] array4 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array4[num2] = 59;
			this.textPos = this.bufPos;
		}

		public unsafe override void WriteChars(char[] buffer, int index, int count)
		{
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
			fixed (char* ptr = &buffer[index])
			{
				char* ptr2 = ptr;
				if (this.inAttributeValue)
				{
					this.WriteAttributeTextBlock(ptr2, ptr2 + count);
				}
				else
				{
					this.WriteElementTextBlock(ptr2, ptr2 + count);
				}
			}
		}

		public unsafe override void WriteRaw(char[] buffer, int index, int count)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			fixed (char* ptr = &buffer[index])
			{
				char* ptr2 = ptr;
				this.WriteRawWithCharChecking(ptr2, ptr2 + count);
			}
			this.textPos = this.bufPos;
		}

		public unsafe override void WriteRaw(string data)
		{
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			fixed (string text = data)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.WriteRawWithCharChecking(ptr, ptr + data.Length);
			}
			this.textPos = this.bufPos;
		}

		public override void Close()
		{
			try
			{
				this.FlushBuffer();
				this.FlushEncoder();
			}
			finally
			{
				this.writeToNull = true;
				if (this.stream != null)
				{
					try
					{
						this.stream.Flush();
						goto IL_007D;
					}
					finally
					{
						try
						{
							if (this.closeOutput)
							{
								this.stream.Close();
							}
						}
						finally
						{
							this.stream = null;
						}
					}
				}
				if (this.writer != null)
				{
					try
					{
						this.writer.Flush();
					}
					finally
					{
						try
						{
							if (this.closeOutput)
							{
								this.writer.Close();
							}
						}
						finally
						{
							this.writer = null;
						}
					}
				}
				IL_007D:;
			}
		}

		public override void Flush()
		{
			this.FlushBuffer();
			this.FlushEncoder();
			if (this.stream != null)
			{
				this.stream.Flush();
				return;
			}
			if (this.writer != null)
			{
				this.writer.Flush();
			}
		}

		protected virtual void FlushBuffer()
		{
			try
			{
				if (!this.writeToNull)
				{
					if (this.stream != null)
					{
						if (this.trackTextContent)
						{
							this.charEntityFallback.Reset(this.textContentMarks, this.lastMarkPos);
							if ((this.lastMarkPos & 1) != 0)
							{
								this.textContentMarks[1] = 1;
								this.lastMarkPos = 1;
							}
							else
							{
								this.lastMarkPos = 0;
							}
						}
						this.EncodeChars(1, this.bufPos, true);
					}
					else
					{
						this.writer.Write(this.bufChars, 1, this.bufPos - 1);
					}
				}
			}
			catch
			{
				this.writeToNull = true;
				throw;
			}
			finally
			{
				this.bufChars[0] = this.bufChars[this.bufPos - 1];
				this.textPos = ((this.textPos == this.bufPos) ? 1 : 0);
				this.attrEndPos = ((this.attrEndPos == this.bufPos) ? 1 : 0);
				this.contentPos = 0;
				this.cdataPos = 0;
				this.bufPos = 1;
			}
		}

		private void EncodeChars(int startOffset, int endOffset, bool writeAllToStream)
		{
			while (startOffset < endOffset)
			{
				if (this.charEntityFallback != null)
				{
					this.charEntityFallback.StartOffset = startOffset;
				}
				int num;
				int num2;
				bool flag;
				this.encoder.Convert(this.bufChars, startOffset, endOffset - startOffset, this.bufBytes, this.bufBytesUsed, this.bufBytes.Length - this.bufBytesUsed, false, out num, out num2, out flag);
				startOffset += num;
				this.bufBytesUsed += num2;
				if (this.bufBytesUsed >= this.bufBytes.Length - 16)
				{
					this.stream.Write(this.bufBytes, 0, this.bufBytesUsed);
					this.bufBytesUsed = 0;
				}
			}
			if (writeAllToStream && this.bufBytesUsed > 0)
			{
				this.stream.Write(this.bufBytes, 0, this.bufBytesUsed);
				this.bufBytesUsed = 0;
			}
		}

		private void FlushEncoder()
		{
			if (this.stream != null)
			{
				int num;
				int num2;
				bool flag;
				this.encoder.Convert(this.bufChars, 1, 0, this.bufBytes, 0, this.bufBytes.Length, true, out num, out num2, out flag);
				if (num2 != 0)
				{
					this.stream.Write(this.bufBytes, 0, num2);
				}
			}
		}

		protected unsafe void WriteAttributeTextBlock(char* pSrc, char* pSrcEnd)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = ptr + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr3 = ptr2 + (long)(pSrcEnd - pSrc) * 2L / 2L;
				if (ptr3 != ptr + this.bufLen)
				{
					ptr3 = ptr + this.bufLen;
				}
				while (ptr2 < ptr3 && (this.xmlCharType.charProperties[num = (int)(*pSrc)] & 128) != 0)
				{
					*ptr2 = (char)num;
					ptr2++;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					break;
				}
				if (ptr2 >= ptr3)
				{
					this.bufPos = (int)((long)(ptr2 - ptr));
					this.FlushBuffer();
					ptr2 = ptr + 1;
				}
				else
				{
					if (num <= 38)
					{
						switch (num)
						{
						case 9:
							if (this.newLineHandling == NewLineHandling.None)
							{
								*ptr2 = (char)num;
								ptr2++;
								goto IL_01D3;
							}
							ptr2 = XmlEncodedRawTextWriter.TabEntity(ptr2);
							goto IL_01D3;
						case 10:
							if (this.newLineHandling == NewLineHandling.None)
							{
								*ptr2 = (char)num;
								ptr2++;
								goto IL_01D3;
							}
							ptr2 = XmlEncodedRawTextWriter.LineFeedEntity(ptr2);
							goto IL_01D3;
						case 11:
						case 12:
							break;
						case 13:
							if (this.newLineHandling == NewLineHandling.None)
							{
								*ptr2 = (char)num;
								ptr2++;
								goto IL_01D3;
							}
							ptr2 = XmlEncodedRawTextWriter.CarriageReturnEntity(ptr2);
							goto IL_01D3;
						default:
							if (num == 34)
							{
								ptr2 = XmlEncodedRawTextWriter.QuoteEntity(ptr2);
								goto IL_01D3;
							}
							if (num == 38)
							{
								ptr2 = XmlEncodedRawTextWriter.AmpEntity(ptr2);
								goto IL_01D3;
							}
							break;
						}
					}
					else
					{
						if (num == 39)
						{
							*ptr2 = (char)num;
							ptr2++;
							goto IL_01D3;
						}
						if (num == 60)
						{
							ptr2 = XmlEncodedRawTextWriter.LtEntity(ptr2);
							goto IL_01D3;
						}
						if (num == 62)
						{
							ptr2 = XmlEncodedRawTextWriter.GtEntity(ptr2);
							goto IL_01D3;
						}
					}
					if (XmlCharType.IsSurrogate(num))
					{
						ptr2 = XmlEncodedRawTextWriter.EncodeSurrogate(pSrc, pSrcEnd, ptr2);
						pSrc += 2;
						continue;
					}
					if (num <= 127 || num >= 65534)
					{
						ptr2 = this.InvalidXmlChar(num, ptr2, true);
						pSrc++;
						continue;
					}
					*ptr2 = (char)num;
					ptr2++;
					pSrc++;
					continue;
					IL_01D3:
					pSrc++;
				}
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			array = null;
		}

		protected unsafe void WriteElementTextBlock(char* pSrc, char* pSrcEnd)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = ptr + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr3 = ptr2 + (long)(pSrcEnd - pSrc) * 2L / 2L;
				if (ptr3 != ptr + this.bufLen)
				{
					ptr3 = ptr + this.bufLen;
				}
				while (ptr2 < ptr3 && (this.xmlCharType.charProperties[num = (int)(*pSrc)] & 128) != 0)
				{
					*ptr2 = (char)num;
					ptr2++;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					break;
				}
				if (ptr2 < ptr3)
				{
					if (num <= 38)
					{
						switch (num)
						{
						case 9:
							goto IL_010F;
						case 10:
							if (this.newLineHandling == NewLineHandling.Replace)
							{
								ptr2 = this.WriteNewLine(ptr2);
								goto IL_01D6;
							}
							*ptr2 = (char)num;
							ptr2++;
							goto IL_01D6;
						case 11:
						case 12:
							break;
						case 13:
							switch (this.newLineHandling)
							{
							case NewLineHandling.Replace:
								if (pSrc[1] == '\n')
								{
									pSrc++;
								}
								ptr2 = this.WriteNewLine(ptr2);
								goto IL_01D6;
							case NewLineHandling.Entitize:
								ptr2 = XmlEncodedRawTextWriter.CarriageReturnEntity(ptr2);
								goto IL_01D6;
							case NewLineHandling.None:
								*ptr2 = (char)num;
								ptr2++;
								goto IL_01D6;
							default:
								goto IL_01D6;
							}
							break;
						default:
							if (num == 34)
							{
								goto IL_010F;
							}
							if (num == 38)
							{
								ptr2 = XmlEncodedRawTextWriter.AmpEntity(ptr2);
								goto IL_01D6;
							}
							break;
						}
					}
					else
					{
						if (num == 39)
						{
							goto IL_010F;
						}
						if (num == 60)
						{
							ptr2 = XmlEncodedRawTextWriter.LtEntity(ptr2);
							goto IL_01D6;
						}
						if (num == 62)
						{
							ptr2 = XmlEncodedRawTextWriter.GtEntity(ptr2);
							goto IL_01D6;
						}
					}
					if (XmlCharType.IsSurrogate(num))
					{
						ptr2 = XmlEncodedRawTextWriter.EncodeSurrogate(pSrc, pSrcEnd, ptr2);
						pSrc += 2;
						continue;
					}
					if (num <= 127 || num >= 65534)
					{
						ptr2 = this.InvalidXmlChar(num, ptr2, true);
						pSrc++;
						continue;
					}
					*ptr2 = (char)num;
					ptr2++;
					pSrc++;
					continue;
					IL_01D6:
					pSrc++;
					continue;
					IL_010F:
					*ptr2 = (char)num;
					ptr2++;
					goto IL_01D6;
				}
				this.bufPos = (int)((long)(ptr2 - ptr));
				this.FlushBuffer();
				ptr2 = ptr + 1;
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			this.textPos = this.bufPos;
			this.contentPos = 0;
			array = null;
		}

		protected unsafe void RawText(string s)
		{
			fixed (string text = s)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.RawText(ptr, ptr + s.Length);
			}
		}

		protected unsafe void RawText(char* pSrcBegin, char* pSrcEnd)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = ptr + this.bufPos;
			char* ptr3 = pSrcBegin;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr2 + (long)(pSrcEnd - ptr3) * 2L / 2L;
				if (ptr4 != ptr + this.bufLen)
				{
					ptr4 = ptr + this.bufLen;
				}
				while (ptr2 < ptr4 && (num = (int)(*ptr3)) < 55296)
				{
					ptr3++;
					*ptr2 = (char)num;
					ptr2++;
				}
				if (ptr3 >= pSrcEnd)
				{
					break;
				}
				if (ptr2 >= ptr4)
				{
					this.bufPos = (int)((long)(ptr2 - ptr));
					this.FlushBuffer();
					ptr2 = ptr + 1;
				}
				else if (XmlCharType.IsSurrogate(num))
				{
					ptr2 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr3, pSrcEnd, ptr2);
					ptr3 += 2;
				}
				else if (num <= 127 || num >= 65534)
				{
					ptr2 = this.InvalidXmlChar(num, ptr2, false);
					ptr3++;
				}
				else
				{
					*ptr2 = (char)num;
					ptr2++;
					ptr3++;
				}
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			array = null;
		}

		protected unsafe void WriteRawWithCharChecking(char* pSrcBegin, char* pSrcEnd)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = pSrcBegin;
			char* ptr3 = ptr + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr3 + (long)(pSrcEnd - ptr2) * 2L / 2L;
				if (ptr4 != ptr + this.bufLen)
				{
					ptr4 = ptr + this.bufLen;
				}
				while (ptr3 < ptr4 && (this.xmlCharType.charProperties[num = (int)(*ptr2)] & 64) != 0)
				{
					*ptr3 = (char)num;
					ptr3++;
					ptr2++;
				}
				if (ptr2 >= pSrcEnd)
				{
					break;
				}
				if (ptr3 < ptr4)
				{
					if (num <= 38)
					{
						switch (num)
						{
						case 9:
							goto IL_00DF;
						case 10:
							if (this.newLineHandling == NewLineHandling.Replace)
							{
								ptr3 = this.WriteNewLine(ptr3);
								goto IL_0186;
							}
							*ptr3 = (char)num;
							ptr3++;
							goto IL_0186;
						case 11:
						case 12:
							break;
						case 13:
							if (this.newLineHandling == NewLineHandling.Replace)
							{
								if (ptr2[1] == '\n')
								{
									ptr2++;
								}
								ptr3 = this.WriteNewLine(ptr3);
								goto IL_0186;
							}
							*ptr3 = (char)num;
							ptr3++;
							goto IL_0186;
						default:
							if (num == 38)
							{
								goto IL_00DF;
							}
							break;
						}
					}
					else if (num == 60 || num == 93)
					{
						goto IL_00DF;
					}
					if (XmlCharType.IsSurrogate(num))
					{
						ptr3 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr2, pSrcEnd, ptr3);
						ptr2 += 2;
						continue;
					}
					if (num <= 127 || num >= 65534)
					{
						ptr3 = this.InvalidXmlChar(num, ptr3, false);
						ptr2++;
						continue;
					}
					*ptr3 = (char)num;
					ptr3++;
					ptr2++;
					continue;
					IL_0186:
					ptr2++;
					continue;
					IL_00DF:
					*ptr3 = (char)num;
					ptr3++;
					goto IL_0186;
				}
				this.bufPos = (int)((long)(ptr3 - ptr));
				this.FlushBuffer();
				ptr3 = ptr + 1;
			}
			this.bufPos = (int)((long)(ptr3 - ptr));
			array = null;
		}

		protected unsafe void WriteCommentOrPi(string text, int stopChar)
		{
			if (text.Length == 0)
			{
				if (this.bufPos >= this.bufLen)
				{
					this.FlushBuffer();
				}
				return;
			}
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char[] array;
				char* ptr2;
				if ((array = this.bufChars) == null || array.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array[0];
				}
				char* ptr3 = ptr;
				char* ptr4 = ptr + text.Length;
				char* ptr5 = ptr2 + this.bufPos;
				int num = 0;
				for (;;)
				{
					char* ptr6 = ptr5 + (long)(ptr4 - ptr3) * 2L / 2L;
					if (ptr6 != ptr2 + this.bufLen)
					{
						ptr6 = ptr2 + this.bufLen;
					}
					while (ptr5 < ptr6 && (this.xmlCharType.charProperties[num = (int)(*ptr3)] & 64) != 0 && num != stopChar)
					{
						*ptr5 = (char)num;
						ptr5++;
						ptr3++;
					}
					if (ptr3 >= ptr4)
					{
						break;
					}
					if (ptr5 < ptr6)
					{
						if (num <= 45)
						{
							switch (num)
							{
							case 9:
								goto IL_0226;
							case 10:
								if (this.newLineHandling == NewLineHandling.Replace)
								{
									ptr5 = this.WriteNewLine(ptr5);
									goto IL_0296;
								}
								*ptr5 = (char)num;
								ptr5++;
								goto IL_0296;
							case 11:
							case 12:
								break;
							case 13:
								if (this.newLineHandling == NewLineHandling.Replace)
								{
									if (ptr3[1] == '\n')
									{
										ptr3++;
									}
									ptr5 = this.WriteNewLine(ptr5);
									goto IL_0296;
								}
								*ptr5 = (char)num;
								ptr5++;
								goto IL_0296;
							default:
								if (num == 38)
								{
									goto IL_0226;
								}
								if (num == 45)
								{
									*ptr5 = '-';
									ptr5++;
									if (num == stopChar && (ptr3 + 1 == ptr4 || ptr3[1] == '-'))
									{
										*ptr5 = ' ';
										ptr5++;
										goto IL_0296;
									}
									goto IL_0296;
								}
								break;
							}
						}
						else
						{
							if (num == 60)
							{
								goto IL_0226;
							}
							if (num != 63)
							{
								if (num == 93)
								{
									*ptr5 = ']';
									ptr5++;
									goto IL_0296;
								}
							}
							else
							{
								*ptr5 = '?';
								ptr5++;
								if (num == stopChar && ptr3 + 1 < ptr4 && ptr3[1] == '>')
								{
									*ptr5 = ' ';
									ptr5++;
									goto IL_0296;
								}
								goto IL_0296;
							}
						}
						if (XmlCharType.IsSurrogate(num))
						{
							ptr5 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr3, ptr4, ptr5);
							ptr3 += 2;
							continue;
						}
						if (num <= 127 || num >= 65534)
						{
							ptr5 = this.InvalidXmlChar(num, ptr5, false);
							ptr3++;
							continue;
						}
						*ptr5 = (char)num;
						ptr5++;
						ptr3++;
						continue;
						IL_0296:
						ptr3++;
						continue;
						IL_0226:
						*ptr5 = (char)num;
						ptr5++;
						goto IL_0296;
					}
					this.bufPos = (int)((long)(ptr5 - ptr2));
					this.FlushBuffer();
					ptr5 = ptr2 + 1;
				}
				this.bufPos = (int)((long)(ptr5 - ptr2));
				array = null;
			}
		}

		protected unsafe void WriteCDataSection(string text)
		{
			if (text.Length == 0)
			{
				if (this.bufPos >= this.bufLen)
				{
					this.FlushBuffer();
				}
				return;
			}
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char[] array;
				char* ptr2;
				if ((array = this.bufChars) == null || array.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array[0];
				}
				char* ptr3 = ptr;
				char* ptr4 = ptr + text.Length;
				char* ptr5 = ptr2 + this.bufPos;
				int num = 0;
				for (;;)
				{
					char* ptr6 = ptr5 + (long)(ptr4 - ptr3) * 2L / 2L;
					if (ptr6 != ptr2 + this.bufLen)
					{
						ptr6 = ptr2 + this.bufLen;
					}
					while (ptr5 < ptr6 && (this.xmlCharType.charProperties[num = (int)(*ptr3)] & 128) != 0 && num != 93)
					{
						*ptr5 = (char)num;
						ptr5++;
						ptr3++;
					}
					if (ptr3 >= ptr4)
					{
						break;
					}
					if (ptr5 < ptr6)
					{
						if (num <= 39)
						{
							switch (num)
							{
							case 9:
								goto IL_0210;
							case 10:
								if (this.newLineHandling == NewLineHandling.Replace)
								{
									ptr5 = this.WriteNewLine(ptr5);
									goto IL_0280;
								}
								*ptr5 = (char)num;
								ptr5++;
								goto IL_0280;
							case 11:
							case 12:
								break;
							case 13:
								if (this.newLineHandling == NewLineHandling.Replace)
								{
									if (ptr3[1] == '\n')
									{
										ptr3++;
									}
									ptr5 = this.WriteNewLine(ptr5);
									goto IL_0280;
								}
								*ptr5 = (char)num;
								ptr5++;
								goto IL_0280;
							default:
								if (num == 34 || num - 38 <= 1)
								{
									goto IL_0210;
								}
								break;
							}
						}
						else
						{
							if (num == 60)
							{
								goto IL_0210;
							}
							if (num == 62)
							{
								if (this.hadDoubleBracket && ptr5[-1] == ']')
								{
									ptr5 = XmlEncodedRawTextWriter.RawEndCData(ptr5);
									ptr5 = XmlEncodedRawTextWriter.RawStartCData(ptr5);
								}
								*ptr5 = '>';
								ptr5++;
								goto IL_0280;
							}
							if (num == 93)
							{
								if (ptr5[-1] == ']')
								{
									this.hadDoubleBracket = true;
								}
								else
								{
									this.hadDoubleBracket = false;
								}
								*ptr5 = ']';
								ptr5++;
								goto IL_0280;
							}
						}
						if (XmlCharType.IsSurrogate(num))
						{
							ptr5 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr3, ptr4, ptr5);
							ptr3 += 2;
							continue;
						}
						if (num <= 127 || num >= 65534)
						{
							ptr5 = this.InvalidXmlChar(num, ptr5, false);
							ptr3++;
							continue;
						}
						*ptr5 = (char)num;
						ptr5++;
						ptr3++;
						continue;
						IL_0280:
						ptr3++;
						continue;
						IL_0210:
						*ptr5 = (char)num;
						ptr5++;
						goto IL_0280;
					}
					this.bufPos = (int)((long)(ptr5 - ptr2));
					this.FlushBuffer();
					ptr5 = ptr2 + 1;
				}
				this.bufPos = (int)((long)(ptr5 - ptr2));
				array = null;
			}
		}

		private unsafe static char* EncodeSurrogate(char* pSrc, char* pSrcEnd, char* pDst)
		{
			int num = (int)(*pSrc);
			if (num > 56319)
			{
				throw XmlConvert.CreateInvalidHighSurrogateCharException((char)num);
			}
			if (pSrc + 1 >= pSrcEnd)
			{
				throw new ArgumentException(Res.GetString("The surrogate pair is invalid. Missing a low surrogate character."));
			}
			int num2 = (int)pSrc[1];
			if (num2 >= 56320 && (LocalAppContextSwitches.DontThrowOnInvalidSurrogatePairs || num2 <= 57343))
			{
				*pDst = (char)num;
				pDst[1] = (char)num2;
				pDst += 2;
				return pDst;
			}
			throw XmlConvert.CreateInvalidSurrogatePairException((char)num2, (char)num);
		}

		private unsafe char* InvalidXmlChar(int ch, char* pDst, bool entitize)
		{
			if (this.checkCharacters)
			{
				throw XmlConvert.CreateInvalidCharException((char)ch, '\0');
			}
			if (entitize)
			{
				return XmlEncodedRawTextWriter.CharEntity(pDst, (char)ch);
			}
			*pDst = (char)ch;
			pDst++;
			return pDst;
		}

		internal unsafe void EncodeChar(ref char* pSrc, char* pSrcEnd, ref char* pDst)
		{
			int num = (int)(*pSrc);
			if (XmlCharType.IsSurrogate(num))
			{
				pDst = XmlEncodedRawTextWriter.EncodeSurrogate(pSrc, pSrcEnd, pDst);
				pSrc += (IntPtr)2 * 2;
				return;
			}
			if (num <= 127 || num >= 65534)
			{
				pDst = this.InvalidXmlChar(num, pDst, false);
				pSrc += 2;
				return;
			}
			*pDst = (short)((ushort)num);
			pDst += 2;
			pSrc += 2;
		}

		protected void ChangeTextContentMark(bool value)
		{
			this.inTextContent = value;
			if (this.lastMarkPos + 1 == this.textContentMarks.Length)
			{
				this.GrowTextContentMarks();
			}
			int[] array = this.textContentMarks;
			int num = this.lastMarkPos + 1;
			this.lastMarkPos = num;
			array[num] = this.bufPos;
		}

		private void GrowTextContentMarks()
		{
			int[] array = new int[this.textContentMarks.Length * 2];
			Array.Copy(this.textContentMarks, array, this.textContentMarks.Length);
			this.textContentMarks = array;
		}

		protected unsafe char* WriteNewLine(char* pDst)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			this.bufPos = (int)((long)(pDst - ptr));
			this.RawText(this.newLineChars);
			return ptr + this.bufPos;
		}

		protected unsafe static char* LtEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = 'l';
			pDst[2] = 't';
			pDst[3] = ';';
			return pDst + 4;
		}

		protected unsafe static char* GtEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = 'g';
			pDst[2] = 't';
			pDst[3] = ';';
			return pDst + 4;
		}

		protected unsafe static char* AmpEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = 'a';
			pDst[2] = 'm';
			pDst[3] = 'p';
			pDst[4] = ';';
			return pDst + 5;
		}

		protected unsafe static char* QuoteEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = 'q';
			pDst[2] = 'u';
			pDst[3] = 'o';
			pDst[4] = 't';
			pDst[5] = ';';
			return pDst + 6;
		}

		protected unsafe static char* TabEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = '#';
			pDst[2] = 'x';
			pDst[3] = '9';
			pDst[4] = ';';
			return pDst + 5;
		}

		protected unsafe static char* LineFeedEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = '#';
			pDst[2] = 'x';
			pDst[3] = 'A';
			pDst[4] = ';';
			return pDst + 5;
		}

		protected unsafe static char* CarriageReturnEntity(char* pDst)
		{
			*pDst = '&';
			pDst[1] = '#';
			pDst[2] = 'x';
			pDst[3] = 'D';
			pDst[4] = ';';
			return pDst + 5;
		}

		private unsafe static char* CharEntity(char* pDst, char ch)
		{
			int num = (int)ch;
			string text = num.ToString("X", NumberFormatInfo.InvariantInfo);
			*pDst = '&';
			pDst[1] = '#';
			pDst[2] = 'x';
			pDst += 3;
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				while ((*(pDst++) = *(ptr2++)) != '\0')
				{
				}
			}
			pDst[-1] = ';';
			return pDst;
		}

		protected unsafe static char* RawStartCData(char* pDst)
		{
			*pDst = '<';
			pDst[1] = '!';
			pDst[2] = '[';
			pDst[3] = 'C';
			pDst[4] = 'D';
			pDst[5] = 'A';
			pDst[6] = 'T';
			pDst[7] = 'A';
			pDst[8] = '[';
			return pDst + 9;
		}

		protected unsafe static char* RawEndCData(char* pDst)
		{
			*pDst = ']';
			pDst[1] = ']';
			pDst[2] = '>';
			return pDst + 3;
		}

		protected void ValidateContentChars(string chars, string propertyName, bool allowOnlyWhitespace)
		{
			if (!allowOnlyWhitespace)
			{
				for (int i = 0; i < chars.Length; i++)
				{
					if (!this.xmlCharType.IsTextChar(chars[i]))
					{
						char c = chars[i];
						if (c <= '&')
						{
							switch (c)
							{
							case '\t':
							case '\n':
							case '\r':
								goto IL_0119;
							case '\v':
							case '\f':
								goto IL_00A0;
							default:
								if (c != '&')
								{
									goto IL_00A0;
								}
								break;
							}
						}
						else if (c != '<' && c != ']')
						{
							goto IL_00A0;
						}
						string text = Res.GetString("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(chars, i));
						goto IL_012A;
						IL_00A0:
						if (XmlCharType.IsHighSurrogate((int)chars[i]))
						{
							if (i + 1 < chars.Length && XmlCharType.IsLowSurrogate((int)chars[i + 1]))
							{
								i++;
								goto IL_0119;
							}
							text = Res.GetString("The surrogate pair is invalid. Missing a low surrogate character.");
						}
						else
						{
							if (!XmlCharType.IsLowSurrogate((int)chars[i]))
							{
								goto IL_0119;
							}
							text = Res.GetString("Invalid high surrogate character (0x{0}). A high surrogate character must have a value from range (0xD800 - 0xDBFF).", new object[] { ((uint)chars[i]).ToString("X", CultureInfo.InvariantCulture) });
						}
						IL_012A:
						throw new ArgumentException(Res.GetString("XmlWriterSettings.{0} can contain only valid XML text content characters when XmlWriterSettings.CheckCharacters is true. {1}", new string[] { propertyName, text }));
					}
					IL_0119:;
				}
				return;
			}
			if (!this.xmlCharType.IsOnlyWhitespace(chars))
			{
				throw new ArgumentException(Res.GetString("XmlWriterSettings.{0} can contain only valid XML white space characters when XmlWriterSettings.CheckCharacters and XmlWriterSettings.NewLineOnAttributes are true.", new object[] { propertyName }));
			}
		}

		protected void CheckAsyncCall()
		{
			if (!this.useAsync)
			{
				throw new InvalidOperationException(Res.GetString("Set XmlWriterSettings.Async to true if you want to use Async Methods."));
			}
		}

		internal override async Task WriteXmlDeclarationAsync(XmlStandalone standalone)
		{
			this.CheckAsyncCall();
			if (!this.omitXmlDeclaration && !this.autoXmlDeclaration)
			{
				if (this.trackTextContent && this.inTextContent)
				{
					this.ChangeTextContentMark(false);
				}
				await this.RawTextAsync("<?xml version=\"").ConfigureAwait(false);
				await this.RawTextAsync("1.0").ConfigureAwait(false);
				if (this.encoding != null)
				{
					await this.RawTextAsync("\" encoding=\"").ConfigureAwait(false);
					await this.RawTextAsync(this.encoding.WebName).ConfigureAwait(false);
				}
				if (standalone != XmlStandalone.Omit)
				{
					await this.RawTextAsync("\" standalone=\"").ConfigureAwait(false);
					await this.RawTextAsync((standalone == XmlStandalone.Yes) ? "yes" : "no").ConfigureAwait(false);
				}
				await this.RawTextAsync("\"?>").ConfigureAwait(false);
			}
		}

		internal override Task WriteXmlDeclarationAsync(string xmldecl)
		{
			this.CheckAsyncCall();
			if (!this.omitXmlDeclaration && !this.autoXmlDeclaration)
			{
				return this.WriteProcessingInstructionAsync("xml", xmldecl);
			}
			return AsyncHelper.DoneTask;
		}

		public override async Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			await this.RawTextAsync("<!DOCTYPE ").ConfigureAwait(false);
			await this.RawTextAsync(name).ConfigureAwait(false);
			int num;
			if (pubid != null)
			{
				await this.RawTextAsync(" PUBLIC \"").ConfigureAwait(false);
				await this.RawTextAsync(pubid).ConfigureAwait(false);
				await this.RawTextAsync("\" \"").ConfigureAwait(false);
				if (sysid != null)
				{
					await this.RawTextAsync(sysid).ConfigureAwait(false);
				}
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 34;
			}
			else if (sysid != null)
			{
				await this.RawTextAsync(" SYSTEM \"").ConfigureAwait(false);
				await this.RawTextAsync(sysid).ConfigureAwait(false);
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 34;
			}
			else
			{
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 32;
			}
			if (subset != null)
			{
				char[] array4 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array4[num] = 91;
				await this.RawTextAsync(subset).ConfigureAwait(false);
				char[] array5 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array5[num] = 93;
			}
			char[] array6 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array6[num] = 62;
		}

		public override Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			Task task;
			if (prefix != null && prefix.Length != 0)
			{
				task = this.RawTextAsync(prefix + ":" + localName);
			}
			else
			{
				task = this.RawTextAsync(localName);
			}
			return task.CallVoidFuncWhenFinish(new Action(this.WriteStartElementAsync_SetAttEndPos));
		}

		private void WriteStartElementAsync_SetAttEndPos()
		{
			this.attrEndPos = this.bufPos;
		}

		internal override Task WriteEndElementAsync(string prefix, string localName, string ns)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num;
			if (this.contentPos == this.bufPos)
			{
				this.bufPos--;
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 32;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 47;
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 62;
				return AsyncHelper.DoneTask;
			}
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 60;
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 47;
			if (prefix != null && prefix.Length != 0)
			{
				return this.RawTextAsync(prefix + ":" + localName + ">");
			}
			return this.RawTextAsync(localName + ">");
		}

		internal override Task WriteFullEndElementAsync(string prefix, string localName, string ns)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 47;
			if (prefix != null && prefix.Length != 0)
			{
				return this.RawTextAsync(prefix + ":" + localName + ">");
			}
			return this.RawTextAsync(localName + ">");
		}

		protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			if (this.attrEndPos == this.bufPos)
			{
				char[] array = this.bufChars;
				int num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 32;
			}
			Task task;
			if (prefix != null && prefix.Length > 0)
			{
				task = this.RawTextAsync(prefix + ":" + localName);
			}
			else
			{
				task = this.RawTextAsync(localName);
			}
			return task.CallVoidFuncWhenFinish(new Action(this.WriteStartAttribute_SetInAttribute));
		}

		private void WriteStartAttribute_SetInAttribute()
		{
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 61;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 34;
			this.inAttributeValue = true;
		}

		protected internal override Task WriteEndAttributeAsync()
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 34;
			this.inAttributeValue = false;
			this.attrEndPos = this.bufPos;
			return AsyncHelper.DoneTask;
		}

		internal override async Task WriteNamespaceDeclarationAsync(string prefix, string namespaceName)
		{
			this.CheckAsyncCall();
			await this.WriteStartNamespaceDeclarationAsync(prefix).ConfigureAwait(false);
			await this.WriteStringAsync(namespaceName).ConfigureAwait(false);
			await this.WriteEndNamespaceDeclarationAsync().ConfigureAwait(false);
		}

		internal override async Task WriteStartNamespaceDeclarationAsync(string prefix)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			if (prefix.Length == 0)
			{
				await this.RawTextAsync(" xmlns=\"").ConfigureAwait(false);
			}
			else
			{
				await this.RawTextAsync(" xmlns:").ConfigureAwait(false);
				await this.RawTextAsync(prefix).ConfigureAwait(false);
				char[] array = this.bufChars;
				int num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 61;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 34;
			}
			this.inAttributeValue = true;
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
		}

		internal override Task WriteEndNamespaceDeclarationAsync()
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			this.inAttributeValue = false;
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 34;
			this.attrEndPos = this.bufPos;
			return AsyncHelper.DoneTask;
		}

		public override async Task WriteCDataAsync(string text)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num;
			if (this.mergeCDataSections && this.bufPos == this.cdataPos)
			{
				this.bufPos -= 3;
			}
			else
			{
				char[] array = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array[num] = 60;
				char[] array2 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array2[num] = 33;
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 91;
				char[] array4 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array4[num] = 67;
				char[] array5 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array5[num] = 68;
				char[] array6 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array6[num] = 65;
				char[] array7 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array7[num] = 84;
				char[] array8 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array8[num] = 65;
				char[] array9 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array9[num] = 91;
			}
			await this.WriteCDataSectionAsync(text).ConfigureAwait(false);
			char[] array10 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array10[num] = 93;
			char[] array11 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array11[num] = 93;
			char[] array12 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array12[num] = 62;
			this.textPos = this.bufPos;
			this.cdataPos = this.bufPos;
		}

		public override async Task WriteCommentAsync(string text)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 33;
			char[] array3 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array3[num] = 45;
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 45;
			await this.WriteCommentOrPiAsync(text, 45).ConfigureAwait(false);
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 45;
			char[] array6 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array6[num] = 45;
			char[] array7 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array7[num] = 62;
		}

		public override async Task WriteProcessingInstructionAsync(string name, string text)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 60;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 63;
			await this.RawTextAsync(name).ConfigureAwait(false);
			if (text.Length > 0)
			{
				char[] array3 = this.bufChars;
				num = this.bufPos;
				this.bufPos = num + 1;
				array3[num] = 32;
				await this.WriteCommentOrPiAsync(text, 63).ConfigureAwait(false);
			}
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 63;
			char[] array5 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array5[num] = 62;
		}

		public override async Task WriteEntityRefAsync(string name)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			int num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 38;
			await this.RawTextAsync(name).ConfigureAwait(false);
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 59;
			if (this.bufPos > this.bufLen)
			{
				await this.FlushBufferAsync().ConfigureAwait(false);
			}
			this.textPos = this.bufPos;
		}

		public override async Task WriteCharEntityAsync(char ch)
		{
			this.CheckAsyncCall();
			int num = (int)ch;
			string text = num.ToString("X", NumberFormatInfo.InvariantInfo);
			if (this.checkCharacters && !this.xmlCharType.IsCharData(ch))
			{
				throw XmlConvert.CreateInvalidCharException(ch, '\0');
			}
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			char[] array = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array[num] = 38;
			char[] array2 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array2[num] = 35;
			char[] array3 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array3[num] = 120;
			await this.RawTextAsync(text).ConfigureAwait(false);
			char[] array4 = this.bufChars;
			num = this.bufPos;
			this.bufPos = num + 1;
			array4[num] = 59;
			if (this.bufPos > this.bufLen)
			{
				await this.FlushBufferAsync().ConfigureAwait(false);
			}
			this.textPos = this.bufPos;
		}

		public override Task WriteWhitespaceAsync(string ws)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			if (this.inAttributeValue)
			{
				return this.WriteAttributeTextBlockAsync(ws);
			}
			return this.WriteElementTextBlockAsync(ws);
		}

		public override Task WriteStringAsync(string text)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
			if (this.inAttributeValue)
			{
				return this.WriteAttributeTextBlockAsync(text);
			}
			return this.WriteElementTextBlockAsync(text);
		}

		public override async Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			int num = XmlCharType.CombineSurrogateChar((int)lowChar, (int)highChar);
			char[] array = this.bufChars;
			int num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array[num2] = 38;
			char[] array2 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array2[num2] = 35;
			char[] array3 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array3[num2] = 120;
			await this.RawTextAsync(num.ToString("X", NumberFormatInfo.InvariantInfo)).ConfigureAwait(false);
			char[] array4 = this.bufChars;
			num2 = this.bufPos;
			this.bufPos = num2 + 1;
			array4[num2] = 59;
			this.textPos = this.bufPos;
		}

		public override Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && !this.inTextContent)
			{
				this.ChangeTextContentMark(true);
			}
			if (this.inAttributeValue)
			{
				return this.WriteAttributeTextBlockAsync(buffer, index, count);
			}
			return this.WriteElementTextBlockAsync(buffer, index, count);
		}

		public override async Task WriteRawAsync(char[] buffer, int index, int count)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			await this.WriteRawWithCharCheckingAsync(buffer, index, count).ConfigureAwait(false);
			this.textPos = this.bufPos;
		}

		public override async Task WriteRawAsync(string data)
		{
			this.CheckAsyncCall();
			if (this.trackTextContent && this.inTextContent)
			{
				this.ChangeTextContentMark(false);
			}
			await this.WriteRawWithCharCheckingAsync(data).ConfigureAwait(false);
			this.textPos = this.bufPos;
		}

		public override async Task FlushAsync()
		{
			this.CheckAsyncCall();
			await this.FlushBufferAsync().ConfigureAwait(false);
			await this.FlushEncoderAsync().ConfigureAwait(false);
			if (this.stream != null)
			{
				await this.stream.FlushAsync().ConfigureAwait(false);
			}
			else if (this.writer != null)
			{
				await this.writer.FlushAsync().ConfigureAwait(false);
			}
		}

		protected virtual async Task FlushBufferAsync()
		{
			try
			{
				if (!this.writeToNull)
				{
					if (this.stream != null)
					{
						if (this.trackTextContent)
						{
							this.charEntityFallback.Reset(this.textContentMarks, this.lastMarkPos);
							if ((this.lastMarkPos & 1) != 0)
							{
								this.textContentMarks[1] = 1;
								this.lastMarkPos = 1;
							}
							else
							{
								this.lastMarkPos = 0;
							}
						}
						await this.EncodeCharsAsync(1, this.bufPos, true).ConfigureAwait(false);
					}
					else
					{
						await this.writer.WriteAsync(this.bufChars, 1, this.bufPos - 1).ConfigureAwait(false);
					}
				}
			}
			catch
			{
				this.writeToNull = true;
				throw;
			}
			finally
			{
				this.bufChars[0] = this.bufChars[this.bufPos - 1];
				this.textPos = ((this.textPos == this.bufPos) ? 1 : 0);
				this.attrEndPos = ((this.attrEndPos == this.bufPos) ? 1 : 0);
				this.contentPos = 0;
				this.cdataPos = 0;
				this.bufPos = 1;
			}
		}

		private async Task EncodeCharsAsync(int startOffset, int endOffset, bool writeAllToStream)
		{
			while (startOffset < endOffset)
			{
				if (this.charEntityFallback != null)
				{
					this.charEntityFallback.StartOffset = startOffset;
				}
				int num;
				int num2;
				bool flag;
				this.encoder.Convert(this.bufChars, startOffset, endOffset - startOffset, this.bufBytes, this.bufBytesUsed, this.bufBytes.Length - this.bufBytesUsed, false, out num, out num2, out flag);
				startOffset += num;
				this.bufBytesUsed += num2;
				if (this.bufBytesUsed >= this.bufBytes.Length - 16)
				{
					await this.stream.WriteAsync(this.bufBytes, 0, this.bufBytesUsed).ConfigureAwait(false);
					this.bufBytesUsed = 0;
				}
			}
			if (writeAllToStream && this.bufBytesUsed > 0)
			{
				await this.stream.WriteAsync(this.bufBytes, 0, this.bufBytesUsed).ConfigureAwait(false);
				this.bufBytesUsed = 0;
			}
		}

		private Task FlushEncoderAsync()
		{
			if (this.stream != null)
			{
				int num;
				int num2;
				bool flag;
				this.encoder.Convert(this.bufChars, 1, 0, this.bufBytes, 0, this.bufBytes.Length, true, out num, out num2, out flag);
				if (num2 != 0)
				{
					return this.stream.WriteAsync(this.bufBytes, 0, num2);
				}
			}
			return AsyncHelper.DoneTask;
		}

		[SecuritySafeCritical]
		protected unsafe int WriteAttributeTextBlockNoFlush(char* pSrc, char* pSrcEnd)
		{
			char* ptr = pSrc;
			char[] array;
			char* ptr2;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array[0];
			}
			char* ptr3 = ptr2 + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr3 + (long)(pSrcEnd - pSrc) * 2L / 2L;
				if (ptr4 != ptr2 + this.bufLen)
				{
					ptr4 = ptr2 + this.bufLen;
				}
				while (ptr3 < ptr4 && (this.xmlCharType.charProperties[num = (int)(*pSrc)] & 128) != 0)
				{
					*ptr3 = (char)num;
					ptr3++;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					goto IL_01EE;
				}
				if (ptr3 >= ptr4)
				{
					break;
				}
				if (num <= 38)
				{
					switch (num)
					{
					case 9:
						if (this.newLineHandling == NewLineHandling.None)
						{
							*ptr3 = (char)num;
							ptr3++;
							goto IL_01E4;
						}
						ptr3 = XmlEncodedRawTextWriter.TabEntity(ptr3);
						goto IL_01E4;
					case 10:
						if (this.newLineHandling == NewLineHandling.None)
						{
							*ptr3 = (char)num;
							ptr3++;
							goto IL_01E4;
						}
						ptr3 = XmlEncodedRawTextWriter.LineFeedEntity(ptr3);
						goto IL_01E4;
					case 11:
					case 12:
						break;
					case 13:
						if (this.newLineHandling == NewLineHandling.None)
						{
							*ptr3 = (char)num;
							ptr3++;
							goto IL_01E4;
						}
						ptr3 = XmlEncodedRawTextWriter.CarriageReturnEntity(ptr3);
						goto IL_01E4;
					default:
						if (num == 34)
						{
							ptr3 = XmlEncodedRawTextWriter.QuoteEntity(ptr3);
							goto IL_01E4;
						}
						if (num == 38)
						{
							ptr3 = XmlEncodedRawTextWriter.AmpEntity(ptr3);
							goto IL_01E4;
						}
						break;
					}
				}
				else
				{
					if (num == 39)
					{
						*ptr3 = (char)num;
						ptr3++;
						goto IL_01E4;
					}
					if (num == 60)
					{
						ptr3 = XmlEncodedRawTextWriter.LtEntity(ptr3);
						goto IL_01E4;
					}
					if (num == 62)
					{
						ptr3 = XmlEncodedRawTextWriter.GtEntity(ptr3);
						goto IL_01E4;
					}
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr3 = XmlEncodedRawTextWriter.EncodeSurrogate(pSrc, pSrcEnd, ptr3);
					pSrc += 2;
					continue;
				}
				if (num <= 127 || num >= 65534)
				{
					ptr3 = this.InvalidXmlChar(num, ptr3, true);
					pSrc++;
					continue;
				}
				*ptr3 = (char)num;
				ptr3++;
				pSrc++;
				continue;
				IL_01E4:
				pSrc++;
			}
			this.bufPos = (int)((long)(ptr3 - ptr2));
			return (int)((long)(pSrc - ptr));
			IL_01EE:
			this.bufPos = (int)((long)(ptr3 - ptr2));
			array = null;
			return -1;
		}

		[SecuritySafeCritical]
		protected unsafe int WriteAttributeTextBlockNoFlush(char[] chars, int index, int count)
		{
			if (count == 0)
			{
				return -1;
			}
			fixed (char* ptr = &chars[index])
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + count;
				return this.WriteAttributeTextBlockNoFlush(ptr2, ptr3);
			}
		}

		[SecuritySafeCritical]
		protected unsafe int WriteAttributeTextBlockNoFlush(string text, int index, int count)
		{
			if (count == 0)
			{
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char* ptr3 = ptr2 + count;
			return this.WriteAttributeTextBlockNoFlush(ptr2, ptr3);
		}

		protected async Task WriteAttributeTextBlockAsync(char[] chars, int index, int count)
		{
			int writeLen = 0;
			int curIndex = index;
			int leftCount = count;
			do
			{
				writeLen = this.WriteAttributeTextBlockNoFlush(chars, curIndex, leftCount);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0);
		}

		protected Task WriteAttributeTextBlockAsync(string text)
		{
			int num = 0;
			int num2 = text.Length;
			int num3 = this.WriteAttributeTextBlockNoFlush(text, num, num2);
			num += num3;
			num2 -= num3;
			if (num3 >= 0)
			{
				return this._WriteAttributeTextBlockAsync(text, num, num2);
			}
			return AsyncHelper.DoneTask;
		}

		private async Task _WriteAttributeTextBlockAsync(string text, int curIndex, int leftCount)
		{
			await this.FlushBufferAsync().ConfigureAwait(false);
			int writeLen;
			do
			{
				writeLen = this.WriteAttributeTextBlockNoFlush(text, curIndex, leftCount);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0);
		}

		[SecuritySafeCritical]
		protected unsafe int WriteElementTextBlockNoFlush(char* pSrc, char* pSrcEnd, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			char* ptr = pSrc;
			char[] array;
			char* ptr2;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array[0];
			}
			char* ptr3 = ptr2 + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr3 + (long)(pSrcEnd - pSrc) * 2L / 2L;
				if (ptr4 != ptr2 + this.bufLen)
				{
					ptr4 = ptr2 + this.bufLen;
				}
				while (ptr3 < ptr4 && (this.xmlCharType.charProperties[num = (int)(*pSrc)] & 128) != 0)
				{
					*ptr3 = (char)num;
					ptr3++;
					pSrc++;
				}
				if (pSrc >= pSrcEnd)
				{
					goto IL_020F;
				}
				if (ptr3 >= ptr4)
				{
					break;
				}
				if (num <= 38)
				{
					switch (num)
					{
					case 9:
						goto IL_011A;
					case 10:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_13;
						}
						*ptr3 = (char)num;
						ptr3++;
						goto IL_0205;
					case 11:
					case 12:
						break;
					case 13:
						switch (this.newLineHandling)
						{
						case NewLineHandling.Replace:
							goto IL_0176;
						case NewLineHandling.Entitize:
							ptr3 = XmlEncodedRawTextWriter.CarriageReturnEntity(ptr3);
							goto IL_0205;
						case NewLineHandling.None:
							*ptr3 = (char)num;
							ptr3++;
							goto IL_0205;
						default:
							goto IL_0205;
						}
						break;
					default:
						if (num == 34)
						{
							goto IL_011A;
						}
						if (num == 38)
						{
							ptr3 = XmlEncodedRawTextWriter.AmpEntity(ptr3);
							goto IL_0205;
						}
						break;
					}
				}
				else
				{
					if (num == 39)
					{
						goto IL_011A;
					}
					if (num == 60)
					{
						ptr3 = XmlEncodedRawTextWriter.LtEntity(ptr3);
						goto IL_0205;
					}
					if (num == 62)
					{
						ptr3 = XmlEncodedRawTextWriter.GtEntity(ptr3);
						goto IL_0205;
					}
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr3 = XmlEncodedRawTextWriter.EncodeSurrogate(pSrc, pSrcEnd, ptr3);
					pSrc += 2;
					continue;
				}
				if (num <= 127 || num >= 65534)
				{
					ptr3 = this.InvalidXmlChar(num, ptr3, true);
					pSrc++;
					continue;
				}
				*ptr3 = (char)num;
				ptr3++;
				pSrc++;
				continue;
				IL_0205:
				pSrc++;
				continue;
				IL_011A:
				*ptr3 = (char)num;
				ptr3++;
				goto IL_0205;
			}
			this.bufPos = (int)((long)(ptr3 - ptr2));
			return (int)((long)(pSrc - ptr));
			Block_13:
			this.bufPos = (int)((long)(ptr3 - ptr2));
			needWriteNewLine = true;
			return (int)((long)(pSrc - ptr));
			IL_0176:
			if (pSrc[1] == '\n')
			{
				pSrc++;
			}
			this.bufPos = (int)((long)(ptr3 - ptr2));
			needWriteNewLine = true;
			return (int)((long)(pSrc - ptr));
			IL_020F:
			this.bufPos = (int)((long)(ptr3 - ptr2));
			this.textPos = this.bufPos;
			this.contentPos = 0;
			array = null;
			return -1;
		}

		[SecuritySafeCritical]
		protected unsafe int WriteElementTextBlockNoFlush(char[] chars, int index, int count, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				this.contentPos = 0;
				return -1;
			}
			fixed (char* ptr = &chars[index])
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + count;
				return this.WriteElementTextBlockNoFlush(ptr2, ptr3, out needWriteNewLine);
			}
		}

		[SecuritySafeCritical]
		protected unsafe int WriteElementTextBlockNoFlush(string text, int index, int count, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				this.contentPos = 0;
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char* ptr3 = ptr2 + count;
			return this.WriteElementTextBlockNoFlush(ptr2, ptr3, out needWriteNewLine);
		}

		protected async Task WriteElementTextBlockAsync(char[] chars, int index, int count)
		{
			int writeLen = 0;
			int curIndex = index;
			int leftCount = count;
			bool needWriteNewLine = false;
			do
			{
				writeLen = this.WriteElementTextBlockNoFlush(chars, curIndex, leftCount, out needWriteNewLine);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (needWriteNewLine)
				{
					await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
					curIndex++;
					leftCount--;
				}
				else if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0 || needWriteNewLine);
		}

		protected Task WriteElementTextBlockAsync(string text)
		{
			int num = 0;
			int num2 = text.Length;
			bool flag = false;
			int num3 = this.WriteElementTextBlockNoFlush(text, num, num2, out flag);
			num += num3;
			num2 -= num3;
			if (flag)
			{
				return this._WriteElementTextBlockAsync(true, text, num, num2);
			}
			if (num3 >= 0)
			{
				return this._WriteElementTextBlockAsync(false, text, num, num2);
			}
			return AsyncHelper.DoneTask;
		}

		private async Task _WriteElementTextBlockAsync(bool newLine, string text, int curIndex, int leftCount)
		{
			int writeLen = 0;
			bool needWriteNewLine = false;
			if (newLine)
			{
				await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
				curIndex++;
				leftCount--;
			}
			else
			{
				await this.FlushBufferAsync().ConfigureAwait(false);
			}
			do
			{
				writeLen = this.WriteElementTextBlockNoFlush(text, curIndex, leftCount, out needWriteNewLine);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (needWriteNewLine)
				{
					await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
					curIndex++;
					leftCount--;
				}
				else if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0 || needWriteNewLine);
		}

		[SecuritySafeCritical]
		protected unsafe int RawTextNoFlush(char* pSrcBegin, char* pSrcEnd)
		{
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = ptr + this.bufPos;
			char* ptr3 = pSrcBegin;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr2 + (long)(pSrcEnd - ptr3) * 2L / 2L;
				if (ptr4 != ptr + this.bufLen)
				{
					ptr4 = ptr + this.bufLen;
				}
				while (ptr2 < ptr4 && (num = (int)(*ptr3)) < 55296)
				{
					ptr3++;
					*ptr2 = (char)num;
					ptr2++;
				}
				if (ptr3 >= pSrcEnd)
				{
					goto IL_00F9;
				}
				if (ptr2 >= ptr4)
				{
					break;
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr2 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr3, pSrcEnd, ptr2);
					ptr3 += 2;
				}
				else if (num <= 127 || num >= 65534)
				{
					ptr2 = this.InvalidXmlChar(num, ptr2, false);
					ptr3++;
				}
				else
				{
					*ptr2 = (char)num;
					ptr2++;
					ptr3++;
				}
			}
			this.bufPos = (int)((long)(ptr2 - ptr));
			return (int)((long)(ptr3 - pSrcBegin));
			IL_00F9:
			this.bufPos = (int)((long)(ptr2 - ptr));
			array = null;
			return -1;
		}

		[SecuritySafeCritical]
		protected unsafe int RawTextNoFlush(string text, int index, int count)
		{
			if (count == 0)
			{
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char* ptr3 = ptr2 + count;
			return this.RawTextNoFlush(ptr2, ptr3);
		}

		protected Task RawTextAsync(string text)
		{
			int num = 0;
			int num2 = text.Length;
			int num3 = this.RawTextNoFlush(text, num, num2);
			num += num3;
			num2 -= num3;
			if (num3 >= 0)
			{
				return this._RawTextAsync(text, num, num2);
			}
			return AsyncHelper.DoneTask;
		}

		private async Task _RawTextAsync(string text, int curIndex, int leftCount)
		{
			await this.FlushBufferAsync().ConfigureAwait(false);
			int writeLen = 0;
			do
			{
				writeLen = this.RawTextNoFlush(text, curIndex, leftCount);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0);
		}

		[SecuritySafeCritical]
		protected unsafe int WriteRawWithCharCheckingNoFlush(char* pSrcBegin, char* pSrcEnd, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			char[] array;
			char* ptr;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			char* ptr2 = pSrcBegin;
			char* ptr3 = ptr + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr4 = ptr3 + (long)(pSrcEnd - ptr2) * 2L / 2L;
				if (ptr4 != ptr + this.bufLen)
				{
					ptr4 = ptr + this.bufLen;
				}
				while (ptr3 < ptr4 && (this.xmlCharType.charProperties[num = (int)(*ptr2)] & 64) != 0)
				{
					*ptr3 = (char)num;
					ptr3++;
					ptr2++;
				}
				if (ptr2 >= pSrcEnd)
				{
					goto IL_01CC;
				}
				if (ptr3 >= ptr4)
				{
					break;
				}
				if (num <= 38)
				{
					switch (num)
					{
					case 9:
						goto IL_00EB;
					case 10:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_12;
						}
						*ptr3 = (char)num;
						ptr3++;
						goto IL_01C3;
					case 11:
					case 12:
						break;
					case 13:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_10;
						}
						*ptr3 = (char)num;
						ptr3++;
						goto IL_01C3;
					default:
						if (num == 38)
						{
							goto IL_00EB;
						}
						break;
					}
				}
				else if (num == 60 || num == 93)
				{
					goto IL_00EB;
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr3 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr2, pSrcEnd, ptr3);
					ptr2 += 2;
					continue;
				}
				if (num <= 127 || num >= 65534)
				{
					ptr3 = this.InvalidXmlChar(num, ptr3, false);
					ptr2++;
					continue;
				}
				*ptr3 = (char)num;
				ptr3++;
				ptr2++;
				continue;
				IL_01C3:
				ptr2++;
				continue;
				IL_00EB:
				*ptr3 = (char)num;
				ptr3++;
				goto IL_01C3;
			}
			this.bufPos = (int)((long)(ptr3 - ptr));
			return (int)((long)(ptr2 - pSrcBegin));
			Block_10:
			if (ptr2[1] == '\n')
			{
				ptr2++;
			}
			this.bufPos = (int)((long)(ptr3 - ptr));
			needWriteNewLine = true;
			return (int)((long)(ptr2 - pSrcBegin));
			Block_12:
			this.bufPos = (int)((long)(ptr3 - ptr));
			needWriteNewLine = true;
			return (int)((long)(ptr2 - pSrcBegin));
			IL_01CC:
			this.bufPos = (int)((long)(ptr3 - ptr));
			array = null;
			return -1;
		}

		[SecuritySafeCritical]
		protected unsafe int WriteRawWithCharCheckingNoFlush(char[] chars, int index, int count, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				return -1;
			}
			fixed (char* ptr = &chars[index])
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + count;
				return this.WriteRawWithCharCheckingNoFlush(ptr2, ptr3, out needWriteNewLine);
			}
		}

		[SecuritySafeCritical]
		protected unsafe int WriteRawWithCharCheckingNoFlush(string text, int index, int count, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char* ptr3 = ptr2 + count;
			return this.WriteRawWithCharCheckingNoFlush(ptr2, ptr3, out needWriteNewLine);
		}

		protected async Task WriteRawWithCharCheckingAsync(char[] chars, int index, int count)
		{
			int writeLen = 0;
			int curIndex = index;
			int leftCount = count;
			bool needWriteNewLine = false;
			do
			{
				writeLen = this.WriteRawWithCharCheckingNoFlush(chars, curIndex, leftCount, out needWriteNewLine);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (needWriteNewLine)
				{
					await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
					curIndex++;
					leftCount--;
				}
				else if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0 || needWriteNewLine);
		}

		protected async Task WriteRawWithCharCheckingAsync(string text)
		{
			int writeLen = 0;
			int curIndex = 0;
			int leftCount = text.Length;
			bool needWriteNewLine = false;
			do
			{
				writeLen = this.WriteRawWithCharCheckingNoFlush(text, curIndex, leftCount, out needWriteNewLine);
				curIndex += writeLen;
				leftCount -= writeLen;
				if (needWriteNewLine)
				{
					await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
					curIndex++;
					leftCount--;
				}
				else if (writeLen >= 0)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			while (writeLen >= 0 || needWriteNewLine);
		}

		[SecuritySafeCritical]
		protected unsafe int WriteCommentOrPiNoFlush(string text, int index, int count, int stopChar, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char[] array;
			char* ptr3;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &array[0];
			}
			char* ptr4 = ptr2;
			char* ptr5 = ptr4;
			char* ptr6 = ptr2 + count;
			char* ptr7 = ptr3 + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr8 = ptr7 + (long)(ptr6 - ptr4) * 2L / 2L;
				if (ptr8 != ptr3 + this.bufLen)
				{
					ptr8 = ptr3 + this.bufLen;
				}
				while (ptr7 < ptr8 && (this.xmlCharType.charProperties[num = (int)(*ptr4)] & 64) != 0 && num != stopChar)
				{
					*ptr7 = (char)num;
					ptr7++;
					ptr4++;
				}
				if (ptr4 >= ptr6)
				{
					goto IL_02AB;
				}
				if (ptr7 >= ptr8)
				{
					break;
				}
				if (num <= 45)
				{
					switch (num)
					{
					case 9:
						goto IL_0230;
					case 10:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_23;
						}
						*ptr7 = (char)num;
						ptr7++;
						goto IL_02A0;
					case 11:
					case 12:
						break;
					case 13:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_21;
						}
						*ptr7 = (char)num;
						ptr7++;
						goto IL_02A0;
					default:
						if (num == 38)
						{
							goto IL_0230;
						}
						if (num == 45)
						{
							*ptr7 = '-';
							ptr7++;
							if (num == stopChar && (ptr4 + 1 == ptr6 || ptr4[1] == '-'))
							{
								*ptr7 = ' ';
								ptr7++;
								goto IL_02A0;
							}
							goto IL_02A0;
						}
						break;
					}
				}
				else
				{
					if (num == 60)
					{
						goto IL_0230;
					}
					if (num != 63)
					{
						if (num == 93)
						{
							*ptr7 = ']';
							ptr7++;
							goto IL_02A0;
						}
					}
					else
					{
						*ptr7 = '?';
						ptr7++;
						if (num == stopChar && ptr4 + 1 < ptr6 && ptr4[1] == '>')
						{
							*ptr7 = ' ';
							ptr7++;
							goto IL_02A0;
						}
						goto IL_02A0;
					}
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr7 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr4, ptr6, ptr7);
					ptr4 += 2;
					continue;
				}
				if (num <= 127 || num >= 65534)
				{
					ptr7 = this.InvalidXmlChar(num, ptr7, false);
					ptr4++;
					continue;
				}
				*ptr7 = (char)num;
				ptr7++;
				ptr4++;
				continue;
				IL_02A0:
				ptr4++;
				continue;
				IL_0230:
				*ptr7 = (char)num;
				ptr7++;
				goto IL_02A0;
			}
			this.bufPos = (int)((long)(ptr7 - ptr3));
			return (int)((long)(ptr4 - ptr5));
			Block_21:
			if (ptr4[1] == '\n')
			{
				ptr4++;
			}
			this.bufPos = (int)((long)(ptr7 - ptr3));
			needWriteNewLine = true;
			return (int)((long)(ptr4 - ptr5));
			Block_23:
			this.bufPos = (int)((long)(ptr7 - ptr3));
			needWriteNewLine = true;
			return (int)((long)(ptr4 - ptr5));
			IL_02AB:
			this.bufPos = (int)((long)(ptr7 - ptr3));
			array = null;
			return -1;
		}

		protected async Task WriteCommentOrPiAsync(string text, int stopChar)
		{
			if (text.Length == 0)
			{
				if (this.bufPos >= this.bufLen)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			else
			{
				int writeLen = 0;
				int curIndex = 0;
				int leftCount = text.Length;
				bool needWriteNewLine = false;
				do
				{
					writeLen = this.WriteCommentOrPiNoFlush(text, curIndex, leftCount, stopChar, out needWriteNewLine);
					curIndex += writeLen;
					leftCount -= writeLen;
					if (needWriteNewLine)
					{
						await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
						curIndex++;
						leftCount--;
					}
					else if (writeLen >= 0)
					{
						await this.FlushBufferAsync().ConfigureAwait(false);
					}
				}
				while (writeLen >= 0 || needWriteNewLine);
			}
		}

		[SecuritySafeCritical]
		protected unsafe int WriteCDataSectionNoFlush(string text, int index, int count, out bool needWriteNewLine)
		{
			needWriteNewLine = false;
			if (count == 0)
			{
				return -1;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + index;
			char[] array;
			char* ptr3;
			if ((array = this.bufChars) == null || array.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &array[0];
			}
			char* ptr4 = ptr2;
			char* ptr5 = ptr2 + count;
			char* ptr6 = ptr4;
			char* ptr7 = ptr3 + this.bufPos;
			int num = 0;
			for (;;)
			{
				char* ptr8 = ptr7 + (long)(ptr5 - ptr4) * 2L / 2L;
				if (ptr8 != ptr3 + this.bufLen)
				{
					ptr8 = ptr3 + this.bufLen;
				}
				while (ptr7 < ptr8 && (this.xmlCharType.charProperties[num = (int)(*ptr4)] & 128) != 0 && num != 93)
				{
					*ptr7 = (char)num;
					ptr7++;
					ptr4++;
				}
				if (ptr4 >= ptr5)
				{
					goto IL_0292;
				}
				if (ptr7 >= ptr8)
				{
					break;
				}
				if (num <= 39)
				{
					switch (num)
					{
					case 9:
						goto IL_0217;
					case 10:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_21;
						}
						*ptr7 = (char)num;
						ptr7++;
						goto IL_0287;
					case 11:
					case 12:
						break;
					case 13:
						if (this.newLineHandling == NewLineHandling.Replace)
						{
							goto Block_19;
						}
						*ptr7 = (char)num;
						ptr7++;
						goto IL_0287;
					default:
						if (num == 34 || num - 38 <= 1)
						{
							goto IL_0217;
						}
						break;
					}
				}
				else
				{
					if (num == 60)
					{
						goto IL_0217;
					}
					if (num == 62)
					{
						if (this.hadDoubleBracket && ptr7[-1] == ']')
						{
							ptr7 = XmlEncodedRawTextWriter.RawEndCData(ptr7);
							ptr7 = XmlEncodedRawTextWriter.RawStartCData(ptr7);
						}
						*ptr7 = '>';
						ptr7++;
						goto IL_0287;
					}
					if (num == 93)
					{
						if (ptr7[-1] == ']')
						{
							this.hadDoubleBracket = true;
						}
						else
						{
							this.hadDoubleBracket = false;
						}
						*ptr7 = ']';
						ptr7++;
						goto IL_0287;
					}
				}
				if (XmlCharType.IsSurrogate(num))
				{
					ptr7 = XmlEncodedRawTextWriter.EncodeSurrogate(ptr4, ptr5, ptr7);
					ptr4 += 2;
					continue;
				}
				if (num <= 127 || num >= 65534)
				{
					ptr7 = this.InvalidXmlChar(num, ptr7, false);
					ptr4++;
					continue;
				}
				*ptr7 = (char)num;
				ptr7++;
				ptr4++;
				continue;
				IL_0287:
				ptr4++;
				continue;
				IL_0217:
				*ptr7 = (char)num;
				ptr7++;
				goto IL_0287;
			}
			this.bufPos = (int)((long)(ptr7 - ptr3));
			return (int)((long)(ptr4 - ptr6));
			Block_19:
			if (ptr4[1] == '\n')
			{
				ptr4++;
			}
			this.bufPos = (int)((long)(ptr7 - ptr3));
			needWriteNewLine = true;
			return (int)((long)(ptr4 - ptr6));
			Block_21:
			this.bufPos = (int)((long)(ptr7 - ptr3));
			needWriteNewLine = true;
			return (int)((long)(ptr4 - ptr6));
			IL_0292:
			this.bufPos = (int)((long)(ptr7 - ptr3));
			array = null;
			return -1;
		}

		protected async Task WriteCDataSectionAsync(string text)
		{
			if (text.Length == 0)
			{
				if (this.bufPos >= this.bufLen)
				{
					await this.FlushBufferAsync().ConfigureAwait(false);
				}
			}
			else
			{
				int writeLen = 0;
				int curIndex = 0;
				int leftCount = text.Length;
				bool needWriteNewLine = false;
				do
				{
					writeLen = this.WriteCDataSectionNoFlush(text, curIndex, leftCount, out needWriteNewLine);
					curIndex += writeLen;
					leftCount -= writeLen;
					if (needWriteNewLine)
					{
						await this.RawTextAsync(this.newLineChars).ConfigureAwait(false);
						curIndex++;
						leftCount--;
					}
					else if (writeLen >= 0)
					{
						await this.FlushBufferAsync().ConfigureAwait(false);
					}
				}
				while (writeLen >= 0 || needWriteNewLine);
			}
		}

		private readonly bool useAsync;

		protected byte[] bufBytes;

		protected Stream stream;

		protected Encoding encoding;

		protected XmlCharType xmlCharType = XmlCharType.Instance;

		protected int bufPos = 1;

		protected int textPos = 1;

		protected int contentPos;

		protected int cdataPos;

		protected int attrEndPos;

		protected int bufLen = 6144;

		protected bool writeToNull;

		protected bool hadDoubleBracket;

		protected bool inAttributeValue;

		protected int bufBytesUsed;

		protected char[] bufChars;

		protected Encoder encoder;

		protected TextWriter writer;

		protected bool trackTextContent;

		protected bool inTextContent;

		private int lastMarkPos;

		private int[] textContentMarks;

		private CharEntityEncoderFallback charEntityFallback;

		protected NewLineHandling newLineHandling;

		protected bool closeOutput;

		protected bool omitXmlDeclaration;

		protected string newLineChars;

		protected bool checkCharacters;

		protected XmlStandalone standalone;

		protected XmlOutputMethod outputMethod;

		protected bool autoXmlDeclaration;

		protected bool mergeCDataSections;

		private const int BUFSIZE = 6144;

		private const int ASYNCBUFSIZE = 65536;

		private const int OVERFLOW = 32;

		private const int INIT_MARKS_COUNT = 64;
	}
}
