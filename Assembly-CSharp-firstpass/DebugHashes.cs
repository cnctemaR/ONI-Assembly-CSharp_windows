using System;
using System.Collections.Generic;

public static class DebugHashes
{
	public static void Add(string name)
	{
		int num = Hash.SDBMLower(name);
		DebugHashes.hashMap[num] = name;
	}

	public static string GetName(int hash)
	{
		if (DebugHashes.hashMap.ContainsKey(hash))
		{
			return DebugHashes.hashMap[hash];
		}
		return "Unknown HASH [0x" + hash.ToString("X") + "]";
	}

	private static Dictionary<int, string> hashMap = new Dictionary<int, string>();
}
