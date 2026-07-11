using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class ReadContentAsBinaryHelper
	{
		internal ReadContentAsBinaryHelper(XmlReader reader)
		{
			this.reader = reader;
			this.canReadValueChunk = reader.CanReadValueChunk;
			if (this.canReadValueChunk)
			{
				this.valueChunk = new char[256];
			}
		}

		internal static ReadContentAsBinaryHelper CreateOrReset(ReadContentAsBinaryHelper helper, XmlReader reader)
		{
			if (helper == null)
			{
				return new ReadContentAsBinaryHelper(reader);
			}
			helper.Reset();
			return helper;
		}

		internal int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
				if (!this.reader.CanReadContentAs())
				{
					throw this.reader.CreateReadContentAsException("ReadContentAsBase64");
				}
				if (!this.Init())
				{
					return 0;
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadContent:
				if (this.decoder == this.base64Decoder)
				{
					return this.ReadContentAsBinary(buffer, index, count);
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			default:
				return 0;
			}
			this.InitBase64Decoder();
			return this.ReadContentAsBinary(buffer, index, count);
		}

		internal int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
				if (!this.reader.CanReadContentAs())
				{
					throw this.reader.CreateReadContentAsException("ReadContentAsBinHex");
				}
				if (!this.Init())
				{
					return 0;
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadContent:
				if (this.decoder == this.binHexDecoder)
				{
					return this.ReadContentAsBinary(buffer, index, count);
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			default:
				return 0;
			}
			this.InitBinHexDecoder();
			return this.ReadContentAsBinary(buffer, index, count);
		}

		internal int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
				if (this.reader.NodeType != XmlNodeType.Element)
				{
					throw this.reader.CreateReadElementContentAsException("ReadElementContentAsBase64");
				}
				if (!this.InitOnElement())
				{
					return 0;
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				if (this.decoder == this.base64Decoder)
				{
					return this.ReadElementContentAsBinary(buffer, index, count);
				}
				break;
			default:
				return 0;
			}
			this.InitBase64Decoder();
			return this.ReadElementContentAsBinary(buffer, index, count);
		}

		internal int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
				if (this.reader.NodeType != XmlNodeType.Element)
				{
					throw this.reader.CreateReadElementContentAsException("ReadElementContentAsBinHex");
				}
				if (!this.InitOnElement())
				{
					return 0;
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				if (this.decoder == this.binHexDecoder)
				{
					return this.ReadElementContentAsBinary(buffer, index, count);
				}
				break;
			default:
				return 0;
			}
			this.InitBinHexDecoder();
			return this.ReadElementContentAsBinary(buffer, index, count);
		}

		internal void Finish()
		{
			if (this.state != ReadContentAsBinaryHelper.State.None)
			{
				while (this.MoveToNextContentNode(true))
				{
				}
				if (this.state == ReadContentAsBinaryHelper.State.InReadElementContent)
				{
					if (this.reader.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
					}
					this.reader.Read();
				}
			}
			this.Reset();
		}

		internal void Reset()
		{
			this.state = ReadContentAsBinaryHelper.State.None;
			this.isEnd = false;
			this.valueOffset = 0;
		}

		private bool Init()
		{
			if (!this.MoveToNextContentNode(false))
			{
				return false;
			}
			this.state = ReadContentAsBinaryHelper.State.InReadContent;
			this.isEnd = false;
			return true;
		}

		private bool InitOnElement()
		{
			bool isEmptyElement = this.reader.IsEmptyElement;
			this.reader.Read();
			if (isEmptyElement)
			{
				return false;
			}
			if (this.MoveToNextContentNode(false))
			{
				this.state = ReadContentAsBinaryHelper.State.InReadElementContent;
				this.isEnd = false;
				return true;
			}
			if (this.reader.NodeType != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
			}
			this.reader.Read();
			return false;
		}

		private void InitBase64Decoder()
		{
			if (this.base64Decoder == null)
			{
				this.base64Decoder = new Base64Decoder();
			}
			else
			{
				this.base64Decoder.Reset();
			}
			this.decoder = this.base64Decoder;
		}

		private void InitBinHexDecoder()
		{
			if (this.binHexDecoder == null)
			{
				this.binHexDecoder = new BinHexDecoder();
			}
			else
			{
				this.binHexDecoder.Reset();
			}
			this.decoder = this.binHexDecoder;
		}

		private int ReadContentAsBinary(byte[] buffer, int index, int count)
		{
			if (this.isEnd)
			{
				this.Reset();
				return 0;
			}
			this.decoder.SetNextOutputBuffer(buffer, index, count);
			for (;;)
			{
				if (this.canReadValueChunk)
				{
					for (;;)
					{
						if (this.valueOffset < this.valueChunkLength)
						{
							int num = this.decoder.Decode(this.valueChunk, this.valueOffset, this.valueChunkLength - this.valueOffset);
							this.valueOffset += num;
						}
						if (this.decoder.IsFull)
						{
							goto Block_3;
						}
						if ((this.valueChunkLength = this.reader.ReadValueChunk(this.valueChunk, 0, 256)) == 0)
						{
							break;
						}
						this.valueOffset = 0;
					}
				}
				else
				{
					string value = this.reader.Value;
					int num2 = this.decoder.Decode(value, this.valueOffset, value.Length - this.valueOffset);
					this.valueOffset += num2;
					if (this.decoder.IsFull)
					{
						goto Block_5;
					}
				}
				this.valueOffset = 0;
				if (!this.MoveToNextContentNode(true))
				{
					goto Block_6;
				}
			}
			Block_3:
			return this.decoder.DecodedCount;
			Block_5:
			return this.decoder.DecodedCount;
			Block_6:
			this.isEnd = true;
			return this.decoder.DecodedCount;
		}

		private int ReadElementContentAsBinary(byte[] buffer, int index, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			int num = this.ReadContentAsBinary(buffer, index, count);
			if (num > 0)
			{
				return num;
			}
			if (this.reader.NodeType != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
			}
			this.reader.Read();
			this.state = ReadContentAsBinaryHelper.State.None;
			return 0;
		}

		private bool MoveToNextContentNode(bool moveIfOnContentNode)
		{
			for (;;)
			{
				switch (this.reader.NodeType)
				{
				case XmlNodeType.Attribute:
					goto IL_0052;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if (!moveIfOnContentNode)
					{
						return true;
					}
					goto IL_0078;
				case XmlNodeType.EntityReference:
					if (this.reader.CanResolveEntity)
					{
						this.reader.ResolveEntity();
						goto IL_0078;
					}
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.EndEntity:
					goto IL_0078;
				}
				break;
				IL_0078:
				moveIfOnContentNode = false;
				if (!this.reader.Read())
				{
					return false;
				}
			}
			return false;
			IL_0052:
			return !moveIfOnContentNode;
		}

		internal async Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
			{
				if (!this.reader.CanReadContentAs())
				{
					throw this.reader.CreateReadContentAsException("ReadContentAsBase64");
				}
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.InitAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					return 0;
				}
				break;
			}
			case ReadContentAsBinaryHelper.State.InReadContent:
				if (this.decoder == this.base64Decoder)
				{
					return await this.ReadContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			default:
				return 0;
			}
			this.InitBase64Decoder();
			return await this.ReadContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
		}

		internal async Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
			{
				if (!this.reader.CanReadContentAs())
				{
					throw this.reader.CreateReadContentAsException("ReadContentAsBinHex");
				}
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.InitAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					return 0;
				}
				break;
			}
			case ReadContentAsBinaryHelper.State.InReadContent:
				if (this.decoder == this.binHexDecoder)
				{
					return await this.ReadContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
				}
				break;
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			default:
				return 0;
			}
			this.InitBinHexDecoder();
			return await this.ReadContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
		}

		internal async Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
			{
				if (this.reader.NodeType != XmlNodeType.Element)
				{
					throw this.reader.CreateReadElementContentAsException("ReadElementContentAsBase64");
				}
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.InitOnElementAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					return 0;
				}
				break;
			}
			case ReadContentAsBinaryHelper.State.InReadContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				if (this.decoder == this.base64Decoder)
				{
					return await this.ReadElementContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
				}
				break;
			default:
				return 0;
			}
			this.InitBase64Decoder();
			return await this.ReadElementContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
		}

		internal async Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (this.state)
			{
			case ReadContentAsBinaryHelper.State.None:
			{
				if (this.reader.NodeType != XmlNodeType.Element)
				{
					throw this.reader.CreateReadElementContentAsException("ReadElementContentAsBinHex");
				}
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.InitOnElementAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					return 0;
				}
				break;
			}
			case ReadContentAsBinaryHelper.State.InReadContent:
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
			case ReadContentAsBinaryHelper.State.InReadElementContent:
				if (this.decoder == this.binHexDecoder)
				{
					return await this.ReadElementContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
				}
				break;
			default:
				return 0;
			}
			this.InitBinHexDecoder();
			return await this.ReadElementContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
		}

		internal async Task FinishAsync()
		{
			if (this.state != ReadContentAsBinaryHelper.State.None)
			{
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter;
				do
				{
					configuredTaskAwaiter = this.MoveToNextContentNodeAsync(true).ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
					}
				}
				while (configuredTaskAwaiter.GetResult());
				if (this.state == ReadContentAsBinaryHelper.State.InReadElementContent)
				{
					if (this.reader.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
					}
					await this.reader.ReadAsync().ConfigureAwait(false);
				}
			}
			this.Reset();
		}

		private async Task<bool> InitAsync()
		{
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.MoveToNextContentNodeAsync(false).ConfigureAwait(false).GetAwaiter();
			if (!configuredTaskAwaiter.IsCompleted)
			{
				await configuredTaskAwaiter;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
				configuredTaskAwaiter = configuredTaskAwaiter2;
				configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
			}
			bool flag;
			if (!configuredTaskAwaiter.GetResult())
			{
				flag = false;
			}
			else
			{
				this.state = ReadContentAsBinaryHelper.State.InReadContent;
				this.isEnd = false;
				flag = true;
			}
			return flag;
		}

		private async Task<bool> InitOnElementAsync()
		{
			bool isEmpty = this.reader.IsEmptyElement;
			await this.reader.ReadAsync().ConfigureAwait(false);
			bool flag;
			if (isEmpty)
			{
				flag = false;
			}
			else
			{
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.MoveToNextContentNodeAsync(false).ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					if (this.reader.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
					}
					await this.reader.ReadAsync().ConfigureAwait(false);
					flag = false;
				}
				else
				{
					this.state = ReadContentAsBinaryHelper.State.InReadElementContent;
					this.isEnd = false;
					flag = true;
				}
			}
			return flag;
		}

		private async Task<int> ReadContentAsBinaryAsync(byte[] buffer, int index, int count)
		{
			int num;
			if (this.isEnd)
			{
				this.Reset();
				num = 0;
			}
			else
			{
				this.decoder.SetNextOutputBuffer(buffer, index, count);
				for (;;)
				{
					if (this.canReadValueChunk)
					{
						for (;;)
						{
							if (this.valueOffset < this.valueChunkLength)
							{
								int num2 = this.decoder.Decode(this.valueChunk, this.valueOffset, this.valueChunkLength - this.valueOffset);
								this.valueOffset += num2;
							}
							if (this.decoder.IsFull)
							{
								goto Block_3;
							}
							int num3 = await this.reader.ReadValueChunkAsync(this.valueChunk, 0, 256).ConfigureAwait(false);
							int num4 = num3;
							this.valueChunkLength = num4;
							if (num4 == 0)
							{
								break;
							}
							this.valueOffset = 0;
						}
					}
					else
					{
						string text = await this.reader.GetValueAsync().ConfigureAwait(false);
						int num5 = this.decoder.Decode(text, this.valueOffset, text.Length - this.valueOffset);
						this.valueOffset += num5;
						if (this.decoder.IsFull)
						{
							goto Block_5;
						}
					}
					this.valueOffset = 0;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.MoveToNextContentNodeAsync(true).ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
					}
					if (!configuredTaskAwaiter.GetResult())
					{
						goto Block_7;
					}
				}
				Block_3:
				return this.decoder.DecodedCount;
				Block_5:
				return this.decoder.DecodedCount;
				Block_7:
				this.isEnd = true;
				num = this.decoder.DecodedCount;
			}
			return num;
		}

		private async Task<int> ReadElementContentAsBinaryAsync(byte[] buffer, int index, int count)
		{
			int num;
			if (count == 0)
			{
				num = 0;
			}
			else
			{
				int num2 = await this.ReadContentAsBinaryAsync(buffer, index, count).ConfigureAwait(false);
				if (num2 > 0)
				{
					num = num2;
				}
				else
				{
					if (this.reader.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException("'{0}' is an invalid XmlNodeType.", this.reader.NodeType.ToString(), this.reader as IXmlLineInfo);
					}
					await this.reader.ReadAsync().ConfigureAwait(false);
					this.state = ReadContentAsBinaryHelper.State.None;
					num = 0;
				}
			}
			return num;
		}

		private async Task<bool> MoveToNextContentNodeAsync(bool moveIfOnContentNode)
		{
			for (;;)
			{
				switch (this.reader.NodeType)
				{
				case XmlNodeType.Attribute:
					goto IL_0066;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if (!moveIfOnContentNode)
					{
						goto Block_1;
					}
					goto IL_00A5;
				case XmlNodeType.EntityReference:
					if (this.reader.CanResolveEntity)
					{
						this.reader.ResolveEntity();
						goto IL_00A5;
					}
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.EndEntity:
					goto IL_00A5;
				}
				break;
				IL_00A5:
				moveIfOnContentNode = false;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.reader.ReadAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					goto Block_4;
				}
			}
			goto IL_009E;
			IL_0066:
			return !moveIfOnContentNode;
			Block_1:
			return true;
			IL_009E:
			return false;
			Block_4:
			return false;
		}

		private XmlReader reader;

		private ReadContentAsBinaryHelper.State state;

		private int valueOffset;

		private bool isEnd;

		private bool canReadValueChunk;

		private char[] valueChunk;

		private int valueChunkLength;

		private IncrementalReadDecoder decoder;

		private Base64Decoder base64Decoder;

		private BinHexDecoder binHexDecoder;

		private const int ChunkSize = 256;

		private enum State
		{
			None,
			InReadContent,
			InReadElementContent
		}
	}
}
