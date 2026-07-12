using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlWriter : IDisposable
	{
		public virtual XmlWriterSettings Settings
		{
			get
			{
				return null;
			}
		}

		public abstract void WriteStartDocument();

		public abstract void WriteStartDocument(bool standalone);

		public abstract void WriteEndDocument();

		public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

		public void WriteStartElement(string localName, string ns)
		{
			this.WriteStartElement(null, localName, ns);
		}

		public abstract void WriteStartElement(string prefix, string localName, string ns);

		public void WriteStartElement(string localName)
		{
			this.WriteStartElement(null, localName, null);
		}

		public abstract void WriteEndElement();

		public abstract void WriteFullEndElement();

		public void WriteAttributeString(string localName, string ns, string value)
		{
			this.WriteStartAttribute(null, localName, ns);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		public void WriteAttributeString(string localName, string value)
		{
			this.WriteStartAttribute(null, localName, null);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		public void WriteAttributeString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartAttribute(prefix, localName, ns);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		public void WriteStartAttribute(string localName, string ns)
		{
			this.WriteStartAttribute(null, localName, ns);
		}

		public abstract void WriteStartAttribute(string prefix, string localName, string ns);

		public void WriteStartAttribute(string localName)
		{
			this.WriteStartAttribute(null, localName, null);
		}

		public abstract void WriteEndAttribute();

		public abstract void WriteCData(string text);

		public abstract void WriteComment(string text);

		public abstract void WriteProcessingInstruction(string name, string text);

		public abstract void WriteEntityRef(string name);

		public abstract void WriteCharEntity(char ch);

		public abstract void WriteWhitespace(string ws);

		public abstract void WriteString(string text);

		public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

		public abstract void WriteChars(char[] buffer, int index, int count);

		public abstract void WriteRaw(char[] buffer, int index, int count);

		public abstract void WriteRaw(string data);

		public abstract void WriteBase64(byte[] buffer, int index, int count);

		public virtual void WriteBinHex(byte[] buffer, int index, int count)
		{
			BinHexEncoder.Encode(buffer, index, count, this);
		}

		public abstract WriteState WriteState { get; }

		public virtual void Close()
		{
		}

		public abstract void Flush();

		public abstract string LookupPrefix(string ns);

		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.Default;
			}
		}

		public virtual string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual void WriteNmToken(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The empty string '' is not a valid name."));
			}
			this.WriteString(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
		}

		public virtual void WriteName(string name)
		{
			this.WriteString(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
		}

		public virtual void WriteQualifiedName(string localName, string ns)
		{
			if (ns != null && ns.Length > 0)
			{
				string text = this.LookupPrefix(ns);
				if (text == null)
				{
					throw new ArgumentException(Res.GetString("The '{0}' namespace is not defined.", new object[] { ns }));
				}
				this.WriteString(text);
				this.WriteString(":");
			}
			this.WriteString(localName);
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.WriteString(XmlUntypedConverter.Untyped.ToString(value, null));
		}

		public virtual void WriteValue(string value)
		{
			if (value == null)
			{
				return;
			}
			this.WriteString(value);
		}

		public virtual void WriteValue(bool value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(DateTime value)
		{
			this.WriteString(XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
		}

		public virtual void WriteValue(DateTimeOffset value)
		{
			if (value.Offset != TimeSpan.Zero)
			{
				this.WriteValue(value.LocalDateTime);
				return;
			}
			this.WriteValue(value.UtcDateTime);
		}

		public virtual void WriteValue(double value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(float value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(decimal value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(int value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(long value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteAttributes(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				if (reader.MoveToFirstAttribute())
				{
					this.WriteAttributes(reader, defattr);
					reader.MoveToElement();
					return;
				}
			}
			else
			{
				if (reader.NodeType != XmlNodeType.Attribute)
				{
					throw new XmlException("The current position on the Reader is neither an element nor an attribute.", string.Empty);
				}
				do
				{
					if (defattr || !reader.IsDefaultInternal)
					{
						this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						while (reader.ReadAttributeValue())
						{
							if (reader.NodeType == XmlNodeType.EntityReference)
							{
								this.WriteEntityRef(reader.Name);
							}
							else
							{
								this.WriteString(reader.Value);
							}
						}
						this.WriteEndAttribute();
					}
				}
				while (reader.MoveToNextAttribute());
			}
		}

		public virtual void WriteNode(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			bool canReadValueChunk = reader.CanReadValueChunk;
			int num = ((reader.NodeType == XmlNodeType.None) ? (-1) : reader.Depth);
			do
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					this.WriteAttributes(reader, defattr);
					if (reader.IsEmptyElement)
					{
						this.WriteEndElement();
					}
					break;
				case XmlNodeType.Text:
					if (canReadValueChunk)
					{
						if (this.writeNodeBuffer == null)
						{
							this.writeNodeBuffer = new char[1024];
						}
						int num2;
						while ((num2 = reader.ReadValueChunk(this.writeNodeBuffer, 0, 1024)) > 0)
						{
							this.WriteChars(this.writeNodeBuffer, 0, num2);
						}
					}
					else
					{
						this.WriteString(reader.Value);
					}
					break;
				case XmlNodeType.CDATA:
					this.WriteCData(reader.Value);
					break;
				case XmlNodeType.EntityReference:
					this.WriteEntityRef(reader.Name);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.XmlDeclaration:
					this.WriteProcessingInstruction(reader.Name, reader.Value);
					break;
				case XmlNodeType.Comment:
					this.WriteComment(reader.Value);
					break;
				case XmlNodeType.DocumentType:
					this.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					this.WriteWhitespace(reader.Value);
					break;
				case XmlNodeType.EndElement:
					this.WriteFullEndElement();
					break;
				}
			}
			while (reader.Read() && (num < reader.Depth || (num == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
		}

		public virtual void WriteNode(XPathNavigator navigator, bool defattr)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			int num = 0;
			navigator = navigator.Clone();
			for (;;)
			{
				IL_0018:
				bool flag = false;
				switch (navigator.NodeType)
				{
				case XPathNodeType.Root:
					flag = true;
					break;
				case XPathNodeType.Element:
					this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
					if (navigator.MoveToFirstAttribute())
					{
						do
						{
							IXmlSchemaInfo schemaInfo = navigator.SchemaInfo;
							if (defattr || schemaInfo == null || !schemaInfo.IsDefault)
							{
								this.WriteStartAttribute(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
								this.WriteString(navigator.Value);
								this.WriteEndAttribute();
							}
						}
						while (navigator.MoveToNextAttribute());
						navigator.MoveToParent();
					}
					if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
					{
						this.WriteLocalNamespaces(navigator);
						navigator.MoveToParent();
					}
					flag = true;
					break;
				case XPathNodeType.Text:
					this.WriteString(navigator.Value);
					break;
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					this.WriteWhitespace(navigator.Value);
					break;
				case XPathNodeType.ProcessingInstruction:
					this.WriteProcessingInstruction(navigator.LocalName, navigator.Value);
					break;
				case XPathNodeType.Comment:
					this.WriteComment(navigator.Value);
					break;
				}
				if (flag)
				{
					if (navigator.MoveToFirstChild())
					{
						num++;
						continue;
					}
					if (navigator.NodeType == XPathNodeType.Element)
					{
						if (navigator.IsEmptyElement)
						{
							this.WriteEndElement();
						}
						else
						{
							this.WriteFullEndElement();
						}
					}
				}
				while (num != 0)
				{
					if (navigator.MoveToNext())
					{
						goto IL_0018;
					}
					num--;
					navigator.MoveToParent();
					if (navigator.NodeType == XPathNodeType.Element)
					{
						this.WriteFullEndElement();
					}
				}
				break;
			}
		}

		public void WriteElementString(string localName, string value)
		{
			this.WriteElementString(localName, null, value);
		}

		public void WriteElementString(string localName, string ns, string value)
		{
			this.WriteStartElement(localName, ns);
			if (value != null && value.Length != 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		public void WriteElementString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartElement(prefix, localName, ns);
			if (value != null && value.Length != 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.WriteState != WriteState.Closed)
			{
				this.Close();
			}
		}

		private void WriteLocalNamespaces(XPathNavigator nsNav)
		{
			string localName = nsNav.LocalName;
			string value = nsNav.Value;
			if (nsNav.MoveToNextNamespace(XPathNamespaceScope.Local))
			{
				this.WriteLocalNamespaces(nsNav);
			}
			if (localName.Length == 0)
			{
				this.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", value);
				return;
			}
			this.WriteAttributeString("xmlns", localName, "http://www.w3.org/2000/xmlns/", value);
		}

		public static XmlWriter Create(string outputFileName)
		{
			return XmlWriter.Create(outputFileName, null);
		}

		public static XmlWriter Create(string outputFileName, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(outputFileName);
		}

		public static XmlWriter Create(Stream output)
		{
			return XmlWriter.Create(output, null);
		}

		public static XmlWriter Create(Stream output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		public static XmlWriter Create(TextWriter output)
		{
			return XmlWriter.Create(output, null);
		}

		public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		public static XmlWriter Create(StringBuilder output)
		{
			return XmlWriter.Create(output, null);
		}

		public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			return settings.CreateWriter(new StringWriter(output, CultureInfo.InvariantCulture));
		}

		public static XmlWriter Create(XmlWriter output)
		{
			return XmlWriter.Create(output, null);
		}

		public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		public virtual Task WriteStartDocumentAsync()
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteStartDocumentAsync(bool standalone)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteEndDocumentAsync()
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteEndElementAsync()
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteFullEndElementAsync()
		{
			throw new NotImplementedException();
		}

		public Task WriteAttributeStringAsync(string prefix, string localName, string ns, string value)
		{
			Task task = this.WriteStartAttributeAsync(prefix, localName, ns);
			if (task.IsSuccess())
			{
				return this.WriteStringAsync(value).CallTaskFuncWhenFinish(new Func<Task>(this.WriteEndAttributeAsync));
			}
			return this.WriteAttributeStringAsyncHelper(task, value);
		}

		private async Task WriteAttributeStringAsyncHelper(Task task, string value)
		{
			await task.ConfigureAwait(false);
			await this.WriteStringAsync(value).ConfigureAwait(false);
			await this.WriteEndAttributeAsync().ConfigureAwait(false);
		}

		protected internal virtual Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			throw new NotImplementedException();
		}

		protected internal virtual Task WriteEndAttributeAsync()
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteCDataAsync(string text)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteCommentAsync(string text)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteProcessingInstructionAsync(string name, string text)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteEntityRefAsync(string name)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteCharEntityAsync(char ch)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteWhitespaceAsync(string ws)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteStringAsync(string text)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteRawAsync(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteRawAsync(string data)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteBinHexAsync(byte[] buffer, int index, int count)
		{
			return BinHexEncoder.EncodeAsync(buffer, index, count, this);
		}

		public virtual Task FlushAsync()
		{
			throw new NotImplementedException();
		}

		public virtual Task WriteNmTokenAsync(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The empty string '' is not a valid name."));
			}
			return this.WriteStringAsync(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
		}

		public virtual Task WriteNameAsync(string name)
		{
			return this.WriteStringAsync(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
		}

		public virtual async Task WriteQualifiedNameAsync(string localName, string ns)
		{
			if (ns != null && ns.Length > 0)
			{
				string text = this.LookupPrefix(ns);
				if (text == null)
				{
					throw new ArgumentException(Res.GetString("The '{0}' namespace is not defined.", new object[] { ns }));
				}
				await this.WriteStringAsync(text).ConfigureAwait(false);
				await this.WriteStringAsync(":").ConfigureAwait(false);
			}
			await this.WriteStringAsync(localName).ConfigureAwait(false);
		}

		public virtual async Task WriteAttributesAsync(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				if (reader.MoveToFirstAttribute())
				{
					await this.WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
					reader.MoveToElement();
				}
			}
			else
			{
				if (reader.NodeType != XmlNodeType.Attribute)
				{
					throw new XmlException("The current position on the Reader is neither an element nor an attribute.", string.Empty);
				}
				do
				{
					if (defattr || !reader.IsDefaultInternal)
					{
						await this.WriteStartAttributeAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
						while (reader.ReadAttributeValue())
						{
							if (reader.NodeType == XmlNodeType.EntityReference)
							{
								await this.WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
							}
							else
							{
								await this.WriteStringAsync(reader.Value).ConfigureAwait(false);
							}
						}
						await this.WriteEndAttributeAsync().ConfigureAwait(false);
					}
				}
				while (reader.MoveToNextAttribute());
			}
		}

		public virtual Task WriteNodeAsync(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.Settings != null && reader.Settings.Async)
			{
				return this.WriteNodeAsync_CallAsyncReader(reader, defattr);
			}
			return this.WriteNodeAsync_CallSyncReader(reader, defattr);
		}

		internal async Task WriteNodeAsync_CallSyncReader(XmlReader reader, bool defattr)
		{
			bool canReadChunk = reader.CanReadValueChunk;
			int d = ((reader.NodeType == XmlNodeType.None) ? (-1) : reader.Depth);
			do
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					await this.WriteStartElementAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
					await this.WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
					if (reader.IsEmptyElement)
					{
						await this.WriteEndElementAsync().ConfigureAwait(false);
					}
					break;
				case XmlNodeType.Text:
					if (canReadChunk)
					{
						if (this.writeNodeBuffer == null)
						{
							this.writeNodeBuffer = new char[1024];
						}
						for (;;)
						{
							int num = reader.ReadValueChunk(this.writeNodeBuffer, 0, 1024);
							if (num <= 0)
							{
								break;
							}
							await this.WriteCharsAsync(this.writeNodeBuffer, 0, num).ConfigureAwait(false);
						}
					}
					else
					{
						await this.WriteStringAsync(reader.Value).ConfigureAwait(false);
					}
					break;
				case XmlNodeType.CDATA:
					await this.WriteCDataAsync(reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.EntityReference:
					await this.WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.XmlDeclaration:
					await this.WriteProcessingInstructionAsync(reader.Name, reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.Comment:
					await this.WriteCommentAsync(reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.DocumentType:
					await this.WriteDocTypeAsync(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					await this.WriteWhitespaceAsync(reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.EndElement:
					await this.WriteFullEndElementAsync().ConfigureAwait(false);
					break;
				}
			}
			while (reader.Read() && (d < reader.Depth || (d == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
		}

		internal async Task WriteNodeAsync_CallAsyncReader(XmlReader reader, bool defattr)
		{
			bool canReadChunk = reader.CanReadValueChunk;
			int d = ((reader.NodeType == XmlNodeType.None) ? (-1) : reader.Depth);
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter;
			do
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					await this.WriteStartElementAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
					await this.WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
					if (reader.IsEmptyElement)
					{
						await this.WriteEndElementAsync().ConfigureAwait(false);
					}
					break;
				case XmlNodeType.Text:
					if (canReadChunk)
					{
						if (this.writeNodeBuffer == null)
						{
							this.writeNodeBuffer = new char[1024];
						}
						for (;;)
						{
							object obj = await reader.ReadValueChunkAsync(this.writeNodeBuffer, 0, 1024).ConfigureAwait(false);
							if (obj <= 0)
							{
								break;
							}
							await this.WriteCharsAsync(this.writeNodeBuffer, 0, obj).ConfigureAwait(false);
						}
					}
					else
					{
						await this.WriteStringAsync(await reader.GetValueAsync().ConfigureAwait(false)).ConfigureAwait(false);
					}
					break;
				case XmlNodeType.CDATA:
					await this.WriteCDataAsync(reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.EntityReference:
					await this.WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.XmlDeclaration:
					await this.WriteProcessingInstructionAsync(reader.Name, reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.Comment:
					await this.WriteCommentAsync(reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.DocumentType:
					await this.WriteDocTypeAsync(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value).ConfigureAwait(false);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					await this.WriteWhitespaceAsync(await reader.GetValueAsync().ConfigureAwait(false)).ConfigureAwait(false);
					break;
				case XmlNodeType.EndElement:
					await this.WriteFullEndElementAsync().ConfigureAwait(false);
					break;
				}
				configuredTaskAwaiter = reader.ReadAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
			}
			while (configuredTaskAwaiter.GetResult() && (d < reader.Depth || (d == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
		}

		public virtual async Task WriteNodeAsync(XPathNavigator navigator, bool defattr)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			int iLevel = 0;
			navigator = navigator.Clone();
			for (;;)
			{
				IL_006F:
				bool mayHaveChildren = false;
				switch (navigator.NodeType)
				{
				case XPathNodeType.Root:
					mayHaveChildren = true;
					break;
				case XPathNodeType.Element:
					await this.WriteStartElementAsync(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI).ConfigureAwait(false);
					if (navigator.MoveToFirstAttribute())
					{
						do
						{
							IXmlSchemaInfo schemaInfo = navigator.SchemaInfo;
							if (defattr || schemaInfo == null || !schemaInfo.IsDefault)
							{
								await this.WriteStartAttributeAsync(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI).ConfigureAwait(false);
								await this.WriteStringAsync(navigator.Value).ConfigureAwait(false);
								await this.WriteEndAttributeAsync().ConfigureAwait(false);
							}
						}
						while (navigator.MoveToNextAttribute());
						navigator.MoveToParent();
					}
					if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
					{
						await this.WriteLocalNamespacesAsync(navigator).ConfigureAwait(false);
						navigator.MoveToParent();
					}
					mayHaveChildren = true;
					break;
				case XPathNodeType.Text:
					await this.WriteStringAsync(navigator.Value).ConfigureAwait(false);
					break;
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					await this.WriteWhitespaceAsync(navigator.Value).ConfigureAwait(false);
					break;
				case XPathNodeType.ProcessingInstruction:
					await this.WriteProcessingInstructionAsync(navigator.LocalName, navigator.Value).ConfigureAwait(false);
					break;
				case XPathNodeType.Comment:
					await this.WriteCommentAsync(navigator.Value).ConfigureAwait(false);
					break;
				}
				if (mayHaveChildren)
				{
					if (navigator.MoveToFirstChild())
					{
						iLevel++;
						continue;
					}
					if (navigator.NodeType == XPathNodeType.Element)
					{
						if (navigator.IsEmptyElement)
						{
							await this.WriteEndElementAsync().ConfigureAwait(false);
						}
						else
						{
							await this.WriteFullEndElementAsync().ConfigureAwait(false);
						}
					}
				}
				while (iLevel != 0)
				{
					if (navigator.MoveToNext())
					{
						goto IL_006F;
					}
					iLevel--;
					navigator.MoveToParent();
					if (navigator.NodeType == XPathNodeType.Element)
					{
						await this.WriteFullEndElementAsync().ConfigureAwait(false);
					}
				}
				break;
			}
		}

		public async Task WriteElementStringAsync(string prefix, string localName, string ns, string value)
		{
			await this.WriteStartElementAsync(prefix, localName, ns).ConfigureAwait(false);
			if (value != null && value.Length != 0)
			{
				await this.WriteStringAsync(value).ConfigureAwait(false);
			}
			await this.WriteEndElementAsync().ConfigureAwait(false);
		}

		private async Task WriteLocalNamespacesAsync(XPathNavigator nsNav)
		{
			string prefix = nsNav.LocalName;
			string ns = nsNav.Value;
			if (nsNav.MoveToNextNamespace(XPathNamespaceScope.Local))
			{
				await this.WriteLocalNamespacesAsync(nsNav).ConfigureAwait(false);
			}
			if (prefix.Length == 0)
			{
				await this.WriteAttributeStringAsync(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", ns).ConfigureAwait(false);
			}
			else
			{
				await this.WriteAttributeStringAsync("xmlns", prefix, "http://www.w3.org/2000/xmlns/", ns).ConfigureAwait(false);
			}
		}

		private char[] writeNodeBuffer;

		private const int WriteNodeBufferSize = 1024;
	}
}
