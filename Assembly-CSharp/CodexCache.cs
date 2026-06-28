using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

public static class CodexCache
{
	public static string FormatLinkID(string linkID)
	{
		linkID = linkID.ToUpper();
		linkID = linkID.Replace("_", string.Empty);
		return linkID;
	}

	public static void Init()
	{
		CodexCache.entries = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		string text = CodexCache.FormatLinkID("creatures");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.CREATURES, CodexEntryGenerator.GenerateCreatureEntries()));
		text = CodexCache.FormatLinkID("plants");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.PLANTS, CodexEntryGenerator.GeneratePlantEntries()));
		text = CodexCache.FormatLinkID("food");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.FOOD, CodexEntryGenerator.GenerateFoodEntries()));
		text = CodexCache.FormatLinkID("buildings");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.BUILDINGS, CodexEntryGenerator.GenerateBuildingEntries()));
		text = CodexCache.FormatLinkID("tech");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.TECH, CodexEntryGenerator.GenerateTechEntries()));
		text = CodexCache.FormatLinkID("roles");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ROLES, CodexEntryGenerator.GenerateRoleEntries()));
		text = CodexCache.FormatLinkID("disease");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.DISEASE, CodexEntryGenerator.GenerateDiseaseEntries()));
		text = CodexCache.FormatLinkID("elements");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ELEMENTS, CodexEntryGenerator.GenerateElementEntries()));
		text = CodexCache.FormatLinkID("geysers");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.GEYSERS, CodexEntryGenerator.GenerateGeyserEntries()));
		CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID("HOME"), UI.CODEX.CATEGORYNAMES.ROOT, dictionary);
		CodexEntryGenerator.GeneratePageNotFound();
		CodexCache.baseEntryPath = Application.streamingAssetsPath + "/codex";
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in dictionary)
		{
			list.Add(keyValuePair.Value as CategoryEntry);
		}
		List<CodexEntry> list2 = CodexCache.CollectEntries(string.Empty);
		foreach (CodexEntry codexEntry in list2)
		{
			if (codexEntry != null && codexEntry.id != null && codexEntry.contentContainers != null)
			{
				if (CodexCache.entries.ContainsKey(CodexCache.FormatLinkID(codexEntry.id)))
				{
					CodexCache.MergeEntry(codexEntry.id, codexEntry);
				}
				else
				{
					CodexCache.AddEntry(codexEntry.id, codexEntry, list);
				}
			}
		}
		foreach (string text2 in Directory.GetDirectories(CodexCache.baseEntryPath))
		{
			List<CodexEntry> list3 = CodexCache.CollectEntries(Path.GetFileNameWithoutExtension(text2));
			foreach (CodexEntry codexEntry2 in list3)
			{
				if (codexEntry2 != null && codexEntry2.id != null && codexEntry2.contentContainers != null)
				{
					if (CodexCache.entries.ContainsKey(CodexCache.FormatLinkID(codexEntry2.id)))
					{
						CodexCache.MergeEntry(codexEntry2.id, codexEntry2);
					}
					else
					{
						CodexCache.AddEntry(codexEntry2.id, codexEntry2, list);
					}
				}
			}
		}
		list.Add(categoryEntry);
		CodexEntryGenerator.PopulateCategoryEntries(list);
	}

	public static void AddEntry(string id, CodexEntry entry, List<CategoryEntry> categoryEntries = null)
	{
		id = CodexCache.FormatLinkID(id);
		CodexCache.entries.Add(id, entry);
		entry.id = id;
		if (entry.name == null)
		{
			entry.name = Strings.Get(entry.title);
		}
		if (!string.IsNullOrEmpty(entry.iconPrefabID))
		{
			try
			{
				entry.icon = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(entry.iconPrefabID).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
			}
			catch
			{
				global::Debug.LogWarningFormat("Unable to get icon for prefabID {0}", new object[] { entry.iconPrefabID });
			}
		}
		if (categoryEntries != null)
		{
			CodexEntry codexEntry = categoryEntries.Find((CategoryEntry group) => group.id == entry.parentId);
			if (codexEntry == null)
			{
				return;
			}
			(codexEntry as CategoryEntry).entriesInCategory.Add(entry);
		}
	}

	public static void MergeEntry(string id, CodexEntry entry)
	{
		id = CodexCache.FormatLinkID(entry.id);
		entry.id = id;
		CodexEntry codexEntry = CodexCache.entries[id];
		for (int i = entry.contentContainers.Count - 1; i >= 0; i--)
		{
			codexEntry.contentContainers.Insert(0, entry.contentContainers[i]);
		}
		if (entry.disabled)
		{
			codexEntry.disabled = entry.disabled;
		}
	}

	public static void Clear()
	{
		CodexCache.entries = null;
		CodexCache.baseEntryPath = null;
	}

	public static string GetEntryPath()
	{
		return CodexCache.baseEntryPath;
	}

	public static CodexEntry GetTemplate(string templatePath)
	{
		if (!CodexCache.entries.ContainsKey(templatePath))
		{
			CodexCache.entries.Add(templatePath, null);
		}
		if (CodexCache.entries[templatePath] == null)
		{
			string text = Path.Combine(CodexCache.baseEntryPath, templatePath);
			CodexEntry codexEntry = YamlIO<CodexEntry>.LoadFile(text + ".yaml");
			if (codexEntry == null)
			{
				global::Debug.LogWarning("Missing template [" + text + ".yaml]", null);
			}
			CodexCache.entries[templatePath] = codexEntry;
		}
		return CodexCache.entries[templatePath];
	}

	public static List<CodexEntry> CollectEntries(string folder)
	{
		List<CodexEntry> list = new List<CodexEntry>();
		string text = ((!(folder == string.Empty)) ? Path.Combine(CodexCache.baseEntryPath, folder) : CodexCache.baseEntryPath);
		string[] files = Directory.GetFiles(text, "*.yaml");
		foreach (string text2 in files)
		{
			CodexEntry codexEntry = YamlIO<CodexEntry>.LoadFile(text2);
			if (codexEntry != null)
			{
				codexEntry.category = folder.ToUpper();
				list.Add(codexEntry);
			}
		}
		list.Sort((CodexEntry x, CodexEntry y) => x.title.CompareTo(y.title));
		return list;
	}

	private static string baseEntryPath;

	public static Dictionary<string, CodexEntry> entries;
}
