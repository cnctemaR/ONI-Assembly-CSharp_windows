using System;

namespace System.Runtime.Serialization.Json
{
	internal class JsonQNameDataContract : JsonDataContract
	{
		public JsonQNameDataContract(QNameDataContract traditionalQNameDataContract)
			: base(traditionalQNameDataContract)
		{
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			if (context != null)
			{
				return JsonDataContract.HandleReadValue(jsonReader.ReadElementContentAsQName(), context);
			}
			if (!JsonDataContract.TryReadNullAtTopLevel(jsonReader))
			{
				return jsonReader.ReadElementContentAsQName();
			}
			return null;
		}
	}
}
