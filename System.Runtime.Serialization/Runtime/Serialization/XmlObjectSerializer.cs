using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.Runtime.Serialization.Diagnostics;
using System.Security;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization
{
	public abstract class XmlObjectSerializer
	{
		public abstract void WriteStartObject(XmlDictionaryWriter writer, object graph);

		public abstract void WriteObjectContent(XmlDictionaryWriter writer, object graph);

		public abstract void WriteEndObject(XmlDictionaryWriter writer);

		public virtual void WriteObject(Stream stream, object graph)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false);
			this.WriteObject(xmlDictionaryWriter, graph);
			xmlDictionaryWriter.Flush();
		}

		public virtual void WriteObject(XmlWriter writer, object graph)
		{
			XmlObjectSerializer.CheckNull(writer, "writer");
			this.WriteObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public virtual void WriteStartObject(XmlWriter writer, object graph)
		{
			XmlObjectSerializer.CheckNull(writer, "writer");
			this.WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public virtual void WriteObjectContent(XmlWriter writer, object graph)
		{
			XmlObjectSerializer.CheckNull(writer, "writer");
			this.WriteObjectContent(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public virtual void WriteEndObject(XmlWriter writer)
		{
			XmlObjectSerializer.CheckNull(writer, "writer");
			this.WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
		}

		public virtual void WriteObject(XmlDictionaryWriter writer, object graph)
		{
			this.WriteObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		internal void WriteObjectHandleExceptions(XmlWriterDelegator writer, object graph)
		{
			this.WriteObjectHandleExceptions(writer, graph, null);
		}

		internal void WriteObjectHandleExceptions(XmlWriterDelegator writer, object graph, DataContractResolver dataContractResolver)
		{
			try
			{
				XmlObjectSerializer.CheckNull(writer, "writer");
				if (DiagnosticUtility.ShouldTraceInformation)
				{
					TraceUtility.Trace(TraceEventType.Information, 196609, global::System.Runtime.Serialization.SR.GetString("WriteObject begins"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetSerializeType(graph))));
					this.InternalWriteObject(writer, graph, dataContractResolver);
					TraceUtility.Trace(TraceEventType.Information, 196610, global::System.Runtime.Serialization.SR.GetString("WriteObject ends"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetSerializeType(graph))));
				}
				else
				{
					this.InternalWriteObject(writer, graph, dataContractResolver);
				}
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error serializing the object {0}. {1}", this.GetSerializeType(graph), ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error serializing the object {0}. {1}", this.GetSerializeType(graph), ex2), ex2));
			}
		}

		internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			get
			{
				return null;
			}
		}

		internal virtual void InternalWriteObject(XmlWriterDelegator writer, object graph)
		{
			this.WriteStartObject(writer.Writer, graph);
			this.WriteObjectContent(writer.Writer, graph);
			this.WriteEndObject(writer.Writer);
		}

		internal virtual void InternalWriteObject(XmlWriterDelegator writer, object graph, DataContractResolver dataContractResolver)
		{
			this.InternalWriteObject(writer, graph);
		}

		internal virtual void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		internal virtual void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		internal virtual void InternalWriteEndObject(XmlWriterDelegator writer)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		internal void WriteStartObjectHandleExceptions(XmlWriterDelegator writer, object graph)
		{
			try
			{
				XmlObjectSerializer.CheckNull(writer, "writer");
				this.InternalWriteStartObject(writer, graph);
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error writing start element of object {0}. {1}", this.GetSerializeType(graph), ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error writing start element of object {0}. {1}", this.GetSerializeType(graph), ex2), ex2));
			}
		}

		internal void WriteObjectContentHandleExceptions(XmlWriterDelegator writer, object graph)
		{
			try
			{
				XmlObjectSerializer.CheckNull(writer, "writer");
				if (DiagnosticUtility.ShouldTraceInformation)
				{
					TraceUtility.Trace(TraceEventType.Information, 196611, global::System.Runtime.Serialization.SR.GetString("WriteObjectContent begins"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetSerializeType(graph))));
					if (writer.WriteState != WriteState.Element)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("WriteState '{0}' not valid. Caller must write start element before serializing in contentOnly mode.", new object[] { writer.WriteState })));
					}
					this.InternalWriteObjectContent(writer, graph);
					TraceUtility.Trace(TraceEventType.Information, 196612, global::System.Runtime.Serialization.SR.GetString("WriteObjectContent ends"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetSerializeType(graph))));
				}
				else
				{
					if (writer.WriteState != WriteState.Element)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("WriteState '{0}' not valid. Caller must write start element before serializing in contentOnly mode.", new object[] { writer.WriteState })));
					}
					this.InternalWriteObjectContent(writer, graph);
				}
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error serializing the object {0}. {1}", this.GetSerializeType(graph), ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error serializing the object {0}. {1}", this.GetSerializeType(graph), ex2), ex2));
			}
		}

		internal void WriteEndObjectHandleExceptions(XmlWriterDelegator writer)
		{
			try
			{
				XmlObjectSerializer.CheckNull(writer, "writer");
				this.InternalWriteEndObject(writer);
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error writing end element of object {0}. {1}", null, ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error writing end element of object {0}. {1}", null, ex2), ex2));
			}
		}

		internal void WriteRootElement(XmlWriterDelegator writer, DataContract contract, XmlDictionaryString name, XmlDictionaryString ns, bool needsContractNsAtRoot)
		{
			if (name != null)
			{
				contract.WriteRootElement(writer, name, ns);
				if (needsContractNsAtRoot)
				{
					writer.WriteNamespaceDecl(contract.Namespace);
				}
				return;
			}
			if (!contract.HasRoot)
			{
				return;
			}
			contract.WriteRootElement(writer, contract.TopLevelElementName, contract.TopLevelElementNamespace);
		}

		internal bool CheckIfNeedsContractNsAtRoot(XmlDictionaryString name, XmlDictionaryString ns, DataContract contract)
		{
			if (name == null)
			{
				return false;
			}
			if (contract.IsBuiltInDataContract || !contract.CanContainReferences || contract.IsISerializable)
			{
				return false;
			}
			string @string = XmlDictionaryString.GetString(contract.Namespace);
			return !string.IsNullOrEmpty(@string) && !(@string == XmlDictionaryString.GetString(ns));
		}

		internal static void WriteNull(XmlWriterDelegator writer)
		{
			writer.WriteAttributeBool("i", DictionaryGlobals.XsiNilLocalName, DictionaryGlobals.SchemaInstanceNamespace, true);
		}

		internal static bool IsContractDeclared(DataContract contract, DataContract declaredContract)
		{
			return (contract.Name == declaredContract.Name && contract.Namespace == declaredContract.Namespace) || (contract.Name.Value == declaredContract.Name.Value && contract.Namespace.Value == declaredContract.Namespace.Value);
		}

		public virtual object ReadObject(Stream stream)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			return this.ReadObject(XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max));
		}

		public virtual object ReadObject(XmlReader reader)
		{
			XmlObjectSerializer.CheckNull(reader, "reader");
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public virtual object ReadObject(XmlDictionaryReader reader)
		{
			return this.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), true);
		}

		public virtual object ReadObject(XmlReader reader, bool verifyObjectName)
		{
			XmlObjectSerializer.CheckNull(reader, "reader");
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), verifyObjectName);
		}

		public abstract object ReadObject(XmlDictionaryReader reader, bool verifyObjectName);

		public virtual bool IsStartObject(XmlReader reader)
		{
			XmlObjectSerializer.CheckNull(reader, "reader");
			return this.IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public abstract bool IsStartObject(XmlDictionaryReader reader);

		internal virtual object InternalReadObject(XmlReaderDelegator reader, bool verifyObjectName)
		{
			return this.ReadObject(reader.UnderlyingReader, verifyObjectName);
		}

		internal virtual object InternalReadObject(XmlReaderDelegator reader, bool verifyObjectName, DataContractResolver dataContractResolver)
		{
			return this.InternalReadObject(reader, verifyObjectName);
		}

		internal virtual bool InternalIsStartObject(XmlReaderDelegator reader)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		internal object ReadObjectHandleExceptions(XmlReaderDelegator reader, bool verifyObjectName)
		{
			return this.ReadObjectHandleExceptions(reader, verifyObjectName, null);
		}

		internal object ReadObjectHandleExceptions(XmlReaderDelegator reader, bool verifyObjectName, DataContractResolver dataContractResolver)
		{
			object obj2;
			try
			{
				XmlObjectSerializer.CheckNull(reader, "reader");
				if (DiagnosticUtility.ShouldTraceInformation)
				{
					TraceUtility.Trace(TraceEventType.Information, 196613, global::System.Runtime.Serialization.SR.GetString("ReadObject begins"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetDeserializeType())));
					object obj = this.InternalReadObject(reader, verifyObjectName, dataContractResolver);
					TraceUtility.Trace(TraceEventType.Information, 196614, global::System.Runtime.Serialization.SR.GetString("ReadObject ends"), new StringTraceRecord("Type", XmlObjectSerializer.GetTypeInfo(this.GetDeserializeType())));
					obj2 = obj;
				}
				else
				{
					obj2 = this.InternalReadObject(reader, verifyObjectName, dataContractResolver);
				}
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error deserializing the object {0}. {1}", this.GetDeserializeType(), ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error deserializing the object {0}. {1}", this.GetDeserializeType(), ex2), ex2));
			}
			return obj2;
		}

		internal bool IsStartObjectHandleExceptions(XmlReaderDelegator reader)
		{
			bool flag;
			try
			{
				XmlObjectSerializer.CheckNull(reader, "reader");
				flag = this.InternalIsStartObject(reader);
			}
			catch (XmlException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error checking start element of object {0}. {1}", this.GetDeserializeType(), ex), ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("There was an error checking start element of object {0}. {1}", this.GetDeserializeType(), ex2), ex2));
			}
			return flag;
		}

		internal bool IsRootXmlAny(XmlDictionaryString rootName, DataContract contract)
		{
			return rootName == null && !contract.HasRoot;
		}

		internal bool IsStartElement(XmlReaderDelegator reader)
		{
			return reader.MoveToElement() || reader.IsStartElement();
		}

		internal bool IsRootElement(XmlReaderDelegator reader, DataContract contract, XmlDictionaryString name, XmlDictionaryString ns)
		{
			reader.MoveToElement();
			if (name != null)
			{
				return reader.IsStartElement(name, ns);
			}
			if (!contract.HasRoot)
			{
				return reader.IsStartElement();
			}
			if (reader.IsStartElement(contract.TopLevelElementName, contract.TopLevelElementNamespace))
			{
				return true;
			}
			ClassDataContract classDataContract = contract as ClassDataContract;
			if (classDataContract != null)
			{
				classDataContract = classDataContract.BaseContract;
			}
			while (classDataContract != null)
			{
				if (reader.IsStartElement(classDataContract.TopLevelElementName, classDataContract.TopLevelElementNamespace))
				{
					return true;
				}
				classDataContract = classDataContract.BaseContract;
			}
			if (classDataContract == null)
			{
				DataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(Globals.TypeOfObject);
				if (reader.IsStartElement(primitiveDataContract.TopLevelElementName, primitiveDataContract.TopLevelElementNamespace))
				{
					return true;
				}
			}
			return false;
		}

		internal static void CheckNull(object obj, string name)
		{
			if (obj == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(name));
			}
		}

		internal static string TryAddLineInfo(XmlReaderDelegator reader, string errorMessage)
		{
			if (reader.HasLineInfo())
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", global::System.Runtime.Serialization.SR.GetString("Error in line {0} position {1}.", new object[] { reader.LineNumber, reader.LinePosition }), errorMessage);
			}
			return errorMessage;
		}

		internal static Exception CreateSerializationExceptionWithReaderDetails(string errorMessage, XmlReaderDelegator reader)
		{
			return XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(reader, global::System.Runtime.Serialization.SR.GetString("{0}. Encountered '{1}'  with name '{2}', namespace '{3}'.", new object[] { errorMessage, reader.NodeType, reader.LocalName, reader.NamespaceURI })));
		}

		internal static SerializationException CreateSerializationException(string errorMessage)
		{
			return XmlObjectSerializer.CreateSerializationException(errorMessage, null);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static SerializationException CreateSerializationException(string errorMessage, Exception innerException)
		{
			return new SerializationException(errorMessage, innerException);
		}

		private static string GetTypeInfo(Type type)
		{
			if (!(type == null))
			{
				return DataContract.GetClrTypeFullName(type);
			}
			return string.Empty;
		}

		private static string GetTypeInfoError(string errorMessage, Type type, Exception innerException)
		{
			string text = ((type == null) ? string.Empty : global::System.Runtime.Serialization.SR.GetString("of type {0}", new object[] { DataContract.GetClrTypeFullName(type) }));
			string text2 = ((innerException == null) ? string.Empty : innerException.Message);
			return global::System.Runtime.Serialization.SR.GetString(errorMessage, new object[] { text, text2 });
		}

		internal virtual Type GetSerializeType(object graph)
		{
			if (graph != null)
			{
				return graph.GetType();
			}
			return null;
		}

		internal virtual Type GetDeserializeType()
		{
			return null;
		}

		internal static IFormatterConverter FormatterConverter
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlObjectSerializer.formatterConverter == null)
				{
					XmlObjectSerializer.formatterConverter = new FormatterConverter();
				}
				return XmlObjectSerializer.formatterConverter;
			}
		}

		[SecurityCritical]
		private static IFormatterConverter formatterConverter;
	}
}
