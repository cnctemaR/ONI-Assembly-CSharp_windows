using System;
using System.Collections.Generic;
using System.Reflection;

public class LocString
{
	public LocString(string text)
	{
		this.text = text;
		this.key = default(StringKey);
	}

	public LocString(string text, string keystring)
	{
		this.text = text;
		this.key = new StringKey(keystring);
	}

	public LocString(string text, bool isLocalized)
	{
		this.text = text;
		this.key = default(StringKey);
	}

	public string text { get; private set; }

	public StringKey key { get; private set; }

	public static implicit operator LocString(string text)
	{
		return new LocString(text);
	}

	public static implicit operator string(LocString loc_string)
	{
		return loc_string.text;
	}

	public override string ToString()
	{
		return Strings.Get(this.key).String;
	}

	public void SetKey(string key_name)
	{
		this.key = new StringKey(key_name);
	}

	public void SetKey(StringKey key)
	{
		this.key = key;
	}

	public static void CreateLocStringKeys(Type type, string parent_path = "STRINGS.")
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		string text = parent_path;
		if (text == null)
		{
			text = string.Empty;
		}
		text = text + type.Name + ".";
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(LocString))
			{
				string text2 = text + fieldInfo.Name;
				LocString locString = (LocString)fieldInfo.GetValue(null);
				locString.SetKey(text2);
				string text3 = locString.text;
				Strings.Add(new string[] { text2, text3 });
				fieldInfo.SetValue(null, locString);
			}
		}
		foreach (Type type2 in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			LocString.CreateLocStringKeys(type2, text);
		}
	}

	public static string[] GetStrings(Type type)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo fieldInfo in fields)
		{
			LocString locString = (LocString)fieldInfo.GetValue(null);
			list.Add(locString.text);
		}
		return list.ToArray();
	}
}
