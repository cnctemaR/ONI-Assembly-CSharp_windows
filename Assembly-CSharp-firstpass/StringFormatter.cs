using System;
using System.Collections.Generic;

public static class StringFormatter
{
	public static string Replace(string format, string token, string replacement)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = null;
		if (!StringFormatter.cachedReplacements.TryGetValue(format, out dictionary))
		{
			dictionary = new Dictionary<string, Dictionary<string, string>>();
			StringFormatter.cachedReplacements[format] = dictionary;
		}
		Dictionary<string, string> dictionary2 = null;
		if (!dictionary.TryGetValue(token, out dictionary2))
		{
			dictionary2 = new Dictionary<string, string>();
			dictionary[token] = dictionary2;
		}
		string text = null;
		if (!dictionary2.TryGetValue(replacement, out text))
		{
			text = format.Replace(token, replacement);
			dictionary2[replacement] = text;
		}
		return text;
	}

	public static string Combine(string a, string b, string c)
	{
		return StringFormatter.Combine(StringFormatter.Combine(a, b), c);
	}

	public static string Combine(string a, string b, string c, string d)
	{
		return StringFormatter.Combine(StringFormatter.Combine(StringFormatter.Combine(a, b), c), d);
	}

	public static string Combine(string a, string b)
	{
		Dictionary<string, string> dictionary = null;
		if (!StringFormatter.cachedCombines.TryGetValue(a, out dictionary))
		{
			dictionary = new Dictionary<string, string>();
			StringFormatter.cachedCombines[a] = dictionary;
		}
		string text = null;
		if (!dictionary.TryGetValue(b, out text))
		{
			text = a + b;
			dictionary[b] = text;
		}
		return text;
	}

	public static string ToUpper(string a)
	{
		HashedString hashedString = a;
		string text = null;
		if (!StringFormatter.cachedToUppers.TryGetValue(hashedString, out text))
		{
			text = a.ToUpper();
			StringFormatter.cachedToUppers[hashedString] = text;
		}
		return text;
	}

	private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> cachedReplacements = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

	private static Dictionary<string, Dictionary<string, string>> cachedCombines = new Dictionary<string, Dictionary<string, string>>();

	private static Dictionary<HashedString, string> cachedToUppers = new Dictionary<HashedString, string>();
}
