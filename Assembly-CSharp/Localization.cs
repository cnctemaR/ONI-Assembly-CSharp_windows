using System;
using System.Collections;
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

	public static void Initialize(bool dontCheckSteam = false)
	{
		global::Debug.Log("Localization.Initialize!", null);
		Localization.SelectedLanguageType selectedLanguageType = (Localization.SelectedLanguageType)Enum.Parse(typeof(Localization.SelectedLanguageType), KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()), true);
		if (selectedLanguageType == Localization.SelectedLanguageType.Preinstalled)
		{
			global::Debug.Log("Initialize... Preinstalled localization", null);
			string @string = KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_CODE_KEY, string.Empty);
			Localization.LoadPreinstalledTranslation(@string);
		}
		else if (selectedLanguageType == Localization.SelectedLanguageType.UGC && !dontCheckSteam && SteamManager.Initialized && LanguageOptionsScreen.HasInstalledLanguage())
		{
			global::Debug.Log("Initialize... SteamUGCService", null);
			PublishedFileId_t invalid = PublishedFileId_t.Invalid;
			LanguageOptionsScreen.LoadTranslation(ref invalid);
			if (invalid != PublishedFileId_t.Invalid)
			{
				Console.WriteLine("LOCALIZATION: Loaded steamworks file id: " + invalid.ToString());
			}
			else
			{
				Console.WriteLine("LOCALIZATION: Failed to load steamworks file id: " + invalid.ToString());
			}
		}
		else
		{
			global::Debug.Log("Initialize... Local mod localization", null);
			string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
			Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.None, modLocalizationFilePath);
		}
	}

	public static void LoadPreinstalledTranslation(string code)
	{
		if (!string.IsNullOrEmpty(code) && code != Localization.DEFAULT_LANGUAGE_CODE)
		{
			string preinstalledLocalizationFilePath = Localization.GetPreinstalledLocalizationFilePath(code);
			bool flag = Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.Preinstalled, preinstalledLocalizationFilePath);
			if (flag)
			{
				KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_CODE_KEY, code);
			}
		}
		else
		{
			Localization.ClearLanguage();
		}
	}

	public static bool LoadLocalTranslationFile(Localization.SelectedLanguageType source, string path)
	{
		if (File.Exists(path))
		{
			string[] array = File.ReadAllLines(path, Encoding.UTF8);
			bool flag = Localization.LoadTranslationFromLines(array);
			if (flag)
			{
				KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, source.ToString());
			}
			else
			{
				Localization.ClearLanguage();
			}
			return flag;
		}
		return false;
	}

	private static bool LoadTranslationFromLines(string[] lines)
	{
		bool flag = false;
		if (lines != null && lines.Length > 0)
		{
			Localization.sLocale = Localization.GetLocale(lines);
			flag = Localization.LoadTranslation(lines, false);
			if (flag)
			{
				Localization.currentFontName = Localization.GetFontName(lines);
				Localization.SwapToLocalizedFont(Localization.currentFontName);
			}
		}
		return flag;
	}

	private static bool LoadTranslation(string[] lines, bool isTemplate = false)
	{
		bool flag;
		try
		{
			Dictionary<string, string> dictionary = Localization.ExtractTranslatedStrings(lines, isTemplate);
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

	public static Dictionary<string, string> LoadStringsFile(string path, bool isTemplate)
	{
		string[] array = File.ReadAllLines(path, Encoding.UTF8);
		return Localization.ExtractTranslatedStrings(array, isTemplate);
	}

	private static Dictionary<string, string> ExtractTranslatedStrings(string[] lines, bool isTemplate = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Localization.Entry entry = default(Localization.Entry);
		string text = ((!isTemplate) ? "msgstr" : "msgid");
		for (int i = 0; i < lines.Length; i++)
		{
			string text2 = lines[i];
			if (text2 == null || text2.Length == 0)
			{
				entry = default(Localization.Entry);
			}
			else
			{
				string text3 = Localization.GetParameter("msgctxt", i, lines);
				if (text3 != null)
				{
					entry.msgctxt = text3;
				}
				text3 = Localization.GetParameter(text, i, lines);
				if (text3 != null)
				{
					entry.msgstr = text3;
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
		result = result.Replace("<color=^p", "<color=#");
		return result;
	}

	private static string GetParameter(string key, int idx, string[] all_lines)
	{
		if (!all_lines[idx].StartsWith(key))
		{
			return null;
		}
		List<string> list = new List<string>();
		string text = all_lines[idx];
		text = text.Substring(key.Length + 1, text.Length - key.Length - 1);
		list.Add(text);
		for (int i = idx + 1; i < all_lines.Length; i++)
		{
			string text2 = all_lines[i];
			if (!text2.StartsWith("\""))
			{
				break;
			}
			list.Add(text2);
		}
		string text3 = string.Empty;
		foreach (string text4 in list)
		{
			string text5 = text4;
			if (text5.EndsWith("\r"))
			{
				text5 = text5.Substring(0, text5.Length - 1);
			}
			text5 = text5.Substring(1, text5.Length - 2);
			text5 = Localization.FixupString(text5);
			text3 += text5;
		}
		return text3;
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings)
	{
		Assembly assembly = Assembly.GetAssembly(typeof(UI));
		IEnumerable<Type> enumerable = from t in assembly.GetTypes()
			where t.IsClass && t.Namespace == "STRINGS" && !t.IsNested
			select t;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		List<Type> list = enumerable.ToList<Type>();
		foreach (Type type in list)
		{
			string text = "STRINGS." + type.Name;
			Localization.OverloadStrings(translated_strings, text, type, ref empty, ref empty2, ref empty3);
		}
		if (!string.IsNullOrEmpty(empty))
		{
			global::Debug.Log("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + empty, null);
		}
		if (!string.IsNullOrEmpty(empty2))
		{
			global::Debug.Log("TRANSLATION ERROR! The following have mismatched <link> tags:\n" + empty2, null);
		}
		if (!string.IsNullOrEmpty(empty3))
		{
			global::Debug.Log("TRANSLATION ERROR! The following do not have the same amount of <link> tags as the english string which can cause nested link errors:\n" + empty3, null);
		}
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings, string path, Type t, ref string parameter_errors, ref string link_errors, ref string link_count_errors)
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
						if (Localization.HasSameOrLessLinkCountAsEnglish(locString.text, text2))
						{
							if (Localization.HasMatchingLinkTags(text2, 0))
							{
								fieldInfo.SetValue(null, locString2);
							}
							else
							{
								link_errors = link_errors + "\t" + text + "\n";
							}
						}
						else
						{
							link_count_errors = link_count_errors + "\t" + text + "\n";
						}
					}
					else
					{
						parameter_errors = parameter_errors + "\t" + text + "\n";
					}
				}
			}
		}
		Type[] nestedTypes = t.GetNestedTypes();
		foreach (Type type in nestedTypes)
		{
			string text3 = path + "." + type.Name;
			Localization.OverloadStrings(translated_strings, text3, type, ref parameter_errors, ref link_errors, ref link_count_errors);
		}
	}

	public static string GetDefaultLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings_template.pot");
	}

	public static string GetModLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings.po");
	}

	public static string GetPreinstalledLocalizationFilePath(string code)
	{
		string text = "Mods/strings_preinstalled_" + code + ".po";
		return Path.Combine(Application.streamingAssetsPath, text);
	}

	public static string GetPreinstalledLocalizationTitle(string code)
	{
		return Strings.Get("STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PREINSTALLED_LANGUAGES." + code.ToUpper());
	}

	public static Texture2D GetPreinstalledLocalizationImage(string code)
	{
		string text = Path.Combine(Application.streamingAssetsPath, "Mods/preinstalled_icon_" + code + ".png");
		if (File.Exists(text))
		{
			byte[] array = File.ReadAllBytes(text);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(array);
			return texture2D;
		}
		return null;
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

	public static TMP_FontAsset GetFontForLocale(string code)
	{
		Localization.Locale locale = Localization.GetLocaleForCode(code);
		if (locale == null)
		{
			locale = Localization.GetDefaultLocale();
		}
		return Resources.Load<TMP_FontAsset>(locale.FontName);
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
		Localization.SwapToLocalizedFont(Localization.currentFontName);
	}

	public static void SwapToLocalizedFont(string fontname)
	{
		if (!string.IsNullOrEmpty(fontname))
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

	private static bool HasSameOrLessTokenCount(string english_string, string translated_string, string token)
	{
		int num = english_string.Split(new string[] { token }, StringSplitOptions.None).Length;
		int num2 = translated_string.Split(new string[] { token }, StringSplitOptions.None).Length;
		return num >= num2;
	}

	private static bool HasSameOrLessLinkCountAsEnglish(string english_string, string translated_string)
	{
		return Localization.HasSameOrLessTokenCount(english_string, translated_string, "<link") && Localization.HasSameOrLessTokenCount(english_string, translated_string, "</link");
	}

	private static bool HasMatchingLinkTags(string str, int idx = 0)
	{
		int num = str.IndexOf("<link", idx);
		int num2 = str.IndexOf("</link", idx);
		if (num == -1 && num2 == -1)
		{
			return true;
		}
		if (num == -1 && num2 != -1)
		{
			return false;
		}
		if (num != -1 && num2 == -1)
		{
			return false;
		}
		if (num2 < num)
		{
			return false;
		}
		int num3 = str.IndexOf("<link", num + 1);
		return (num < 0 || num3 == -1 || num3 >= num2) && Localization.HasMatchingLinkTags(str, num2 + 1);
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
			IEnumerator enumerator = matchCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					string text = obj.ToString();
					bool flag2 = false;
					IEnumerator enumerator2 = matchCollection2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							string text2 = obj2.ToString();
							if (text == text2)
							{
								flag2 = true;
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator2 as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
					if (!flag2)
					{
						flag = false;
						break;
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = enumerator as IDisposable) != null)
				{
					disposable2.Dispose();
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

	public static void ClearLanguage()
	{
		Localization.sFontAsset = null;
		Localization.sLocale = null;
		KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString());
		KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_CODE_KEY, string.Empty);
		Localization.SwapToLocalizedFont(Localization.GetDefaultLocale().FontName);
		string defaultLocalizationFilePath = Localization.GetDefaultLocalizationFilePath();
		if (File.Exists(defaultLocalizationFilePath))
		{
			string[] array = File.ReadAllLines(defaultLocalizationFilePath, Encoding.UTF8);
			Localization.LoadTranslation(array, true);
		}
		LanguageOptionsScreen.CleanUpCurrentModLanguage();
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

	private static string currentFontName = null;

	public static string DEFAULT_LANGUAGE_CODE = "en";

	public static readonly List<string> PreinstalledLanguages = new List<string>
	{
		Localization.DEFAULT_LANGUAGE_CODE,
		"zh_klei",
		"ko_klei",
		"ru_klei"
	};

	public static string SELECTED_LANGUAGE_TYPE_KEY = "SelectedLanguageType";

	public static string SELECTED_LANGUAGE_CODE_KEY = "SelectedLanguageCode";

	private const string start_link_token = "<link";

	private const string end_link_token = "</link";

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

	public enum SelectedLanguageType
	{
		None,
		Preinstalled,
		UGC
	}
}
