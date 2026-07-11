using System;
using System.Collections.Generic;

public class StringTable
{
	public StringEntry Get(StringKey key0)
	{
		int hash = key0.Hash;
		StringEntry stringEntry = null;
		this.Entries.TryGetValue(hash, out stringEntry);
		return stringEntry;
	}

	public void Add(int idx, string[] value)
	{
		string text = value[idx];
		int hashCode = text.GetHashCode();
		this.KeyNames[hashCode] = text;
		if (idx == value.Length - 2)
		{
			StringEntry stringEntry = new StringEntry(value[idx + 1]);
			this.Entries[hashCode] = stringEntry;
		}
		else
		{
			StringTable stringTable = null;
			if (!this.SubTables.TryGetValue(hashCode, out stringTable))
			{
				stringTable = new StringTable();
				this.SubTables[hashCode] = stringTable;
			}
			stringTable.Add(idx + 1, value);
		}
	}

	public void Print(string parent_path)
	{
		foreach (KeyValuePair<int, StringEntry> keyValuePair in this.Entries)
		{
			Debug.Log(string.Concat(new string[]
			{
				parent_path,
				".",
				this.KeyNames[keyValuePair.Key],
				".",
				keyValuePair.Value.String
			}));
		}
		string text = parent_path;
		if (text != string.Empty)
		{
			text += ".";
		}
		foreach (KeyValuePair<int, StringTable> keyValuePair2 in this.SubTables)
		{
			keyValuePair2.Value.Print(text + this.KeyNames[keyValuePair2.Key]);
		}
	}

	private Dictionary<int, string> KeyNames = new Dictionary<int, string>();

	private Dictionary<int, StringTable> SubTables = new Dictionary<int, StringTable>();

	private Dictionary<int, StringEntry> Entries = new Dictionary<int, StringEntry>();
}
