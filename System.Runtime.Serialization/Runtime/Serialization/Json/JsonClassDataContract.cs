using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonClassDataContract : JsonDataContract
	{
		[SecuritySafeCritical]
		public JsonClassDataContract(ClassDataContract traditionalDataContract)
			: base(new JsonClassDataContract.JsonClassDataContractCriticalHelper(traditionalDataContract))
		{
			this.helper = base.Helper as JsonClassDataContract.JsonClassDataContractCriticalHelper;
		}

		internal JsonFormatClassReaderDelegate JsonFormatReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.JsonFormatReaderDelegate == null)
				{
					lock (this)
					{
						if (this.helper.JsonFormatReaderDelegate == null)
						{
							if (this.TraditionalClassDataContract.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.TraditionalClassDataContract.DeserializationExceptionMessage, null);
							}
							JsonFormatClassReaderDelegate jsonFormatClassReaderDelegate = new JsonFormatReaderGenerator().GenerateClassReader(this.TraditionalClassDataContract);
							Thread.MemoryBarrier();
							this.helper.JsonFormatReaderDelegate = jsonFormatClassReaderDelegate;
						}
					}
				}
				return this.helper.JsonFormatReaderDelegate;
			}
		}

		internal JsonFormatClassWriterDelegate JsonFormatWriterDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.JsonFormatWriterDelegate == null)
				{
					lock (this)
					{
						if (this.helper.JsonFormatWriterDelegate == null)
						{
							JsonFormatClassWriterDelegate jsonFormatClassWriterDelegate = new JsonFormatWriterGenerator().GenerateClassWriter(this.TraditionalClassDataContract);
							Thread.MemoryBarrier();
							this.helper.JsonFormatWriterDelegate = jsonFormatClassWriterDelegate;
						}
					}
				}
				return this.helper.JsonFormatWriterDelegate;
			}
		}

		internal XmlDictionaryString[] MemberNames
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.MemberNames;
			}
		}

		internal override string TypeName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TypeName;
			}
		}

		private ClassDataContract TraditionalClassDataContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TraditionalClassDataContract;
			}
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			jsonReader.Read();
			object obj = this.JsonFormatReaderDelegate(jsonReader, context, XmlDictionaryString.Empty, this.MemberNames);
			jsonReader.ReadEndElement();
			return obj;
		}

		public override void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			jsonWriter.WriteAttributeString(null, "type", null, "object");
			this.JsonFormatWriterDelegate(jsonWriter, obj, context, this.TraditionalClassDataContract, this.MemberNames);
		}

		[SecurityCritical]
		private JsonClassDataContract.JsonClassDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class JsonClassDataContractCriticalHelper : JsonDataContract.JsonDataContractCriticalHelper
		{
			public JsonClassDataContractCriticalHelper(ClassDataContract traditionalDataContract)
				: base(traditionalDataContract)
			{
				this.typeName = (string.IsNullOrEmpty(traditionalDataContract.Namespace.Value) ? traditionalDataContract.Name.Value : (traditionalDataContract.Name.Value + ":" + XmlObjectSerializerWriteContextComplexJson.TruncateDefaultDataContractNamespace(traditionalDataContract.Namespace.Value)));
				this.traditionalClassDataContract = traditionalDataContract;
				this.CopyMembersAndCheckDuplicateNames();
			}

			internal JsonFormatClassReaderDelegate JsonFormatReaderDelegate
			{
				get
				{
					return this.jsonFormatReaderDelegate;
				}
				set
				{
					this.jsonFormatReaderDelegate = value;
				}
			}

			internal JsonFormatClassWriterDelegate JsonFormatWriterDelegate
			{
				get
				{
					return this.jsonFormatWriterDelegate;
				}
				set
				{
					this.jsonFormatWriterDelegate = value;
				}
			}

			internal XmlDictionaryString[] MemberNames
			{
				get
				{
					return this.memberNames;
				}
			}

			internal ClassDataContract TraditionalClassDataContract
			{
				get
				{
					return this.traditionalClassDataContract;
				}
			}

			private void CopyMembersAndCheckDuplicateNames()
			{
				if (this.traditionalClassDataContract.MemberNames != null)
				{
					int num = this.traditionalClassDataContract.MemberNames.Length;
					Dictionary<string, object> dictionary = new Dictionary<string, object>(num);
					XmlDictionaryString[] array = new XmlDictionaryString[num];
					for (int i = 0; i < num; i++)
					{
						if (dictionary.ContainsKey(this.traditionalClassDataContract.MemberNames[i].Value))
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Duplicate member, including '{1}', is found in JSON input, in type '{0}'.", new object[]
							{
								DataContract.GetClrTypeFullName(this.traditionalClassDataContract.UnderlyingType),
								this.traditionalClassDataContract.MemberNames[i].Value
							})));
						}
						dictionary.Add(this.traditionalClassDataContract.MemberNames[i].Value, null);
						array[i] = DataContractJsonSerializer.ConvertXmlNameToJsonName(this.traditionalClassDataContract.MemberNames[i]);
					}
					this.memberNames = array;
				}
			}

			private JsonFormatClassReaderDelegate jsonFormatReaderDelegate;

			private JsonFormatClassWriterDelegate jsonFormatWriterDelegate;

			private XmlDictionaryString[] memberNames;

			private ClassDataContract traditionalClassDataContract;

			private string typeName;
		}
	}
}
