using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Harmony
{
	public static class PatchInfoSerialization
	{
		public static byte[] Serialize(this PatchInfo patchInfo)
		{
			byte[] buffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, patchInfo);
				buffer = memoryStream.GetBuffer();
			}
			return buffer;
		}

		public static PatchInfo Deserialize(byte[] bytes)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter
			{
				Binder = new PatchInfoSerialization.Binder()
			};
			MemoryStream memoryStream = new MemoryStream(bytes);
			return (PatchInfo)binaryFormatter.Deserialize(memoryStream);
		}

		public static int PriorityComparer(object obj, int index, int priority, string[] before, string[] after)
		{
			Traverse traverse = Traverse.Create(obj);
			string value = traverse.Field("owner").GetValue<string>();
			int value2 = traverse.Field("priority").GetValue<int>();
			int value3 = traverse.Field("index").GetValue<int>();
			bool flag = before != null && Array.IndexOf<string>(before, value) > -1;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = after != null && Array.IndexOf<string>(after, value) > -1;
				if (flag2)
				{
					num = 1;
				}
				else
				{
					bool flag3 = priority != value2;
					if (flag3)
					{
						num = -priority.CompareTo(value2);
					}
					else
					{
						num = index.CompareTo(value3);
					}
				}
			}
			return num;
		}

		private class Binder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				Type[] array = new Type[]
				{
					typeof(PatchInfo),
					typeof(Patch[]),
					typeof(Patch)
				};
				foreach (Type type in array)
				{
					bool flag = typeName == type.FullName;
					if (flag)
					{
						return type;
					}
				}
				return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
			}
		}
	}
}
