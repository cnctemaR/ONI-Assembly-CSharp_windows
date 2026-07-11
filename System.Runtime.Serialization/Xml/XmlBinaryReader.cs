using System;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security;

namespace System.Xml
{
	internal class XmlBinaryReader : XmlBaseReader, IXmlBinaryReaderInitializer
	{
		public void SetInput(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			this.MoveToInitial(quotas, session, onClose);
			base.BufferReader.SetBuffer(buffer, offset, count, dictionary, session);
			this.buffered = true;
		}

		public void SetInput(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.MoveToInitial(quotas, session, onClose);
			base.BufferReader.SetBuffer(stream, dictionary, session);
			this.buffered = false;
		}

		private void MoveToInitial(XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			base.MoveToInitial(quotas);
			this.maxBytesPerRead = quotas.MaxBytesPerRead;
			this.arrayState = XmlBinaryReader.ArrayState.None;
			this.onClose = onClose;
			this.isTextWithEndElement = false;
		}

		public override void Close()
		{
			base.Close();
			OnXmlDictionaryReaderClose onXmlDictionaryReaderClose = this.onClose;
			this.onClose = null;
			if (onXmlDictionaryReaderClose != null)
			{
				try
				{
					onXmlDictionaryReaderClose(this);
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(ex);
				}
			}
		}

		public override string ReadElementContentAsString()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (!this.CanOptimizeReadElementContent())
			{
				return base.ReadElementContentAsString();
			}
			XmlBinaryNodeType nodeType = this.GetNodeType();
			string text;
			if (nodeType != XmlBinaryNodeType.Chars8TextWithEndElement)
			{
				if (nodeType != XmlBinaryNodeType.DictionaryTextWithEndElement)
				{
					text = base.ReadElementContentAsString();
				}
				else
				{
					this.SkipNodeType();
					text = base.BufferReader.GetDictionaryString(this.ReadDictionaryKey()).Value;
					this.ReadTextWithEndElement();
				}
			}
			else
			{
				this.SkipNodeType();
				text = base.BufferReader.ReadUTF8String(this.ReadUInt8());
				this.ReadTextWithEndElement();
			}
			if (text.Length > this.Quotas.MaxStringContentLength)
			{
				XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, this.Quotas.MaxStringContentLength);
			}
			return text;
		}

		public override bool ReadElementContentAsBoolean()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (!this.CanOptimizeReadElementContent())
			{
				return base.ReadElementContentAsBoolean();
			}
			XmlBinaryNodeType nodeType = this.GetNodeType();
			bool flag;
			if (nodeType != XmlBinaryNodeType.FalseTextWithEndElement)
			{
				if (nodeType != XmlBinaryNodeType.TrueTextWithEndElement)
				{
					if (nodeType != XmlBinaryNodeType.BoolTextWithEndElement)
					{
						flag = base.ReadElementContentAsBoolean();
					}
					else
					{
						this.SkipNodeType();
						flag = base.BufferReader.ReadUInt8() != 0;
						this.ReadTextWithEndElement();
					}
				}
				else
				{
					this.SkipNodeType();
					flag = true;
					this.ReadTextWithEndElement();
				}
			}
			else
			{
				this.SkipNodeType();
				flag = false;
				this.ReadTextWithEndElement();
			}
			return flag;
		}

		public override int ReadElementContentAsInt()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (!this.CanOptimizeReadElementContent())
			{
				return base.ReadElementContentAsInt();
			}
			XmlBinaryNodeType nodeType = this.GetNodeType();
			int num;
			if (nodeType != XmlBinaryNodeType.ZeroTextWithEndElement)
			{
				if (nodeType != XmlBinaryNodeType.OneTextWithEndElement)
				{
					switch (nodeType)
					{
					case XmlBinaryNodeType.Int8TextWithEndElement:
						this.SkipNodeType();
						num = base.BufferReader.ReadInt8();
						this.ReadTextWithEndElement();
						return num;
					case XmlBinaryNodeType.Int16TextWithEndElement:
						this.SkipNodeType();
						num = base.BufferReader.ReadInt16();
						this.ReadTextWithEndElement();
						return num;
					case XmlBinaryNodeType.Int32TextWithEndElement:
						this.SkipNodeType();
						num = base.BufferReader.ReadInt32();
						this.ReadTextWithEndElement();
						return num;
					}
					num = base.ReadElementContentAsInt();
				}
				else
				{
					this.SkipNodeType();
					num = 1;
					this.ReadTextWithEndElement();
				}
			}
			else
			{
				this.SkipNodeType();
				num = 0;
				this.ReadTextWithEndElement();
			}
			return num;
		}

		private bool CanOptimizeReadElementContent()
		{
			return this.arrayState == XmlBinaryReader.ArrayState.None && !base.Signing;
		}

		public override float ReadElementContentAsFloat()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.FloatTextWithEndElement)
			{
				this.SkipNodeType();
				float num = base.BufferReader.ReadSingle();
				this.ReadTextWithEndElement();
				return num;
			}
			return base.ReadElementContentAsFloat();
		}

		public override double ReadElementContentAsDouble()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.DoubleTextWithEndElement)
			{
				this.SkipNodeType();
				double num = base.BufferReader.ReadDouble();
				this.ReadTextWithEndElement();
				return num;
			}
			return base.ReadElementContentAsDouble();
		}

		public override decimal ReadElementContentAsDecimal()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.DecimalTextWithEndElement)
			{
				this.SkipNodeType();
				decimal num = base.BufferReader.ReadDecimal();
				this.ReadTextWithEndElement();
				return num;
			}
			return base.ReadElementContentAsDecimal();
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.DateTimeTextWithEndElement)
			{
				this.SkipNodeType();
				DateTime dateTime = base.BufferReader.ReadDateTime();
				this.ReadTextWithEndElement();
				return dateTime;
			}
			return base.ReadElementContentAsDateTime();
		}

		public override TimeSpan ReadElementContentAsTimeSpan()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.TimeSpanTextWithEndElement)
			{
				this.SkipNodeType();
				TimeSpan timeSpan = base.BufferReader.ReadTimeSpan();
				this.ReadTextWithEndElement();
				return timeSpan;
			}
			return base.ReadElementContentAsTimeSpan();
		}

		public override Guid ReadElementContentAsGuid()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.GuidTextWithEndElement)
			{
				this.SkipNodeType();
				Guid guid = base.BufferReader.ReadGuid();
				this.ReadTextWithEndElement();
				return guid;
			}
			return base.ReadElementContentAsGuid();
		}

		public override UniqueId ReadElementContentAsUniqueId()
		{
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.CanOptimizeReadElementContent() && this.GetNodeType() == XmlBinaryNodeType.UniqueIdTextWithEndElement)
			{
				this.SkipNodeType();
				UniqueId uniqueId = base.BufferReader.ReadUniqueId();
				this.ReadTextWithEndElement();
				return uniqueId;
			}
			return base.ReadElementContentAsUniqueId();
		}

		public override bool TryGetBase64ContentLength(out int length)
		{
			length = 0;
			if (!this.buffered)
			{
				return false;
			}
			if (this.arrayState != XmlBinaryReader.ArrayState.None)
			{
				return false;
			}
			int num;
			if (!base.Node.Value.TryGetByteArrayLength(out num))
			{
				return false;
			}
			int offset = base.BufferReader.Offset;
			bool flag2;
			try
			{
				bool flag = false;
				while (!flag && !base.BufferReader.EndOfFile)
				{
					XmlBinaryNodeType nodeType = this.GetNodeType();
					this.SkipNodeType();
					int num2;
					if (nodeType != XmlBinaryNodeType.EndElement)
					{
						switch (nodeType)
						{
						case XmlBinaryNodeType.Bytes8Text:
							num2 = base.BufferReader.ReadUInt8();
							break;
						case XmlBinaryNodeType.Bytes8TextWithEndElement:
							num2 = base.BufferReader.ReadUInt8();
							flag = true;
							break;
						case XmlBinaryNodeType.Bytes16Text:
							num2 = base.BufferReader.ReadUInt16();
							break;
						case XmlBinaryNodeType.Bytes16TextWithEndElement:
							num2 = base.BufferReader.ReadUInt16();
							flag = true;
							break;
						case XmlBinaryNodeType.Bytes32Text:
							num2 = base.BufferReader.ReadUInt31();
							break;
						case XmlBinaryNodeType.Bytes32TextWithEndElement:
							num2 = base.BufferReader.ReadUInt31();
							flag = true;
							break;
						default:
							return false;
						}
					}
					else
					{
						num2 = 0;
						flag = true;
					}
					base.BufferReader.Advance(num2);
					if (num > 2147483647 - num2)
					{
						return false;
					}
					num += num2;
				}
				length = num;
				flag2 = true;
			}
			finally
			{
				base.BufferReader.Offset = offset;
			}
			return flag2;
		}

		private void ReadTextWithEndElement()
		{
			base.ExitScope();
			this.ReadNode();
		}

		private XmlBaseReader.XmlAtomicTextNode MoveToAtomicTextWithEndElement()
		{
			this.isTextWithEndElement = true;
			return base.MoveToAtomicText();
		}

		public override bool Read()
		{
			if (base.Node.ReadState == ReadState.Closed)
			{
				return false;
			}
			base.SignNode();
			if (this.isTextWithEndElement)
			{
				this.isTextWithEndElement = false;
				base.MoveToEndElement();
				return true;
			}
			if (this.arrayState == XmlBinaryReader.ArrayState.Content)
			{
				if (this.arrayCount != 0)
				{
					this.MoveToArrayElement();
					return true;
				}
				this.arrayState = XmlBinaryReader.ArrayState.None;
			}
			if (base.Node.ExitScope)
			{
				base.ExitScope();
			}
			return this.ReadNode();
		}

		private bool ReadNode()
		{
			if (!this.buffered)
			{
				base.BufferReader.SetWindow(base.ElementNode.BufferOffset, this.maxBytesPerRead);
			}
			if (base.BufferReader.EndOfFile)
			{
				base.MoveToEndOfFile();
				return false;
			}
			XmlBinaryNodeType nodeType;
			if (this.arrayState == XmlBinaryReader.ArrayState.None)
			{
				nodeType = this.GetNodeType();
				this.SkipNodeType();
			}
			else
			{
				nodeType = this.arrayNodeType;
				this.arrayCount--;
				this.arrayState = XmlBinaryReader.ArrayState.Content;
			}
			switch (nodeType)
			{
			case XmlBinaryNodeType.EndElement:
				base.MoveToEndElement();
				return true;
			case XmlBinaryNodeType.Comment:
				this.ReadName(base.MoveToComment().Value);
				return true;
			case XmlBinaryNodeType.Array:
				this.ReadArray();
				return true;
			case XmlBinaryNodeType.MinElement:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				xmlElementNode.Prefix.SetValue(PrefixHandleType.Empty);
				this.ReadName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(PrefixHandleType.Empty);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.Element:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				this.ReadName(xmlElementNode.Prefix);
				this.ReadName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(xmlElementNode.Prefix);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.ShortDictionaryElement:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				xmlElementNode.Prefix.SetValue(PrefixHandleType.Empty);
				this.ReadDictionaryName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(PrefixHandleType.Empty);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.DictionaryElement:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				this.ReadName(xmlElementNode.Prefix);
				this.ReadDictionaryName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(xmlElementNode.Prefix);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.PrefixDictionaryElementA:
			case XmlBinaryNodeType.PrefixDictionaryElementB:
			case XmlBinaryNodeType.PrefixDictionaryElementC:
			case XmlBinaryNodeType.PrefixDictionaryElementD:
			case XmlBinaryNodeType.PrefixDictionaryElementE:
			case XmlBinaryNodeType.PrefixDictionaryElementF:
			case XmlBinaryNodeType.PrefixDictionaryElementG:
			case XmlBinaryNodeType.PrefixDictionaryElementH:
			case XmlBinaryNodeType.PrefixDictionaryElementI:
			case XmlBinaryNodeType.PrefixDictionaryElementJ:
			case XmlBinaryNodeType.PrefixDictionaryElementK:
			case XmlBinaryNodeType.PrefixDictionaryElementL:
			case XmlBinaryNodeType.PrefixDictionaryElementM:
			case XmlBinaryNodeType.PrefixDictionaryElementN:
			case XmlBinaryNodeType.PrefixDictionaryElementO:
			case XmlBinaryNodeType.PrefixDictionaryElementP:
			case XmlBinaryNodeType.PrefixDictionaryElementQ:
			case XmlBinaryNodeType.PrefixDictionaryElementR:
			case XmlBinaryNodeType.PrefixDictionaryElementS:
			case XmlBinaryNodeType.PrefixDictionaryElementT:
			case XmlBinaryNodeType.PrefixDictionaryElementU:
			case XmlBinaryNodeType.PrefixDictionaryElementV:
			case XmlBinaryNodeType.PrefixDictionaryElementW:
			case XmlBinaryNodeType.PrefixDictionaryElementX:
			case XmlBinaryNodeType.PrefixDictionaryElementY:
			case XmlBinaryNodeType.PrefixDictionaryElementZ:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				PrefixHandleType prefixHandleType = PrefixHandle.GetAlphaPrefix(nodeType - XmlBinaryNodeType.PrefixDictionaryElementA);
				xmlElementNode.Prefix.SetValue(prefixHandleType);
				this.ReadDictionaryName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(prefixHandleType);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.PrefixElementA:
			case XmlBinaryNodeType.PrefixElementB:
			case XmlBinaryNodeType.PrefixElementC:
			case XmlBinaryNodeType.PrefixElementD:
			case XmlBinaryNodeType.PrefixElementE:
			case XmlBinaryNodeType.PrefixElementF:
			case XmlBinaryNodeType.PrefixElementG:
			case XmlBinaryNodeType.PrefixElementH:
			case XmlBinaryNodeType.PrefixElementI:
			case XmlBinaryNodeType.PrefixElementJ:
			case XmlBinaryNodeType.PrefixElementK:
			case XmlBinaryNodeType.PrefixElementL:
			case XmlBinaryNodeType.PrefixElementM:
			case XmlBinaryNodeType.PrefixElementN:
			case XmlBinaryNodeType.PrefixElementO:
			case XmlBinaryNodeType.PrefixElementP:
			case XmlBinaryNodeType.PrefixElementQ:
			case XmlBinaryNodeType.PrefixElementR:
			case XmlBinaryNodeType.PrefixElementS:
			case XmlBinaryNodeType.PrefixElementT:
			case XmlBinaryNodeType.PrefixElementU:
			case XmlBinaryNodeType.PrefixElementV:
			case XmlBinaryNodeType.PrefixElementW:
			case XmlBinaryNodeType.PrefixElementX:
			case XmlBinaryNodeType.PrefixElementY:
			case XmlBinaryNodeType.PrefixElementZ:
			{
				XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
				PrefixHandleType prefixHandleType = PrefixHandle.GetAlphaPrefix(nodeType - XmlBinaryNodeType.PrefixElementA);
				xmlElementNode.Prefix.SetValue(prefixHandleType);
				this.ReadName(xmlElementNode.LocalName);
				this.ReadAttributes();
				xmlElementNode.Namespace = base.LookupNamespace(prefixHandleType);
				xmlElementNode.BufferOffset = base.BufferReader.Offset;
				return true;
			}
			case XmlBinaryNodeType.ZeroTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue(ValueHandleType.Zero);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.OneTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue(ValueHandleType.One);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.FalseTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue(ValueHandleType.False);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.TrueTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue(ValueHandleType.True);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.Int8TextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Int8, 1);
				return true;
			case XmlBinaryNodeType.Int16TextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Int16, 2);
				return true;
			case XmlBinaryNodeType.Int32TextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Int32, 4);
				return true;
			case XmlBinaryNodeType.Int64TextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Int64, 8);
				return true;
			case XmlBinaryNodeType.FloatTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Single, 4);
				return true;
			case XmlBinaryNodeType.DoubleTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Double, 8);
				return true;
			case XmlBinaryNodeType.DecimalTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Decimal, 16);
				return true;
			case XmlBinaryNodeType.DateTimeTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.DateTime, 8);
				return true;
			case XmlBinaryNodeType.Chars8Text:
				if (this.buffered)
				{
					this.ReadText(base.MoveToComplexText(), ValueHandleType.UTF8, this.ReadUInt8());
				}
				else
				{
					this.ReadPartialUTF8Text(false, this.ReadUInt8());
				}
				return true;
			case XmlBinaryNodeType.Chars8TextWithEndElement:
				if (this.buffered)
				{
					this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UTF8, this.ReadUInt8());
				}
				else
				{
					this.ReadPartialUTF8Text(true, this.ReadUInt8());
				}
				return true;
			case XmlBinaryNodeType.Chars16Text:
				if (this.buffered)
				{
					this.ReadText(base.MoveToComplexText(), ValueHandleType.UTF8, this.ReadUInt16());
				}
				else
				{
					this.ReadPartialUTF8Text(false, this.ReadUInt16());
				}
				return true;
			case XmlBinaryNodeType.Chars16TextWithEndElement:
				if (this.buffered)
				{
					this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UTF8, this.ReadUInt16());
				}
				else
				{
					this.ReadPartialUTF8Text(true, this.ReadUInt16());
				}
				return true;
			case XmlBinaryNodeType.Chars32Text:
				if (this.buffered)
				{
					this.ReadText(base.MoveToComplexText(), ValueHandleType.UTF8, this.ReadUInt31());
				}
				else
				{
					this.ReadPartialUTF8Text(false, this.ReadUInt31());
				}
				return true;
			case XmlBinaryNodeType.Chars32TextWithEndElement:
				if (this.buffered)
				{
					this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UTF8, this.ReadUInt31());
				}
				else
				{
					this.ReadPartialUTF8Text(true, this.ReadUInt31());
				}
				return true;
			case XmlBinaryNodeType.Bytes8Text:
				if (this.buffered)
				{
					this.ReadBinaryText(base.MoveToComplexText(), this.ReadUInt8());
				}
				else
				{
					this.ReadPartialBinaryText(false, this.ReadUInt8());
				}
				return true;
			case XmlBinaryNodeType.Bytes8TextWithEndElement:
				if (this.buffered)
				{
					this.ReadBinaryText(this.MoveToAtomicTextWithEndElement(), this.ReadUInt8());
				}
				else
				{
					this.ReadPartialBinaryText(true, this.ReadUInt8());
				}
				return true;
			case XmlBinaryNodeType.Bytes16Text:
				if (this.buffered)
				{
					this.ReadBinaryText(base.MoveToComplexText(), this.ReadUInt16());
				}
				else
				{
					this.ReadPartialBinaryText(false, this.ReadUInt16());
				}
				return true;
			case XmlBinaryNodeType.Bytes16TextWithEndElement:
				if (this.buffered)
				{
					this.ReadBinaryText(this.MoveToAtomicTextWithEndElement(), this.ReadUInt16());
				}
				else
				{
					this.ReadPartialBinaryText(true, this.ReadUInt16());
				}
				return true;
			case XmlBinaryNodeType.Bytes32Text:
				if (this.buffered)
				{
					this.ReadBinaryText(base.MoveToComplexText(), this.ReadUInt31());
				}
				else
				{
					this.ReadPartialBinaryText(false, this.ReadUInt31());
				}
				return true;
			case XmlBinaryNodeType.Bytes32TextWithEndElement:
				if (this.buffered)
				{
					this.ReadBinaryText(this.MoveToAtomicTextWithEndElement(), this.ReadUInt31());
				}
				else
				{
					this.ReadPartialBinaryText(true, this.ReadUInt31());
				}
				return true;
			case XmlBinaryNodeType.EmptyTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue(ValueHandleType.Empty);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.DictionaryTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetDictionaryValue(this.ReadDictionaryKey());
				return true;
			case XmlBinaryNodeType.UniqueIdTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UniqueId, 16);
				return true;
			case XmlBinaryNodeType.TimeSpanTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.TimeSpan, 8);
				return true;
			case XmlBinaryNodeType.GuidTextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Guid, 16);
				return true;
			case XmlBinaryNodeType.UInt64TextWithEndElement:
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UInt64, 8);
				return true;
			case XmlBinaryNodeType.BoolTextWithEndElement:
				this.MoveToAtomicTextWithEndElement().Value.SetValue((this.ReadUInt8() != 0) ? ValueHandleType.True : ValueHandleType.False);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				return true;
			case XmlBinaryNodeType.UnicodeChars8Text:
				this.ReadUnicodeText(false, this.ReadUInt8());
				return true;
			case XmlBinaryNodeType.UnicodeChars8TextWithEndElement:
				this.ReadUnicodeText(true, this.ReadUInt8());
				return true;
			case XmlBinaryNodeType.UnicodeChars16Text:
				this.ReadUnicodeText(false, this.ReadUInt16());
				return true;
			case XmlBinaryNodeType.UnicodeChars16TextWithEndElement:
				this.ReadUnicodeText(true, this.ReadUInt16());
				return true;
			case XmlBinaryNodeType.UnicodeChars32Text:
				this.ReadUnicodeText(false, this.ReadUInt31());
				return true;
			case XmlBinaryNodeType.UnicodeChars32TextWithEndElement:
				this.ReadUnicodeText(true, this.ReadUInt31());
				return true;
			case XmlBinaryNodeType.QNameDictionaryTextWithEndElement:
				base.BufferReader.ReadQName(this.MoveToAtomicTextWithEndElement().Value);
				return true;
			}
			base.BufferReader.ReadValue(nodeType, base.MoveToComplexText().Value);
			return true;
		}

		private void VerifyWhitespace()
		{
			if (!base.Node.Value.IsWhitespace())
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
		}

		private void ReadAttributes()
		{
			XmlBinaryNodeType nodeType = this.GetNodeType();
			if (nodeType < XmlBinaryNodeType.MinAttribute || nodeType > XmlBinaryNodeType.PrefixAttributeZ)
			{
				return;
			}
			this.ReadAttributes2();
		}

		private void ReadAttributes2()
		{
			int num = 0;
			if (this.buffered)
			{
				num = base.BufferReader.Offset;
			}
			for (;;)
			{
				XmlBinaryNodeType nodeType = this.GetNodeType();
				switch (nodeType)
				{
				case XmlBinaryNodeType.MinAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					xmlAttributeNode.Prefix.SetValue(PrefixHandleType.Empty);
					this.ReadName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					continue;
				}
				case XmlBinaryNodeType.Attribute:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					this.ReadName(xmlAttributeNode.Prefix);
					this.ReadName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					base.FixXmlAttribute(xmlAttributeNode);
					continue;
				}
				case XmlBinaryNodeType.ShortDictionaryAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					xmlAttributeNode.Prefix.SetValue(PrefixHandleType.Empty);
					this.ReadDictionaryName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					continue;
				}
				case XmlBinaryNodeType.DictionaryAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					this.ReadName(xmlAttributeNode.Prefix);
					this.ReadDictionaryName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					continue;
				}
				case XmlBinaryNodeType.ShortXmlnsAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.Namespace @namespace = base.AddNamespace();
					@namespace.Prefix.SetValue(PrefixHandleType.Empty);
					this.ReadName(@namespace.Uri);
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddXmlnsAttribute(@namespace);
					continue;
				}
				case XmlBinaryNodeType.XmlnsAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.Namespace @namespace = base.AddNamespace();
					this.ReadName(@namespace.Prefix);
					this.ReadName(@namespace.Uri);
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddXmlnsAttribute(@namespace);
					continue;
				}
				case XmlBinaryNodeType.ShortDictionaryXmlnsAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.Namespace @namespace = base.AddNamespace();
					@namespace.Prefix.SetValue(PrefixHandleType.Empty);
					this.ReadDictionaryName(@namespace.Uri);
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddXmlnsAttribute(@namespace);
					continue;
				}
				case XmlBinaryNodeType.DictionaryXmlnsAttribute:
				{
					this.SkipNodeType();
					XmlBaseReader.Namespace @namespace = base.AddNamespace();
					this.ReadName(@namespace.Prefix);
					this.ReadDictionaryName(@namespace.Uri);
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddXmlnsAttribute(@namespace);
					continue;
				}
				case XmlBinaryNodeType.PrefixDictionaryAttributeA:
				case XmlBinaryNodeType.PrefixDictionaryAttributeB:
				case XmlBinaryNodeType.PrefixDictionaryAttributeC:
				case XmlBinaryNodeType.PrefixDictionaryAttributeD:
				case XmlBinaryNodeType.PrefixDictionaryAttributeE:
				case XmlBinaryNodeType.PrefixDictionaryAttributeF:
				case XmlBinaryNodeType.PrefixDictionaryAttributeG:
				case XmlBinaryNodeType.PrefixDictionaryAttributeH:
				case XmlBinaryNodeType.PrefixDictionaryAttributeI:
				case XmlBinaryNodeType.PrefixDictionaryAttributeJ:
				case XmlBinaryNodeType.PrefixDictionaryAttributeK:
				case XmlBinaryNodeType.PrefixDictionaryAttributeL:
				case XmlBinaryNodeType.PrefixDictionaryAttributeM:
				case XmlBinaryNodeType.PrefixDictionaryAttributeN:
				case XmlBinaryNodeType.PrefixDictionaryAttributeO:
				case XmlBinaryNodeType.PrefixDictionaryAttributeP:
				case XmlBinaryNodeType.PrefixDictionaryAttributeQ:
				case XmlBinaryNodeType.PrefixDictionaryAttributeR:
				case XmlBinaryNodeType.PrefixDictionaryAttributeS:
				case XmlBinaryNodeType.PrefixDictionaryAttributeT:
				case XmlBinaryNodeType.PrefixDictionaryAttributeU:
				case XmlBinaryNodeType.PrefixDictionaryAttributeV:
				case XmlBinaryNodeType.PrefixDictionaryAttributeW:
				case XmlBinaryNodeType.PrefixDictionaryAttributeX:
				case XmlBinaryNodeType.PrefixDictionaryAttributeY:
				case XmlBinaryNodeType.PrefixDictionaryAttributeZ:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					PrefixHandleType prefixHandleType = PrefixHandle.GetAlphaPrefix(nodeType - XmlBinaryNodeType.PrefixDictionaryAttributeA);
					xmlAttributeNode.Prefix.SetValue(prefixHandleType);
					this.ReadDictionaryName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					continue;
				}
				case XmlBinaryNodeType.PrefixAttributeA:
				case XmlBinaryNodeType.PrefixAttributeB:
				case XmlBinaryNodeType.PrefixAttributeC:
				case XmlBinaryNodeType.PrefixAttributeD:
				case XmlBinaryNodeType.PrefixAttributeE:
				case XmlBinaryNodeType.PrefixAttributeF:
				case XmlBinaryNodeType.PrefixAttributeG:
				case XmlBinaryNodeType.PrefixAttributeH:
				case XmlBinaryNodeType.PrefixAttributeI:
				case XmlBinaryNodeType.PrefixAttributeJ:
				case XmlBinaryNodeType.PrefixAttributeK:
				case XmlBinaryNodeType.PrefixAttributeL:
				case XmlBinaryNodeType.PrefixAttributeM:
				case XmlBinaryNodeType.PrefixAttributeN:
				case XmlBinaryNodeType.PrefixAttributeO:
				case XmlBinaryNodeType.PrefixAttributeP:
				case XmlBinaryNodeType.PrefixAttributeQ:
				case XmlBinaryNodeType.PrefixAttributeR:
				case XmlBinaryNodeType.PrefixAttributeS:
				case XmlBinaryNodeType.PrefixAttributeT:
				case XmlBinaryNodeType.PrefixAttributeU:
				case XmlBinaryNodeType.PrefixAttributeV:
				case XmlBinaryNodeType.PrefixAttributeW:
				case XmlBinaryNodeType.PrefixAttributeX:
				case XmlBinaryNodeType.PrefixAttributeY:
				case XmlBinaryNodeType.PrefixAttributeZ:
				{
					this.SkipNodeType();
					XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
					PrefixHandleType prefixHandleType = PrefixHandle.GetAlphaPrefix(nodeType - XmlBinaryNodeType.PrefixAttributeA);
					xmlAttributeNode.Prefix.SetValue(prefixHandleType);
					this.ReadName(xmlAttributeNode.LocalName);
					this.ReadAttributeText(xmlAttributeNode.AttributeText);
					continue;
				}
				}
				break;
			}
			if (this.buffered && base.BufferReader.Offset - num > this.maxBytesPerRead)
			{
				XmlExceptionHelper.ThrowMaxBytesPerReadExceeded(this, this.maxBytesPerRead);
			}
			base.ProcessAttributes();
		}

		private void ReadText(XmlBaseReader.XmlTextNode textNode, ValueHandleType type, int length)
		{
			int num = base.BufferReader.ReadBytes(length);
			textNode.Value.SetValue(type, num, length);
			if (base.OutsideRootElement)
			{
				this.VerifyWhitespace();
			}
		}

		private void ReadBinaryText(XmlBaseReader.XmlTextNode textNode, int length)
		{
			this.ReadText(textNode, ValueHandleType.Base64, length);
		}

		private void ReadPartialUTF8Text(bool withEndElement, int length)
		{
			int num = Math.Max(this.maxBytesPerRead - 5, 0);
			if (length > num)
			{
				int num2 = Math.Max(num - 5, 0);
				int num3 = base.BufferReader.ReadBytes(num2);
				int i;
				for (i = num3 + num2 - 1; i >= num3; i--)
				{
					byte @byte = base.BufferReader.GetByte(i);
					if ((@byte & 128) == 0 || (@byte & 192) == 192)
					{
						break;
					}
				}
				int num4 = num3 + num2 - i;
				base.BufferReader.Offset = base.BufferReader.Offset - num4;
				num2 -= num4;
				base.MoveToComplexText().Value.SetValue(ValueHandleType.UTF8, num3, num2);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				XmlBinaryNodeType xmlBinaryNodeType = (withEndElement ? XmlBinaryNodeType.Chars32TextWithEndElement : XmlBinaryNodeType.Chars32Text);
				this.InsertNode(xmlBinaryNodeType, length - num2);
				return;
			}
			if (withEndElement)
			{
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.UTF8, length);
				return;
			}
			this.ReadText(base.MoveToComplexText(), ValueHandleType.UTF8, length);
		}

		private void ReadUnicodeText(bool withEndElement, int length)
		{
			if ((length & 1) != 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			if (!this.buffered)
			{
				this.ReadPartialUnicodeText(withEndElement, length);
				return;
			}
			if (withEndElement)
			{
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Unicode, length);
				return;
			}
			this.ReadText(base.MoveToComplexText(), ValueHandleType.Unicode, length);
		}

		private void ReadPartialUnicodeText(bool withEndElement, int length)
		{
			int num = Math.Max(this.maxBytesPerRead - 5, 0);
			if (length > num)
			{
				int num2 = Math.Max(num - 5, 0);
				if ((num2 & 1) != 0)
				{
					num2--;
				}
				int num3 = base.BufferReader.ReadBytes(num2);
				int num4 = 0;
				char c = (char)base.BufferReader.GetInt16(num3 + num2 - 2);
				if (c >= '\ud800' && c < '\udc00')
				{
					num4 = 2;
				}
				base.BufferReader.Offset = base.BufferReader.Offset - num4;
				num2 -= num4;
				base.MoveToComplexText().Value.SetValue(ValueHandleType.Unicode, num3, num2);
				if (base.OutsideRootElement)
				{
					this.VerifyWhitespace();
				}
				XmlBinaryNodeType xmlBinaryNodeType = (withEndElement ? XmlBinaryNodeType.UnicodeChars32TextWithEndElement : XmlBinaryNodeType.UnicodeChars32Text);
				this.InsertNode(xmlBinaryNodeType, length - num2);
				return;
			}
			if (withEndElement)
			{
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Unicode, length);
				return;
			}
			this.ReadText(base.MoveToComplexText(), ValueHandleType.Unicode, length);
		}

		private void ReadPartialBinaryText(bool withEndElement, int length)
		{
			int num = Math.Max(this.maxBytesPerRead - 5, 0);
			if (length > num)
			{
				int num2 = num;
				if (num2 > 3)
				{
					num2 -= num2 % 3;
				}
				this.ReadText(base.MoveToComplexText(), ValueHandleType.Base64, num2);
				XmlBinaryNodeType xmlBinaryNodeType = (withEndElement ? XmlBinaryNodeType.Bytes32TextWithEndElement : XmlBinaryNodeType.Bytes32Text);
				this.InsertNode(xmlBinaryNodeType, length - num2);
				return;
			}
			if (withEndElement)
			{
				this.ReadText(this.MoveToAtomicTextWithEndElement(), ValueHandleType.Base64, length);
				return;
			}
			this.ReadText(base.MoveToComplexText(), ValueHandleType.Base64, length);
		}

		private void InsertNode(XmlBinaryNodeType nodeType, int length)
		{
			byte[] array = new byte[5];
			array[0] = (byte)nodeType;
			array[1] = (byte)length;
			length >>= 8;
			array[2] = (byte)length;
			length >>= 8;
			array[3] = (byte)length;
			length >>= 8;
			array[4] = (byte)length;
			base.BufferReader.InsertBytes(array, 0, array.Length);
		}

		private void ReadAttributeText(XmlBaseReader.XmlAttributeTextNode textNode)
		{
			XmlBinaryNodeType nodeType = this.GetNodeType();
			this.SkipNodeType();
			base.BufferReader.ReadValue(nodeType, textNode.Value);
		}

		private void ReadName(ValueHandle value)
		{
			int num = this.ReadMultiByteUInt31();
			int num2 = base.BufferReader.ReadBytes(num);
			value.SetValue(ValueHandleType.UTF8, num2, num);
		}

		private void ReadName(StringHandle handle)
		{
			int num = this.ReadMultiByteUInt31();
			int num2 = base.BufferReader.ReadBytes(num);
			handle.SetValue(num2, num);
		}

		private void ReadName(PrefixHandle prefix)
		{
			int num = this.ReadMultiByteUInt31();
			int num2 = base.BufferReader.ReadBytes(num);
			prefix.SetValue(num2, num);
		}

		private void ReadDictionaryName(StringHandle s)
		{
			int num = this.ReadDictionaryKey();
			s.SetValue(num);
		}

		private XmlBinaryNodeType GetNodeType()
		{
			return base.BufferReader.GetNodeType();
		}

		private void SkipNodeType()
		{
			base.BufferReader.SkipNodeType();
		}

		private int ReadDictionaryKey()
		{
			return base.BufferReader.ReadDictionaryKey();
		}

		private int ReadMultiByteUInt31()
		{
			return base.BufferReader.ReadMultiByteUInt31();
		}

		private int ReadUInt8()
		{
			return base.BufferReader.ReadUInt8();
		}

		private int ReadUInt16()
		{
			return base.BufferReader.ReadUInt16();
		}

		private int ReadUInt31()
		{
			return base.BufferReader.ReadUInt31();
		}

		private bool IsValidArrayType(XmlBinaryNodeType nodeType)
		{
			if (nodeType <= XmlBinaryNodeType.TimeSpanTextWithEndElement)
			{
				switch (nodeType)
				{
				case XmlBinaryNodeType.Int16TextWithEndElement:
				case XmlBinaryNodeType.Int32TextWithEndElement:
				case XmlBinaryNodeType.Int64TextWithEndElement:
				case XmlBinaryNodeType.FloatTextWithEndElement:
				case XmlBinaryNodeType.DoubleTextWithEndElement:
				case XmlBinaryNodeType.DecimalTextWithEndElement:
				case XmlBinaryNodeType.DateTimeTextWithEndElement:
					break;
				case XmlBinaryNodeType.Int32Text:
				case XmlBinaryNodeType.Int64Text:
				case XmlBinaryNodeType.FloatText:
				case XmlBinaryNodeType.DoubleText:
				case XmlBinaryNodeType.DecimalText:
				case XmlBinaryNodeType.DateTimeText:
					return false;
				default:
					if (nodeType != XmlBinaryNodeType.TimeSpanTextWithEndElement)
					{
						return false;
					}
					break;
				}
			}
			else if (nodeType != XmlBinaryNodeType.GuidTextWithEndElement && nodeType != XmlBinaryNodeType.BoolTextWithEndElement)
			{
				return false;
			}
			return true;
		}

		private void ReadArray()
		{
			if (this.GetNodeType() == XmlBinaryNodeType.Array)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			this.ReadNode();
			if (base.Node.NodeType != XmlNodeType.Element)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			if (this.GetNodeType() == XmlBinaryNodeType.Array)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			this.ReadNode();
			if (base.Node.NodeType != XmlNodeType.EndElement)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			this.arrayState = XmlBinaryReader.ArrayState.Element;
			this.arrayNodeType = this.GetNodeType();
			if (!this.IsValidArrayType(this.arrayNodeType))
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			this.SkipNodeType();
			this.arrayCount = this.ReadMultiByteUInt31();
			if (this.arrayCount == 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			this.MoveToArrayElement();
		}

		private void MoveToArrayElement()
		{
			this.arrayState = XmlBinaryReader.ArrayState.Element;
			base.MoveToNode(base.ElementNode);
		}

		private void SkipArrayElements(int count)
		{
			this.arrayCount -= count;
			if (this.arrayCount == 0)
			{
				this.arrayState = XmlBinaryReader.ArrayState.None;
				base.ExitScope();
				this.ReadNode();
			}
		}

		public override bool IsStartArray(out Type type)
		{
			type = null;
			if (this.arrayState != XmlBinaryReader.ArrayState.Element)
			{
				return false;
			}
			XmlBinaryNodeType xmlBinaryNodeType = this.arrayNodeType;
			switch (xmlBinaryNodeType)
			{
			case XmlBinaryNodeType.Int16TextWithEndElement:
				type = typeof(short);
				return true;
			case XmlBinaryNodeType.Int32Text:
			case XmlBinaryNodeType.Int64Text:
			case XmlBinaryNodeType.FloatText:
			case XmlBinaryNodeType.DoubleText:
			case XmlBinaryNodeType.DecimalText:
			case XmlBinaryNodeType.DateTimeText:
				break;
			case XmlBinaryNodeType.Int32TextWithEndElement:
				type = typeof(int);
				return true;
			case XmlBinaryNodeType.Int64TextWithEndElement:
				type = typeof(long);
				return true;
			case XmlBinaryNodeType.FloatTextWithEndElement:
				type = typeof(float);
				return true;
			case XmlBinaryNodeType.DoubleTextWithEndElement:
				type = typeof(double);
				return true;
			case XmlBinaryNodeType.DecimalTextWithEndElement:
				type = typeof(decimal);
				return true;
			case XmlBinaryNodeType.DateTimeTextWithEndElement:
				type = typeof(DateTime);
				return true;
			default:
				switch (xmlBinaryNodeType)
				{
				case XmlBinaryNodeType.UniqueIdTextWithEndElement:
					type = typeof(UniqueId);
					return true;
				case XmlBinaryNodeType.TimeSpanText:
				case XmlBinaryNodeType.GuidText:
					break;
				case XmlBinaryNodeType.TimeSpanTextWithEndElement:
					type = typeof(TimeSpan);
					return true;
				case XmlBinaryNodeType.GuidTextWithEndElement:
					type = typeof(Guid);
					return true;
				default:
					if (xmlBinaryNodeType == XmlBinaryNodeType.BoolTextWithEndElement)
					{
						type = typeof(bool);
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}

		public override bool TryGetArrayLength(out int count)
		{
			count = 0;
			if (!this.buffered)
			{
				return false;
			}
			if (this.arrayState != XmlBinaryReader.ArrayState.Element)
			{
				return false;
			}
			count = this.arrayCount;
			return true;
		}

		private bool IsStartArray(string localName, string namespaceUri, XmlBinaryNodeType nodeType)
		{
			return this.IsStartElement(localName, namespaceUri) && this.arrayState == XmlBinaryReader.ArrayState.Element && this.arrayNodeType == nodeType && !base.Signing;
		}

		private bool IsStartArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, XmlBinaryNodeType nodeType)
		{
			return this.IsStartElement(localName, namespaceUri) && this.arrayState == XmlBinaryReader.ArrayState.Element && this.arrayNodeType == nodeType && !base.Signing;
		}

		private void CheckArray(Array array, int offset, int count)
		{
			if (array == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("array"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > array.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { array.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > array.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { array.Length - offset })));
			}
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(bool[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (bool* ptr = &array[offset])
			{
				bool* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.BoolTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.BoolTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(short[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (short* ptr = &array[offset])
			{
				short* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, short[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int16TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int16TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(int[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (int* ptr = &array[offset])
			{
				int* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, int[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int32TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int32TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(long[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (long* ptr = &array[offset])
			{
				long* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, long[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int64TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.Int64TextWithEndElement) && BitConverter.IsLittleEndian)
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(float[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (float* ptr = &array[offset])
			{
				float* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, float[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.FloatTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.FloatTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(double[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (double* ptr = &array[offset])
			{
				double* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + num));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, double[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DoubleTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DoubleTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe int ReadArray(decimal[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			fixed (decimal* ptr = &array[offset])
			{
				decimal* ptr2 = ptr;
				base.BufferReader.UnsafeReadArray((byte*)ptr2, (byte*)(ptr2 + (IntPtr)num * 16 / (IntPtr)sizeof(decimal)));
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DecimalTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DecimalTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		private int ReadArray(DateTime[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			for (int i = 0; i < num; i++)
			{
				array[offset + i] = base.BufferReader.ReadDateTime();
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DateTimeTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.DateTimeTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		private int ReadArray(Guid[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			for (int i = 0; i < num; i++)
			{
				array[offset + i] = base.BufferReader.ReadGuid();
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.GuidTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.GuidTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		private int ReadArray(TimeSpan[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = Math.Min(count, this.arrayCount);
			for (int i = 0; i < num; i++)
			{
				array[offset + i] = base.BufferReader.ReadTimeSpan();
			}
			this.SkipArrayElements(num);
			return num;
		}

		public override int ReadArray(string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.TimeSpanTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		public override int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			if (this.IsStartArray(localName, namespaceUri, XmlBinaryNodeType.TimeSpanTextWithEndElement))
			{
				return this.ReadArray(array, offset, count);
			}
			return base.ReadArray(localName, namespaceUri, array, offset, count);
		}

		protected override XmlSigningNodeWriter CreateSigningNodeWriter()
		{
			return new XmlSigningNodeWriter(false);
		}

		private bool isTextWithEndElement;

		private bool buffered;

		private XmlBinaryReader.ArrayState arrayState;

		private int arrayCount;

		private int maxBytesPerRead;

		private XmlBinaryNodeType arrayNodeType;

		private OnXmlDictionaryReaderClose onClose;

		private enum ArrayState
		{
			None,
			Element,
			Content
		}
	}
}
