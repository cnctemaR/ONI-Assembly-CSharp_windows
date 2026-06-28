using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using STRINGS;
using TMPro;
using UnityEngine;

public static class Localization
{
	public static bool LoadTranslation(string filename)
	{
		bool flag;
		try
		{
			Dictionary<string, string> dictionary = Localization.LoadTranslatedStrings(filename);
			Localization.OverloadStrings(dictionary);
			Localization.isLocalized = true;
			flag = true;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex, null);
			flag = false;
		}
		return flag;
	}

	public static bool LoadTranslation(string[] lines)
	{
		bool flag;
		try
		{
			Dictionary<string, string> dictionary = Localization.LoadTranslatedStrings(lines);
			Localization.OverloadStrings(dictionary);
			Localization.isLocalized = true;
			flag = true;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex, null);
			flag = false;
		}
		return flag;
	}

	private static Dictionary<string, string> LoadTranslatedStrings(string filename)
	{
		string[] array = File.ReadAllLines(filename, Encoding.UTF8);
		return Localization.LoadTranslatedStrings(array);
	}

	private static Dictionary<string, string> LoadTranslatedStrings(string[] lines)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Localization.Entry entry = default(Localization.Entry);
		for (int i = 0; i < lines.Length; i++)
		{
			string text = lines[i];
			if (text == null || text.Length == 0)
			{
				entry = default(Localization.Entry);
			}
			else
			{
				string text2 = Localization.GetParameter("msgctxt", text);
				if (text2 != null)
				{
					if (text2 == string.Empty)
					{
						string text3 = lines[i + 1];
						if (text3.StartsWith("\""))
						{
							text2 = text3.Substring(1, text3.Length - 2);
							i++;
						}
					}
					entry.msgctxt = text2;
				}
				text2 = Localization.GetParameter("msgstr", text);
				if (text2 != null)
				{
					entry.msgstr = text2;
				}
				if (text.StartsWith("msgstr"))
				{
					for (int j = i + 1; j < lines.Length; j++)
					{
						string text4 = lines[j];
						if (!text4.StartsWith("\""))
						{
							break;
						}
						text4 = Localization.FixupString(text4);
						if (text4 != null && text4.Length > 2)
						{
							if (text2 == null)
							{
								text2 = text4.Substring(1, text4.Length - 2);
							}
							else
							{
								text2 += text4.Substring(1, text4.Length - 2);
							}
						}
						i++;
					}
					if (text2 != null)
					{
						entry.msgstr = text2.Replace("<color=^p", "<color=#");
					}
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

	private static string FixupString(string result)
	{
		result = result.Replace("\\n", "\n");
		result = result.Replace("\\\"", "\"");
		result = result.Replace("<style=“", "<style=\"");
		result = result.Replace("”>", "\">");
		return result;
	}

	private static string GetParameter(string key, string line)
	{
		string text = null;
		if (line.EndsWith("\r"))
		{
			line = line.Substring(0, line.Length - 1);
		}
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
				text = Localization.FixupString(text);
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
					LocString locString2 = (LocString)fieldInfo.GetValue(null);
					if (Localization.AreParametersPreserved(locString2.text, text2))
					{
						fieldInfo.SetValue(null, locString);
					}
					else
					{
						global::Debug.Log("TRANSLATION ERROR! " + text + " has missing or mismatched parameters", null);
					}
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

	private static string GetFontParam(string line)
	{
		string text = null;
		if (line.StartsWith("\"Font:"))
		{
			text = line.Substring("\"Font:".Length).Trim();
			text = text.Replace("\\n", string.Empty);
			text = text.Replace("\"", string.Empty);
		}
		return text;
	}

	private static string GetLanguageParam(string line)
	{
		string text = null;
		if (line.StartsWith("\"Language:"))
		{
			text = line.Substring("\"Language:".Length).Trim();
			text = text.Replace("\\n", string.Empty);
			text = text.Replace("\"", string.Empty);
		}
		return text;
	}

	public static string GetLanguageCode(string[] lines)
	{
		foreach (string text in lines)
		{
			if (text != null && text.Length != 0)
			{
				string languageParam = Localization.GetLanguageParam(text);
				if (languageParam != null)
				{
					return languageParam;
				}
			}
		}
		return null;
	}

	private static string GetFontForLocalization(string filename)
	{
		string[] array = File.ReadAllLines(filename, Encoding.UTF8);
		return Localization.GetFontForLocalization(array);
	}

	public static string GetFontNameForLocalization(string fontParam)
	{
		foreach (Localization.LocInfo locInfo in Localization.Localizations)
		{
			if (locInfo.MatchesFont(fontParam) || locInfo.MatchesCode(fontParam))
			{
				return locInfo.FontName;
			}
		}
		return null;
	}

	public static string GetFontForLocalization(string[] lines)
	{
		string text = null;
		foreach (string text2 in lines)
		{
			if (text2 != null && text2.Length != 0)
			{
				string fontParam = Localization.GetFontParam(text2);
				if (fontParam != null)
				{
					text = Localization.GetFontNameForLocalization(fontParam);
				}
			}
			if (text != null)
			{
				break;
			}
		}
		if (text == null)
		{
			foreach (string text3 in lines)
			{
				if (text3 != null && text3.Length != 0)
				{
					string languageParam = Localization.GetLanguageParam(text3);
					if (languageParam != null)
					{
						text = Localization.GetFontNameForLocalization(languageParam);
					}
					if (text != null)
					{
						break;
					}
				}
			}
		}
		if (text == null)
		{
			foreach (Localization.LocInfo locInfo in Localization.Localizations)
			{
				if (locInfo.Lang == Localization.Language.Unspecified)
				{
					text = locInfo.FontName;
				}
			}
		}
		return text;
	}

	public static void SwapToLocalizedFont()
	{
		string localizationFilePath = Localization.GetLocalizationFilePath();
		if (File.Exists(localizationFilePath))
		{
			string fontForLocalization = Localization.GetFontForLocalization(localizationFilePath);
			Localization.SwapToLocalizedFont(fontForLocalization);
		}
	}

	public static void SwapToLocalizedFont(string fontname)
	{
		if (fontname != null)
		{
			TMP_FontAsset tmp_FontAsset = Resources.Load<TMP_FontAsset>(fontname);
			if (tmp_FontAsset != null)
			{
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
			else
			{
				Console.WriteLine("LOCALIZATION ERROR! Font [" + fontname + "] not found");
			}
		}
	}

	private static bool AreParametersPreserved(string old_string, string new_string)
	{
		MatchCollection matchCollection = Regex.Matches(old_string, "{.*?}");
		MatchCollection matchCollection2 = Regex.Matches(new_string, "{.*?}");
		bool flag = false;
		if (matchCollection == null && matchCollection2 == null)
		{
			flag = true;
		}
		else if (matchCollection != null && matchCollection2 != null && matchCollection.Count == matchCollection2.Count)
		{
			flag = true;
			foreach (object obj in matchCollection)
			{
				string text = obj.ToString();
				bool flag2 = false;
				foreach (object obj2 in matchCollection2)
				{
					string text2 = obj2.ToString();
					if (text == text2)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	public static bool HasDirtyWords(string str)
	{
		return Localization.FilterDirtyWords(str) != str;
	}

	public static string FilterDirtyWords(string str)
	{
		return DistributionPlatform.Inst.ApplyWordFilter(str);
	}

	public static string GetFileDateFormat(int format_idx)
	{
		return "{" + format_idx.ToString() + ":dd / MMM / yyyy}";
	}

	public static bool isLocalized;

	private static readonly List<Localization.LocInfo> Localizations = new List<Localization.LocInfo>
	{
		new Localization.LocInfo(Localization.Language.Chinese, "zh", "NotoSansCJKsc-Regular"),
		new Localization.LocInfo(Localization.Language.Japanese, "ja", "NotoSansCJKjp-Regular"),
		new Localization.LocInfo(Localization.Language.Korean, "ko", "NotoSansCJKkr-Regular"),
		new Localization.LocInfo(Localization.Language.Russian, "ru", "RobotoCondensed-Regular"),
		new Localization.LocInfo(Localization.Language.Thai, "th", "NotoSansThai-Regular"),
		new Localization.LocInfo(Localization.Language.Unspecified, string.Empty, "RobotoCondensed-Regular")
	};

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

	private enum Language
	{
		Chinese,
		Japanese,
		Korean,
		Russian,
		Thai,
		Unspecified
	}

	private class LocInfo
	{
		public LocInfo(Localization.Language language, string code, string fontName)
		{
			this.mLanguage = language;
			this.mCode = code.ToLower();
			this.mFontName = fontName;
		}

		public Localization.Language Lang
		{
			get
			{
				return this.mLanguage;
			}
		}

		public string Code
		{
			get
			{
				return this.mCode;
			}
		}

		public string FontName
		{
			get
			{
				return this.mFontName;
			}
		}

		public bool MatchesCode(string language_code)
		{
			return language_code.ToLower().Contains(this.mCode);
		}

		public bool MatchesFont(string fontname)
		{
			return fontname.ToLower() == this.mFontName.ToLower();
		}

		private Localization.Language mLanguage;

		private string mCode;

		private string mFontName;
	}
}
