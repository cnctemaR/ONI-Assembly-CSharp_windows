using System;

namespace System.Runtime.Serialization.Json
{
	internal class JsonUriDataContract : JsonDataContract
	{
		public JsonUriDataContract(UriDataContract traditionalUriDataContract)
			: base(traditionalUriDataContract)
		{
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			if (context != null)
			{
				return JsonDataContract.HandleReadValue(jsonReader.ReadElementContentAsUri(), context);
			}
			if (!JsonDataContract.TryReadNullAtTopLevel(jsonReader))
			{
				return jsonReader.ReadElementContentAsUri();
			}
			return null;
		}
	}
}
