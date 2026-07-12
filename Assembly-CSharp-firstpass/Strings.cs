using System;
using System.Collections.Generic;

public static class Strings
{
	private static StringEntry GetInvalidString(params StringKey[] keys)
	{
		string text = "MISSING";
		foreach (StringKey stringKey in keys)
		{
			if (text != "")
			{
				text += ".";
			}
			text += stringKey.String;
		}
		Strings.invalidKeys.Add(text);
		return new StringEntry(text);
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

	public static void Add(params string[] value)
	{
		Strings.RootTable.Add(0, value);
	}

	public static void PrintTable()
	{
		Strings.RootTable.Print("");
	}

	public static void VisitEntries(StringTable.EntryVisitor visit)
	{
		Strings.RootTable.VisitEntries(visit);
	}

	private static StringTable RootTable = new StringTable();

	private static HashSet<string> invalidKeys = new HashSet<string>();
}
