using System;
using System.Collections.Generic;

public class TagManager
{
	public static Tag Create(string tag_string)
	{
		Tag tag = default(Tag);
		tag.Name = tag_string;
		if (!TagManager.ProperNames.ContainsKey(tag))
		{
			TagManager.ProperNames[tag] = "";
			TagManager.ProperNamesNoLinks[tag] = "";
		}
		return tag;
	}

	public static Tag Create(string tag_string, string proper_name)
	{
		Tag tag = TagManager.Create(tag_string);
		if (string.IsNullOrEmpty(proper_name))
		{
			DebugUtil.Assert(false, "Attempting to set proper name for tag: " + tag_string + "to null or empty.");
		}
		TagManager.ProperNames[tag] = proper_name;
		TagManager.ProperNamesNoLinks[tag] = TagManager.StripLinkFormatting(proper_name);
		return tag;
	}

	public static Tag[] Create(IList<string> strings)
	{
		Tag[] array = new Tag[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = TagManager.Create(strings[i]);
		}
		return array;
	}

	public static void FillMissingProperNames()
	{
		foreach (Tag tag in new List<Tag>(TagManager.ProperNames.Keys))
		{
			if (string.IsNullOrEmpty(TagManager.ProperNames[tag]))
			{
				TagManager.ProperNames[tag] = TagDescriptions.GetDescription(tag.Name);
				TagManager.ProperNamesNoLinks[tag] = TagManager.StripLinkFormatting(TagManager.ProperNames[tag]);
			}
		}
	}

	public static string GetProperName(Tag tag, bool stripLink = false)
	{
		string text = null;
		if (stripLink && TagManager.ProperNamesNoLinks.TryGetValue(tag, out text))
		{
			return text;
		}
		if (!stripLink && TagManager.ProperNames.TryGetValue(tag, out text))
		{
			return text;
		}
		text = tag.Name;
		return text;
	}

	public static string StripLinkFormatting(string text)
	{
		string text2 = text;
		try
		{
			while (text2.Contains("<link="))
			{
				int num = text2.IndexOf("</link>");
				if (num > -1)
				{
					text2 = text2.Remove(num, 7);
				}
				else
				{
					Debug.LogWarningFormat("String has no closing link tag: {0}", Array.Empty<object>());
				}
				int num2 = text2.IndexOf("<link=");
				if (num2 != -1)
				{
					text2 = text2.Remove(num2, 7);
				}
				else
				{
					Debug.LogWarningFormat("String has no open link tag: {0}", Array.Empty<object>());
				}
				int num3 = text2.IndexOf("\">");
				if (num3 != -1)
				{
					text2 = text2.Remove(num2, num3 - num2 + 2);
				}
				else
				{
					Debug.LogWarningFormat("String has no open link tag: {0}", Array.Empty<object>());
				}
			}
		}
		catch
		{
			Debug.Log("STRIP LINK FORMATTING FAILED ON: " + text);
			text2 = text;
		}
		return text2;
	}

	private static Dictionary<Tag, string> ProperNames = new Dictionary<Tag, string>();

	private static Dictionary<Tag, string> ProperNamesNoLinks = new Dictionary<Tag, string>();

	public static readonly Tag Invalid = default(Tag);
}
