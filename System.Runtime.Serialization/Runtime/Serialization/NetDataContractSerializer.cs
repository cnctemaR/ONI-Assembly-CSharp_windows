using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Xml;

namespace System.Runtime.Serialization
{
	public sealed class NetDataContractSerializer : XmlObjectSerializer, IFormatter
	{
		public NetDataContractSerializer()
		{
		}

		public NetDataContractSerializer(StreamingContext context)
		{
			this.context = context;
		}

		public NetDataContractSerializer(string rootName, string rootNamespace)
		{
			this.FillDictionaryString(rootName, rootNamespace);
		}

		public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
		{
			if (rootName == null)
			{
				throw new ArgumentNullException("rootName");
			}
			if (rootNamespace == null)
			{
				throw new ArgumentNullException("rootNamespace");
			}
			this.root_name = rootName;
			this.root_ns = rootNamespace;
		}

		public NetDataContractSerializer(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			this.context = context;
			this.max_items = maxItemsInObjectGraph;
			this.ignore_extensions = ignoreExtensibleDataObject;
			this.ass_style = assemblyFormat;
			this.selector = surrogateSelector;
		}

		public NetDataContractSerializer(string rootName, string rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
			: this(context, maxItemsInObjectGraph, ignoreExtensibleDataObject, assemblyFormat, surrogateSelector)
		{
			this.FillDictionaryString(rootName, rootNamespace);
		}

		public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
			: this(context, maxItemsInObjectGraph, ignoreExtensibleDataObject, assemblyFormat, surrogateSelector)
		{
			if (rootName == null)
			{
				throw new ArgumentNullException("rootName");
			}
			if (rootNamespace == null)
			{
				throw new ArgumentNullException("rootNamespace");
			}
			this.root_name = rootName;
			this.root_ns = rootNamespace;
		}

		private void FillDictionaryString(string rootName, string rootNamespace)
		{
			if (rootName == null)
			{
				throw new ArgumentNullException("rootName");
			}
			if (rootNamespace == null)
			{
				throw new ArgumentNullException("rootNamespace");
			}
			XmlDictionary xmlDictionary = new XmlDictionary();
			this.root_name = xmlDictionary.Add(rootName);
			this.root_ns = xmlDictionary.Add(rootNamespace);
		}

		public FormatterAssemblyStyle AssemblyFormat
		{
			get
			{
				return this.ass_style;
			}
			set
			{
				this.ass_style = value;
			}
		}

		public SerializationBinder Binder
		{
			get
			{
				return this.binder;
			}
			set
			{
				this.binder = value;
			}
		}

		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignore_extensions;
			}
		}

		public ISurrogateSelector SurrogateSelector
		{
			get
			{
				return this.selector;
			}
			set
			{
				this.selector = value;
			}
		}

		public StreamingContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.max_items;
			}
		}

		public object Deserialize(Stream stream)
		{
			return this.ReadObject(stream);
		}

		[MonoTODO]
		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			throw new NotImplementedException();
		}

		public override object ReadObject(XmlDictionaryReader reader, bool readContentOnly)
		{
			throw new NotImplementedException();
		}

		public void Serialize(Stream stream, object graph)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stream))
			{
				this.WriteObject(xmlWriter, graph);
			}
		}

		[MonoTODO("support arrays; support Serializable; support SharedType; use DataContractSurrogate")]
		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			throw new NotImplementedException();
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			throw new NotImplementedException();
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			writer.WriteEndElement();
		}

		private const string xmlns = "http://www.w3.org/2000/xmlns/";

		private const string default_ns = "http://schemas.datacontract.org/2004/07/";

		private StreamingContext context;

		private SerializationBinder binder;

		private ISurrogateSelector selector;

		private int max_items = 65536;

		private bool ignore_extensions;

		private FormatterAssemblyStyle ass_style;

		private XmlDictionaryString root_name;

		private XmlDictionaryString root_ns;
	}
}
