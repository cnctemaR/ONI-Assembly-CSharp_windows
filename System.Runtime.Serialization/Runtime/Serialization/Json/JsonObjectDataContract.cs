using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonObjectDataContract : JsonDataContract
	{
		public JsonObjectDataContract(DataContract traditionalDataContract)
			: base(traditionalDataContract)
		{
		}

		public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			string attribute = jsonReader.GetAttribute("type");
			object obj;
			if (!(attribute == "null"))
			{
				if (!(attribute == "boolean"))
				{
					if (!(attribute == "string") && attribute != null)
					{
						if (!(attribute == "number"))
						{
							if (!(attribute == "object"))
							{
								if (!(attribute == "array"))
								{
									throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected attribute value '{0}'.", new object[] { attribute })));
								}
								return DataContractJsonSerializer.ReadJsonValue(DataContract.GetDataContract(Globals.TypeOfObjectArray), jsonReader, context);
							}
							else
							{
								jsonReader.Skip();
								obj = new object();
							}
						}
						else
						{
							obj = JsonObjectDataContract.ParseJsonNumber(jsonReader.ReadElementContentAsString());
						}
					}
					else
					{
						obj = jsonReader.ReadElementContentAsString();
					}
				}
				else
				{
					obj = jsonReader.ReadElementContentAsBoolean();
				}
			}
			else
			{
				jsonReader.Skip();
				obj = null;
			}
			if (context != null)
			{
				context.AddNewObject(obj);
			}
			return obj;
		}

		public override void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			jsonWriter.WriteAttributeString(null, "type", null, "object");
		}

		internal static object ParseJsonNumber(string value, out TypeCode objectTypeCode)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[]
				{
					value,
					Globals.TypeOfInt
				})));
			}
			if (value.IndexOfAny(JsonGlobals.floatingPointCharacters) == -1)
			{
				int num;
				if (int.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num))
				{
					objectTypeCode = TypeCode.Int32;
					return num;
				}
				long num2;
				if (long.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num2))
				{
					objectTypeCode = TypeCode.Int64;
					return num2;
				}
			}
			decimal num3;
			if (decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num3))
			{
				objectTypeCode = TypeCode.Decimal;
				if (num3 == 0m)
				{
					double num4 = XmlConverter.ToDouble(value);
					if (num4 != 0.0)
					{
						objectTypeCode = TypeCode.Double;
						return num4;
					}
				}
				return num3;
			}
			objectTypeCode = TypeCode.Double;
			return XmlConverter.ToDouble(value);
		}

		private static object ParseJsonNumber(string value)
		{
			TypeCode typeCode;
			return JsonObjectDataContract.ParseJsonNumber(value, out typeCode);
		}
	}
}
