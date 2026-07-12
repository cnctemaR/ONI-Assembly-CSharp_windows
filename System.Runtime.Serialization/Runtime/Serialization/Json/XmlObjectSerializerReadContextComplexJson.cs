using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class XmlObjectSerializerReadContextComplexJson : XmlObjectSerializerReadContextComplex
	{
		public XmlObjectSerializerReadContextComplexJson(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
			: base(serializer, serializer.MaxItemsInObjectGraph, new StreamingContext(StreamingContextStates.All), serializer.IgnoreExtensionDataObject)
		{
			this.rootTypeDataContract = rootTypeDataContract;
			this.serializerKnownTypeList = serializer.knownTypeList;
			this.dataContractSurrogate = serializer.DataContractSurrogate;
			this.dateTimeFormat = serializer.DateTimeFormat;
			this.useSimpleDictionaryFormat = serializer.UseSimpleDictionaryFormat;
		}

		internal IList<Type> SerializerKnownTypeList
		{
			get
			{
				return this.serializerKnownTypeList;
			}
		}

		public bool UseSimpleDictionaryFormat
		{
			get
			{
				return this.useSimpleDictionaryFormat;
			}
		}

		protected override void StartReadExtensionDataValue(XmlReaderDelegator xmlReader)
		{
			this.extensionDataValueType = xmlReader.GetAttribute("type");
		}

		protected override IDataNode ReadPrimitiveExtensionDataValue(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			string text = this.extensionDataValueType;
			IDataNode dataNode;
			if (text != null && !(text == "string"))
			{
				if (!(text == "boolean"))
				{
					if (!(text == "number"))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected attribute value '{0}'.", new object[] { this.extensionDataValueType })));
					}
					dataNode = this.ReadNumericalPrimitiveExtensionDataValue(xmlReader);
				}
				else
				{
					dataNode = new DataNode<bool>(xmlReader.ReadContentAsBoolean());
				}
			}
			else
			{
				dataNode = new DataNode<string>(xmlReader.ReadContentAsString());
			}
			xmlReader.ReadEndElement();
			return dataNode;
		}

		private IDataNode ReadNumericalPrimitiveExtensionDataValue(XmlReaderDelegator xmlReader)
		{
			TypeCode typeCode;
			object obj = JsonObjectDataContract.ParseJsonNumber(xmlReader.ReadContentAsString(), out typeCode);
			switch (typeCode)
			{
			case TypeCode.SByte:
				return new DataNode<sbyte>((sbyte)obj);
			case TypeCode.Byte:
				return new DataNode<byte>((byte)obj);
			case TypeCode.Int16:
				return new DataNode<short>((short)obj);
			case TypeCode.UInt16:
				return new DataNode<ushort>((ushort)obj);
			case TypeCode.Int32:
				return new DataNode<int>((int)obj);
			case TypeCode.UInt32:
				return new DataNode<uint>((uint)obj);
			case TypeCode.Int64:
				return new DataNode<long>((long)obj);
			case TypeCode.UInt64:
				return new DataNode<ulong>((ulong)obj);
			case TypeCode.Single:
				return new DataNode<float>((float)obj);
			case TypeCode.Double:
				return new DataNode<double>((double)obj);
			case TypeCode.Decimal:
				return new DataNode<decimal>((decimal)obj);
			default:
				throw Fx.AssertAndThrow("JsonObjectDataContract.ParseJsonNumber shouldn't return a TypeCode that we're not expecting");
			}
		}

		internal static XmlObjectSerializerReadContextComplexJson CreateContext(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
		{
			return new XmlObjectSerializerReadContextComplexJson(serializer, rootTypeDataContract);
		}

		internal override int GetArraySize()
		{
			return -1;
		}

		protected override object ReadDataContractValue(DataContract dataContract, XmlReaderDelegator reader)
		{
			return DataContractJsonSerializer.ReadJsonValue(dataContract, reader, this);
		}

		internal override void ReadAttributes(XmlReaderDelegator xmlReader)
		{
			if (this.attributes == null)
			{
				this.attributes = new Attributes();
			}
			this.attributes.Reset();
			if (xmlReader.MoveToAttribute("type") && xmlReader.Value == "null")
			{
				this.attributes.XsiNil = true;
			}
			else if (xmlReader.MoveToAttribute("__type"))
			{
				XmlQualifiedName xmlQualifiedName = JsonReaderDelegator.ParseQualifiedName(xmlReader.Value);
				this.attributes.XsiTypeName = xmlQualifiedName.Name;
				string text = xmlQualifiedName.Namespace;
				if (!string.IsNullOrEmpty(text))
				{
					char c = text[0];
					if (c != '#')
					{
						if (c == '\\')
						{
							if (text.Length >= 2)
							{
								char c2 = text[1];
								if (c2 == '#' || c2 == '\\')
								{
									text = text.Substring(1);
								}
							}
						}
					}
					else
					{
						text = "http://schemas.datacontract.org/2004/07/" + text.Substring(1);
					}
				}
				this.attributes.XsiTypeNamespace = text;
			}
			xmlReader.MoveToElement();
		}

		public int GetJsonMemberIndex(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, int memberIndex, ExtensionDataObject extensionData)
		{
			int num = memberNames.Length;
			if (num != 0)
			{
				int i = 0;
				int num2 = (memberIndex + 1) % num;
				while (i < num)
				{
					if (xmlReader.IsStartElement(memberNames[num2], XmlDictionaryString.Empty))
					{
						return num2;
					}
					i++;
					num2 = (num2 + 1) % num;
				}
				string text;
				if (XmlObjectSerializerReadContextComplexJson.TryGetJsonLocalName(xmlReader, out text))
				{
					int j = 0;
					int num3 = (memberIndex + 1) % num;
					while (j < num)
					{
						if (memberNames[num3].Value == text)
						{
							return num3;
						}
						j++;
						num3 = (num3 + 1) % num;
					}
				}
			}
			base.HandleMemberNotFound(xmlReader, extensionData, memberIndex);
			return num;
		}

		internal static bool TryGetJsonLocalName(XmlReaderDelegator xmlReader, out string name)
		{
			if (xmlReader.IsStartElement(JsonGlobals.itemDictionaryString, JsonGlobals.itemDictionaryString) && xmlReader.MoveToAttribute("item"))
			{
				name = xmlReader.Value;
				return true;
			}
			name = null;
			return false;
		}

		public static string GetJsonMemberName(XmlReaderDelegator xmlReader)
		{
			string localName;
			if (!XmlObjectSerializerReadContextComplexJson.TryGetJsonLocalName(xmlReader, out localName))
			{
				localName = xmlReader.LocalName;
			}
			return localName;
		}

		public static void ThrowMissingRequiredMembers(object obj, XmlDictionaryString[] memberNames, byte[] expectedElements, byte[] requiredElements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < memberNames.Length; i++)
			{
				if (XmlObjectSerializerReadContextComplexJson.IsBitSet(expectedElements, i) && XmlObjectSerializerReadContextComplexJson.IsBitSet(requiredElements, i))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(memberNames[i]);
					num++;
				}
			}
			if (num == 1)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Required member {1} in type '{0}' is not found.", new object[]
				{
					DataContract.GetClrTypeFullName(obj.GetType()),
					stringBuilder.ToString()
				})));
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Required members {0} in type '{1}' are not found.", new object[]
			{
				DataContract.GetClrTypeFullName(obj.GetType()),
				stringBuilder.ToString()
			})));
		}

		public static void ThrowDuplicateMemberException(object obj, XmlDictionaryString[] memberNames, int memberIndex)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Duplicate member '{0}' is found in JSON input.", new object[]
			{
				DataContract.GetClrTypeFullName(obj.GetType()),
				memberNames[memberIndex]
			})));
		}

		[SecuritySafeCritical]
		private static bool IsBitSet(byte[] bytes, int bitIndex)
		{
			return BitFlagsGenerator.IsBitSet(bytes, bitIndex);
		}

		protected override bool IsReadingCollectionExtensionData(XmlReaderDelegator xmlReader)
		{
			return xmlReader.GetAttribute("type") == "array";
		}

		protected override bool IsReadingClassExtensionData(XmlReaderDelegator xmlReader)
		{
			return xmlReader.GetAttribute("type") == "object";
		}

		protected override XmlReaderDelegator CreateReaderDelegatorForReader(XmlReader xmlReader)
		{
			return new JsonReaderDelegator(xmlReader, this.dateTimeFormat);
		}

		internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContract = base.GetDataContract(typeHandle, type);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		internal override DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContractSkipValidation = base.GetDataContractSkipValidation(typeId, typeHandle, type);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContractSkipValidation);
			return dataContractSkipValidation;
		}

		internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
		{
			DataContract dataContract = base.GetDataContract(id, typeHandle);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		protected override DataContract ResolveDataContractFromRootDataContract(XmlQualifiedName typeQName)
		{
			return XmlObjectSerializerWriteContextComplexJson.ResolveJsonDataContractFromRootDataContract(this, typeQName, this.rootTypeDataContract);
		}

		private string extensionDataValueType;

		private DateTimeFormat dateTimeFormat;

		private bool useSimpleDictionaryFormat;
	}
}
