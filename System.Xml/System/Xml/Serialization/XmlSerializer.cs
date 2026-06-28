using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Microsoft.CSharp;

namespace System.Xml.Serialization
{
	public class XmlSerializer
	{
		protected XmlSerializer()
		{
			this.customSerializer = true;
		}

		public XmlSerializer(Type type)
			: this(type, null, null, null, null)
		{
		}

		public XmlSerializer(XmlTypeMapping xmlTypeMapping)
		{
			this.typeMapping = xmlTypeMapping;
		}

		internal XmlSerializer(XmlMapping mapping, XmlSerializer.SerializerData data)
		{
			this.typeMapping = mapping;
			this.serializerData = data;
		}

		public XmlSerializer(Type type, string defaultNamespace)
			: this(type, null, null, null, defaultNamespace)
		{
		}

		public XmlSerializer(Type type, Type[] extraTypes)
			: this(type, null, extraTypes, null, null)
		{
		}

		public XmlSerializer(Type type, XmlAttributeOverrides overrides)
			: this(type, overrides, null, null, null)
		{
		}

		public XmlSerializer(Type type, XmlRootAttribute root)
			: this(type, null, null, root, null)
		{
		}

		public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			XmlReflectionImporter xmlReflectionImporter = new XmlReflectionImporter(overrides, defaultNamespace);
			if (extraTypes != null)
			{
				foreach (Type type2 in extraTypes)
				{
					xmlReflectionImporter.IncludeType(type2);
				}
			}
			this.typeMapping = xmlReflectionImporter.ImportTypeMapping(type, root, defaultNamespace);
		}

		[MonoTODO]
		public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
		{
		}

		static XmlSerializer()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_XMLSERIALIZER_DEBUG");
			string text = Environment.GetEnvironmentVariable("MONO_XMLSERIALIZER_THS");
			if (text == null)
			{
				XmlSerializer.generationThreshold = 50;
				XmlSerializer.backgroundGeneration = true;
			}
			else
			{
				int num = text.IndexOf(',');
				if (num != -1)
				{
					if (text.Substring(num + 1) == "nofallback")
					{
						XmlSerializer.generatorFallback = false;
					}
					text = text.Substring(0, num);
				}
				if (text.ToLower(CultureInfo.InvariantCulture) == "no")
				{
					XmlSerializer.generationThreshold = -1;
				}
				else
				{
					XmlSerializer.generationThreshold = int.Parse(text, CultureInfo.InvariantCulture);
					XmlSerializer.backgroundGeneration = XmlSerializer.generationThreshold != 0;
					if (XmlSerializer.generationThreshold < 1)
					{
						XmlSerializer.generationThreshold = 1;
					}
				}
			}
			XmlSerializer.deleteTempFiles = environmentVariable == null || environmentVariable == "no";
			IDictionary dictionary = (IDictionary)ConfigurationSettings.GetConfig("system.diagnostics");
			if (dictionary != null)
			{
				dictionary = (IDictionary)dictionary["switches"];
				if (dictionary != null)
				{
					string text2 = (string)dictionary["XmlSerialization.Compilation"];
					if (text2 == "1")
					{
						XmlSerializer.deleteTempFiles = false;
					}
				}
			}
		}

		public event XmlAttributeEventHandler UnknownAttribute
		{
			add
			{
				this.onUnknownAttribute = (XmlAttributeEventHandler)Delegate.Combine(this.onUnknownAttribute, value);
			}
			remove
			{
				this.onUnknownAttribute = (XmlAttributeEventHandler)Delegate.Remove(this.onUnknownAttribute, value);
			}
		}

		public event XmlElementEventHandler UnknownElement
		{
			add
			{
				this.onUnknownElement = (XmlElementEventHandler)Delegate.Combine(this.onUnknownElement, value);
			}
			remove
			{
				this.onUnknownElement = (XmlElementEventHandler)Delegate.Remove(this.onUnknownElement, value);
			}
		}

		public event XmlNodeEventHandler UnknownNode
		{
			add
			{
				this.onUnknownNode = (XmlNodeEventHandler)Delegate.Combine(this.onUnknownNode, value);
			}
			remove
			{
				this.onUnknownNode = (XmlNodeEventHandler)Delegate.Remove(this.onUnknownNode, value);
			}
		}

		public event UnreferencedObjectEventHandler UnreferencedObject
		{
			add
			{
				this.onUnreferencedObject = (UnreferencedObjectEventHandler)Delegate.Combine(this.onUnreferencedObject, value);
			}
			remove
			{
				this.onUnreferencedObject = (UnreferencedObjectEventHandler)Delegate.Remove(this.onUnreferencedObject, value);
			}
		}

		internal XmlMapping Mapping
		{
			get
			{
				return this.typeMapping;
			}
		}

		internal virtual void OnUnknownAttribute(XmlAttributeEventArgs e)
		{
			if (this.onUnknownAttribute != null)
			{
				this.onUnknownAttribute(this, e);
			}
		}

		internal virtual void OnUnknownElement(XmlElementEventArgs e)
		{
			if (this.onUnknownElement != null)
			{
				this.onUnknownElement(this, e);
			}
		}

		internal virtual void OnUnknownNode(XmlNodeEventArgs e)
		{
			if (this.onUnknownNode != null)
			{
				this.onUnknownNode(this, e);
			}
		}

		internal virtual void OnUnreferencedObject(UnreferencedObjectEventArgs e)
		{
			if (this.onUnreferencedObject != null)
			{
				this.onUnreferencedObject(this, e);
			}
		}

		public virtual bool CanDeserialize(XmlReader xmlReader)
		{
			xmlReader.MoveToContent();
			return this.typeMapping is XmlMembersMapping || ((XmlTypeMapping)this.typeMapping).ElementName == xmlReader.LocalName;
		}

		protected virtual XmlSerializationReader CreateReader()
		{
			throw new NotImplementedException();
		}

		protected virtual XmlSerializationWriter CreateWriter()
		{
			throw new NotImplementedException();
		}

		public object Deserialize(Stream stream)
		{
			return this.Deserialize(new XmlTextReader(stream)
			{
				Normalization = true,
				WhitespaceHandling = WhitespaceHandling.Significant
			});
		}

		public object Deserialize(TextReader textReader)
		{
			return this.Deserialize(new XmlTextReader(textReader)
			{
				Normalization = true,
				WhitespaceHandling = WhitespaceHandling.Significant
			});
		}

		public object Deserialize(XmlReader xmlReader)
		{
			XmlSerializationReader xmlSerializationReader;
			if (this.customSerializer)
			{
				xmlSerializationReader = this.CreateReader();
			}
			else
			{
				xmlSerializationReader = this.CreateReader(this.typeMapping);
			}
			xmlSerializationReader.Initialize(xmlReader, this);
			return this.Deserialize(xmlSerializationReader);
		}

		protected virtual object Deserialize(XmlSerializationReader reader)
		{
			if (this.customSerializer)
			{
				throw new NotImplementedException();
			}
			object obj;
			try
			{
				if (reader is XmlSerializationReaderInterpreter)
				{
					obj = ((XmlSerializationReaderInterpreter)reader).ReadRoot();
				}
				else
				{
					obj = this.serializerData.ReaderMethod.Invoke(reader, null);
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidOperationException || ex is InvalidCastException)
				{
					throw new InvalidOperationException("There is an error in XML document.", ex);
				}
				throw;
			}
			return obj;
		}

		public static XmlSerializer[] FromMappings(XmlMapping[] mappings)
		{
			XmlSerializer[] array = new XmlSerializer[mappings.Length];
			XmlSerializer.SerializerData[] array2 = new XmlSerializer.SerializerData[mappings.Length];
			XmlSerializer.GenerationBatch generationBatch = new XmlSerializer.GenerationBatch();
			generationBatch.Maps = mappings;
			generationBatch.Datas = array2;
			for (int i = 0; i < mappings.Length; i++)
			{
				if (mappings[i] != null)
				{
					XmlSerializer.SerializerData serializerData = new XmlSerializer.SerializerData();
					serializerData.Batch = generationBatch;
					array[i] = new XmlSerializer(mappings[i], serializerData);
					array2[i] = serializerData;
				}
			}
			return array;
		}

		public static XmlSerializer[] FromTypes(Type[] mappings)
		{
			XmlSerializer[] array = new XmlSerializer[mappings.Length];
			for (int i = 0; i < mappings.Length; i++)
			{
				array[i] = new XmlSerializer(mappings[i]);
			}
			return array;
		}

		protected virtual void Serialize(object o, XmlSerializationWriter writer)
		{
			if (this.customSerializer)
			{
				throw new NotImplementedException();
			}
			if (writer is XmlSerializationWriterInterpreter)
			{
				((XmlSerializationWriterInterpreter)writer).WriteRoot(o);
			}
			else
			{
				this.serializerData.WriterMethod.Invoke(writer, new object[] { o });
			}
		}

		public void Serialize(Stream stream, object o)
		{
			this.Serialize(new XmlTextWriter(stream, Encoding.Default)
			{
				Formatting = Formatting.Indented
			}, o, null);
		}

		public void Serialize(TextWriter textWriter, object o)
		{
			this.Serialize(new XmlTextWriter(textWriter)
			{
				Formatting = Formatting.Indented
			}, o, null);
		}

		public void Serialize(XmlWriter xmlWriter, object o)
		{
			this.Serialize(xmlWriter, o, null);
		}

		public void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces)
		{
			this.Serialize(new XmlTextWriter(stream, Encoding.Default)
			{
				Formatting = Formatting.Indented
			}, o, namespaces);
		}

		public void Serialize(TextWriter textWriter, object o, XmlSerializerNamespaces namespaces)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(textWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			this.Serialize(xmlTextWriter, o, namespaces);
			xmlTextWriter.Flush();
		}

		public void Serialize(XmlWriter writer, object o, XmlSerializerNamespaces namespaces)
		{
			try
			{
				XmlSerializationWriter xmlSerializationWriter;
				if (this.customSerializer)
				{
					xmlSerializationWriter = this.CreateWriter();
				}
				else
				{
					xmlSerializationWriter = this.CreateWriter(this.typeMapping);
				}
				if (namespaces == null || namespaces.Count == 0)
				{
					namespaces = new XmlSerializerNamespaces();
					namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
					namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
				}
				xmlSerializationWriter.Initialize(writer, namespaces);
				this.Serialize(o, xmlSerializationWriter);
				writer.Flush();
			}
			catch (Exception innerException)
			{
				if (innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				if (innerException is InvalidOperationException || innerException is InvalidCastException)
				{
					throw new InvalidOperationException("There was an error generating the XML document.", innerException);
				}
				throw;
			}
		}

		[MonoTODO]
		public object Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object Deserialize(XmlReader xmlReader, string encodingStyle)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Evidence evidence)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Type type)
		{
			throw new NotImplementedException();
		}

		public static Assembly GenerateSerializer(Type[] types, XmlMapping[] mappings)
		{
			return XmlSerializer.GenerateSerializer(types, mappings, null);
		}

		[MonoTODO]
		public static Assembly GenerateSerializer(Type[] types, XmlMapping[] mappings, CompilerParameters parameters)
		{
			XmlSerializer.GenerationBatch generationBatch = new XmlSerializer.GenerationBatch();
			generationBatch.Maps = mappings;
			generationBatch.Datas = new XmlSerializer.SerializerData[mappings.Length];
			for (int i = 0; i < mappings.Length; i++)
			{
				XmlSerializer.SerializerData serializerData = new XmlSerializer.SerializerData();
				serializerData.Batch = generationBatch;
				generationBatch.Datas[i] = serializerData;
			}
			return XmlSerializer.GenerateSerializers(generationBatch, parameters);
		}

		public static string GetXmlSerializerAssemblyName(Type type)
		{
			return type.Assembly.GetName().Name + ".XmlSerializers";
		}

		public static string GetXmlSerializerAssemblyName(Type type, string defaultNamespace)
		{
			return XmlSerializer.GetXmlSerializerAssemblyName(type) + "." + defaultNamespace.GetHashCode();
		}

		[MonoTODO]
		public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("")]
		public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
		{
			throw new NotImplementedException();
		}

		private XmlSerializationWriter CreateWriter(XmlMapping typeMapping)
		{
			lock (this)
			{
				if (this.serializerData != null)
				{
					XmlSerializer.SerializerData serializerData = this.serializerData;
					XmlSerializationWriter xmlSerializationWriter;
					lock (serializerData)
					{
						xmlSerializationWriter = this.serializerData.CreateWriter();
					}
					if (xmlSerializationWriter != null)
					{
						return xmlSerializationWriter;
					}
				}
			}
			if (!typeMapping.Source.CanBeGenerated || XmlSerializer.generationThreshold == -1)
			{
				return new XmlSerializationWriterInterpreter(typeMapping);
			}
			this.CheckGeneratedTypes(typeMapping);
			lock (this)
			{
				XmlSerializer.SerializerData serializerData2 = this.serializerData;
				XmlSerializationWriter xmlSerializationWriter;
				lock (serializerData2)
				{
					xmlSerializationWriter = this.serializerData.CreateWriter();
				}
				if (xmlSerializationWriter != null)
				{
					return xmlSerializationWriter;
				}
				if (!XmlSerializer.generatorFallback)
				{
					throw new InvalidOperationException("Error while generating serializer");
				}
			}
			return new XmlSerializationWriterInterpreter(typeMapping);
		}

		private XmlSerializationReader CreateReader(XmlMapping typeMapping)
		{
			lock (this)
			{
				if (this.serializerData != null)
				{
					XmlSerializer.SerializerData serializerData = this.serializerData;
					XmlSerializationReader xmlSerializationReader;
					lock (serializerData)
					{
						xmlSerializationReader = this.serializerData.CreateReader();
					}
					if (xmlSerializationReader != null)
					{
						return xmlSerializationReader;
					}
				}
			}
			if (!typeMapping.Source.CanBeGenerated || XmlSerializer.generationThreshold == -1)
			{
				return new XmlSerializationReaderInterpreter(typeMapping);
			}
			this.CheckGeneratedTypes(typeMapping);
			lock (this)
			{
				XmlSerializer.SerializerData serializerData2 = this.serializerData;
				XmlSerializationReader xmlSerializationReader;
				lock (serializerData2)
				{
					xmlSerializationReader = this.serializerData.CreateReader();
				}
				if (xmlSerializationReader != null)
				{
					return xmlSerializationReader;
				}
				if (!XmlSerializer.generatorFallback)
				{
					throw new InvalidOperationException("Error while generating serializer");
				}
			}
			return new XmlSerializationReaderInterpreter(typeMapping);
		}

		private void CheckGeneratedTypes(XmlMapping typeMapping)
		{
			lock (this)
			{
				if (this.serializerData == null)
				{
					Hashtable hashtable = XmlSerializer.serializerTypes;
					lock (hashtable)
					{
						this.serializerData = (XmlSerializer.SerializerData)XmlSerializer.serializerTypes[typeMapping.Source];
						if (this.serializerData == null)
						{
							this.serializerData = new XmlSerializer.SerializerData();
							XmlSerializer.serializerTypes[typeMapping.Source] = this.serializerData;
						}
					}
				}
			}
			bool flag = false;
			XmlSerializer.SerializerData serializerData = this.serializerData;
			lock (serializerData)
			{
				flag = ++this.serializerData.UsageCount == XmlSerializer.generationThreshold;
			}
			if (flag)
			{
				if (this.serializerData.Batch != null)
				{
					this.GenerateSerializersAsync(this.serializerData.Batch);
				}
				else
				{
					this.GenerateSerializersAsync(new XmlSerializer.GenerationBatch
					{
						Maps = new XmlMapping[] { typeMapping },
						Datas = new XmlSerializer.SerializerData[] { this.serializerData }
					});
				}
			}
		}

		private void GenerateSerializersAsync(XmlSerializer.GenerationBatch batch)
		{
			if (batch.Maps.Length != batch.Datas.Length)
			{
				throw new ArgumentException("batch");
			}
			lock (batch)
			{
				if (batch.Done)
				{
					return;
				}
				batch.Done = true;
			}
			if (XmlSerializer.backgroundGeneration)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunSerializerGeneration), batch);
			}
			else
			{
				this.RunSerializerGeneration(batch);
			}
		}

		private void RunSerializerGeneration(object obj)
		{
			try
			{
				XmlSerializer.GenerationBatch generationBatch = (XmlSerializer.GenerationBatch)obj;
				generationBatch = this.LoadFromSatelliteAssembly(generationBatch);
				if (generationBatch != null)
				{
					XmlSerializer.GenerateSerializers(generationBatch, null);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private static Assembly GenerateSerializers(XmlSerializer.GenerationBatch batch, CompilerParameters cp)
		{
			DateTime now = DateTime.Now;
			XmlMapping[] maps = batch.Maps;
			if (cp == null)
			{
				cp = new CompilerParameters();
				cp.IncludeDebugInformation = false;
				cp.GenerateInMemory = true;
				cp.TempFiles.KeepFiles = !XmlSerializer.deleteTempFiles;
			}
			string text = cp.TempFiles.AddExtension("cs");
			StreamWriter streamWriter = new StreamWriter(text);
			if (!XmlSerializer.deleteTempFiles)
			{
				Console.WriteLine("Generating " + text);
			}
			SerializationCodeGenerator serializationCodeGenerator = new SerializationCodeGenerator(maps);
			try
			{
				serializationCodeGenerator.GenerateSerializers(streamWriter);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Serializer could not be generated");
				Console.WriteLine(ex);
				cp.TempFiles.Delete();
				return null;
			}
			streamWriter.Close();
			CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
			ICodeCompiler codeCompiler = csharpCodeProvider.CreateCompiler();
			cp.GenerateExecutable = false;
			foreach (object obj in serializationCodeGenerator.ReferencedTypes)
			{
				Type type = (Type)obj;
				string localPath = new Uri(type.Assembly.CodeBase).LocalPath;
				if (!cp.ReferencedAssemblies.Contains(localPath))
				{
					cp.ReferencedAssemblies.Add(localPath);
				}
			}
			if (!cp.ReferencedAssemblies.Contains("System.dll"))
			{
				cp.ReferencedAssemblies.Add("System.dll");
			}
			if (!cp.ReferencedAssemblies.Contains("System.Xml"))
			{
				cp.ReferencedAssemblies.Add("System.Xml");
			}
			if (!cp.ReferencedAssemblies.Contains("System.Data"))
			{
				cp.ReferencedAssemblies.Add("System.Data");
			}
			CompilerResults compilerResults = codeCompiler.CompileAssemblyFromFile(cp, text);
			if (compilerResults.Errors.HasErrors || compilerResults.CompiledAssembly == null)
			{
				Console.WriteLine("Error while compiling generated serializer");
				foreach (object obj2 in compilerResults.Errors)
				{
					CompilerError compilerError = (CompilerError)obj2;
					Console.WriteLine(compilerError);
				}
				cp.TempFiles.Delete();
				return null;
			}
			GenerationResult[] generationResults = serializationCodeGenerator.GenerationResults;
			for (int i = 0; i < generationResults.Length; i++)
			{
				GenerationResult generationResult = generationResults[i];
				XmlSerializer.SerializerData serializerData = batch.Datas[i];
				XmlSerializer.SerializerData serializerData2 = serializerData;
				lock (serializerData2)
				{
					serializerData.WriterType = compilerResults.CompiledAssembly.GetType(generationResult.Namespace + "." + generationResult.WriterClassName);
					serializerData.ReaderType = compilerResults.CompiledAssembly.GetType(generationResult.Namespace + "." + generationResult.ReaderClassName);
					serializerData.WriterMethod = serializerData.WriterType.GetMethod(generationResult.WriteMethodName);
					serializerData.ReaderMethod = serializerData.ReaderType.GetMethod(generationResult.ReadMethodName);
					serializerData.Batch = null;
				}
			}
			cp.TempFiles.Delete();
			if (!XmlSerializer.deleteTempFiles)
			{
				Console.WriteLine("Generation finished - " + (DateTime.Now - now).TotalMilliseconds + " ms");
			}
			return compilerResults.CompiledAssembly;
		}

		private XmlSerializer.GenerationBatch LoadFromSatelliteAssembly(XmlSerializer.GenerationBatch batch)
		{
			return batch;
		}

		internal const string WsdlNamespace = "http://schemas.xmlsoap.org/wsdl/";

		internal const string EncodingNamespace = "http://schemas.xmlsoap.org/soap/encoding/";

		internal const string WsdlTypesNamespace = "http://microsoft.com/wsdl/types/";

		private static int generationThreshold;

		private static bool backgroundGeneration = true;

		private static bool deleteTempFiles = true;

		private static bool generatorFallback = true;

		private bool customSerializer;

		private XmlMapping typeMapping;

		private XmlSerializer.SerializerData serializerData;

		private static Hashtable serializerTypes = new Hashtable();

		private XmlAttributeEventHandler onUnknownAttribute;

		private XmlElementEventHandler onUnknownElement;

		private XmlNodeEventHandler onUnknownNode;

		private UnreferencedObjectEventHandler onUnreferencedObject;

		internal class SerializerData
		{
			public XmlSerializationReader CreateReader()
			{
				if (this.ReaderType != null)
				{
					return (XmlSerializationReader)Activator.CreateInstance(this.ReaderType);
				}
				if (this.Implementation != null)
				{
					return this.Implementation.Reader;
				}
				return null;
			}

			public XmlSerializationWriter CreateWriter()
			{
				if (this.WriterType != null)
				{
					return (XmlSerializationWriter)Activator.CreateInstance(this.WriterType);
				}
				if (this.Implementation != null)
				{
					return this.Implementation.Writer;
				}
				return null;
			}

			public int UsageCount;

			public Type ReaderType;

			public MethodInfo ReaderMethod;

			public Type WriterType;

			public MethodInfo WriterMethod;

			public XmlSerializer.GenerationBatch Batch;

			public XmlSerializerImplementation Implementation;
		}

		internal class GenerationBatch
		{
			public bool Done;

			public XmlMapping[] Maps;

			public XmlSerializer.SerializerData[] Datas;
		}
	}
}
