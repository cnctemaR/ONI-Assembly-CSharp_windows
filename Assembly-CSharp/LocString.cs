using System;
using System.Reflection;

public class LocString
{
	public LocString(string text)
	{
		this.text = text;
		this.key = default(StringKey);
	}

	public string text { get; private set; }

	public StringKey key { get; private set; }

	public override string ToString()
	{
		return Strings.Get(this.key);
	}

	public void SetKey(string key_name)
	{
		this.key = new StringKey(key_name);
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
			string text2 = text + fieldInfo.Name;
			LocString locString = (LocString)fieldInfo.GetValue(null);
			locString.SetKey(text2);
			string text3 = locString.text;
			Strings.Add(new string[] { text2, text3 });
			fieldInfo.SetValue(null, locString);
		}
		foreach (Type type2 in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			LocString.CreateLocStringKeys(type2, text);
		}
	}

	public static implicit operator LocString(string text)
	{
		return new LocString(text);
	}

	public static implicit operator string(LocString loc_string)
	{
		return loc_string.ToString();
	}
}
