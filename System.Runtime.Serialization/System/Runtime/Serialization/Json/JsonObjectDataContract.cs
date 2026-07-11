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
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(attribute);
			object obj;
			if (num <= 467038368U)
			{
				if (num != 0U)
				{
					if (num != 398550328U)
					{
						if (num != 467038368U)
						{
							goto IL_011C;
						}
						if (!(attribute == "number"))
						{
							goto IL_011C;
						}
						obj = JsonObjectDataContract.ParseJsonNumber(jsonReader.ReadElementContentAsString());
						goto IL_013B;
					}
					else if (!(attribute == "string"))
					{
						goto IL_011C;
					}
				}
				else if (attribute != null)
				{
					goto IL_011C;
				}
				obj = jsonReader.ReadElementContentAsString();
				goto IL_013B;
			}
			if (num <= 1996966820U)
			{
				if (num != 1710517951U)
				{
					if (num == 1996966820U)
					{
						if (attribute == "null")
						{
							jsonReader.Skip();
							obj = null;
							goto IL_013B;
						}
					}
				}
				else if (attribute == "boolean")
				{
					obj = jsonReader.ReadElementContentAsBoolean();
					goto IL_013B;
				}
			}
			else if (num != 2321067302U)
			{
				if (num == 3099987130U)
				{
					if (attribute == "object")
					{
						jsonReader.Skip();
						obj = new object();
						goto IL_013B;
					}
				}
			}
			else if (attribute == "array")
			{
				return DataContractJsonSerializer.ReadJsonValue(DataContract.GetDataContract(Globals.TypeOfObjectArray), jsonReader, context);
			}
			IL_011C:
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected attribute value '{0}'.", new object[] { attribute })));
			IL_013B:
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
