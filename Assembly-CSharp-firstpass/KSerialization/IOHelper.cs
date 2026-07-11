using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace KSerialization
{
	public static class IOHelper
	{
		public static void WriteKleiString(this BinaryWriter writer, string str)
		{
			if (str == null)
			{
				writer.Write(-1);
				return;
			}
			Encoding utf = Encoding.UTF8;
			int byteCount = utf.GetByteCount(str);
			writer.Write(byteCount);
			if (byteCount < IOHelper.s_stringBuffer.Length)
			{
				utf.GetBytes(str, 0, str.Length, IOHelper.s_stringBuffer, 0);
				writer.Write(IOHelper.s_stringBuffer, 0, byteCount);
				return;
			}
			global::Debug.LogWarning(string.Format("Writing large string {0} of {1} bytes", str, byteCount));
			writer.Write(utf.GetBytes(str));
		}

		public unsafe static void WriteSingleFast(this BinaryWriter writer, float value)
		{
			byte* ptr = (byte*)(&value);
			if (BitConverter.IsLittleEndian)
			{
				IOHelper.s_singleBuffer[0] = *ptr;
				IOHelper.s_singleBuffer[1] = ptr[1];
				IOHelper.s_singleBuffer[2] = ptr[2];
				IOHelper.s_singleBuffer[3] = ptr[3];
			}
			else
			{
				IOHelper.s_singleBuffer[0] = ptr[3];
				IOHelper.s_singleBuffer[1] = ptr[2];
				IOHelper.s_singleBuffer[2] = ptr[1];
				IOHelper.s_singleBuffer[3] = *ptr;
			}
			writer.Write(IOHelper.s_singleBuffer);
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void WriteBoundaryTag(this BinaryWriter writer, object tag)
		{
			writer.Write((uint)tag);
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void CheckBoundaryTag(this IReader reader, object expected)
		{
			uint num = reader.ReadUInt32();
			if ((uint)expected != num)
			{
				global::Debug.LogError(string.Format("Expected Tag {0}(0x{1:X}) but got 0x{2:X} instead", expected.ToString(), (uint)expected, num));
			}
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void Assert(bool condition)
		{
			DebugUtil.Assert(condition);
		}

		public static Vector2I ReadVector2I(this IReader reader)
		{
			Vector2I vector2I;
			vector2I.x = reader.ReadInt32();
			vector2I.y = reader.ReadInt32();
			return vector2I;
		}

		public static Vector2 ReadVector2(this IReader reader)
		{
			Vector2 vector;
			vector.x = reader.ReadSingle();
			vector.y = reader.ReadSingle();
			return vector;
		}

		public static Vector3 ReadVector3(this IReader reader)
		{
			Vector3 vector;
			vector.x = reader.ReadSingle();
			vector.y = reader.ReadSingle();
			vector.z = reader.ReadSingle();
			return vector;
		}

		public static Color ReadColour(this IReader reader)
		{
			byte b = reader.ReadByte();
			byte b2 = reader.ReadByte();
			byte b3 = reader.ReadByte();
			byte b4 = reader.ReadByte();
			Color color;
			color.r = (float)b / 255f;
			color.g = (float)b2 / 255f;
			color.b = (float)b3 / 255f;
			color.a = (float)b4 / 255f;
			return color;
		}

		private static byte[] s_stringBuffer = new byte[1024];

		private static byte[] s_singleBuffer = new byte[4];
	}
}
