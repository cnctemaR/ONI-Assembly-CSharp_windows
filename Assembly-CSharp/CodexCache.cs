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
		linkID = linkID.Replace("_", "");
		return linkID;
	}

	public static void CodexCacheInit()
	{
		CodexCache.entries = new Dictionary<string, CodexEntry>();
		CodexCache.subEntries = new Dictionary<string, SubEntry>();
		CodexCache.unlockedEntryLookup = new Dictionary<string, List<string>>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		if (CodexCache.widgetTagMappings == null)
		{
			CodexCache.widgetTagMappings = new List<global::Tuple<string, Type>>
			{
				new global::Tuple<string, Type>("!CodexText", typeof(CodexText)),
				new global::Tuple<string, Type>("!CodexImage", typeof(CodexImage)),
				new global::Tuple<string, Type>("!CodexDividerLine", typeof(CodexDividerLine)),
				new global::Tuple<string, Type>("!CodexSpacer", typeof(CodexSpacer)),
				new global::Tuple<string, Type>("!CodexLabelWithIcon", typeof(CodexLabelWithIcon)),
				new global::Tuple<string, Type>("!CodexLabelWithLargeIcon", typeof(CodexLabelWithLargeIcon)),
				new global::Tuple<string, Type>("!CodexContentLockedIndicator", typeof(CodexContentLockedIndicator)),
				new global::Tuple<string, Type>("!CodexLargeSpacer", typeof(CodexLargeSpacer)),
				new global::Tuple<string, Type>("!CodexVideo", typeof(CodexVideo))
			};
		}
		string text = CodexCache.FormatLinkID("LESSONS");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.TIPS, CodexEntryGenerator.GenerateTutorialNotificationEntries(), Assets.GetSprite("codexIconLessons"), true, false, UI.CODEX.CATEGORYNAMES.VIDEOS));
		text = CodexCache.FormatLinkID("creatures");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.CREATURES, CodexEntryGenerator.GenerateCreatureEntries(), Assets.GetSprite("codexIconCritters"), true, true, null));
		text = CodexCache.FormatLinkID("plants");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.PLANTS, CodexEntryGenerator.GeneratePlantEntries(), Assets.GetSprite("codexIconPlants"), true, true, null));
		text = CodexCache.FormatLinkID("food");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.FOOD, CodexEntryGenerator.GenerateFoodEntries(), Assets.GetSprite("codexIconFood"), true, true, null));
		text = CodexCache.FormatLinkID("buildings");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.BUILDINGS, CodexEntryGenerator.GenerateBuildingEntries(), Assets.GetSprite("codexIconBuildings"), true, true, null));
		text = CodexCache.FormatLinkID("tech");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.TECH, CodexEntryGenerator.GenerateTechEntries(), Assets.GetSprite("codexIconResearch"), true, true, null));
		text = CodexCache.FormatLinkID("roles");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ROLES, CodexEntryGenerator.GenerateRoleEntries(), Assets.GetSprite("codexIconSkills"), true, true, null));
		text = CodexCache.FormatLinkID("disease");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.DISEASE, CodexEntryGenerator.GenerateDiseaseEntries(), Assets.GetSprite("codexIconDisease"), false, true, null));
		text = CodexCache.FormatLinkID("elements");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ELEMENTS, CodexEntryGenerator.GenerateElementEntries(), Assets.GetSprite("codexIconElements"), true, false, null));
		text = CodexCache.FormatLinkID("geysers");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.GEYSERS, CodexEntryGenerator.GenerateGeyserEntries(), Assets.GetSprite("codexIconGeysers"), true, true, null));
		text = CodexCache.FormatLinkID("equipment");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.EQUIPMENT, CodexEntryGenerator.GenerateEquipmentEntries(), Assets.GetSprite("codexIconEquipment"), true, true, null));
		text = CodexCache.FormatLinkID("biomes");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.BIOMES, CodexEntryGenerator.GenerateBiomeEntries(), Assets.GetSprite("codexIconGeysers"), true, true, null));
		CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID("HOME"), UI.CODEX.CATEGORYNAMES.ROOT, dictionary, null, true, true, null);
		CodexEntryGenerator.GeneratePageNotFound();
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in dictionary)
		{
			list.Add(keyValuePair.Value as CategoryEntry);
		}
		CodexCache.CollectYAMLEntries(list);
		CodexCache.CollectYAMLSubEntries(list);
		CodexCache.CheckUnlockableContent();
		list.Add(categoryEntry);
		foreach (KeyValuePair<string, CodexEntry> keyValuePair2 in CodexCache.entries)
		{
			if (keyValuePair2.Value.subEntries.Count > 0)
			{
				keyValuePair2.Value.subEntries.Sort((SubEntry a, SubEntry b) => a.layoutPriority.CompareTo(b.layoutPriority));
				if (keyValuePair2.Value.icon == null)
				{
					keyValuePair2.Value.icon = keyValuePair2.Value.subEntries[0].icon;
					keyValuePair2.Value.iconColor = keyValuePair2.Value.subEntries[0].iconColor;
				}
				int num = 0;
				foreach (SubEntry subEntry in keyValuePair2.Value.subEntries)
				{
					if (subEntry.lockID != null && !Game.Instance.unlocks.IsUnlocked(subEntry.lockID))
					{
						num++;
					}
				}
				if (keyValuePair2.Value.subEntries.Count > 1)
				{
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					list2.Add(new CodexSpacer());
					list2.Add(new CodexText(string.Format(CODEX.HEADERS.SUBENTRIES, keyValuePair2.Value.subEntries.Count - num, keyValuePair2.Value.subEntries.Count), CodexTextStyle.Subtitle, null));
					foreach (SubEntry subEntry2 in keyValuePair2.Value.subEntries)
					{
						if (subEntry2.lockID != null && !Game.Instance.unlocks.IsUnlocked(subEntry2.lockID))
						{
							list2.Add(new CodexText(UI.FormatAsLink(CODEX.HEADERS.CONTENTLOCKED, UI.ExtractLinkID(subEntry2.name)), CodexTextStyle.Body, null));
						}
						else
						{
							string text2 = UI.StripLinkFormatting(subEntry2.name);
							text2 = UI.FormatAsLink(text2, subEntry2.id);
							list2.Add(new CodexText(text2, CodexTextStyle.Body, null));
						}
					}
					list2.Add(new CodexSpacer());
					keyValuePair2.Value.contentContainers.Insert(keyValuePair2.Value.customContentLength, new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
				}
			}
			for (int i = 0; i < keyValuePair2.Value.subEntries.Count; i++)
			{
				keyValuePair2.Value.AddContentContainerRange(keyValuePair2.Value.subEntries[i].contentContainers);
			}
		}
		CodexEntryGenerator.PopulateCategoryEntries(list, delegate(CodexEntry a, CodexEntry b)
		{
			if (a.name == UI.CODEX.CATEGORYNAMES.TIPS)
			{
				return -1;
			}
			if (b.name == UI.CODEX.CATEGORYNAMES.TIPS)
			{
				return 1;
			}
			return UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name));
		});
	}

	public static CodexEntry FindEntry(string id)
	{
		if (CodexCache.entries == null)
		{
			global::Debug.LogWarning("Can't search Codex cache while it's stil null");
			return null;
		}
		if (CodexCache.entries.ContainsKey(id))
		{
			return CodexCache.entries[id];
		}
		global::Debug.LogWarning("Could not find codex entry with id: " + id);
		return null;
	}

	public static SubEntry FindSubEntry(string id)
	{
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			foreach (SubEntry subEntry in keyValuePair.Value.subEntries)
			{
				if (subEntry.id.ToUpper() == id.ToUpper())
				{
					return subEntry;
				}
			}
		}
		return null;
	}

	private static void CheckUnlockableContent()
	{
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			foreach (SubEntry subEntry in keyValuePair.Value.subEntries)
			{
				if (subEntry.lockedContentContainer != null)
				{
					subEntry.lockedContentContainer.content.Clear();
					subEntry.contentContainers.Remove(subEntry.lockedContentContainer);
				}
			}
		}
	}

	private static void CollectYAMLEntries(List<CategoryEntry> categories)
	{
		CodexCache.baseEntryPath = Application.streamingAssetsPath + "/codex";
		foreach (CodexEntry codexEntry in CodexCache.CollectEntries(""))
		{
			if (codexEntry != null && codexEntry.id != null && codexEntry.contentContainers != null)
			{
				if (CodexCache.entries.ContainsKey(CodexCache.FormatLinkID(codexEntry.id)))
				{
					CodexCache.MergeEntry(codexEntry.id, codexEntry);
				}
				else
				{
					CodexCache.AddEntry(codexEntry.id, codexEntry, categories);
				}
			}
		}
		string[] directories = Directory.GetDirectories(CodexCache.baseEntryPath);
		for (int i = 0; i < directories.Length; i++)
		{
			foreach (CodexEntry codexEntry2 in CodexCache.CollectEntries(Path.GetFileNameWithoutExtension(directories[i])))
			{
				if (codexEntry2 != null && codexEntry2.id != null && codexEntry2.contentContainers != null)
				{
					if (CodexCache.entries.ContainsKey(CodexCache.FormatLinkID(codexEntry2.id)))
					{
						CodexCache.MergeEntry(codexEntry2.id, codexEntry2);
					}
					else
					{
						CodexCache.AddEntry(codexEntry2.id, codexEntry2, categories);
					}
				}
			}
		}
	}

	private static void CollectYAMLSubEntries(List<CategoryEntry> categories)
	{
		CodexCache.baseEntryPath = Application.streamingAssetsPath + "/codex";
		using (List<SubEntry>.Enumerator enumerator = CodexCache.CollectSubEntries("").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SubEntry v = enumerator.Current;
				if (v.parentEntryID != null && v.id != null)
				{
					if (CodexCache.entries.ContainsKey(v.parentEntryID.ToUpper()))
					{
						SubEntry subEntry = CodexCache.entries[v.parentEntryID.ToUpper()].subEntries.Find((SubEntry match) => match.id == v.id);
						if (!string.IsNullOrEmpty(v.lockID))
						{
							foreach (ContentContainer contentContainer in v.contentContainers)
							{
								contentContainer.lockID = v.lockID;
							}
						}
						if (subEntry != null)
						{
							if (!string.IsNullOrEmpty(v.lockID))
							{
								foreach (ContentContainer contentContainer2 in subEntry.contentContainers)
								{
									contentContainer2.lockID = v.lockID;
								}
								subEntry.lockID = v.lockID;
							}
							for (int i = 0; i < v.contentContainers.Count; i++)
							{
								if (!string.IsNullOrEmpty(v.contentContainers[i].lockID))
								{
									int num = subEntry.contentContainers.IndexOf(subEntry.lockedContentContainer);
									subEntry.contentContainers.Insert(num + 1, v.contentContainers[i]);
								}
								else if (v.contentContainers[i].showBeforeGeneratedContent)
								{
									subEntry.contentContainers.Insert(0, v.contentContainers[i]);
								}
								else
								{
									subEntry.contentContainers.Add(v.contentContainers[i]);
								}
							}
							subEntry.contentContainers.Add(new ContentContainer(new List<ICodexWidget>
							{
								new CodexLargeSpacer()
							}, ContentContainer.ContentLayout.Vertical));
							subEntry.layoutPriority = v.layoutPriority;
						}
						else
						{
							CodexCache.entries[v.parentEntryID.ToUpper()].subEntries.Add(v);
						}
					}
					else
					{
						global::Debug.LogWarningFormat("Codex SubEntry {0} cannot find parent codex entry with id {1}", new object[] { v.name, v.parentEntryID });
					}
				}
			}
		}
	}

	private static void AddLockLookup(string lockId, string articleId)
	{
		if (!CodexCache.unlockedEntryLookup.ContainsKey(lockId))
		{
			CodexCache.unlockedEntryLookup[lockId] = new List<string>();
		}
		CodexCache.unlockedEntryLookup[lockId].Add(articleId);
	}

	public static string GetEntryForLock(string lockId)
	{
		if (CodexCache.unlockedEntryLookup == null)
		{
			global::Debug.LogWarningFormat("Trying to get lock entry {0} before codex cache has been initialized.", new object[] { lockId });
			return null;
		}
		if (string.IsNullOrEmpty(lockId))
		{
			return null;
		}
		if (CodexCache.unlockedEntryLookup.ContainsKey(lockId) && CodexCache.unlockedEntryLookup[lockId] != null && CodexCache.unlockedEntryLookup[lockId].Count > 0)
		{
			return CodexCache.unlockedEntryLookup[lockId][0];
		}
		return null;
	}

	public static void AddEntry(string id, CodexEntry entry, List<CategoryEntry> categoryEntries = null)
	{
		id = CodexCache.FormatLinkID(id);
		if (CodexCache.entries.ContainsKey(id))
		{
			global::Debug.LogError("Tried to add " + id + " to the Codex screen multiple times");
		}
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
				entry.icon = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(entry.iconPrefabID).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false, "");
			}
			catch
			{
				global::Debug.LogWarningFormat("Unable to get icon for prefabID {0}", new object[] { entry.iconPrefabID });
			}
		}
		if (!entry.parentId.IsNullOrWhiteSpace() && CodexCache.entries.ContainsKey(entry.parentId))
		{
			(CodexCache.entries[entry.parentId] as CategoryEntry).entriesInCategory.Add(entry);
		}
		foreach (ContentContainer contentContainer in entry.contentContainers)
		{
			if (contentContainer.lockID != null)
			{
				CodexCache.AddLockLookup(contentContainer.lockID, entry.id);
			}
		}
	}

	public static void AddSubEntry(string id, SubEntry entry)
	{
	}

	public static void MergeSubEntry(string id, SubEntry entry)
	{
	}

	public static void MergeEntry(string id, CodexEntry entry)
	{
		id = CodexCache.FormatLinkID(entry.id);
		entry.id = id;
		CodexEntry codexEntry = CodexCache.entries[id];
		codexEntry.dlcIds = entry.dlcIds;
		for (int i = 0; i < entry.log.modificationRecords.Count; i++)
		{
		}
		codexEntry.customContentLength = entry.contentContainers.Count;
		for (int j = entry.contentContainers.Count - 1; j >= 0; j--)
		{
			codexEntry.InsertContentContainer(0, entry.contentContainers[j]);
		}
		if (entry.disabled)
		{
			codexEntry.disabled = entry.disabled;
		}
		codexEntry.showBeforeGeneratedCategoryLinks = entry.showBeforeGeneratedCategoryLinks;
		foreach (ContentContainer contentContainer in entry.contentContainers)
		{
			if (contentContainer.lockID != null)
			{
				CodexCache.AddLockLookup(contentContainer.lockID, entry.id);
			}
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
			CodexEntry codexEntry = YamlIO.LoadFile<CodexEntry>(text + ".yaml", null, CodexCache.widgetTagMappings);
			if (codexEntry == null)
			{
				global::Debug.LogWarning("Missing template [" + text + ".yaml]");
			}
			CodexCache.entries[templatePath] = codexEntry;
		}
		return CodexCache.entries[templatePath];
	}

	private static void YamlParseErrorCB(YamlIO.Error error, bool force_log_as_warning)
	{
		throw new Exception(string.Format("{0} parse error in {1}\n{2}", error.severity, error.file.full_path, error.message), error.inner_exception);
	}

	public static List<CodexEntry> CollectEntries(string folder)
	{
		List<CodexEntry> list = new List<CodexEntry>();
		string text = ((folder == "") ? CodexCache.baseEntryPath : Path.Combine(CodexCache.baseEntryPath, folder));
		string[] array = new string[0];
		try
		{
			array = Directory.GetFiles(text, "*.yaml");
		}
		catch (UnauthorizedAccessException ex)
		{
			global::Debug.LogWarning(ex);
		}
		string text2 = folder.ToUpper();
		foreach (string text3 in array)
		{
			try
			{
				CodexEntry codexEntry = YamlIO.LoadFile<CodexEntry>(text3, new YamlIO.ErrorHandler(CodexCache.YamlParseErrorCB), CodexCache.widgetTagMappings);
				if (codexEntry != null)
				{
					codexEntry.category = text2;
					list.Add(codexEntry);
				}
			}
			catch (Exception ex2)
			{
				DebugUtil.DevLogErrorFormat("CodexCache.CollectEntries failed to load [{0}]: {1}", new object[]
				{
					text3,
					ex2.ToString()
				});
			}
		}
		foreach (CodexEntry codexEntry2 in list)
		{
			if (string.IsNullOrEmpty(codexEntry2.sortString))
			{
				codexEntry2.sortString = Strings.Get(codexEntry2.title);
			}
		}
		list.Sort((CodexEntry x, CodexEntry y) => x.sortString.CompareTo(y.sortString));
		return list;
	}

	public static List<SubEntry> CollectSubEntries(string folder)
	{
		List<SubEntry> list = new List<SubEntry>();
		string text = ((folder == "") ? CodexCache.baseEntryPath : Path.Combine(CodexCache.baseEntryPath, folder));
		string[] array = new string[0];
		try
		{
			array = Directory.GetFiles(text, "*.yaml", SearchOption.AllDirectories);
		}
		catch (UnauthorizedAccessException ex)
		{
			global::Debug.LogWarning(ex);
		}
		foreach (string text2 in array)
		{
			try
			{
				SubEntry subEntry = YamlIO.LoadFile<SubEntry>(text2, new YamlIO.ErrorHandler(CodexCache.YamlParseErrorCB), CodexCache.widgetTagMappings);
				if (subEntry != null)
				{
					list.Add(subEntry);
				}
			}
			catch (Exception ex2)
			{
				DebugUtil.DevLogErrorFormat("CodexCache.CollectSubEntries failed to load [{0}]: {1}", new object[]
				{
					text2,
					ex2.ToString()
				});
			}
		}
		list.Sort((SubEntry x, SubEntry y) => x.title.CompareTo(y.title));
		return list;
	}

	private static string baseEntryPath;

	public static Dictionary<string, CodexEntry> entries;

	public static Dictionary<string, SubEntry> subEntries;

	private static Dictionary<string, List<string>> unlockedEntryLookup;

	private static List<global::Tuple<string, Type>> widgetTagMappings;
}
