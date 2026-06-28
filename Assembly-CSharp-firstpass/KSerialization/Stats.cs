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
			if (Stats.serializationStats.Count > 0)
			{
			}
			if (Stats.deserializationStats.Count > 0)
			{
			}
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		private static void Print(string header, Dictionary<Type, Stats.StatInfo> stats)
		{
			string text = header + "\n";
			foreach (KeyValuePair<Type, Stats.StatInfo> keyValuePair in stats)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					keyValuePair.Key.ToString(),
					",",
					keyValuePair.Value.numOccurrences,
					",",
					keyValuePair.Value.numBytes,
					"\n"
				});
			}
			Output.Log(new object[] { text });
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
