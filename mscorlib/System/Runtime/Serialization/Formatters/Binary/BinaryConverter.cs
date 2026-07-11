using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal static class BinaryConverter
	{
		internal static BinaryTypeEnum GetBinaryTypeInfo(Type type, WriteObjectInfo objectInfo, string typeName, ObjectWriter objectWriter, out object typeInformation, out int assemId)
		{
			assemId = 0;
			typeInformation = null;
			BinaryTypeEnum binaryTypeEnum;
			if (type == Converter.typeofString)
			{
				binaryTypeEnum = BinaryTypeEnum.String;
			}
			else if ((objectInfo == null || (objectInfo != null && !objectInfo.isSi)) && type == Converter.typeofObject)
			{
				binaryTypeEnum = BinaryTypeEnum.Object;
			}
			else if (type == Converter.typeofStringArray)
			{
				binaryTypeEnum = BinaryTypeEnum.StringArray;
			}
			else if (type == Converter.typeofObjectArray)
			{
				binaryTypeEnum = BinaryTypeEnum.ObjectArray;
			}
			else if (Converter.IsPrimitiveArray(type, out typeInformation))
			{
				binaryTypeEnum = BinaryTypeEnum.PrimitiveArray;
			}
			else
			{
				InternalPrimitiveTypeE internalPrimitiveTypeE = objectWriter.ToCode(type);
				if (internalPrimitiveTypeE == InternalPrimitiveTypeE.Invalid)
				{
					string text;
					if (objectInfo == null)
					{
						text = type.Assembly.FullName;
						typeInformation = type.FullName;
					}
					else
					{
						text = objectInfo.GetAssemblyString();
						typeInformation = objectInfo.GetTypeFullName();
					}
					if (text.Equals(Converter.urtAssemblyString))
					{
						binaryTypeEnum = BinaryTypeEnum.ObjectUrt;
						assemId = 0;
					}
					else
					{
						binaryTypeEnum = BinaryTypeEnum.ObjectUser;
						assemId = (int)objectInfo.assemId;
						if (assemId == 0)
						{
							throw new SerializationException(Environment.GetResourceString("No assembly ID for object type '{0}'.", new object[] { typeInformation }));
						}
					}
				}
				else
				{
					binaryTypeEnum = BinaryTypeEnum.Primitive;
					typeInformation = internalPrimitiveTypeE;
				}
			}
			return binaryTypeEnum;
		}

		internal static BinaryTypeEnum GetParserBinaryTypeInfo(Type type, out object typeInformation)
		{
			typeInformation = null;
			BinaryTypeEnum binaryTypeEnum;
			if (type == Converter.typeofString)
			{
				binaryTypeEnum = BinaryTypeEnum.String;
			}
			else if (type == Converter.typeofObject)
			{
				binaryTypeEnum = BinaryTypeEnum.Object;
			}
			else if (type == Converter.typeofObjectArray)
			{
				binaryTypeEnum = BinaryTypeEnum.ObjectArray;
			}
			else if (type == Converter.typeofStringArray)
			{
				binaryTypeEnum = BinaryTypeEnum.StringArray;
			}
			else if (Converter.IsPrimitiveArray(type, out typeInformation))
			{
				binaryTypeEnum = BinaryTypeEnum.PrimitiveArray;
			}
			else
			{
				InternalPrimitiveTypeE internalPrimitiveTypeE = Converter.ToCode(type);
				if (internalPrimitiveTypeE == InternalPrimitiveTypeE.Invalid)
				{
					if (Assembly.GetAssembly(type) == Converter.urtAssembly)
					{
						binaryTypeEnum = BinaryTypeEnum.ObjectUrt;
					}
					else
					{
						binaryTypeEnum = BinaryTypeEnum.ObjectUser;
					}
					typeInformation = type.FullName;
				}
				else
				{
					binaryTypeEnum = BinaryTypeEnum.Primitive;
					typeInformation = internalPrimitiveTypeE;
				}
			}
			return binaryTypeEnum;
		}

		internal static void WriteTypeInfo(BinaryTypeEnum binaryTypeEnum, object typeInformation, int assemId, __BinaryWriter sout)
		{
			switch (binaryTypeEnum)
			{
			case BinaryTypeEnum.Primitive:
			case BinaryTypeEnum.PrimitiveArray:
				sout.WriteByte((byte)((InternalPrimitiveTypeE)typeInformation));
				return;
			case BinaryTypeEnum.String:
			case BinaryTypeEnum.Object:
			case BinaryTypeEnum.ObjectArray:
			case BinaryTypeEnum.StringArray:
				return;
			case BinaryTypeEnum.ObjectUrt:
				sout.WriteString(typeInformation.ToString());
				return;
			case BinaryTypeEnum.ObjectUser:
				sout.WriteString(typeInformation.ToString());
				sout.WriteInt32(assemId);
				return;
			default:
				throw new SerializationException(Environment.GetResourceString("Invalid write type request '{0}'.", new object[] { binaryTypeEnum.ToString() }));
			}
		}

		internal static object ReadTypeInfo(BinaryTypeEnum binaryTypeEnum, __BinaryParser input, out int assemId)
		{
			object obj = null;
			int num = 0;
			switch (binaryTypeEnum)
			{
			case BinaryTypeEnum.Primitive:
			case BinaryTypeEnum.PrimitiveArray:
				obj = (InternalPrimitiveTypeE)input.ReadByte();
				break;
			case BinaryTypeEnum.String:
			case BinaryTypeEnum.Object:
			case BinaryTypeEnum.ObjectArray:
			case BinaryTypeEnum.StringArray:
				break;
			case BinaryTypeEnum.ObjectUrt:
				obj = input.ReadString();
				break;
			case BinaryTypeEnum.ObjectUser:
				obj = input.ReadString();
				num = input.ReadInt32();
				break;
			default:
				throw new SerializationException(Environment.GetResourceString("Invalid read type request '{0}'.", new object[] { binaryTypeEnum.ToString() }));
			}
			assemId = num;
			return obj;
		}

		[SecurityCritical]
		internal static void TypeFromInfo(BinaryTypeEnum binaryTypeEnum, object typeInformation, ObjectReader objectReader, BinaryAssemblyInfo assemblyInfo, out InternalPrimitiveTypeE primitiveTypeEnum, out string typeString, out Type type, out bool isVariant)
		{
			isVariant = false;
			primitiveTypeEnum = InternalPrimitiveTypeE.Invalid;
			typeString = null;
			type = null;
			switch (binaryTypeEnum)
			{
			case BinaryTypeEnum.Primitive:
				primitiveTypeEnum = (InternalPrimitiveTypeE)typeInformation;
				typeString = Converter.ToComType(primitiveTypeEnum);
				type = Converter.ToType(primitiveTypeEnum);
				return;
			case BinaryTypeEnum.String:
				type = Converter.typeofString;
				return;
			case BinaryTypeEnum.Object:
				type = Converter.typeofObject;
				isVariant = true;
				return;
			case BinaryTypeEnum.ObjectUrt:
			case BinaryTypeEnum.ObjectUser:
				if (typeInformation != null)
				{
					typeString = typeInformation.ToString();
					type = objectReader.GetType(assemblyInfo, typeString);
					if (type == Converter.typeofObject)
					{
						isVariant = true;
						return;
					}
				}
				return;
			case BinaryTypeEnum.ObjectArray:
				type = Converter.typeofObjectArray;
				return;
			case BinaryTypeEnum.StringArray:
				type = Converter.typeofStringArray;
				return;
			case BinaryTypeEnum.PrimitiveArray:
				primitiveTypeEnum = (InternalPrimitiveTypeE)typeInformation;
				type = Converter.ToArrayType(primitiveTypeEnum);
				return;
			default:
				throw new SerializationException(Environment.GetResourceString("Invalid read type request '{0}'.", new object[] { binaryTypeEnum.ToString() }));
			}
		}
	}
}
