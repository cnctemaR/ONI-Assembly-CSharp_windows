using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[ComVisible(true)]
	public sealed class BinaryFormatter : IRemotingFormatter, IFormatter
	{
		public BinaryFormatter()
		{
			this.surrogate_selector = BinaryFormatter.DefaultSurrogateSelector;
			this.context = new StreamingContext(StreamingContextStates.All);
		}

		public BinaryFormatter(ISurrogateSelector selector, StreamingContext context)
		{
			this.surrogate_selector = selector;
			this.context = context;
		}

		public static ISurrogateSelector DefaultSurrogateSelector { get; set; }

		public FormatterAssemblyStyle AssemblyFormat
		{
			get
			{
				return this.assembly_format;
			}
			set
			{
				this.assembly_format = value;
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

		public ISurrogateSelector SurrogateSelector
		{
			get
			{
				return this.surrogate_selector;
			}
			set
			{
				this.surrogate_selector = value;
			}
		}

		public FormatterTypeStyle TypeFormat
		{
			get
			{
				return this.type_format;
			}
			set
			{
				this.type_format = value;
			}
		}

		public TypeFilterLevel FilterLevel
		{
			get
			{
				return this.filter_level;
			}
			set
			{
				this.filter_level = value;
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public object Deserialize(Stream serializationStream)
		{
			return this.NoCheckDeserialize(serializationStream, null);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public object Deserialize(Stream serializationStream, HeaderHandler handler)
		{
			return this.NoCheckDeserialize(serializationStream, handler);
		}

		private object NoCheckDeserialize(Stream serializationStream, HeaderHandler handler)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			if (serializationStream.CanSeek && serializationStream.Length == 0L)
			{
				throw new SerializationException("serializationStream supports seeking, but its length is 0");
			}
			BinaryReader binaryReader = new BinaryReader(serializationStream);
			bool flag;
			this.ReadBinaryHeader(binaryReader, out flag);
			BinaryElement binaryElement = (BinaryElement)binaryReader.Read();
			if (binaryElement == BinaryElement.MethodCall)
			{
				return MessageFormatter.ReadMethodCall(binaryElement, binaryReader, flag, handler, this);
			}
			if (binaryElement == BinaryElement.MethodResponse)
			{
				return MessageFormatter.ReadMethodResponse(binaryElement, binaryReader, flag, handler, null, this);
			}
			ObjectReader objectReader = new ObjectReader(this);
			object obj;
			Header[] array;
			objectReader.ReadObjectGraph(binaryElement, binaryReader, flag, out obj, out array);
			if (handler != null)
			{
				handler(array);
			}
			return obj;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public object DeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			return this.NoCheckDeserializeMethodResponse(serializationStream, handler, methodCallMessage);
		}

		private object NoCheckDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			if (serializationStream.CanSeek && serializationStream.Length == 0L)
			{
				throw new SerializationException("serializationStream supports seeking, but its length is 0");
			}
			BinaryReader binaryReader = new BinaryReader(serializationStream);
			bool flag;
			this.ReadBinaryHeader(binaryReader, out flag);
			return MessageFormatter.ReadMethodResponse(binaryReader, flag, handler, methodCallMessage, this);
		}

		public void Serialize(Stream serializationStream, object graph)
		{
			this.Serialize(serializationStream, graph, null);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public void Serialize(Stream serializationStream, object graph, Header[] headers)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			BinaryWriter binaryWriter = new BinaryWriter(serializationStream);
			this.WriteBinaryHeader(binaryWriter, headers != null);
			if (graph is IMethodCallMessage)
			{
				MessageFormatter.WriteMethodCall(binaryWriter, graph, headers, this.surrogate_selector, this.context, this.assembly_format, this.type_format);
			}
			else if (graph is IMethodReturnMessage)
			{
				MessageFormatter.WriteMethodResponse(binaryWriter, graph, headers, this.surrogate_selector, this.context, this.assembly_format, this.type_format);
			}
			else
			{
				ObjectWriter objectWriter = new ObjectWriter(this.surrogate_selector, this.context, this.assembly_format, this.type_format);
				objectWriter.WriteObjectGraph(binaryWriter, graph, headers);
			}
			binaryWriter.Flush();
		}

		[ComVisible(false)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public object UnsafeDeserialize(Stream serializationStream, HeaderHandler handler)
		{
			return this.NoCheckDeserialize(serializationStream, handler);
		}

		[ComVisible(false)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public object UnsafeDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			return this.NoCheckDeserializeMethodResponse(serializationStream, handler, methodCallMessage);
		}

		private void WriteBinaryHeader(BinaryWriter writer, bool hasHeaders)
		{
			writer.Write(0);
			writer.Write(1);
			if (hasHeaders)
			{
				writer.Write(2);
			}
			else
			{
				writer.Write(-1);
			}
			writer.Write(1);
			writer.Write(0);
		}

		private void ReadBinaryHeader(BinaryReader reader, out bool hasHeaders)
		{
			reader.ReadByte();
			reader.ReadInt32();
			int num = reader.ReadInt32();
			hasHeaders = num == 2;
			reader.ReadInt32();
			reader.ReadInt32();
		}

		private FormatterAssemblyStyle assembly_format;

		private SerializationBinder binder;

		private StreamingContext context;

		private ISurrogateSelector surrogate_selector;

		private FormatterTypeStyle type_format = FormatterTypeStyle.TypesAlways;

		private TypeFilterLevel filter_level = TypeFilterLevel.Full;
	}
}
