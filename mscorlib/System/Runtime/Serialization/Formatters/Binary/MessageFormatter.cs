using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class MessageFormatter
	{
		public static void WriteMethodCall(BinaryWriter writer, object obj, Header[] headers, ISurrogateSelector surrogateSelector, StreamingContext context, FormatterAssemblyStyle assemblyFormat, FormatterTypeStyle typeFormat)
		{
			IMethodCallMessage methodCallMessage = (IMethodCallMessage)obj;
			writer.Write(21);
			int num = 0;
			object obj2 = null;
			object[] array = null;
			MethodFlags methodFlags;
			if (methodCallMessage.LogicalCallContext != null && methodCallMessage.LogicalCallContext.HasInfo)
			{
				methodFlags = MethodFlags.IncludesLogicalCallContext;
				num++;
			}
			else
			{
				methodFlags = MethodFlags.ExcludeLogicalCallContext;
			}
			if (RemotingServices.IsMethodOverloaded(methodCallMessage))
			{
				num++;
				methodFlags |= MethodFlags.IncludesSignature;
			}
			if (methodCallMessage.Properties.Count > MethodCallDictionary.InternalKeys.Length)
			{
				array = MessageFormatter.GetExtraProperties(methodCallMessage.Properties, MethodCallDictionary.InternalKeys);
				num++;
			}
			if (methodCallMessage.MethodBase.IsGenericMethod)
			{
				num++;
				methodFlags |= MethodFlags.GenericArguments;
			}
			if (methodCallMessage.ArgCount == 0)
			{
				methodFlags |= MethodFlags.NoArguments;
			}
			else if (MessageFormatter.AllTypesArePrimitive(methodCallMessage.Args))
			{
				methodFlags |= MethodFlags.PrimitiveArguments;
			}
			else if (num == 0)
			{
				methodFlags |= MethodFlags.ArgumentsInSimpleArray;
			}
			else
			{
				methodFlags |= MethodFlags.ArgumentsInMultiArray;
				num++;
			}
			writer.Write((int)methodFlags);
			writer.Write(18);
			writer.Write(methodCallMessage.MethodName);
			writer.Write(18);
			writer.Write(methodCallMessage.TypeName);
			if ((methodFlags & MethodFlags.PrimitiveArguments) > (MethodFlags)0)
			{
				writer.Write((uint)methodCallMessage.Args.Length);
				for (int i = 0; i < methodCallMessage.ArgCount; i++)
				{
					object arg = methodCallMessage.GetArg(i);
					if (arg != null)
					{
						writer.Write(BinaryCommon.GetTypeCode(arg.GetType()));
						ObjectWriter.WritePrimitiveValue(writer, arg);
					}
					else
					{
						writer.Write(17);
					}
				}
			}
			if (num > 0)
			{
				object[] array2 = new object[num];
				int num2 = 0;
				if ((methodFlags & MethodFlags.ArgumentsInMultiArray) > (MethodFlags)0)
				{
					array2[num2++] = methodCallMessage.Args;
				}
				if ((methodFlags & MethodFlags.GenericArguments) > (MethodFlags)0)
				{
					array2[num2++] = methodCallMessage.MethodBase.GetGenericArguments();
				}
				if ((methodFlags & MethodFlags.IncludesSignature) > (MethodFlags)0)
				{
					array2[num2++] = methodCallMessage.MethodSignature;
				}
				if ((methodFlags & MethodFlags.IncludesLogicalCallContext) > (MethodFlags)0)
				{
					array2[num2++] = methodCallMessage.LogicalCallContext;
				}
				if (array != null)
				{
					array2[num2++] = array;
				}
				obj2 = array2;
			}
			else if ((methodFlags & MethodFlags.ArgumentsInSimpleArray) > (MethodFlags)0)
			{
				obj2 = methodCallMessage.Args;
			}
			if (obj2 != null)
			{
				ObjectWriter objectWriter = new ObjectWriter(surrogateSelector, context, assemblyFormat, typeFormat);
				objectWriter.WriteObjectGraph(writer, obj2, headers);
			}
			else
			{
				writer.Write(11);
			}
		}

		public static void WriteMethodResponse(BinaryWriter writer, object obj, Header[] headers, ISurrogateSelector surrogateSelector, StreamingContext context, FormatterAssemblyStyle assemblyFormat, FormatterTypeStyle typeFormat)
		{
			IMethodReturnMessage methodReturnMessage = (IMethodReturnMessage)obj;
			writer.Write(22);
			string[] array = MethodReturnDictionary.InternalReturnKeys;
			int num = 0;
			object obj2 = null;
			object[] array2 = null;
			MethodFlags methodFlags = MethodFlags.ExcludeLogicalCallContext;
			ReturnTypeTag returnTypeTag;
			if (methodReturnMessage.Exception != null)
			{
				returnTypeTag = (ReturnTypeTag)34;
				array = MethodReturnDictionary.InternalExceptionKeys;
				num = 1;
			}
			else if (methodReturnMessage.ReturnValue == null)
			{
				returnTypeTag = ReturnTypeTag.Null;
			}
			else if (MessageFormatter.IsMethodPrimitive(methodReturnMessage.ReturnValue.GetType()))
			{
				returnTypeTag = ReturnTypeTag.PrimitiveType;
			}
			else
			{
				returnTypeTag = ReturnTypeTag.ObjectType;
				num++;
			}
			if (methodReturnMessage.LogicalCallContext != null && methodReturnMessage.LogicalCallContext.HasInfo)
			{
				methodFlags = MethodFlags.IncludesLogicalCallContext;
				num++;
			}
			if (methodReturnMessage.Properties.Count > array.Length && (byte)(returnTypeTag & ReturnTypeTag.Exception) == 0)
			{
				array2 = MessageFormatter.GetExtraProperties(methodReturnMessage.Properties, array);
				num++;
			}
			MethodFlags methodFlags2;
			if (methodReturnMessage.OutArgCount == 0)
			{
				methodFlags2 = MethodFlags.NoArguments;
			}
			else if (MessageFormatter.AllTypesArePrimitive(methodReturnMessage.Args))
			{
				methodFlags2 = MethodFlags.PrimitiveArguments;
			}
			else if (num == 0)
			{
				methodFlags2 = MethodFlags.ArgumentsInSimpleArray;
			}
			else
			{
				methodFlags2 = MethodFlags.ArgumentsInMultiArray;
				num++;
			}
			writer.Write((byte)(methodFlags | methodFlags2));
			writer.Write((byte)returnTypeTag);
			writer.Write(0);
			writer.Write(0);
			if (returnTypeTag == ReturnTypeTag.PrimitiveType)
			{
				writer.Write(BinaryCommon.GetTypeCode(methodReturnMessage.ReturnValue.GetType()));
				ObjectWriter.WritePrimitiveValue(writer, methodReturnMessage.ReturnValue);
			}
			if (methodFlags2 == MethodFlags.PrimitiveArguments)
			{
				writer.Write((uint)methodReturnMessage.ArgCount);
				for (int i = 0; i < methodReturnMessage.ArgCount; i++)
				{
					object arg = methodReturnMessage.GetArg(i);
					if (arg != null)
					{
						writer.Write(BinaryCommon.GetTypeCode(arg.GetType()));
						ObjectWriter.WritePrimitiveValue(writer, arg);
					}
					else
					{
						writer.Write(17);
					}
				}
			}
			if (num > 0)
			{
				object[] array3 = new object[num];
				int num2 = 0;
				if ((byte)(returnTypeTag & ReturnTypeTag.Exception) != 0)
				{
					array3[num2++] = methodReturnMessage.Exception;
				}
				if (methodFlags2 == MethodFlags.ArgumentsInMultiArray)
				{
					array3[num2++] = methodReturnMessage.Args;
				}
				if (returnTypeTag == ReturnTypeTag.ObjectType)
				{
					array3[num2++] = methodReturnMessage.ReturnValue;
				}
				if (methodFlags == MethodFlags.IncludesLogicalCallContext)
				{
					array3[num2++] = methodReturnMessage.LogicalCallContext;
				}
				if (array2 != null)
				{
					array3[num2++] = array2;
				}
				obj2 = array3;
			}
			else if ((methodFlags2 & MethodFlags.ArgumentsInSimpleArray) > (MethodFlags)0)
			{
				obj2 = methodReturnMessage.Args;
			}
			if (obj2 != null)
			{
				ObjectWriter objectWriter = new ObjectWriter(surrogateSelector, context, assemblyFormat, typeFormat);
				objectWriter.WriteObjectGraph(writer, obj2, headers);
			}
			else
			{
				writer.Write(11);
			}
		}

		public static object ReadMethodCall(BinaryReader reader, bool hasHeaders, HeaderHandler headerHandler, BinaryFormatter formatter)
		{
			BinaryElement binaryElement = (BinaryElement)reader.ReadByte();
			return MessageFormatter.ReadMethodCall(binaryElement, reader, hasHeaders, headerHandler, formatter);
		}

		public static object ReadMethodCall(BinaryElement elem, BinaryReader reader, bool hasHeaders, HeaderHandler headerHandler, BinaryFormatter formatter)
		{
			if (elem != BinaryElement.MethodCall)
			{
				throw new SerializationException("Invalid format. Expected BinaryElement.MethodCall, found " + elem);
			}
			MethodFlags methodFlags = (MethodFlags)reader.ReadInt32();
			if (reader.ReadByte() != 18)
			{
				throw new SerializationException("Invalid format");
			}
			string text = reader.ReadString();
			if (reader.ReadByte() != 18)
			{
				throw new SerializationException("Invalid format");
			}
			string text2 = reader.ReadString();
			object[] array = null;
			object obj = null;
			object obj2 = null;
			object[] array2 = null;
			Header[] array3 = null;
			Type[] array4 = null;
			if ((methodFlags & MethodFlags.PrimitiveArguments) > (MethodFlags)0)
			{
				uint num = reader.ReadUInt32();
				array = new object[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					Type typeFromCode = BinaryCommon.GetTypeFromCode((int)reader.ReadByte());
					array[num2] = ObjectReader.ReadPrimitiveTypeValue(reader, typeFromCode);
					num2++;
				}
			}
			if ((methodFlags & MethodFlags.NeedsInfoArrayMask) > (MethodFlags)0)
			{
				ObjectReader objectReader = new ObjectReader(formatter);
				object obj3;
				objectReader.ReadObjectGraph(reader, hasHeaders, out obj3, out array3);
				object[] array5 = (object[])obj3;
				if ((methodFlags & MethodFlags.ArgumentsInSimpleArray) > (MethodFlags)0)
				{
					array = array5;
				}
				else
				{
					int num3 = 0;
					if ((methodFlags & MethodFlags.ArgumentsInMultiArray) > (MethodFlags)0)
					{
						if (array5.Length > 1)
						{
							array = (object[])array5[num3++];
						}
						else
						{
							array = new object[0];
						}
					}
					if ((methodFlags & MethodFlags.GenericArguments) > (MethodFlags)0)
					{
						array4 = (Type[])array5[num3++];
					}
					if ((methodFlags & MethodFlags.IncludesSignature) > (MethodFlags)0)
					{
						obj = array5[num3++];
					}
					if ((methodFlags & MethodFlags.IncludesLogicalCallContext) > (MethodFlags)0)
					{
						obj2 = array5[num3++];
					}
					if (num3 < array5.Length)
					{
						array2 = (object[])array5[num3];
					}
				}
			}
			else
			{
				reader.ReadByte();
			}
			if (array == null)
			{
				array = new object[0];
			}
			string text3 = null;
			if (headerHandler != null)
			{
				text3 = headerHandler(array3) as string;
			}
			MethodCall methodCall = new MethodCall(new Header[]
			{
				new Header("__MethodName", text),
				new Header("__MethodSignature", obj),
				new Header("__TypeName", text2),
				new Header("__Args", array),
				new Header("__CallContext", obj2),
				new Header("__Uri", text3),
				new Header("__GenericArguments", array4)
			});
			if (array2 != null)
			{
				foreach (DictionaryEntry dictionaryEntry in array2)
				{
					methodCall.Properties[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
			return methodCall;
		}

		public static object ReadMethodResponse(BinaryReader reader, bool hasHeaders, HeaderHandler headerHandler, IMethodCallMessage methodCallMessage, BinaryFormatter formatter)
		{
			BinaryElement binaryElement = (BinaryElement)reader.ReadByte();
			return MessageFormatter.ReadMethodResponse(binaryElement, reader, hasHeaders, headerHandler, methodCallMessage, formatter);
		}

		public static object ReadMethodResponse(BinaryElement elem, BinaryReader reader, bool hasHeaders, HeaderHandler headerHandler, IMethodCallMessage methodCallMessage, BinaryFormatter formatter)
		{
			if (elem != BinaryElement.MethodResponse)
			{
				throw new SerializationException("Invalid format. Expected BinaryElement.MethodResponse, found " + elem);
			}
			MethodFlags methodFlags = (MethodFlags)reader.ReadByte();
			ReturnTypeTag returnTypeTag = (ReturnTypeTag)reader.ReadByte();
			bool flag = (methodFlags & MethodFlags.IncludesLogicalCallContext) > (MethodFlags)0;
			reader.ReadByte();
			reader.ReadByte();
			object obj = null;
			object[] array = null;
			LogicalCallContext logicalCallContext = null;
			Exception ex = null;
			object[] array2 = null;
			Header[] array3 = null;
			if ((byte)(returnTypeTag & ReturnTypeTag.PrimitiveType) > 0)
			{
				Type typeFromCode = BinaryCommon.GetTypeFromCode((int)reader.ReadByte());
				obj = ObjectReader.ReadPrimitiveTypeValue(reader, typeFromCode);
			}
			if ((methodFlags & MethodFlags.PrimitiveArguments) > (MethodFlags)0)
			{
				uint num = reader.ReadUInt32();
				array = new object[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					Type typeFromCode2 = BinaryCommon.GetTypeFromCode((int)reader.ReadByte());
					array[num2] = ObjectReader.ReadPrimitiveTypeValue(reader, typeFromCode2);
					num2++;
				}
			}
			if (flag || (byte)(returnTypeTag & ReturnTypeTag.ObjectType) > 0 || (byte)(returnTypeTag & ReturnTypeTag.Exception) > 0 || (methodFlags & MethodFlags.ArgumentsInSimpleArray) > (MethodFlags)0 || (methodFlags & MethodFlags.ArgumentsInMultiArray) > (MethodFlags)0)
			{
				ObjectReader objectReader = new ObjectReader(formatter);
				object obj2;
				objectReader.ReadObjectGraph(reader, hasHeaders, out obj2, out array3);
				object[] array4 = (object[])obj2;
				if ((byte)(returnTypeTag & ReturnTypeTag.Exception) > 0)
				{
					ex = (Exception)array4[0];
					if (flag)
					{
						logicalCallContext = (LogicalCallContext)array4[1];
					}
				}
				else if ((methodFlags & MethodFlags.NoArguments) > (MethodFlags)0 || (methodFlags & MethodFlags.PrimitiveArguments) > (MethodFlags)0)
				{
					int num3 = 0;
					if ((byte)(returnTypeTag & ReturnTypeTag.ObjectType) > 0)
					{
						obj = array4[num3++];
					}
					if (flag)
					{
						logicalCallContext = (LogicalCallContext)array4[num3++];
					}
					if (num3 < array4.Length)
					{
						array2 = (object[])array4[num3];
					}
				}
				else if ((methodFlags & MethodFlags.ArgumentsInSimpleArray) > (MethodFlags)0)
				{
					array = array4;
				}
				else
				{
					int num4 = 0;
					array = (object[])array4[num4++];
					if ((byte)(returnTypeTag & ReturnTypeTag.ObjectType) > 0)
					{
						obj = array4[num4++];
					}
					if (flag)
					{
						logicalCallContext = (LogicalCallContext)array4[num4++];
					}
					if (num4 < array4.Length)
					{
						array2 = (object[])array4[num4];
					}
				}
			}
			else
			{
				reader.ReadByte();
			}
			if (headerHandler != null)
			{
				headerHandler(array3);
			}
			if (ex != null)
			{
				return new ReturnMessage(ex, methodCallMessage);
			}
			int num5 = ((array == null) ? 0 : array.Length);
			ReturnMessage returnMessage = new ReturnMessage(obj, array, num5, logicalCallContext, methodCallMessage);
			if (array2 != null)
			{
				foreach (DictionaryEntry dictionaryEntry in array2)
				{
					returnMessage.Properties[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
			return returnMessage;
		}

		private static bool AllTypesArePrimitive(object[] objects)
		{
			foreach (object obj in objects)
			{
				if (obj != null && !MessageFormatter.IsMethodPrimitive(obj.GetType()))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsMethodPrimitive(Type type)
		{
			return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal);
		}

		private static object[] GetExtraProperties(IDictionary properties, string[] internalKeys)
		{
			object[] array = new object[properties.Count - internalKeys.Length];
			int num = 0;
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!MessageFormatter.IsInternalKey((string)enumerator.Entry.Key, internalKeys))
				{
					array[num++] = enumerator.Entry;
				}
			}
			return array;
		}

		private static bool IsInternalKey(string key, string[] internalKeys)
		{
			foreach (string text in internalKeys)
			{
				if (key == text)
				{
					return true;
				}
			}
			return false;
		}
	}
}
