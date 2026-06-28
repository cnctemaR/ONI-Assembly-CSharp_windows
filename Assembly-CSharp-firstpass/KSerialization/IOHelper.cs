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
			if (str != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				writer.Write(bytes.Length);
				writer.Write(bytes);
			}
			else
			{
				writer.Write(-1);
			}
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
				Output.LogError(new object[] { string.Format("Expected Tag {0}(0x{1:X}) but got 0x{2:X} instead", expected.ToString(), (uint)expected, num) });
			}
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void Assert(bool condition)
		{
			DebugUtil.Assert(condition, "Assert!");
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
	}
}
