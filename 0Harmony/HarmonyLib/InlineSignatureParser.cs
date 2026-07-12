using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace HarmonyLib
{
	internal static class InlineSignatureParser
	{
		internal static InlineSignature ImportCallSite(Module moduleFrom, byte[] data)
		{
			InlineSignatureParser.<>c__DisplayClass0_0 CS$<>8__locals1;
			CS$<>8__locals1.moduleFrom = moduleFrom;
			InlineSignature inlineSignature = new InlineSignature();
			InlineSignature inlineSignature2;
			using (MemoryStream memoryStream = new MemoryStream(data, false))
			{
				CS$<>8__locals1.reader = new BinaryReader(memoryStream);
				try
				{
					InlineSignatureParser.<ImportCallSite>g__ReadMethodSignature|0_0(inlineSignature, ref CS$<>8__locals1);
					inlineSignature2 = inlineSignature;
				}
				finally
				{
					if (CS$<>8__locals1.reader != null)
					{
						((IDisposable)CS$<>8__locals1.reader).Dispose();
					}
				}
			}
			return inlineSignature2;
		}

		[CompilerGenerated]
		internal static void <ImportCallSite>g__ReadMethodSignature|0_0(InlineSignature method, ref InlineSignatureParser.<>c__DisplayClass0_0 A_1)
		{
			byte b = A_1.reader.ReadByte();
			if ((b & 32) != 0)
			{
				method.HasThis = true;
				b = (byte)((int)b & -33);
			}
			if ((b & 64) != 0)
			{
				method.ExplicitThis = true;
				b = (byte)((int)b & -65);
			}
			method.CallingConvention = (int)b + CallingConvention.Winapi;
			if ((b & 16) != 0)
			{
				InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_1);
			}
			uint num = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_1);
			method.ReturnType = InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_1);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				method.Parameters.Add(InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_1));
				num2++;
			}
		}

		[CompilerGenerated]
		internal static uint <ImportCallSite>g__ReadCompressedUInt32|0_1(ref InlineSignatureParser.<>c__DisplayClass0_0 A_0)
		{
			byte b = A_0.reader.ReadByte();
			if ((b & 128) == 0)
			{
				return (uint)b;
			}
			if ((b & 64) == 0)
			{
				return (((uint)b & 4294967167U) << 8) | (uint)A_0.reader.ReadByte();
			}
			return (uint)((((int)b & -193) << 24) | ((int)A_0.reader.ReadByte() << 16) | ((int)A_0.reader.ReadByte() << 8) | (int)A_0.reader.ReadByte());
		}

		[CompilerGenerated]
		internal static int <ImportCallSite>g__ReadCompressedInt32|0_2(ref InlineSignatureParser.<>c__DisplayClass0_0 A_0)
		{
			byte b = A_0.reader.ReadByte();
			A_0.reader.BaseStream.Seek(-1L, SeekOrigin.Current);
			uint num = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
			int num2 = (int)num >> 1;
			if ((num & 1U) == 0U)
			{
				return num2;
			}
			int num3 = (int)(b & 192);
			int num4;
			if (num3 != 0 && num3 != 64)
			{
				if (num3 != 128)
				{
					num4 = num2 - 268435456;
				}
				else
				{
					num4 = num2 - 8192;
				}
			}
			else
			{
				num4 = num2 - 64;
			}
			return num4;
		}

		[CompilerGenerated]
		internal static Type <ImportCallSite>g__GetTypeDefOrRef|0_3(ref InlineSignatureParser.<>c__DisplayClass0_0 A_0)
		{
			uint num = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
			uint num2 = num >> 2;
			uint num3;
			switch (num & 3U)
			{
			case 0U:
				num3 = 33554432U | num2;
				break;
			case 1U:
				num3 = 16777216U | num2;
				break;
			case 2U:
				num3 = 452984832U | num2;
				break;
			default:
				num3 = 0U;
				break;
			}
			uint num4 = num3;
			return A_0.moduleFrom.ResolveType((int)num4);
		}

		[CompilerGenerated]
		internal static object <ImportCallSite>g__ReadTypeSignature|0_4(ref InlineSignatureParser.<>c__DisplayClass0_0 A_0)
		{
			MetadataType metadataType = (MetadataType)A_0.reader.ReadByte();
			switch (metadataType)
			{
			case MetadataType.Void:
				return typeof(void);
			case MetadataType.Boolean:
				return typeof(bool);
			case MetadataType.Char:
				return typeof(char);
			case MetadataType.SByte:
				return typeof(sbyte);
			case MetadataType.Byte:
				return typeof(byte);
			case MetadataType.Int16:
				return typeof(short);
			case MetadataType.UInt16:
				return typeof(ushort);
			case MetadataType.Int32:
				return typeof(int);
			case MetadataType.UInt32:
				return typeof(uint);
			case MetadataType.Int64:
				return typeof(long);
			case MetadataType.UInt64:
				return typeof(ulong);
			case MetadataType.Single:
				return typeof(float);
			case MetadataType.Double:
				return typeof(double);
			case MetadataType.String:
				return typeof(string);
			case MetadataType.Pointer:
				return ((Type)InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0)).MakePointerType();
			case MetadataType.ByReference:
				return ((Type)InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0)).MakePointerType();
			case MetadataType.ValueType:
			case MetadataType.Class:
				return InlineSignatureParser.<ImportCallSite>g__GetTypeDefOrRef|0_3(ref A_0);
			case MetadataType.Var:
			case MetadataType.GenericInstance:
			case MetadataType.MVar:
				throw new NotSupportedException(string.Format("Unsupported generic callsite element: {0}", metadataType));
			case MetadataType.Array:
			{
				Type type = (Type)InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0);
				uint num = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
				uint num2 = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num2))
				{
					InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
					num3++;
				}
				uint num4 = InlineSignatureParser.<ImportCallSite>g__ReadCompressedUInt32|0_1(ref A_0);
				int num5 = 0;
				while ((long)num5 < (long)((ulong)num4))
				{
					InlineSignatureParser.<ImportCallSite>g__ReadCompressedInt32|0_2(ref A_0);
					num5++;
				}
				return type.MakeArrayType((int)num);
			}
			case MetadataType.TypedByReference:
				return typeof(TypedReference);
			case MetadataType.IntPtr:
				return typeof(IntPtr);
			case MetadataType.UIntPtr:
				return typeof(UIntPtr);
			case MetadataType.FunctionPointer:
			{
				InlineSignature inlineSignature = new InlineSignature();
				InlineSignatureParser.<ImportCallSite>g__ReadMethodSignature|0_0(inlineSignature, ref A_0);
				return inlineSignature;
			}
			case MetadataType.Object:
				return typeof(object);
			case (MetadataType)29:
				return ((Type)InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0)).MakeArrayType();
			case MetadataType.RequiredModifier:
				return new InlineSignature.ModifierType
				{
					IsOptional = false,
					Modifier = InlineSignatureParser.<ImportCallSite>g__GetTypeDefOrRef|0_3(ref A_0),
					Type = InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0)
				};
			case MetadataType.OptionalModifier:
				return new InlineSignature.ModifierType
				{
					IsOptional = true,
					Modifier = InlineSignatureParser.<ImportCallSite>g__GetTypeDefOrRef|0_3(ref A_0),
					Type = InlineSignatureParser.<ImportCallSite>g__ReadTypeSignature|0_4(ref A_0)
				};
			}
			throw new NotSupportedException(string.Format("Unsupported callsite element: {0}", metadataType));
		}
	}
}
