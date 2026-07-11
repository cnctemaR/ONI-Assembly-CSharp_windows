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
			}
		}
	}

	public static string GetProperName(Tag tag)
	{
		string text = null;
		if (!TagManager.ProperNames.TryGetValue(tag, out text))
		{
			text = tag.Name;
		}
		return text;
	}

	private static Dictionary<Tag, string> ProperNames = new Dictionary<Tag, string>();

	public static readonly Tag Invalid = default(Tag);
}
