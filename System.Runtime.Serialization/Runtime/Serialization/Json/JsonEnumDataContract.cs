using System;
using System.Security;

namespace System.Runtime.Serialization.Json
{
	internal class JsonEnumDataContract : JsonDataContract
	{
		[SecuritySafeCritical]
		public JsonEnumDataContract(EnumDataContract traditionalDataContract)
			: base(new JsonEnumDataContract.JsonEnumDataContractCriticalHelper(traditionalDataContract))
		{
			this.helper = base.Helper as JsonEnumDataContract.JsonEnumDataContractCriticalHelper;
		}

		public bool IsULong
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsULong;
			}
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			object obj;
			if (this.IsULong)
			{
				obj = Enum.ToObject(base.TraditionalDataContract.UnderlyingType, jsonReader.ReadElementContentAsUnsignedLong());
			}
			else
			{
				obj = Enum.ToObject(base.TraditionalDataContract.UnderlyingType, jsonReader.ReadElementContentAsLong());
			}
			if (context != null)
			{
				context.AddNewObject(obj);
			}
			return obj;
		}

		public override void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			if (this.IsULong)
			{
				jsonWriter.WriteUnsignedLong(((IConvertible)obj).ToUInt64(null));
				return;
			}
			jsonWriter.WriteLong(((IConvertible)obj).ToInt64(null));
		}

		[SecurityCritical]
		private JsonEnumDataContract.JsonEnumDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class JsonEnumDataContractCriticalHelper : JsonDataContract.JsonDataContractCriticalHelper
		{
			public JsonEnumDataContractCriticalHelper(EnumDataContract traditionalEnumDataContract)
				: base(traditionalEnumDataContract)
			{
				this.isULong = traditionalEnumDataContract.IsULong;
			}

			public bool IsULong
			{
				get
				{
					return this.isULong;
				}
			}

			private bool isULong;
		}
	}
}
