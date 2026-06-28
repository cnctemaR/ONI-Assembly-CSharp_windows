using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GarbageProfiler
{
	private static void UnloadUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
	}

	private static void DebugGarbageCollect()
	{
		GC.Collect();
	}

	private static void Dump()
	{
		MemorySnapshot memorySnapshot = new MemorySnapshot();
		MemorySnapshot.TypeData[] array = new MemorySnapshot.TypeData[memorySnapshot.types.Count];
		memorySnapshot.types.Values.CopyTo(array, 0);
		Array.Sort<MemorySnapshot.TypeData>(array, 0, array.Length, new GarbageProfiler.InstanceCountComparer());
		global::System.DateTime now = global::System.DateTime.Now;
		string text = string.Concat(new string[]
		{
			now.Year.ToString(),
			"-",
			now.Month.ToString(),
			"-",
			now.Day.ToString(),
			"-",
			now.Hour.ToString(),
			"h-",
			now.Minute.ToString(),
			"m-",
			now.Second.ToString(),
			"s.csv"
		});
		string text2 = "memory_instances-" + text;
		using (StreamWriter streamWriter = new StreamWriter(text2))
		{
			foreach (MemorySnapshot.TypeData typeData in array)
			{
				if (typeData.instanceCount != 0)
				{
					int num = typeData.instanceCount;
					if (GarbageProfiler.previousSnapshot != null)
					{
						MemorySnapshot.TypeData typeData2 = MemorySnapshot.GetTypeData(typeData.type, GarbageProfiler.previousSnapshot.types);
						num = typeData.instanceCount - typeData2.instanceCount;
					}
					streamWriter.WriteLine(string.Concat(new object[]
					{
						"\"",
						typeData.type.FullName,
						"\",",
						typeData.instanceCount,
						", ",
						num
					}));
				}
			}
		}
		string text3 = "memory_hierarchies-" + text;
		using (StreamWriter streamWriter2 = new StreamWriter(text3))
		{
			foreach (MemorySnapshot.TypeData typeData3 in array)
			{
				if (typeData3.instanceCount != 0)
				{
					foreach (KeyValuePair<MemorySnapshot.HierarchyNode, int> keyValuePair in typeData3.hierarchies)
					{
						int num2 = keyValuePair.Value;
						if (GarbageProfiler.previousSnapshot != null)
						{
							MemorySnapshot.TypeData typeData4 = MemorySnapshot.GetTypeData(typeData3.type, GarbageProfiler.previousSnapshot.types);
							int num3 = 0;
							if (typeData4.hierarchies.TryGetValue(keyValuePair.Key, out num3))
							{
								num2 = keyValuePair.Value - num3;
							}
						}
						streamWriter2.WriteLine(string.Concat(new object[]
						{
							"\"",
							typeData3.type.FullName,
							": ",
							keyValuePair.Key.ToString(),
							"\",",
							keyValuePair.Value,
							", ",
							num2
						}));
					}
				}
			}
		}
		GarbageProfiler.previousSnapshot = memorySnapshot;
		global::Debug.Log("Done writing snapshot!", null);
	}

	public static void DebugDumpGarbageStats()
	{
		MemorySnapshot memorySnapshot = new MemorySnapshot();
		MemorySnapshot.TypeData[] array = new MemorySnapshot.TypeData[memorySnapshot.types.Count];
		memorySnapshot.types.Values.CopyTo(array, 0);
		Array.Sort<MemorySnapshot.TypeData>(array, 0, array.Length, new GarbageProfiler.InstanceCountComparer());
		using (StreamWriter streamWriter = new StreamWriter("garbage_instances.csv"))
		{
			foreach (MemorySnapshot.TypeData typeData in array)
			{
				if (typeData.instanceCount != 0)
				{
					streamWriter.WriteLine(string.Concat(new object[]
					{
						typeData.instanceCount,
						", \"",
						typeData.type.FullName,
						"\""
					}));
				}
			}
		}
		Array.Sort<MemorySnapshot.TypeData>(array, 0, array.Length, new GarbageProfiler.RefCountComparer());
		using (StreamWriter streamWriter2 = new StreamWriter("garbage_refs.csv"))
		{
			foreach (MemorySnapshot.TypeData typeData2 in array)
			{
				if (typeData2.refCount != 0)
				{
					streamWriter2.WriteLine(string.Concat(new object[]
					{
						typeData2.refCount,
						", \"",
						typeData2.type.FullName,
						"\""
					}));
				}
			}
		}
		MemorySnapshot.FieldCount[] array4 = new MemorySnapshot.FieldCount[memorySnapshot.fieldCounts.Count];
		memorySnapshot.fieldCounts.Values.CopyTo(array4, 0);
		Array.Sort<MemorySnapshot.FieldCount>(array4, 0, array4.Length, new GarbageProfiler.FieldCountComparer());
		using (StreamWriter streamWriter3 = new StreamWriter("garbage_fields.csv"))
		{
			foreach (MemorySnapshot.FieldCount fieldCount in array4)
			{
				streamWriter3.WriteLine(string.Concat(new object[] { fieldCount.count, ", \"", fieldCount.name, "\"" }));
			}
		}
		GarbageProfiler.previousSnapshot = memorySnapshot;
		global::Debug.Log("Done writing reference stats!", null);
	}

	private static MemorySnapshot previousSnapshot;

	private class InstanceCountComparer : IComparer<MemorySnapshot.TypeData>
	{
		public int Compare(MemorySnapshot.TypeData a, MemorySnapshot.TypeData b)
		{
			return b.instanceCount - a.instanceCount;
		}
	}

	private class RefCountComparer : IComparer<MemorySnapshot.TypeData>
	{
		public int Compare(MemorySnapshot.TypeData a, MemorySnapshot.TypeData b)
		{
			return b.refCount - a.refCount;
		}
	}

	private class FieldCountComparer : IComparer<MemorySnapshot.FieldCount>
	{
		public int Compare(MemorySnapshot.FieldCount a, MemorySnapshot.FieldCount b)
		{
			return b.count - a.count;
		}
	}
}
