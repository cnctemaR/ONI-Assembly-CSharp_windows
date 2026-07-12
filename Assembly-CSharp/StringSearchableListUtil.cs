using System;
using System.Linq;

public static class StringSearchableListUtil
{
	public static bool DoAnyTagsMatchFilter(string[] lowercaseTags, in string filter)
	{
		string text = filter.Trim().ToLowerInvariant();
		string[] array = text.Split(new char[] { ' ' });
		for (int i = 0; i < lowercaseTags.Length; i++)
		{
			string tag = lowercaseTags[i];
			if (StringSearchableListUtil.DoesTagMatchFilter(tag, in text))
			{
				return true;
			}
			if (array.Select<string, bool>((string f) => StringSearchableListUtil.DoesTagMatchFilter(tag, in f)).All<bool>((bool result) => result))
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesTagMatchFilter(string lowercaseTag, in string filter)
	{
		return string.IsNullOrWhiteSpace(filter) || lowercaseTag.Contains(filter);
	}

	public static bool ShouldUseFilter(string filter)
	{
		return !string.IsNullOrWhiteSpace(filter);
	}
}
