using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KSerialization
{
	public static class Stats
	{
		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Clear()
		{
			Stats.serializationStats.Clear();
			Stats.deserializationStats.Clear();
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Write(Type type, long num_bytes)
		{
			Stats.StatInfo statInfo;
			Stats.serializationStats.TryGetValue(type, out statInfo);
			statInfo.numOccurrences++;
			statInfo.numBytes += num_bytes;
			Stats.serializationStats[type] = statInfo;
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Read(Type type, long num_bytes)
		{
			Stats.StatInfo statInfo;
			Stats.deserializationStats.TryGetValue(type, out statInfo);
			statInfo.numOccurrences++;
			statInfo.numBytes += num_bytes;
			Stats.deserializationStats[type] = statInfo;
		}

		public static void Print()
		{
			int count = Stats.serializationStats.Count;
			int count2 = Stats.deserializationStats.Count;
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		private static void Print(string header, Dictionary<Type, Stats.StatInfo> stats)
		{
			string text = header + "\n";
			foreach (KeyValuePair<Type, Stats.StatInfo> keyValuePair in stats)
			{
				string[] array = new string[7];
				array[0] = text;
				array[1] = keyValuePair.Key.ToString();
				array[2] = ",";
				int num = 3;
				Stats.StatInfo statInfo = keyValuePair.Value;
				array[num] = statInfo.numOccurrences.ToString();
				array[4] = ",";
				int num2 = 5;
				statInfo = keyValuePair.Value;
				array[num2] = statInfo.numBytes.ToString();
				array[6] = "\n";
				text = string.Concat(array);
			}
			DebugUtil.LogArgs(new object[] { text });
		}

		private static Dictionary<Type, Stats.StatInfo> serializationStats = new Dictionary<Type, Stats.StatInfo>();

		private static Dictionary<Type, Stats.StatInfo> deserializationStats = new Dictionary<Type, Stats.StatInfo>();

		private struct StatInfo
		{
			public int numOccurrences;

			public long numBytes;
		}
	}
}
