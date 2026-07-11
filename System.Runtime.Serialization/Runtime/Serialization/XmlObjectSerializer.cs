using System;
using System.IO;
using System.Xml;

namespace System.Runtime.Serialization
{
	public abstract class XmlObjectSerializer
	{
		public virtual bool IsStartObject(XmlReader reader)
		{
			return this.IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public abstract bool IsStartObject(XmlDictionaryReader reader);

		public virtual object ReadObject(Stream stream)
		{
			return this.ReadObject(XmlReader.Create(stream));
		}

		public virtual object ReadObject(XmlReader reader)
		{
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public virtual object ReadObject(XmlDictionaryReader reader)
		{
			return this.ReadObject(reader, true);
		}

		public virtual object ReadObject(XmlReader reader, bool readContentOnly)
		{
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), readContentOnly);
		}

		[MonoTODO]
		public abstract object ReadObject(XmlDictionaryReader reader, bool readContentOnly);

		public virtual void WriteObject(Stream stream, object graph)
		{
			using (XmlWriter xmlWriter = XmlDictionaryWriter.CreateTextWriter(stream))
			{
				this.WriteObject(xmlWriter, graph);
			}
		}

		public virtual void WriteObject(XmlWriter writer, object graph)
		{
			this.WriteObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public virtual void WriteStartObject(XmlWriter writer, object graph)
		{
			this.WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public virtual void WriteObject(XmlDictionaryWriter writer, object graph)
		{
			this.WriteStartObject(writer, graph);
			this.WriteObjectContent(writer, graph);
			this.WriteEndObject(writer);
		}

		public abstract void WriteStartObject(XmlDictionaryWriter writer, object graph);

		public virtual void WriteObjectContent(XmlWriter writer, object graph)
		{
			this.WriteObjectContent(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public abstract void WriteObjectContent(XmlDictionaryWriter writer, object graph);

		public virtual void WriteEndObject(XmlWriter writer)
		{
			this.WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
		}

		public abstract void WriteEndObject(XmlDictionaryWriter writer);

		private IDataContractSurrogate surrogate;

		private SerializationBinder binder;

		private ISurrogateSelector selector;

		private int max_items = 65536;
	}
}
