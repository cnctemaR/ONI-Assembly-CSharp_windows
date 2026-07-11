using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XsdCachingReader : XmlReader, IXmlLineInfo
	{
		internal XsdCachingReader(XmlReader reader, IXmlLineInfo lineInfo, CachingEventHandler handlerMethod)
		{
			this.coreReader = reader;
			this.lineInfo = lineInfo;
			this.cacheHandler = handlerMethod;
			this.attributeEvents = new ValidatingReaderNodeData[8];
			this.contentEvents = new ValidatingReaderNodeData[4];
			this.Init();
		}

		private void Init()
		{
			this.coreReaderNameTable = this.coreReader.NameTable;
			this.cacheState = XsdCachingReader.CachingReaderState.Init;
			this.contentIndex = 0;
			this.currentAttrIndex = -1;
			this.currentContentIndex = -1;
			this.attributeCount = 0;
			this.cachedNode = null;
			this.readAhead = false;
			if (this.coreReader.NodeType == XmlNodeType.Element)
			{
				ValidatingReaderNodeData validatingReaderNodeData = this.AddContent(this.coreReader.NodeType);
				validatingReaderNodeData.SetItemData(this.coreReader.LocalName, this.coreReader.Prefix, this.coreReader.NamespaceURI, this.coreReader.Depth);
				validatingReaderNodeData.SetLineInfo(this.lineInfo);
				this.RecordAttributes();
			}
		}

		internal void Reset(XmlReader reader)
		{
			this.coreReader = reader;
			this.Init();
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return this.coreReader.Settings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.cachedNode.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.cachedNode.GetAtomizedNameWPrefix(this.coreReaderNameTable);
			}
		}

		public override string LocalName
		{
			get
			{
				return this.cachedNode.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.cachedNode.Namespace;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.cachedNode.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return XmlReader.HasValueInternal(this.cachedNode.NodeType);
			}
		}

		public override string Value
		{
			get
			{
				if (!this.returnOriginalStringValues)
				{
					return this.cachedNode.RawValue;
				}
				return this.cachedNode.OriginalStringValue;
			}
		}

		public override int Depth
		{
			get
			{
				return this.cachedNode.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.coreReader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return false;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.coreReader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.coreReader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.coreReader.XmlLang;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.attributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			int num;
			if (name.IndexOf(':') == -1)
			{
				num = this.GetAttributeIndexWithoutPrefix(name);
			}
			else
			{
				num = this.GetAttributeIndexWithPrefix(name);
			}
			if (num < 0)
			{
				return null;
			}
			return this.attributeEvents[num].RawValue;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			namespaceURI = ((namespaceURI == null) ? string.Empty : this.coreReaderNameTable.Get(namespaceURI));
			name = this.coreReaderNameTable.Get(name);
			for (int i = 0; i < this.attributeCount; i++)
			{
				ValidatingReaderNodeData validatingReaderNodeData = this.attributeEvents[i];
				if (Ref.Equal(validatingReaderNodeData.LocalName, name) && Ref.Equal(validatingReaderNodeData.Namespace, namespaceURI))
				{
					return validatingReaderNodeData.RawValue;
				}
			}
			return null;
		}

		public override string GetAttribute(int i)
		{
			if (i < 0 || i >= this.attributeCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			return this.attributeEvents[i].RawValue;
		}

		public override string this[int i]
		{
			get
			{
				return this.GetAttribute(i);
			}
		}

		public override string this[string name]
		{
			get
			{
				return this.GetAttribute(name);
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				return this.GetAttribute(name, namespaceURI);
			}
		}

		public override bool MoveToAttribute(string name)
		{
			int num;
			if (name.IndexOf(':') == -1)
			{
				num = this.GetAttributeIndexWithoutPrefix(name);
			}
			else
			{
				num = this.GetAttributeIndexWithPrefix(name);
			}
			if (num >= 0)
			{
				this.currentAttrIndex = num;
				this.cachedNode = this.attributeEvents[num];
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			ns = ((ns == null) ? string.Empty : this.coreReaderNameTable.Get(ns));
			name = this.coreReaderNameTable.Get(name);
			for (int i = 0; i < this.attributeCount; i++)
			{
				ValidatingReaderNodeData validatingReaderNodeData = this.attributeEvents[i];
				if (Ref.Equal(validatingReaderNodeData.LocalName, name) && Ref.Equal(validatingReaderNodeData.Namespace, ns))
				{
					this.currentAttrIndex = i;
					this.cachedNode = this.attributeEvents[i];
					return true;
				}
			}
			return false;
		}

		public override void MoveToAttribute(int i)
		{
			if (i < 0 || i >= this.attributeCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			this.currentAttrIndex = i;
			this.cachedNode = this.attributeEvents[i];
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.attributeCount == 0)
			{
				return false;
			}
			this.currentAttrIndex = 0;
			this.cachedNode = this.attributeEvents[0];
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.currentAttrIndex + 1 < this.attributeCount)
			{
				ValidatingReaderNodeData[] array = this.attributeEvents;
				int num = this.currentAttrIndex + 1;
				this.currentAttrIndex = num;
				this.cachedNode = array[num];
				return true;
			}
			return false;
		}

		public override bool MoveToElement()
		{
			if (this.cacheState != XsdCachingReader.CachingReaderState.Replay || this.cachedNode.NodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			this.currentContentIndex = 0;
			this.currentAttrIndex = -1;
			this.Read();
			return true;
		}

		public override bool Read()
		{
			switch (this.cacheState)
			{
			case XsdCachingReader.CachingReaderState.Init:
				this.cacheState = XsdCachingReader.CachingReaderState.Record;
				break;
			case XsdCachingReader.CachingReaderState.Record:
				break;
			case XsdCachingReader.CachingReaderState.Replay:
				if (this.currentContentIndex >= this.contentIndex)
				{
					this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
					this.cacheHandler(this);
					return (this.coreReader.NodeType == XmlNodeType.Element && !this.readAhead) || this.coreReader.Read();
				}
				this.cachedNode = this.contentEvents[this.currentContentIndex];
				if (this.currentContentIndex > 0)
				{
					this.ClearAttributesInfo();
				}
				this.currentContentIndex++;
				return true;
			default:
				return false;
			}
			ValidatingReaderNodeData validatingReaderNodeData = null;
			if (this.coreReader.Read())
			{
				switch (this.coreReader.NodeType)
				{
				case XmlNodeType.Element:
					this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
					return false;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					validatingReaderNodeData = this.AddContent(this.coreReader.NodeType);
					validatingReaderNodeData.SetItemData(this.coreReader.Value);
					validatingReaderNodeData.SetLineInfo(this.lineInfo);
					validatingReaderNodeData.Depth = this.coreReader.Depth;
					break;
				case XmlNodeType.EndElement:
					validatingReaderNodeData = this.AddContent(this.coreReader.NodeType);
					validatingReaderNodeData.SetItemData(this.coreReader.LocalName, this.coreReader.Prefix, this.coreReader.NamespaceURI, this.coreReader.Depth);
					validatingReaderNodeData.SetLineInfo(this.lineInfo);
					break;
				}
				this.cachedNode = validatingReaderNodeData;
				return true;
			}
			this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
			return false;
		}

		internal ValidatingReaderNodeData RecordTextNode(string textValue, string originalStringValue, int depth, int lineNo, int linePos)
		{
			ValidatingReaderNodeData validatingReaderNodeData = this.AddContent(XmlNodeType.Text);
			validatingReaderNodeData.SetItemData(textValue, originalStringValue);
			validatingReaderNodeData.SetLineInfo(lineNo, linePos);
			validatingReaderNodeData.Depth = depth;
			return validatingReaderNodeData;
		}

		internal void SwitchTextNodeAndEndElement(string textValue, string originalStringValue)
		{
			ValidatingReaderNodeData validatingReaderNodeData = this.RecordTextNode(textValue, originalStringValue, this.coreReader.Depth + 1, 0, 0);
			int num = this.contentIndex - 2;
			ValidatingReaderNodeData validatingReaderNodeData2 = this.contentEvents[num];
			this.contentEvents[num] = validatingReaderNodeData;
			this.contentEvents[this.contentIndex - 1] = validatingReaderNodeData2;
		}

		internal void RecordEndElementNode()
		{
			ValidatingReaderNodeData validatingReaderNodeData = this.AddContent(XmlNodeType.EndElement);
			validatingReaderNodeData.SetItemData(this.coreReader.LocalName, this.coreReader.Prefix, this.coreReader.NamespaceURI, this.coreReader.Depth);
			validatingReaderNodeData.SetLineInfo(this.coreReader as IXmlLineInfo);
			if (this.coreReader.IsEmptyElement)
			{
				this.readAhead = true;
			}
		}

		internal string ReadOriginalContentAsString()
		{
			this.returnOriginalStringValues = true;
			string text = base.InternalReadContentAsString();
			this.returnOriginalStringValues = false;
			return text;
		}

		public override bool EOF
		{
			get
			{
				return this.cacheState == XsdCachingReader.CachingReaderState.ReaderClosed && this.coreReader.EOF;
			}
		}

		public override void Close()
		{
			this.coreReader.Close();
			this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
		}

		public override ReadState ReadState
		{
			get
			{
				return this.coreReader.ReadState;
			}
		}

		public override void Skip()
		{
			XmlNodeType nodeType = this.cachedNode.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					this.Read();
					return;
				}
				this.MoveToElement();
			}
			if (this.coreReader.NodeType != XmlNodeType.EndElement && !this.readAhead)
			{
				int num = this.coreReader.Depth - 1;
				while (this.coreReader.Read() && this.coreReader.Depth > num)
				{
				}
			}
			this.coreReader.Read();
			this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
			this.cacheHandler(this);
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.coreReaderNameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this.coreReader.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		public override bool ReadAttributeValue()
		{
			if (this.cachedNode.NodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			this.cachedNode = this.CreateDummyTextNode(this.cachedNode.RawValue, this.cachedNode.Depth + 1);
			return true;
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return true;
		}

		int IXmlLineInfo.LineNumber
		{
			get
			{
				return this.cachedNode.LineNumber;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				return this.cachedNode.LinePosition;
			}
		}

		internal void SetToReplayMode()
		{
			this.cacheState = XsdCachingReader.CachingReaderState.Replay;
			this.currentContentIndex = 0;
			this.currentAttrIndex = -1;
			this.Read();
		}

		internal XmlReader GetCoreReader()
		{
			return this.coreReader;
		}

		internal IXmlLineInfo GetLineInfo()
		{
			return this.lineInfo;
		}

		private void ClearAttributesInfo()
		{
			this.attributeCount = 0;
			this.currentAttrIndex = -1;
		}

		private ValidatingReaderNodeData AddAttribute(int attIndex)
		{
			ValidatingReaderNodeData validatingReaderNodeData = this.attributeEvents[attIndex];
			if (validatingReaderNodeData != null)
			{
				validatingReaderNodeData.Clear(XmlNodeType.Attribute);
				return validatingReaderNodeData;
			}
			if (attIndex >= this.attributeEvents.Length - 1)
			{
				ValidatingReaderNodeData[] array = new ValidatingReaderNodeData[this.attributeEvents.Length * 2];
				Array.Copy(this.attributeEvents, 0, array, 0, this.attributeEvents.Length);
				this.attributeEvents = array;
			}
			validatingReaderNodeData = this.attributeEvents[attIndex];
			if (validatingReaderNodeData == null)
			{
				validatingReaderNodeData = new ValidatingReaderNodeData(XmlNodeType.Attribute);
				this.attributeEvents[attIndex] = validatingReaderNodeData;
			}
			return validatingReaderNodeData;
		}

		private ValidatingReaderNodeData AddContent(XmlNodeType nodeType)
		{
			ValidatingReaderNodeData validatingReaderNodeData = this.contentEvents[this.contentIndex];
			if (validatingReaderNodeData != null)
			{
				validatingReaderNodeData.Clear(nodeType);
				this.contentIndex++;
				return validatingReaderNodeData;
			}
			if (this.contentIndex >= this.contentEvents.Length - 1)
			{
				ValidatingReaderNodeData[] array = new ValidatingReaderNodeData[this.contentEvents.Length * 2];
				Array.Copy(this.contentEvents, 0, array, 0, this.contentEvents.Length);
				this.contentEvents = array;
			}
			validatingReaderNodeData = this.contentEvents[this.contentIndex];
			if (validatingReaderNodeData == null)
			{
				validatingReaderNodeData = new ValidatingReaderNodeData(nodeType);
				this.contentEvents[this.contentIndex] = validatingReaderNodeData;
			}
			this.contentIndex++;
			return validatingReaderNodeData;
		}

		private void RecordAttributes()
		{
			this.attributeCount = this.coreReader.AttributeCount;
			if (this.coreReader.MoveToFirstAttribute())
			{
				int num = 0;
				do
				{
					ValidatingReaderNodeData validatingReaderNodeData = this.AddAttribute(num);
					validatingReaderNodeData.SetItemData(this.coreReader.LocalName, this.coreReader.Prefix, this.coreReader.NamespaceURI, this.coreReader.Depth);
					validatingReaderNodeData.SetLineInfo(this.lineInfo);
					validatingReaderNodeData.RawValue = this.coreReader.Value;
					num++;
				}
				while (this.coreReader.MoveToNextAttribute());
				this.coreReader.MoveToElement();
			}
		}

		private int GetAttributeIndexWithoutPrefix(string name)
		{
			name = this.coreReaderNameTable.Get(name);
			if (name == null)
			{
				return -1;
			}
			for (int i = 0; i < this.attributeCount; i++)
			{
				ValidatingReaderNodeData validatingReaderNodeData = this.attributeEvents[i];
				if (Ref.Equal(validatingReaderNodeData.LocalName, name) && validatingReaderNodeData.Prefix.Length == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private int GetAttributeIndexWithPrefix(string name)
		{
			name = this.coreReaderNameTable.Get(name);
			if (name == null)
			{
				return -1;
			}
			for (int i = 0; i < this.attributeCount; i++)
			{
				if (Ref.Equal(this.attributeEvents[i].GetAtomizedNameWPrefix(this.coreReaderNameTable), name))
				{
					return i;
				}
			}
			return -1;
		}

		private ValidatingReaderNodeData CreateDummyTextNode(string attributeValue, int depth)
		{
			if (this.textNode == null)
			{
				this.textNode = new ValidatingReaderNodeData(XmlNodeType.Text);
			}
			this.textNode.Depth = depth;
			this.textNode.RawValue = attributeValue;
			return this.textNode;
		}

		public override Task<string> GetValueAsync()
		{
			if (this.returnOriginalStringValues)
			{
				return Task.FromResult<string>(this.cachedNode.OriginalStringValue);
			}
			return Task.FromResult<string>(this.cachedNode.RawValue);
		}

		public override async Task<bool> ReadAsync()
		{
			switch (this.cacheState)
			{
			case XsdCachingReader.CachingReaderState.Init:
				this.cacheState = XsdCachingReader.CachingReaderState.Record;
				break;
			case XsdCachingReader.CachingReaderState.Record:
				break;
			case XsdCachingReader.CachingReaderState.Replay:
				if (this.currentContentIndex < this.contentIndex)
				{
					this.cachedNode = this.contentEvents[this.currentContentIndex];
					if (this.currentContentIndex > 0)
					{
						this.ClearAttributesInfo();
					}
					this.currentContentIndex++;
					return true;
				}
				this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
				this.cacheHandler(this);
				if (this.coreReader.NodeType != XmlNodeType.Element || this.readAhead)
				{
					return await this.coreReader.ReadAsync().ConfigureAwait(false);
				}
				return true;
			default:
				return false;
			}
			ValidatingReaderNodeData recordedNode = null;
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.coreReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
			if (!configuredTaskAwaiter.IsCompleted)
			{
				await configuredTaskAwaiter;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
				configuredTaskAwaiter = configuredTaskAwaiter2;
				configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
			}
			bool flag;
			if (configuredTaskAwaiter.GetResult())
			{
				switch (this.coreReader.NodeType)
				{
				case XmlNodeType.Element:
					this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
					return false;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
				{
					recordedNode = this.AddContent(this.coreReader.NodeType);
					ValidatingReaderNodeData validatingReaderNodeData = recordedNode;
					validatingReaderNodeData.SetItemData(await this.coreReader.GetValueAsync().ConfigureAwait(false));
					validatingReaderNodeData = null;
					recordedNode.SetLineInfo(this.lineInfo);
					recordedNode.Depth = this.coreReader.Depth;
					break;
				}
				case XmlNodeType.EndElement:
					recordedNode = this.AddContent(this.coreReader.NodeType);
					recordedNode.SetItemData(this.coreReader.LocalName, this.coreReader.Prefix, this.coreReader.NamespaceURI, this.coreReader.Depth);
					recordedNode.SetLineInfo(this.lineInfo);
					break;
				}
				this.cachedNode = recordedNode;
				flag = true;
			}
			else
			{
				this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
				flag = false;
			}
			return flag;
		}

		public override async Task SkipAsync()
		{
			XmlNodeType nodeType = this.cachedNode.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					await this.ReadAsync().ConfigureAwait(false);
					return;
				}
				this.MoveToElement();
			}
			if (this.coreReader.NodeType != XmlNodeType.EndElement && !this.readAhead)
			{
				int startDepth = this.coreReader.Depth - 1;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter;
				do
				{
					configuredTaskAwaiter = this.coreReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
					}
				}
				while (configuredTaskAwaiter.GetResult() && this.coreReader.Depth > startDepth);
			}
			await this.coreReader.ReadAsync().ConfigureAwait(false);
			this.cacheState = XsdCachingReader.CachingReaderState.ReaderClosed;
			this.cacheHandler(this);
		}

		internal Task SetToReplayModeAsync()
		{
			this.cacheState = XsdCachingReader.CachingReaderState.Replay;
			this.currentContentIndex = 0;
			this.currentAttrIndex = -1;
			return this.ReadAsync();
		}

		private XmlReader coreReader;

		private XmlNameTable coreReaderNameTable;

		private ValidatingReaderNodeData[] contentEvents;

		private ValidatingReaderNodeData[] attributeEvents;

		private ValidatingReaderNodeData cachedNode;

		private XsdCachingReader.CachingReaderState cacheState;

		private int contentIndex;

		private int attributeCount;

		private bool returnOriginalStringValues;

		private CachingEventHandler cacheHandler;

		private int currentAttrIndex;

		private int currentContentIndex;

		private bool readAhead;

		private IXmlLineInfo lineInfo;

		private ValidatingReaderNodeData textNode;

		private const int InitialAttributeCount = 8;

		private const int InitialContentCount = 4;

		private enum CachingReaderState
		{
			None,
			Init,
			Record,
			Replay,
			ReaderClosed,
			Error
		}
	}
}
