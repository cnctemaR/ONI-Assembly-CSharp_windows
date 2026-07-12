using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonXmlDataContract : JsonDataContract
	{
		public JsonXmlDataContract(XmlDataContract traditionalXmlDataContract)
			: base(traditionalXmlDataContract)
		{
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			string text = jsonReader.ReadElementContentAsString();
			DataContractSerializer dataContractSerializer = new DataContractSerializer(base.TraditionalDataContract.UnderlyingType, this.GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList), 1, false, false, null);
			MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
			XmlDictionaryReaderQuotas readerQuotas = ((JsonReaderDelegator)jsonReader).ReaderQuotas;
			object obj;
			if (readerQuotas == null)
			{
				obj = dataContractSerializer.ReadObject(memoryStream);
			}
			else
			{
				obj = dataContractSerializer.ReadObject(XmlDictionaryReader.CreateTextReader(memoryStream, readerQuotas));
			}
			if (context != null)
			{
				context.AddNewObject(obj);
			}
			return obj;
		}

		public override void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			XmlObjectSerializer xmlObjectSerializer = new DataContractSerializer(Type.GetTypeFromHandle(declaredTypeHandle), this.GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList), 1, false, false, null);
			MemoryStream memoryStream = new MemoryStream();
			xmlObjectSerializer.WriteObject(memoryStream, obj);
			memoryStream.Position = 0L;
			string text = new StreamReader(memoryStream).ReadToEnd();
			jsonWriter.WriteString(text);
		}

		private List<Type> GetKnownTypesFromContext(XmlObjectSerializerContext context, IList<Type> serializerKnownTypeList)
		{
			List<Type> list = new List<Type>();
			if (context != null)
			{
				List<XmlQualifiedName> list2 = new List<XmlQualifiedName>();
				Dictionary<XmlQualifiedName, DataContract>[] dataContractDictionaries = context.scopedKnownTypes.dataContractDictionaries;
				if (dataContractDictionaries != null)
				{
					foreach (Dictionary<XmlQualifiedName, DataContract> dictionary in dataContractDictionaries)
					{
						if (dictionary != null)
						{
							foreach (KeyValuePair<XmlQualifiedName, DataContract> keyValuePair in dictionary)
							{
								if (!list2.Contains(keyValuePair.Key))
								{
									list2.Add(keyValuePair.Key);
									list.Add(keyValuePair.Value.UnderlyingType);
								}
							}
						}
					}
				}
				if (serializerKnownTypeList != null)
				{
					list.AddRange(serializerKnownTypeList);
				}
			}
			return list;
		}
	}
}
