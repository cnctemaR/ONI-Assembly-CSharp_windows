using System;
using System.Collections.Generic;

public class CategoryEntry : CodexEntry
{
	public CategoryEntry(string category, List<ContentContainer> contentContainers, string name, List<CodexEntry> entriesInCategory)
		: base(category, contentContainers, name)
	{
		this.entriesInCategory = entriesInCategory;
	}

	public List<CodexEntry> entriesInCategory = new List<CodexEntry>();
}
