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
		string text;
		if (DebugHashes.hashMap.ContainsKey(hash))
		{
			text = DebugHashes.hashMap[hash];
		}
		else
		{
			text = "Unknown HASH [0x" + hash.ToString("X") + "]";
		}
		return text;
	}

	private static Dictionary<int, string> hashMap = new Dictionary<int, string>();
}
