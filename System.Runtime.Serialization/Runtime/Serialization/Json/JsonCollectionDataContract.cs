using System;
using System.Security;
using System.Threading;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonCollectionDataContract : JsonDataContract
	{
		[SecuritySafeCritical]
		public JsonCollectionDataContract(CollectionDataContract traditionalDataContract)
			: base(new JsonCollectionDataContract.JsonCollectionDataContractCriticalHelper(traditionalDataContract))
		{
			this.helper = base.Helper as JsonCollectionDataContract.JsonCollectionDataContractCriticalHelper;
		}

		internal JsonFormatCollectionReaderDelegate JsonFormatReaderDelegate
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
							if (this.TraditionalCollectionDataContract.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.TraditionalCollectionDataContract.DeserializationExceptionMessage, null);
							}
							JsonFormatCollectionReaderDelegate jsonFormatCollectionReaderDelegate = new JsonFormatReaderGenerator().GenerateCollectionReader(this.TraditionalCollectionDataContract);
							Thread.MemoryBarrier();
							this.helper.JsonFormatReaderDelegate = jsonFormatCollectionReaderDelegate;
						}
					}
				}
				return this.helper.JsonFormatReaderDelegate;
			}
		}

		internal JsonFormatGetOnlyCollectionReaderDelegate JsonFormatGetOnlyReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.JsonFormatGetOnlyReaderDelegate == null)
				{
					lock (this)
					{
						if (this.helper.JsonFormatGetOnlyReaderDelegate == null)
						{
							CollectionKind kind = this.TraditionalCollectionDataContract.Kind;
							if (base.TraditionalDataContract.UnderlyingType.IsInterface && (kind == CollectionKind.Enumerable || kind == CollectionKind.Collection || kind == CollectionKind.GenericEnumerable))
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{0}', get-only collection must have an Add method.", new object[] { DataContract.GetClrTypeFullName(base.TraditionalDataContract.UnderlyingType) })));
							}
							if (this.TraditionalCollectionDataContract.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.TraditionalCollectionDataContract.DeserializationExceptionMessage, null);
							}
							JsonFormatGetOnlyCollectionReaderDelegate jsonFormatGetOnlyCollectionReaderDelegate = new JsonFormatReaderGenerator().GenerateGetOnlyCollectionReader(this.TraditionalCollectionDataContract);
							Thread.MemoryBarrier();
							this.helper.JsonFormatGetOnlyReaderDelegate = jsonFormatGetOnlyCollectionReaderDelegate;
						}
					}
				}
				return this.helper.JsonFormatGetOnlyReaderDelegate;
			}
		}

		internal JsonFormatCollectionWriterDelegate JsonFormatWriterDelegate
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
							JsonFormatCollectionWriterDelegate jsonFormatCollectionWriterDelegate = new JsonFormatWriterGenerator().GenerateCollectionWriter(this.TraditionalCollectionDataContract);
							Thread.MemoryBarrier();
							this.helper.JsonFormatWriterDelegate = jsonFormatCollectionWriterDelegate;
						}
					}
				}
				return this.helper.JsonFormatWriterDelegate;
			}
		}

		private CollectionDataContract TraditionalCollectionDataContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TraditionalCollectionDataContract;
			}
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			jsonReader.Read();
			object obj = null;
			if (context.IsGetOnlyCollection)
			{
				context.IsGetOnlyCollection = false;
				this.JsonFormatGetOnlyReaderDelegate(jsonReader, context, XmlDictionaryString.Empty, JsonGlobals.itemDictionaryString, this.TraditionalCollectionDataContract);
			}
			else
			{
				obj = this.JsonFormatReaderDelegate(jsonReader, context, XmlDictionaryString.Empty, JsonGlobals.itemDictionaryString, this.TraditionalCollectionDataContract);
			}
			jsonReader.ReadEndElement();
			return obj;
		}

		public override void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			context.IsGetOnlyCollection = false;
			this.JsonFormatWriterDelegate(jsonWriter, obj, context, this.TraditionalCollectionDataContract);
		}

		[SecurityCritical]
		private JsonCollectionDataContract.JsonCollectionDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class JsonCollectionDataContractCriticalHelper : JsonDataContract.JsonDataContractCriticalHelper
		{
			public JsonCollectionDataContractCriticalHelper(CollectionDataContract traditionalDataContract)
				: base(traditionalDataContract)
			{
				this.traditionalCollectionDataContract = traditionalDataContract;
			}

			internal JsonFormatCollectionReaderDelegate JsonFormatReaderDelegate
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

			internal JsonFormatGetOnlyCollectionReaderDelegate JsonFormatGetOnlyReaderDelegate
			{
				get
				{
					return this.jsonFormatGetOnlyReaderDelegate;
				}
				set
				{
					this.jsonFormatGetOnlyReaderDelegate = value;
				}
			}

			internal JsonFormatCollectionWriterDelegate JsonFormatWriterDelegate
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

			internal CollectionDataContract TraditionalCollectionDataContract
			{
				get
				{
					return this.traditionalCollectionDataContract;
				}
			}

			private JsonFormatCollectionReaderDelegate jsonFormatReaderDelegate;

			private JsonFormatGetOnlyCollectionReaderDelegate jsonFormatGetOnlyReaderDelegate;

			private JsonFormatCollectionWriterDelegate jsonFormatWriterDelegate;

			private CollectionDataContract traditionalCollectionDataContract;
		}
	}
}
