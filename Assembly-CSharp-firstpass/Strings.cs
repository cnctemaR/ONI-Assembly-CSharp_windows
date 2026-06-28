using System;
using System.Collections.Generic;
using FileHelpers;
using UnityEngine;

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

	public static void Add(TextAsset[] tables)
	{
		if (tables != null)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(Strings.StringKeyConfig));
			int i = 0;
			while (i < tables.Length)
			{
				TextAsset textAsset = tables[i];
				if (textAsset != null)
				{
					Strings.StringKeyConfig[] array = (Strings.StringKeyConfig[])fileHelperEngine.ReadString(textAsset.text);
					if (array.Length != 0)
					{
						Strings.StringKeyConfig stringKeyConfig = array[0];
						bool flag = stringKeyConfig.name != "NOHEADERS";
						for (int j = 0; j < array.Length; j++)
						{
							if (!flag || j != 0)
							{
								Strings.StringKeyConfig stringKeyConfig2 = array[j];
								if (flag)
								{
									for (int k = 0; k < stringKeyConfig2.values.Length; k++)
									{
										List<string> list = new List<string>();
										list.Add(textAsset.name);
										if (stringKeyConfig2.name != null && !(stringKeyConfig2.name == "") && !stringKeyConfig2.name.Contains(" "))
										{
											list.Add(stringKeyConfig2.name);
											if (stringKeyConfig.name != null && !(stringKeyConfig.name == "") && !stringKeyConfig.name.Contains(" "))
											{
												list.Add(stringKeyConfig.values[k]);
												if (stringKeyConfig2.values[k] != null && !(stringKeyConfig2.values[k] == ""))
												{
													list.Add(stringKeyConfig2.values[k]);
													Strings.Add(list.ToArray());
												}
											}
										}
									}
								}
								else
								{
									List<string> list2 = new List<string>();
									list2.Add(textAsset.name);
									if (stringKeyConfig2.name != null && !(stringKeyConfig2.name == "") && !stringKeyConfig2.name.Contains(" "))
									{
										list2.Add(stringKeyConfig2.name);
										list2.Add(stringKeyConfig2.values[0]);
										Strings.Add(list2.ToArray());
									}
								}
							}
						}
					}
				}
				IL_0247:
				i++;
				continue;
				goto IL_0247;
			}
		}
	}

	public static void PrintTable()
	{
		Strings.RootTable.Print("");
	}

	private static StringTable RootTable = new StringTable();

	private static HashSet<string> invalidKeys = new HashSet<string>();

	[DelimitedRecord(",")]
	[IgnoreEmptyLines]
	public class StringKeyConfig
	{
		[FieldOrder(1)]
		[FieldOptional]
		public string name;

		[FieldOrder(2)]
		[FieldQuoted(QuoteMode.OptionalForRead, MultilineMode.AllowForRead)]
		[FieldNullValue(typeof(string), "")]
		[FieldOptional]
		public string[] values;
	}
}
