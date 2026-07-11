using System;
using System.Collections.Generic;

public static class Strings
{
	private static StringEntry GetInvalidString(params StringKey[] keys)
	{
		string text = "MISSING";
		foreach (StringKey stringKey in keys)
		{
			if (text != string.Empty)
			{
				text += ".";
			}
			text += stringKey.String;
		}
		Strings.invalidKeys.Add(text);
		return new StringEntry(text);
	}

	public static StringEntry Get(StringKey key0, StringKey key1, StringKey key2)
	{
		StringEntry stringEntry = Strings.RootTable.Get(key0, key1, key2);
		if (stringEntry == null)
		{
			stringEntry = Strings.GetInvalidString(new StringKey[] { key0, key1, key2 });
		}
		return stringEntry;
	}

	public static StringEntry Get(StringKey key0, StringKey key1)
	{
		StringEntry stringEntry = Strings.RootTable.Get(key0, key1);
		if (stringEntry == null)
		{
			stringEntry = Strings.GetInvalidString(new StringKey[] { key0, key1 });
		}
		return stringEntry;
	}

	public static StringEntry Get(StringKey key0)
	{
		StringEntry stringEntry = Strings.RootTable.Get(key0);
		if (stringEntry == null)
		{
			stringEntry = Strings.GetInvalidString(new StringKey[] { key0 });
		}
		return stringEntry;
	}

	public static StringEntry Get(string key)
	{
		StringKey stringKey = new StringKey(key);
		StringEntry stringEntry = Strings.RootTable.Get(stringKey);
		if (stringEntry == null)
		{
			stringEntry = Strings.GetInvalidString(new StringKey[] { stringKey });
		}
		return stringEntry;
	}

	public static bool TryGet(StringKey key, out StringEntry result)
	{
		result = Strings.RootTable.Get(key);
		return result != null;
	}

	public static bool TryGet(string key, out StringEntry result)
	{
		return Strings.TryGet(new StringKey(key), out result);
	}

	public static StringTable GetTable(StringKey key0)
	{
		return Strings.RootTable.GetTable(key0);
	}

	public static StringEntry Get(StringKey key, StringKey[] keys)
	{
		StringEntry stringEntry = Strings.RootTable.Get(0, key, keys);
		if (stringEntry == null)
		{
			List<StringKey> list = new List<StringKey>();
			list.Add(key);
			list.AddRange(keys);
			stringEntry = Strings.GetInvalidString(list.ToArray());
		}
		return stringEntry;
	}

	public static StringEntry Get(StringKey[] keys)
	{
		StringEntry stringEntry = Strings.RootTable.Get(0, keys);
		if (stringEntry == null)
		{
			stringEntry = Strings.GetInvalidString(keys);
		}
		return stringEntry;
	}

	public static void Add(params string[] value)
	{
		Strings.RootTable.Add(0, value);
	}

	public static void PrintTable()
	{
		Strings.RootTable.Print(string.Empty);
	}

	private static StringTable RootTable = new StringTable();

	private static HashSet<string> invalidKeys = new HashSet<string>();
}
