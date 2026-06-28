using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using STRINGS;
using TMPro;
using UnityEngine;

public class Localization
{
	public static void LoadTranslation(string filename)
	{
		Dictionary<string, string> dictionary = Localization.LoadTranslatedStrings(filename);
		Localization.OverloadStrings(dictionary);
	}

	private static Dictionary<string, string> LoadTranslatedStrings(string filename)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = File.ReadAllLines(filename);
		Localization.Entry entry = default(Localization.Entry);
		foreach (string text in array)
		{
			if (text == null || text.Length == 0)
			{
				entry = default(Localization.Entry);
			}
			else
			{
				string text2 = Localization.GetParameter("msgctxt", text);
				if (text2 != null)
				{
					entry.msgctxt = text2;
				}
				text2 = Localization.GetParameter("msgstr", text);
				if (text2 != null)
				{
					entry.msgstr = text2;
				}
			}
			if (entry.IsPopulated)
			{
				dictionary[entry.msgctxt] = entry.msgstr;
				entry = default(Localization.Entry);
			}
		}
		return dictionary;
	}

	private static string GetParameter(string key, string line)
	{
		string text = null;
		if (line.StartsWith(key))
		{
			text = line.Substring(key.Length + 1);
			if (text[0] != '"' || text[text.Length - 1] != '"')
			{
				text = null;
			}
			else
			{
				text = text.Substring(1, text.Length - 2);
				text = text.Replace("\\n", "\n");
				text = text.Replace("\\\"", "\"");
			}
		}
		return text;
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings)
	{
		Assembly assembly = Assembly.GetAssembly(typeof(UI));
		IEnumerable<Type> enumerable = from t in assembly.GetTypes()
			where t.IsClass && t.Namespace == "STRINGS" && !t.IsNested
			select t;
		List<Type> list = enumerable.ToList<Type>();
		foreach (Type type in list)
		{
			string text = "STRINGS." + type.Name;
			Localization.OverloadStrings(translated_strings, text, type);
		}
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings, string path, Type t)
	{
		FieldInfo[] fields = t.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(LocString))
			{
				string text = path + "." + fieldInfo.Name;
				string text2 = null;
				if (translated_strings.TryGetValue(text, out text2))
				{
					LocString locString = text2;
					fieldInfo.SetValue(null, locString);
				}
			}
		}
		Type[] nestedTypes = t.GetNestedTypes();
		foreach (Type type in nestedTypes)
		{
			string text3 = path + "." + type.Name;
			Localization.OverloadStrings(translated_strings, text3, type);
		}
	}

	public static string GetLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings.po");
	}

	public static void SwapToLocalizedFont()
	{
		string localizationFilePath = Localization.GetLocalizationFilePath();
		if (File.Exists(localizationFilePath))
		{
			TMP_FontAsset tmp_FontAsset = Resources.Load<TMP_FontAsset>("NotoSans-Regular SDF");
			foreach (TextStyleSetting textStyleSetting in Resources.FindObjectsOfTypeAll<TextStyleSetting>())
			{
				if (textStyleSetting != null)
				{
					textStyleSetting.sdfFont = tmp_FontAsset;
				}
			}
			foreach (LocText locText in Resources.FindObjectsOfTypeAll<LocText>())
			{
				if (locText != null)
				{
					locText.font = tmp_FontAsset;
				}
			}
		}
	}

	private struct Entry
	{
		public bool IsPopulated
		{
			get
			{
				return this.msgctxt != null && this.msgstr != null && this.msgstr.Length > 0;
			}
		}

		public string msgctxt;

		public string msgstr;
	}
}
