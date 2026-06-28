using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class TagManager
{
	public static Tag Create(string tag_string, string proper_name = null)
	{
		Tag tag = default(Tag);
		tag.Name = tag_string;
		if (!TagManager.ProperNames.ContainsKey(tag) || proper_name != null)
		{
			TagManager.SetProperName(tag, proper_name);
		}
		return tag;
	}

	public static Tag[] Create(IList<string> strings)
	{
		Assert.IsTrue(strings != null && strings.Count > 0);
		Tag[] array = new Tag[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = TagManager.Create(strings[i], null);
		}
		return array;
	}

	public static void FillMissingProperNames()
	{
		foreach (Tag tag in new List<Tag>(TagManager.ProperNames.Keys))
		{
			if (TagManager.ProperNames[tag] == null)
			{
				TagManager.ProperNames[tag] = TagDescriptions.GetDescription(tag.Name);
			}
		}
	}

	public static void SetProperName(string tag_name, string name)
	{
		Tag tag = TagManager.Create(tag_name, null);
		TagManager.SetProperName(tag, name);
	}

	public static void SetProperName(Tag tag, string name)
	{
		TagManager.ProperNames[tag] = name;
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
