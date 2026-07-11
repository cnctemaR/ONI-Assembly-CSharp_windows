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
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.CREATURES, CodexEntryGenerator.GenerateCreatureEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("Hatch").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = CodexCache.FormatLinkID("plants");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.PLANTS, CodexEntryGenerator.GeneratePlantEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("PrickleFlower").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = CodexCache.FormatLinkID("food");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.FOOD, CodexEntryGenerator.GenerateFoodEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("CookedMeat").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = CodexCache.FormatLinkID("buildings");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.BUILDINGS, CodexEntryGenerator.GenerateBuildingEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("Generator").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = CodexCache.FormatLinkID("tech");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.TECH, CodexEntryGenerator.GenerateTechEntries(), Assets.GetSprite("hud_research")));
		text = CodexCache.FormatLinkID("roles");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ROLES, CodexEntryGenerator.GenerateRoleEntries(), Assets.GetSprite("hat_role_mining2")));
		text = CodexCache.FormatLinkID("disease");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.DISEASE, CodexEntryGenerator.GenerateDiseaseEntries(), Assets.GetSprite("overlay_disease")));
		text = CodexCache.FormatLinkID("elements");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ELEMENTS, CodexEntryGenerator.GenerateElementEntries(), null));
		text = CodexCache.FormatLinkID("geysers");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.GEYSERS, CodexEntryGenerator.GenerateGeyserEntries(), null));
		CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID("HOME"), UI.CODEX.CATEGORYNAMES.ROOT, dictionary, null);
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
					if (subEntry.lockID != null && Game.Instance.unlocks.IsLocked(subEntry.lockID))
					{
						num++;
					}
				}
				List<CodexWidget> list2 = new List<CodexWidget>();
				list2.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
				list2.Add(new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						string.Concat(new object[]
						{
							CODEX.HEADERS.SUBENTRIES,
							" (",
							keyValuePair2.Value.subEntries.Count - num,
							"/",
							keyValuePair2.Value.subEntries.Count,
							")"
						})
					},
					{ "style", "subtitle" }
				}));
				foreach (SubEntry subEntry2 in keyValuePair2.Value.subEntries)
				{
					if (subEntry2.lockID != null && Game.Instance.unlocks.IsLocked(subEntry2.lockID))
					{
						list2.Add(new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string> { 
						{
							"string",
							UI.FormatAsLink(CODEX.HEADERS.CONTENTLOCKED, UI.ExtractLinkID(subEntry2.name))
						} }));
					}
					else
					{
						list2.Add(new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string> { { "string", subEntry2.name } }));
					}
				}
				list2.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
				keyValuePair2.Value.contentContainers.Insert(keyValuePair2.Value.customContentLength, new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
			}
			for (int i = 0; i < keyValuePair2.Value.subEntries.Count; i++)
			{
				keyValuePair2.Value.contentContainers.AddRange(keyValuePair2.Value.subEntries[i].contentContainers);
			}
		}
		CodexEntryGenerator.PopulateCategoryEntries(list);
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
		List<CodexEntry> list = CodexCache.CollectEntries(string.Empty);
		foreach (CodexEntry codexEntry in list)
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
		foreach (string text in Directory.GetDirectories(CodexCache.baseEntryPath))
		{
			List<CodexEntry> list2 = CodexCache.CollectEntries(Path.GetFileNameWithoutExtension(text));
			foreach (CodexEntry codexEntry2 in list2)
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
		List<SubEntry> list = CodexCache.CollectSubEntries(string.Empty);
		using (List<SubEntry>.Enumerator enumerator = list.GetEnumerator())
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
							subEntry.contentContainers.Add(new ContentContainer(new List<CodexWidget>
							{
								new CodexWidget(CodexWidget.ContentType.LargeSpacer)
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
				entry.icon = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(entry.iconPrefabID).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
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
		codexEntry.customContentLength = entry.contentContainers.Count;
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
		WorkItemCollection<CodexCache.CollectEntryWorkItem, object> workItemCollection = new WorkItemCollection<CodexCache.CollectEntryWorkItem, object>();
		foreach (string text2 in files)
		{
			workItemCollection.Add(new CodexCache.CollectEntryWorkItem
			{
				path = text2
			});
		}
		GlobalJobManager.Run(workItemCollection);
		string text3 = folder.ToUpper();
		for (int j = 0; j < workItemCollection.Count; j++)
		{
			CodexEntry asset = workItemCollection.GetWorkItem(j).asset;
			if (asset != null)
			{
				asset.category = text3;
				list.Add(asset);
			}
		}
		list.Sort((CodexEntry x, CodexEntry y) => x.title.CompareTo(y.title));
		return list;
	}

	public static List<SubEntry> CollectSubEntries(string folder)
	{
		List<SubEntry> list = new List<SubEntry>();
		string text = ((!(folder == string.Empty)) ? Path.Combine(CodexCache.baseEntryPath, folder) : CodexCache.baseEntryPath);
		string[] files = Directory.GetFiles(text, "*.yaml", SearchOption.AllDirectories);
		WorkItemCollection<CodexCache.CollectSubEntryWorkItem, object> workItemCollection = new WorkItemCollection<CodexCache.CollectSubEntryWorkItem, object>();
		foreach (string text2 in files)
		{
			workItemCollection.Add(new CodexCache.CollectSubEntryWorkItem
			{
				path = text2
			});
		}
		GlobalJobManager.Run(workItemCollection);
		for (int j = 0; j < workItemCollection.Count; j++)
		{
			SubEntry asset = workItemCollection.GetWorkItem(j).asset;
			if (asset != null)
			{
				list.Add(asset);
			}
		}
		list.Sort((SubEntry x, SubEntry y) => x.title.CompareTo(y.title));
		return list;
	}

	private static string baseEntryPath;

	public static Dictionary<string, CodexEntry> entries;

	private struct CollectEntryWorkItem : IWorkItem<object>
	{
		public void Run(object shared_data)
		{
			this.asset = YamlIO<CodexEntry>.LoadFile(this.path);
		}

		public string path;

		public CodexEntry asset;
	}

	private struct CollectSubEntryWorkItem : IWorkItem<object>
	{
		public void Run(object shared_data)
		{
			this.asset = YamlIO<SubEntry>.LoadFile(this.path);
		}

		public string path;

		public SubEntry asset;
	}
}
