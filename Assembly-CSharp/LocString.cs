using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class LocString
{
	public string text { get; private set; }

	public StringKey key { get; private set; }

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

	public string Replace(string search, string replacement)
	{
		return this.ToString().Replace(search, replacement);
	}

	public static void CreateLocStringKeys(Type type, string parent_path = "STRINGS.")
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		string text = parent_path;
		if (text == null)
		{
			text = "";
		}
		text = text + type.Name + ".";
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType != typeof(LocString)))
			{
				string text2 = text + fieldInfo.Name;
				LocString locString = (LocString)fieldInfo.GetValue(null);
				locString.SetKey(text2);
				string text3 = locString.text;
				Strings.Add(new string[] { text2, text3 });
				fieldInfo.SetValue(null, locString);
			}
		}
		Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		for (int i = 0; i < nestedTypes.Length; i++)
		{
			LocString.CreateLocStringKeys(nestedTypes[i], text);
		}
	}

	public static string[] GetStrings(Type type)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		for (int i = 0; i < fields.Length; i++)
		{
			LocString locString = (LocString)fields[i].GetValue(null);
			list.Add(locString.text);
		}
		return list.ToArray();
	}

	public const BindingFlags data_member_fields = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
}
