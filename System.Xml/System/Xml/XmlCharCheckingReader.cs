using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlCharCheckingReader : XmlWrappingReader
	{
		internal XmlCharCheckingReader(XmlReader reader, bool checkCharacters, bool ignoreWhitespace, bool ignoreComments, bool ignorePis, DtdProcessing dtdProcessing)
			: base(reader)
		{
			this.state = XmlCharCheckingReader.State.Initial;
			this.checkCharacters = checkCharacters;
			this.ignoreWhitespace = ignoreWhitespace;
			this.ignoreComments = ignoreComments;
			this.ignorePis = ignorePis;
			this.dtdProcessing = dtdProcessing;
			this.lastNodeType = XmlNodeType.None;
			if (checkCharacters)
			{
				this.xmlCharType = XmlCharType.Instance;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = this.reader.Settings;
				if (xmlReaderSettings == null)
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				else
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				if (this.checkCharacters)
				{
					xmlReaderSettings.CheckCharacters = true;
				}
				if (this.ignoreWhitespace)
				{
					xmlReaderSettings.IgnoreWhitespace = true;
				}
				if (this.ignoreComments)
				{
					xmlReaderSettings.IgnoreComments = true;
				}
				if (this.ignorePis)
				{
					xmlReaderSettings.IgnoreProcessingInstructions = true;
				}
				if (this.dtdProcessing != (DtdProcessing)(-1))
				{
					xmlReaderSettings.DtdProcessing = this.dtdProcessing;
				}
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override bool MoveToAttribute(string name)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToAttribute(name, ns);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			this.reader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToElement();
		}

		public override bool Read()
		{
			switch (this.state)
			{
			case XmlCharCheckingReader.State.Initial:
				this.state = XmlCharCheckingReader.State.Interactive;
				if (this.reader.ReadState != ReadState.Initial)
				{
					goto IL_0055;
				}
				break;
			case XmlCharCheckingReader.State.InReadBinary:
				this.FinishReadBinary();
				this.state = XmlCharCheckingReader.State.Interactive;
				break;
			case XmlCharCheckingReader.State.Error:
				return false;
			case XmlCharCheckingReader.State.Interactive:
				break;
			default:
				return false;
			}
			if (!this.reader.Read())
			{
				return false;
			}
			IL_0055:
			XmlNodeType nodeType = this.reader.NodeType;
			if (!this.checkCharacters)
			{
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
					if (this.ignorePis)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.Comment:
					if (this.ignoreComments)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.DocumentType:
					if (this.dtdProcessing == DtdProcessing.Prohibit)
					{
						this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
					}
					else if (this.dtdProcessing == DtdProcessing.Ignore)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.Whitespace:
					if (this.ignoreWhitespace)
					{
						return this.Read();
					}
					break;
				}
				return true;
			}
			switch (nodeType)
			{
			case XmlNodeType.Element:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
					if (this.reader.MoveToFirstAttribute())
					{
						do
						{
							this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
							this.CheckCharacters(this.reader.Value);
						}
						while (this.reader.MoveToNextAttribute());
						this.reader.MoveToElement();
					}
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				if (this.checkCharacters)
				{
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.EntityReference:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
				}
				break;
			case XmlNodeType.ProcessingInstruction:
				if (this.ignorePis)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.Comment:
				if (this.ignoreComments)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.DocumentType:
				if (this.dtdProcessing == DtdProcessing.Prohibit)
				{
					this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
				}
				else if (this.dtdProcessing == DtdProcessing.Ignore)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
					this.CheckCharacters(this.reader.Value);
					string text = this.reader.GetAttribute("SYSTEM");
					if (text != null)
					{
						this.CheckCharacters(text);
					}
					text = this.reader.GetAttribute("PUBLIC");
					int num;
					if (text != null && (num = this.xmlCharType.IsPublicId(text)) >= 0)
					{
						this.Throw("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(text, num));
					}
				}
				break;
			case XmlNodeType.Whitespace:
				if (this.ignoreWhitespace)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.CheckWhitespace(this.reader.Value);
				}
				break;
			case XmlNodeType.SignificantWhitespace:
				if (this.checkCharacters)
				{
					this.CheckWhitespace(this.reader.Value);
				}
				break;
			case XmlNodeType.EndElement:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
				}
				break;
			}
			this.lastNodeType = nodeType;
			return true;
		}

		public override ReadState ReadState
		{
			get
			{
				switch (this.state)
				{
				case XmlCharCheckingReader.State.Initial:
					if (this.reader.ReadState != ReadState.Closed)
					{
						return ReadState.Initial;
					}
					return ReadState.Closed;
				case XmlCharCheckingReader.State.Error:
					return ReadState.Error;
				}
				return this.reader.ReadState;
			}
		}

		public override bool ReadAttributeValue()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.ReadAttributeValue();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadContentAsBase64(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadContentAsBase64(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int num = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadContentAsBinHex(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadContentAsBinHex(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int num = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return num;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
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
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadElementContentAsBase64(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadElementContentAsBase64(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int num = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return num;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
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
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadElementContentAsBinHex(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadElementContentAsBinHex(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int num = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return num;
		}

		private void Throw(string res, string arg)
		{
			this.state = XmlCharCheckingReader.State.Error;
			throw new XmlException(res, arg, null);
		}

		private void Throw(string res, string[] args)
		{
			this.state = XmlCharCheckingReader.State.Error;
			throw new XmlException(res, args, null);
		}

		private void CheckWhitespace(string value)
		{
			int num;
			if ((num = this.xmlCharType.IsOnlyWhitespaceWithPos(value)) != -1)
			{
				this.Throw("The Whitespace or SignificantWhitespace node can contain only XML white space characters. '{0}' is not an XML white space character.", XmlException.BuildCharExceptionArgs(value, num));
			}
		}

		private void ValidateQName(string name)
		{
			string text;
			string text2;
			ValidateNames.ParseQNameThrow(name, out text, out text2);
		}

		private void ValidateQName(string prefix, string localName)
		{
			try
			{
				if (prefix.Length > 0)
				{
					ValidateNames.ParseNCNameThrow(prefix);
				}
				ValidateNames.ParseNCNameThrow(localName);
			}
			catch
			{
				this.state = XmlCharCheckingReader.State.Error;
				throw;
			}
		}

		private void CheckCharacters(string value)
		{
			XmlConvert.VerifyCharData(value, ExceptionType.ArgumentException, ExceptionType.XmlException);
		}

		private void FinishReadBinary()
		{
			this.state = XmlCharCheckingReader.State.Interactive;
			if (this.readBinaryHelper != null)
			{
				this.readBinaryHelper.Finish();
			}
		}

		public override async Task<bool> ReadAsync()
		{
			switch (this.state)
			{
			case XmlCharCheckingReader.State.Initial:
				this.state = XmlCharCheckingReader.State.Interactive;
				if (this.reader.ReadState != ReadState.Initial)
				{
					goto IL_0176;
				}
				break;
			case XmlCharCheckingReader.State.InReadBinary:
				await this.FinishReadBinaryAsync().ConfigureAwait(false);
				this.state = XmlCharCheckingReader.State.Interactive;
				break;
			case XmlCharCheckingReader.State.Error:
				return false;
			case XmlCharCheckingReader.State.Interactive:
				break;
			default:
				return false;
			}
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
				return false;
			}
			IL_0176:
			XmlNodeType nodeType = this.reader.NodeType;
			bool flag;
			if (!this.checkCharacters)
			{
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
					if (this.ignorePis)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					break;
				case XmlNodeType.Comment:
					if (this.ignoreComments)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					break;
				case XmlNodeType.DocumentType:
					if (this.dtdProcessing == DtdProcessing.Prohibit)
					{
						this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
					}
					else if (this.dtdProcessing == DtdProcessing.Ignore)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					break;
				case XmlNodeType.Whitespace:
					if (this.ignoreWhitespace)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					break;
				}
				flag = true;
			}
			else
			{
				switch (nodeType)
				{
				case XmlNodeType.Element:
					if (this.checkCharacters)
					{
						this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
						if (this.reader.MoveToFirstAttribute())
						{
							do
							{
								this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
								this.CheckCharacters(this.reader.Value);
							}
							while (this.reader.MoveToNextAttribute());
							this.reader.MoveToElement();
						}
					}
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					if (this.checkCharacters)
					{
						this.CheckCharacters(await this.reader.GetValueAsync().ConfigureAwait(false));
					}
					break;
				case XmlNodeType.EntityReference:
					if (this.checkCharacters)
					{
						this.ValidateQName(this.reader.Name);
					}
					break;
				case XmlNodeType.ProcessingInstruction:
					if (this.ignorePis)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					if (this.checkCharacters)
					{
						this.ValidateQName(this.reader.Name);
						this.CheckCharacters(this.reader.Value);
					}
					break;
				case XmlNodeType.Comment:
					if (this.ignoreComments)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					if (this.checkCharacters)
					{
						this.CheckCharacters(this.reader.Value);
					}
					break;
				case XmlNodeType.DocumentType:
					if (this.dtdProcessing == DtdProcessing.Prohibit)
					{
						this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
					}
					else if (this.dtdProcessing == DtdProcessing.Ignore)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					if (this.checkCharacters)
					{
						this.ValidateQName(this.reader.Name);
						this.CheckCharacters(this.reader.Value);
						string text = this.reader.GetAttribute("SYSTEM");
						if (text != null)
						{
							this.CheckCharacters(text);
						}
						text = this.reader.GetAttribute("PUBLIC");
						if (text != null)
						{
							int num = this.xmlCharType.IsPublicId(text);
							if (num >= 0)
							{
								this.Throw("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(text, num));
							}
						}
					}
					break;
				case XmlNodeType.Whitespace:
					if (this.ignoreWhitespace)
					{
						return await this.ReadAsync().ConfigureAwait(false);
					}
					if (this.checkCharacters)
					{
						this.CheckWhitespace(await this.reader.GetValueAsync().ConfigureAwait(false));
					}
					break;
				case XmlNodeType.SignificantWhitespace:
					if (this.checkCharacters)
					{
						this.CheckWhitespace(await this.reader.GetValueAsync().ConfigureAwait(false));
					}
					break;
				case XmlNodeType.EndElement:
					if (this.checkCharacters)
					{
						this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
					}
					break;
				}
				this.lastNodeType = nodeType;
				flag = true;
			}
			return flag;
		}

		public override async Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.state != XmlCharCheckingReader.State.InReadBinary)
				{
					if (base.CanReadBinaryContent && !this.checkCharacters)
					{
						this.readBinaryHelper = null;
						this.state = XmlCharCheckingReader.State.InReadBinary;
						return await base.ReadContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
					}
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				}
				else if (this.readBinaryHelper == null)
				{
					return await base.ReadContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				}
				this.state = XmlCharCheckingReader.State.Interactive;
				int num2 = await this.readBinaryHelper.ReadContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.state = XmlCharCheckingReader.State.InReadBinary;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.state != XmlCharCheckingReader.State.InReadBinary)
				{
					if (base.CanReadBinaryContent && !this.checkCharacters)
					{
						this.readBinaryHelper = null;
						this.state = XmlCharCheckingReader.State.InReadBinary;
						return await base.ReadContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
					}
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				}
				else if (this.readBinaryHelper == null)
				{
					return await base.ReadContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				}
				this.state = XmlCharCheckingReader.State.Interactive;
				int num2 = await this.readBinaryHelper.ReadContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.state = XmlCharCheckingReader.State.InReadBinary;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
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
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.state != XmlCharCheckingReader.State.InReadBinary)
				{
					if (base.CanReadBinaryContent && !this.checkCharacters)
					{
						this.readBinaryHelper = null;
						this.state = XmlCharCheckingReader.State.InReadBinary;
						return await base.ReadElementContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
					}
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				}
				else if (this.readBinaryHelper == null)
				{
					return await base.ReadElementContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				}
				this.state = XmlCharCheckingReader.State.Interactive;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.state = XmlCharCheckingReader.State.InReadBinary;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
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
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.state != XmlCharCheckingReader.State.InReadBinary)
				{
					if (base.CanReadBinaryContent && !this.checkCharacters)
					{
						this.readBinaryHelper = null;
						this.state = XmlCharCheckingReader.State.InReadBinary;
						return await base.ReadElementContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
					}
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				}
				else if (this.readBinaryHelper == null)
				{
					return await base.ReadElementContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				}
				this.state = XmlCharCheckingReader.State.Interactive;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.state = XmlCharCheckingReader.State.InReadBinary;
				num = num2;
			}
			return num;
		}

		private async Task FinishReadBinaryAsync()
		{
			this.state = XmlCharCheckingReader.State.Interactive;
			if (this.readBinaryHelper != null)
			{
				await this.readBinaryHelper.FinishAsync().ConfigureAwait(false);
			}
		}

		private XmlCharCheckingReader.State state;

		private bool checkCharacters;

		private bool ignoreWhitespace;

		private bool ignoreComments;

		private bool ignorePis;

		private DtdProcessing dtdProcessing;

		private XmlNodeType lastNodeType;

		private XmlCharType xmlCharType;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private enum State
		{
			Initial,
			InReadBinary,
			Error,
			Interactive
		}
	}
}
