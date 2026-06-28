using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ArabicSupport;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;

public static class Localization
{
	public static TMP_FontAsset FontAsset
	{
		get
		{
			return Localization.sFontAsset;
		}
	}

	public static bool IsRightToLeft
	{
		get
		{
			return Localization.sLocale != null && Localization.sLocale.IsRightToLeft;
		}
	}

	private static Dictionary<string, string> LoadTranslatedStrings(string filename)
	{
		string[] array = File.ReadAllLines(filename, Encoding.UTF8);
		return Localization.LoadTranslatedStrings(array, false);
	}

	public static void Initialize()
	{
		if (SteamManager.Initialized && SteamUGCService.HasInstalledLanguage())
		{
			Localization.sLocale = SteamUGCService.GetLocale();
			SteamUGCService.LoadTranslation(false);
			SteamUGCService.SetFontForLocalization();
		}
		else
		{
			string localizationFilePath = Localization.GetLocalizationFilePath();
			if (File.Exists(localizationFilePath))
			{
				string[] array = File.ReadAllLines(localizationFilePath, Encoding.UTF8);
				Localization.sLocale = Localization.GetLocale(array);
				string fontName = Localization.GetFontName(array);
				Localization.LoadTranslation(array, false);
				Localization.SwapToLocalizedFont(fontName);
			}
		}
	}

	public static bool LoadTranslation(string[] lines, bool reset = false)
	{
		bool flag;
		try
		{
			Dictionary<string, string> dictionary = Localization.LoadTranslatedStrings(lines, reset);
			Localization.OverloadStrings(dictionary);
			flag = true;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex, null);
			flag = false;
		}
		return flag;
	}

	private static Dictionary<string, string> LoadTranslatedStrings(string[] lines, bool isReset = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Localization.Entry entry = default(Localization.Entry);
		string text = ((!isReset) ? "msgstr" : "msgid");
		for (int i = 0; i < lines.Length; i++)
		{
			string text2 = lines[i];
			if (text2 == null || text2.Length == 0)
			{
				entry = default(Localization.Entry);
			}
			else
			{
				string text3 = Localization.GetParameter("msgctxt", text2);
				if (text3 != null)
				{
					if (text3 == string.Empty)
					{
						string text4 = lines[i + 1];
						if (text4.StartsWith("\""))
						{
							text3 = text4.Substring(1, text4.Length - 2);
							i++;
						}
					}
					entry.msgctxt = text3;
				}
				text3 = Localization.GetParameter(text, text2);
				if (text3 != null)
				{
					entry.msgstr = text3;
				}
				if (text2.StartsWith(text))
				{
					for (int j = i + 1; j < lines.Length; j++)
					{
						string text5 = lines[j];
						if (!text5.StartsWith("\""))
						{
							break;
						}
						text5 = Localization.FixupString(text5);
						if (text5 != null && text5.Length > 2)
						{
							if (text3 == null)
							{
								text3 = text5.Substring(1, text5.Length - 2);
							}
							else
							{
								text3 += text5.Substring(1, text5.Length - 2);
							}
						}
						i++;
					}
					if (text3 != null)
					{
						entry.msgstr = text3.Replace("<color=^p", "<color=#");
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
		string empty = string.Empty;
		List<Type> list = enumerable.ToList<Type>();
		foreach (Type type in list)
		{
			string text = "STRINGS." + type.Name;
			Localization.OverloadStrings(translated_strings, text, type, ref empty);
		}
		if (empty != string.Empty)
		{
			global::Debug.Log("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + empty, null);
		}
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings, string path, Type t, ref string errors)
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
					LocString locString = (LocString)fieldInfo.GetValue(null);
					LocString locString2 = new LocString(text2, text);
					if (Localization.AreParametersPreserved(locString.text, text2))
					{
						fieldInfo.SetValue(null, locString2);
					}
					else
					{
						errors = errors + "\t" + text + "\n";
					}
				}
			}
		}
		Type[] nestedTypes = t.GetNestedTypes();
		foreach (Type type in nestedTypes)
		{
			string text3 = path + "." + type.Name;
			Localization.OverloadStrings(translated_strings, text3, type, ref errors);
		}
	}

	public static string GetLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings.po");
	}

	public static void SetLocale(Localization.Locale locale)
	{
		Localization.sLocale = locale;
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

	private static string GetLanguageCode(string line)
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

	private static Localization.Locale GetLocaleForCode(string code)
	{
		Localization.Locale locale = null;
		foreach (Localization.Locale locale2 in Localization.Locales)
		{
			if (locale2.MatchesCode(code))
			{
				locale = locale2;
				break;
			}
		}
		return locale;
	}

	public static Localization.Locale GetLocale(string[] lines)
	{
		Localization.Locale locale = null;
		string text = null;
		if (lines != null && lines.Length > 0)
		{
			foreach (string text2 in lines)
			{
				if (text2 != null && text2.Length != 0)
				{
					text = Localization.GetLanguageCode(text2);
					if (text != null)
					{
						locale = Localization.GetLocaleForCode(text);
					}
					if (text != null)
					{
						break;
					}
				}
			}
		}
		if (locale == null)
		{
			locale = Localization.GetDefaultLocale();
		}
		if (text != null && locale.Code == string.Empty)
		{
			locale.SetCode(text);
		}
		return locale;
	}

	private static string GetFontName(string filename)
	{
		string[] array = File.ReadAllLines(filename, Encoding.UTF8);
		return Localization.GetFontName(array);
	}

	public static Localization.Locale GetDefaultLocale()
	{
		Localization.Locale locale = null;
		foreach (Localization.Locale locale2 in Localization.Locales)
		{
			if (locale2.Lang == Localization.Language.Unspecified)
			{
				locale = new Localization.Locale(locale2);
				break;
			}
		}
		return locale;
	}

	public static string GetDefaultFontName()
	{
		string text = null;
		foreach (Localization.Locale locale in Localization.Locales)
		{
			if (locale.Lang == Localization.Language.Unspecified)
			{
				text = locale.FontName;
				break;
			}
		}
		return text;
	}

	public static string ValidateFontName(string fontName)
	{
		foreach (Localization.Locale locale in Localization.Locales)
		{
			if (locale.MatchesFont(fontName))
			{
				return locale.FontName;
			}
		}
		return null;
	}

	public static string GetFontName(string[] lines)
	{
		string text = null;
		foreach (string text2 in lines)
		{
			if (text2 != null && text2.Length != 0)
			{
				string fontParam = Localization.GetFontParam(text2);
				if (fontParam != null)
				{
					text = Localization.ValidateFontName(fontParam);
				}
			}
			if (text != null)
			{
				break;
			}
		}
		if (text == null)
		{
			if (Localization.sLocale != null)
			{
				text = Localization.sLocale.FontName;
			}
			else
			{
				text = Localization.GetDefaultFontName();
			}
		}
		return text;
	}

	public static void SwapToLocalizedFont()
	{
		string localizationFilePath = Localization.GetLocalizationFilePath();
		if (File.Exists(localizationFilePath))
		{
			string[] array = File.ReadAllLines(localizationFilePath, Encoding.UTF8);
			Localization.sLocale = Localization.GetLocale(array);
			string fontName = Localization.GetFontName(array);
			Localization.SwapToLocalizedFont(fontName);
		}
	}

	public static void SwapToLocalizedFont(string fontname)
	{
		if (fontname != null)
		{
			TMP_FontAsset tmp_FontAsset = Resources.Load<TMP_FontAsset>(fontname);
			if (tmp_FontAsset != null)
			{
				Localization.sFontAsset = tmp_FontAsset;
				foreach (TextStyleSetting textStyleSetting in Resources.FindObjectsOfTypeAll<TextStyleSetting>())
				{
					if (textStyleSetting != null)
					{
						textStyleSetting.sdfFont = tmp_FontAsset;
					}
				}
				bool isRightToLeft = Localization.IsRightToLeft;
				foreach (LocText locText in Resources.FindObjectsOfTypeAll<LocText>())
				{
					if (locText != null)
					{
						locText.SwapFont(tmp_FontAsset, isRightToLeft);
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

	public static void SetLanguage(PublishedFileId_t item)
	{
		SteamUGCService.Instance.SetCurrentLanguage(item);
		SteamUGCService.Instance.OnRefreshLanguage = null;
	}

	public static void ClearLanguage()
	{
		Localization.sFontAsset = null;
		SteamUGCService.Instance.ClearInstall();
	}

	private static string ReverseText(string source)
	{
		char[] array = new char[] { '\n' };
		string[] array2 = source.Split(array);
		string text = string.Empty;
		int num = 0;
		foreach (string text2 in array2)
		{
			num++;
			char[] array4 = new char[text2.Length];
			for (int j = 0; j < text2.Length; j++)
			{
				array4[array4.Length - 1 - j] = text2[j];
			}
			text += new string(array4);
			if (num < array2.Length)
			{
				text += "\n";
			}
		}
		return text;
	}

	public static string Fixup(string text)
	{
		if (Localization.sLocale != null && text != null && text != string.Empty && Localization.sLocale.Lang == Localization.Language.Arabic)
		{
			return Localization.ReverseText(ArabicFixer.Fix(text));
		}
		return text;
	}

	private static TMP_FontAsset sFontAsset = null;

	private static readonly List<Localization.Locale> Locales = new List<Localization.Locale>
	{
		new Localization.Locale(Localization.Language.Chinese, Localization.Direction.LeftToRight, "zh", "NotoSansCJKsc-Regular"),
		new Localization.Locale(Localization.Language.Japanese, Localization.Direction.LeftToRight, "ja", "NotoSansCJKjp-Regular"),
		new Localization.Locale(Localization.Language.Korean, Localization.Direction.LeftToRight, "ko", "NotoSansCJKkr-Regular"),
		new Localization.Locale(Localization.Language.Russian, Localization.Direction.LeftToRight, "ru", "RobotoCondensed-Regular"),
		new Localization.Locale(Localization.Language.Thai, Localization.Direction.LeftToRight, "th", "NotoSansThai-Regular"),
		new Localization.Locale(Localization.Language.Arabic, Localization.Direction.RightToLeft, "ar", "NotoNaskhArabic-Regular"),
		new Localization.Locale(Localization.Language.Hebrew, Localization.Direction.RightToLeft, "he", "NotoSansHebrew-Regular"),
		new Localization.Locale(Localization.Language.Unspecified, Localization.Direction.LeftToRight, string.Empty, "RobotoCondensed-Regular")
	};

	private static Localization.Locale sLocale = null;

	public enum Language
	{
		Chinese,
		Japanese,
		Korean,
		Russian,
		Thai,
		Arabic,
		Hebrew,
		Unspecified
	}

	public enum Direction
	{
		LeftToRight,
		RightToLeft
	}

	public class Locale
	{
		public Locale(Localization.Locale other)
		{
			this.mLanguage = other.mLanguage;
			this.mDirection = other.mDirection;
			this.mCode = other.mCode;
			this.mFontName = other.mFontName;
		}

		public Locale(Localization.Language language, Localization.Direction direction, string code, string fontName)
		{
			this.mLanguage = language;
			this.mDirection = direction;
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

		public void SetCode(string code)
		{
			this.mCode = code;
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

		public bool IsRightToLeft
		{
			get
			{
				return this.mDirection == Localization.Direction.RightToLeft;
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

		public override string ToString()
		{
			return string.Concat(new object[] { this.mCode, ":", this.mLanguage, ":", this.mDirection, ":", this.mFontName });
		}

		private Localization.Language mLanguage;

		private string mCode;

		private string mFontName;

		private Localization.Direction mDirection;
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
