using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexScreen : KScreen
{
	protected override void OnActivate()
	{
		this.ConsumeMouseScroll = true;
		base.OnActivate();
		this.closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		this.clearSearchButton.onClick += delegate
		{
			this.searchInputField.text = string.Empty;
		};
		if (string.IsNullOrEmpty(this.activeEntryID))
		{
			this.ChangeArticle("HOME", false);
		}
		this.searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			this.FilterSearch(value);
		});
		TMP_InputField tmp_InputField = this.searchInputField;
		tmp_InputField.onFocus = (global::System.Action)Delegate.Combine(tmp_InputField.onFocus, new global::System.Action(delegate
		{
			this.editingSearch = true;
		}));
		this.searchInputField.onEndEdit.AddListener(delegate(string value)
		{
			this.editingSearch = false;
		});
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.editingSearch)
		{
			e.Consumed = true;
		}
		base.OnKeyDown(e);
	}

	public override float GetSortKey()
	{
		return 10000f;
	}

	private void Init()
	{
		this.SetupPrefabs();
		this.PopulatePools();
		this.CategorizeEntries();
		this.FilterSearch(string.Empty);
		Game.Instance.Subscribe(1594320620, delegate(object val)
		{
			this.FilterSearch(this.searchInputField.text);
			if (!string.IsNullOrEmpty(this.activeEntryID))
			{
				this.ChangeArticle(this.activeEntryID, false);
			}
		});
	}

	private void SetupPrefabs()
	{
		this.contentContainerPool = new UIGameObjectPool(this.prefabContentContainer);
		this.contentContainerPool.disabledElementParent = this.widgetPool;
		for (int i = 0; i < 7; i++)
		{
			switch (i)
			{
			case 0:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabTextWidget;
				break;
			case 1:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabImageWidget;
				break;
			case 2:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabDividerLineWidget;
				break;
			case 3:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabSpacer;
				break;
			case 4:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabLabelWithIcon;
				break;
			case 5:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabContentLocked;
				break;
			case 6:
				this.ContentPrefabs[(CodexWidget.ContentType)i] = this.prefabLargeSpacer;
				break;
			}
		}
	}

	private List<CodexEntry> FilterSearch(string input)
	{
		this.searchResults.Clear();
		input = input.ToLower();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			if (input == string.Empty)
			{
				if (!keyValuePair.Value.searchOnly)
				{
					this.searchResults.Add(keyValuePair.Value);
				}
			}
			else if (input == keyValuePair.Value.name.ToLower() || input.Contains(keyValuePair.Value.name.ToLower()) || keyValuePair.Value.name.ToLower().Contains(input))
			{
				this.searchResults.Add(keyValuePair.Value);
			}
			else
			{
				foreach (SubEntry subEntry in keyValuePair.Value.subEntries)
				{
					if (input == subEntry.name.ToLower() || input.Contains(subEntry.name.ToLower()) || subEntry.name.ToLower().Contains(input))
					{
						this.searchResults.Add(keyValuePair.Value);
					}
				}
			}
		}
		this.FilterEntries(input != string.Empty);
		return this.searchResults;
	}

	private bool HasUnlockedCategoryEntries(string entryID)
	{
		foreach (ContentContainer contentContainer in CodexCache.entries[entryID].contentContainers)
		{
			if (string.IsNullOrEmpty(contentContainer.lockID) || !Game.Instance.unlocks.IsLocked(contentContainer.lockID))
			{
				return true;
			}
		}
		return false;
	}

	private void FilterEntries(bool allowOpenCategories = true)
	{
		foreach (KeyValuePair<CodexEntry, GameObject> keyValuePair in this.entryButtons)
		{
			keyValuePair.Value.SetActive(this.searchResults.Contains(keyValuePair.Key) && this.HasUnlockedCategoryEntries(keyValuePair.Key.id));
		}
		foreach (GameObject gameObject in this.categoryHeaders)
		{
			bool flag = false;
			Transform transform = gameObject.transform.Find("Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).gameObject.activeSelf)
				{
					flag = true;
				}
			}
			gameObject.SetActive(flag);
			if (allowOpenCategories)
			{
				if (flag)
				{
					this.ToggleCategoryOpen(gameObject, true);
				}
			}
			else
			{
				this.ToggleCategoryOpen(gameObject, false);
			}
		}
	}

	private void ToggleCategoryOpen(GameObject header, bool open)
	{
		MultiToggle reference = header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle");
		reference.ChangeState((!open) ? 0 : 1);
		header.GetComponent<HierarchyReferences>().GetReference("Content").gameObject.SetActive(open);
	}

	private void PopulatePools()
	{
		for (int i = 0; i < 7; i++)
		{
			this.ContentUIPools[(CodexWidget.ContentType)i] = new UIGameObjectPool(this.ContentPrefabs[(CodexWidget.ContentType)i]);
			this.ContentUIPools[(CodexWidget.ContentType)i].disabledElementParent = this.widgetPool;
		}
	}

	private GameObject NewCategoryHeader(KeyValuePair<string, CodexEntry> entryKVP, Dictionary<string, GameObject> categories)
	{
		if (entryKVP.Value.category == string.Empty)
		{
			entryKVP.Value.category = "Root";
		}
		GameObject categoryHeader = Util.KInstantiateUI(this.prefabCategoryHeader, this.navigatorContent.gameObject, true);
		GameObject categoryContent = categoryHeader.GetComponent<HierarchyReferences>().GetReference("Content").gameObject;
		categories.Add(entryKVP.Value.category, categoryContent);
		LocText reference = categoryHeader.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
		if (CodexCache.entries.ContainsKey(entryKVP.Value.category))
		{
			reference.text = CodexCache.entries[entryKVP.Value.category].name;
		}
		else
		{
			reference.text = Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES." + entryKVP.Value.category.ToUpper());
		}
		this.categoryHeaders.Add(categoryHeader);
		categoryContent.SetActive(false);
		MultiToggle reference2 = categoryHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle");
		reference2.onClick = delegate
		{
			this.ToggleCategoryOpen(categoryHeader, !categoryContent.activeSelf);
		};
		return categoryHeader;
	}

	private void CategorizeEntries()
	{
		string text = string.Empty;
		GameObject gameObject = this.navigatorContent.gameObject;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			text = keyValuePair.Value.category;
			if (text == string.Empty || text == "Root")
			{
				text = "Root";
			}
			if (!dictionary.ContainsKey(text))
			{
				this.NewCategoryHeader(keyValuePair, dictionary);
			}
			GameObject gameObject2 = Util.KInstantiateUI(this.prefabNavigatorEntry, dictionary[text], true);
			string id = keyValuePair.Key;
			gameObject2.GetComponent<KButton>().onClick += delegate
			{
				this.ChangeArticle(id, false);
			};
			if (string.IsNullOrEmpty(keyValuePair.Value.name))
			{
				keyValuePair.Value.name = Strings.Get(keyValuePair.Value.title);
			}
			gameObject2.GetComponentInChildren<LocText>().text = keyValuePair.Value.name;
			this.entryButtons.Add(keyValuePair.Value, gameObject2);
		}
		foreach (KeyValuePair<string, CodexEntry> keyValuePair2 in CodexCache.entries)
		{
			if (CodexCache.entries.ContainsKey(keyValuePair2.Value.category) && CodexCache.entries.ContainsKey(CodexCache.entries[keyValuePair2.Value.category].category))
			{
				keyValuePair2.Value.searchOnly = true;
			}
		}
		List<KeyValuePair<string, GameObject>> list = new List<KeyValuePair<string, GameObject>>();
		foreach (KeyValuePair<string, GameObject> keyValuePair3 in dictionary)
		{
			list.Add(keyValuePair3);
		}
		list.Sort((KeyValuePair<string, GameObject> a, KeyValuePair<string, GameObject> b) => string.Compare(a.Value.name, b.Value.name));
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.transform.parent.SetSiblingIndex(i);
		}
		CodexScreen.SetupCategory(dictionary, "PLANTS");
		CodexScreen.SetupCategory(dictionary, "CREATURES");
		CodexScreen.SetupCategory(dictionary, "NOTICES");
		CodexScreen.SetupCategory(dictionary, "RESEARCHNOTES");
		CodexScreen.SetupCategory(dictionary, "JOURNALS");
		CodexScreen.SetupCategory(dictionary, "EMAILS");
		CodexScreen.SetupCategory(dictionary, "MYLOG");
		CodexScreen.SetupCategory(dictionary, "TIPS");
		CodexScreen.SetupCategory(dictionary, "Root");
	}

	private static void SetupCategory(Dictionary<string, GameObject> categories, string category_name)
	{
		if (!categories.ContainsKey(category_name))
		{
			return;
		}
		categories[category_name].transform.parent.SetAsFirstSibling();
	}

	public void ChangeArticle(string id, bool playClickSound = false)
	{
		if (playClickSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		if (this.contentContainerPool == null)
		{
			this.Init();
		}
		SubEntry subEntry = null;
		if (!CodexCache.entries.ContainsKey(id))
		{
			subEntry = CodexCache.FindSubEntry(id);
			if (subEntry != null && !subEntry.disabled)
			{
				id = subEntry.parentEntryID.ToUpper();
			}
		}
		CodexWidget codexWidget = null;
		CodexCache.entries[id].GetFirstWidget();
		RectTransform rectTransform = null;
		if (subEntry != null)
		{
			foreach (ContentContainer contentContainer in CodexCache.entries[id].contentContainers)
			{
				if (contentContainer == subEntry.contentContainers[0])
				{
					codexWidget = contentContainer.content[0];
					break;
				}
			}
		}
		if (!CodexCache.entries.ContainsKey(id) || CodexCache.entries[id].disabled)
		{
			id = "PAGENOTFOUND";
		}
		int num = 0;
		string text = string.Empty;
		while (this.contentContainers.transform.childCount > 0)
		{
			while (!string.IsNullOrEmpty(text) && CodexCache.entries[this.activeEntryID].contentContainers[num].lockID == text)
			{
				num++;
			}
			GameObject gameObject = this.contentContainers.transform.GetChild(0).gameObject;
			int num2 = 0;
			while (gameObject.transform.childCount > 0)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
				CodexWidget.ContentType contentType;
				if (gameObject2.name == "PrefabContentLocked")
				{
					text = CodexCache.entries[this.activeEntryID].contentContainers[num].lockID;
					contentType = CodexWidget.ContentType.ContentLockedIndicator;
				}
				else
				{
					contentType = CodexCache.entries[this.activeEntryID].contentContainers[num].content[num2].type;
				}
				this.ContentUIPools[contentType].ClearElement(gameObject2);
				num2++;
			}
			this.contentContainerPool.ClearElement(this.contentContainers.transform.GetChild(0).gameObject);
			num++;
		}
		bool flag = CodexCache.entries[id] is CategoryEntry;
		this.activeEntryID = id;
		if (CodexCache.entries[id].contentContainers == null)
		{
			CodexCache.entries[id].contentContainers = new List<ContentContainer>();
		}
		bool flag2 = false;
		string text2 = string.Empty;
		for (int i = 0; i < CodexCache.entries[id].contentContainers.Count; i++)
		{
			ContentContainer contentContainer2 = CodexCache.entries[id].contentContainers[i];
			if (!string.IsNullOrEmpty(contentContainer2.lockID) && Game.Instance.unlocks.IsLocked(contentContainer2.lockID))
			{
				if (text2 != contentContainer2.lockID)
				{
					GameObject gameObject3 = this.contentContainerPool.GetFreeElement(this.contentContainers.gameObject, true).gameObject;
					this.ConfigureContentContainer(contentContainer2, gameObject3, flag && flag2);
					text2 = contentContainer2.lockID;
					GameObject gameObject4 = this.ContentUIPools[CodexWidget.ContentType.ContentLockedIndicator].GetFreeElement(gameObject3, true).gameObject;
				}
			}
			else
			{
				GameObject gameObject3 = this.contentContainerPool.GetFreeElement(this.contentContainers.gameObject, true).gameObject;
				this.ConfigureContentContainer(contentContainer2, gameObject3, flag && flag2);
				flag2 = !flag2;
				if (contentContainer2.content != null)
				{
					foreach (CodexWidget codexWidget2 in contentContainer2.content)
					{
						GameObject gameObject5 = this.ContentUIPools[codexWidget2.type].GetFreeElement(gameObject3, true).gameObject;
						this.ConfigureContentWidget(codexWidget2, gameObject5);
						if (codexWidget2 == codexWidget)
						{
							rectTransform = gameObject5.rectTransform();
						}
					}
				}
			}
		}
		string text3 = string.Empty;
		string text4 = id;
		int num3 = 0;
		while (text4 != CodexCache.FormatLinkID("HOME") && num3 < 10)
		{
			num3++;
			if (text4 != null)
			{
				if (text4 != id)
				{
					text3 = text3.Insert(0, CodexCache.entries[text4].name + " > ");
				}
				else
				{
					text3 = text3.Insert(0, CodexCache.entries[text4].name);
				}
				text4 = CodexCache.entries[text4].parentId;
			}
			else
			{
				text4 = CodexCache.entries[CodexCache.FormatLinkID("HOME")].id;
				text3 = text3.Insert(0, CodexCache.entries[text4].name + " > ");
			}
		}
		this.currentLocationText.text = ((!(text3 == string.Empty)) ? text3 : CodexCache.entries["HOME"].name);
		if (this.history.Count == 0)
		{
			this.history.Add(this.activeEntryID);
		}
		else if (this.history[this.history.Count - 1] != this.activeEntryID)
		{
			if (this.history.Count > 1 && this.history[this.history.Count - 2] == this.activeEntryID)
			{
				this.history.RemoveAt(this.history.Count - 1);
			}
			else
			{
				this.history.Add(this.activeEntryID);
			}
		}
		if (this.history.Count > 1)
		{
			this.backButton.text = UI.FormatAsLink(string.Format(UI.CODEX.BACK_BUTTON, UI.StripLinkFormatting(CodexCache.entries[this.history[this.history.Count - 2]].name)), CodexCache.entries[this.history[this.history.Count - 2]].id);
		}
		else
		{
			this.backButton.text = UI.StripLinkFormatting(GameUtil.ColourizeString(Color.grey, string.Format(UI.CODEX.BACK_BUTTON, CodexCache.entries["HOME"].name)));
		}
		if (rectTransform != null)
		{
			if (this.scrollToTargetRoutine != null)
			{
				base.StopCoroutine(this.scrollToTargetRoutine);
			}
			this.scrollToTargetRoutine = base.StartCoroutine(this.ScrollToTarget(rectTransform));
		}
		else
		{
			this.displayScrollRect.content.SetLocalPosition(Vector3.zero);
		}
	}

	private IEnumerator ScrollToTarget(RectTransform targetWidgetTransform)
	{
		yield return 0;
		this.displayScrollRect.content.SetLocalPosition(Vector3.down * (this.displayScrollRect.content.InverseTransformPoint(targetWidgetTransform.GetPosition()).y + 12f));
		yield break;
	}

	private void ConfigureContentContainer(ContentContainer container, GameObject containerGameObject, bool bgColor = false)
	{
		LayoutGroup layoutGroup = containerGameObject.GetComponent<LayoutGroup>();
		if (layoutGroup != null)
		{
			global::UnityEngine.Object.DestroyImmediate(layoutGroup);
		}
		if (!Game.Instance.unlocks.IsLocked(container.lockID))
		{
			ContentContainer.ContentLayout contentLayout = container.contentLayout;
			if (contentLayout != ContentContainer.ContentLayout.Horizontal)
			{
				if (contentLayout != ContentContainer.ContentLayout.Vertical)
				{
					if (contentLayout == ContentContainer.ContentLayout.Grid)
					{
						layoutGroup = containerGameObject.AddComponent<GridLayoutGroup>();
						(layoutGroup as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
						(layoutGroup as GridLayoutGroup).constraintCount = 3;
						(layoutGroup as GridLayoutGroup).cellSize = new Vector2(170f, 32f);
					}
				}
				else
				{
					layoutGroup = containerGameObject.AddComponent<VerticalLayoutGroup>();
					HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup = layoutGroup as HorizontalOrVerticalLayoutGroup;
					bool flag = false;
					(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = flag;
					horizontalOrVerticalLayoutGroup.childForceExpandHeight = flag;
					(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
				}
			}
			else
			{
				layoutGroup = containerGameObject.AddComponent<HorizontalLayoutGroup>();
				layoutGroup.childAlignment = TextAnchor.MiddleLeft;
				HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup2 = layoutGroup as HorizontalOrVerticalLayoutGroup;
				bool flag = false;
				(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = flag;
				horizontalOrVerticalLayoutGroup2.childForceExpandHeight = flag;
				(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
			}
		}
		else
		{
			layoutGroup = containerGameObject.AddComponent<VerticalLayoutGroup>();
			HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup3 = layoutGroup as HorizontalOrVerticalLayoutGroup;
			bool flag = false;
			(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = flag;
			horizontalOrVerticalLayoutGroup3.childForceExpandHeight = flag;
			(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
		}
	}

	private void ConfigureContentWidget(CodexWidget content, GameObject contentGameObject)
	{
		switch (content.type)
		{
		case CodexWidget.ContentType.Text:
		{
			LocText component = contentGameObject.GetComponent<LocText>();
			component.gameObject.SetActive(true);
			string text;
			content.properties.TryGetValue("style", out text);
			if (text == "title")
			{
				component.textStyleSetting = this.textStyleTitle;
				component.AllowLinks = false;
			}
			else if (text == "subtitle")
			{
				component.textStyleSetting = this.textStyleSubtitle;
				component.AllowLinks = false;
			}
			else
			{
				component.textStyleSetting = this.textStyleBody;
				component.AllowLinks = true;
			}
			if (content.properties.ContainsKey("stringKey"))
			{
				content.properties.TryGetValue("stringKey", out text);
				component.text = Strings.Get(text);
			}
			else if (content.properties.ContainsKey("string"))
			{
				content.properties.TryGetValue("string", out text);
				component.text = text;
			}
			component.ApplySettings();
			this.ConfigurePreferredLayout(content, contentGameObject);
			break;
		}
		case CodexWidget.ContentType.Image:
			if (content.properties.ContainsKey("spriteName"))
			{
				string text;
				content.properties.TryGetValue("spriteName", out text);
				Image component2 = contentGameObject.GetComponent<Image>();
				component2.sprite = Assets.GetSprite(text);
				component2.color = Color.white;
			}
			else if (content.properties.ContainsKey("batchedAnimPrefabSourceID"))
			{
				Image component3 = contentGameObject.GetComponent<Image>();
				component3.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(content.properties["batchedAnimPrefabSourceID"]).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
				component3.color = Color.white;
			}
			else if (content.objectProperties.ContainsKey("coloredSprite"))
			{
				Tuple<Sprite, Color> tuple = (Tuple<Sprite, Color>)content.objectProperties["coloredSprite"];
				Image component4 = contentGameObject.GetComponent<Image>();
				if (tuple != null)
				{
					component4.sprite = tuple.first;
					component4.color = tuple.second;
				}
				else
				{
					component4.sprite = null;
					component4.color = Color.clear;
				}
			}
			else
			{
				Image component5 = contentGameObject.GetComponent<Image>();
				component5.sprite = (Sprite)content.objectProperties["sprite"];
				component5.color = Color.white;
			}
			this.ConfigurePreferredLayout(content, contentGameObject);
			break;
		case CodexWidget.ContentType.DividerLine:
			contentGameObject.GetComponent<LayoutElement>().minWidth = this.displayPane.rectTransform().sizeDelta.x - 64f;
			break;
		case CodexWidget.ContentType.Spacer:
			this.ConfigurePreferredLayout(content, contentGameObject);
			break;
		case CodexWidget.ContentType.LabelWithIcon:
			this.ConfigureLabelWithIcon(content, contentGameObject);
			break;
		}
	}

	private void ConfigureLabelWithIcon(CodexWidget content, GameObject contentGameObject)
	{
		LocText componentInChildren = contentGameObject.GetComponentInChildren<LocText>();
		componentInChildren.textStyleSetting = this.textStyleBody;
		componentInChildren.AllowLinks = true;
		string empty = string.Empty;
		if (content.properties.ContainsKey("stringKey"))
		{
			content.properties.TryGetValue("stringKey", out empty);
			componentInChildren.text = Strings.Get(empty);
		}
		else if (content.properties.ContainsKey("string"))
		{
			content.properties.TryGetValue("string", out empty);
			componentInChildren.text = empty;
		}
		componentInChildren.ApplySettings();
		string empty2 = string.Empty;
		if (content.properties.ContainsKey("spriteName"))
		{
			content.properties.TryGetValue("spriteName", out empty2);
			Image component = contentGameObject.GetComponent<Image>();
			component.sprite = Assets.GetSprite(empty2);
			component.color = Color.white;
		}
		else if (content.properties.ContainsKey("batchedAnimPrefabSourceID"))
		{
			Image componentInChildren2 = contentGameObject.GetComponentInChildren<Image>();
			componentInChildren2.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(content.properties["batchedAnimPrefabSourceID"]).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
			componentInChildren2.color = Color.white;
		}
		else if (content.objectProperties.ContainsKey("coloredSprite"))
		{
			Tuple<Sprite, Color> tuple = (Tuple<Sprite, Color>)content.objectProperties["coloredSprite"];
			Image componentInChildren3 = contentGameObject.GetComponentInChildren<Image>();
			if (tuple != null)
			{
				componentInChildren3.sprite = tuple.first;
				componentInChildren3.color = tuple.second;
			}
			else
			{
				componentInChildren3.sprite = null;
				componentInChildren3.color = Color.clear;
			}
		}
		else
		{
			Image componentInChildren4 = contentGameObject.GetComponentInChildren<Image>();
			componentInChildren4.sprite = (Sprite)content.objectProperties["sprite"];
			componentInChildren4.color = Color.white;
		}
	}

	private void ConfigurePreferredLayout(CodexWidget content, GameObject contentGameObject)
	{
		LayoutElement component = contentGameObject.GetComponent<LayoutElement>();
		if (content.properties.ContainsKey("preferredHeight"))
		{
			component.preferredHeight = (float)Convert.ToInt32(content.properties["preferredHeight"]);
		}
		else
		{
			component.preferredHeight = -1f;
		}
		if (content.properties.ContainsKey("preferredWidth"))
		{
			component.preferredWidth = (float)Convert.ToInt32(content.properties["preferredWidth"]);
		}
		else
		{
			component.preferredWidth = -1f;
		}
	}

	private string activeEntryID;

	private Dictionary<CodexWidget.ContentType, UIGameObjectPool> ContentUIPools = new Dictionary<CodexWidget.ContentType, UIGameObjectPool>();

	private Dictionary<CodexWidget.ContentType, GameObject> ContentPrefabs = new Dictionary<CodexWidget.ContentType, GameObject>();

	private List<GameObject> categoryHeaders = new List<GameObject>();

	private Dictionary<CodexEntry, GameObject> entryButtons = new Dictionary<CodexEntry, GameObject>();

	private UIGameObjectPool contentContainerPool;

	[SerializeField]
	private KScrollRect displayScrollRect;

	[SerializeField]
	private RectTransform scrollContentPane;

	private bool editingSearch;

	private List<string> history = new List<string>();

	[Header("Hierarchy")]
	[SerializeField]
	private Transform navigatorContent;

	[SerializeField]
	private Transform displayPane;

	[SerializeField]
	private Transform contentContainers;

	[SerializeField]
	private Transform widgetPool;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private TMP_InputField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private LocText backButton;

	[SerializeField]
	private LocText currentLocationText;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject prefabNavigatorEntry;

	[SerializeField]
	private GameObject prefabCategoryHeader;

	[SerializeField]
	private GameObject prefabContentContainer;

	[SerializeField]
	private GameObject prefabTextWidget;

	[SerializeField]
	private GameObject prefabImageWidget;

	[SerializeField]
	private GameObject prefabDividerLineWidget;

	[SerializeField]
	private GameObject prefabSpacer;

	[SerializeField]
	private GameObject prefabLargeSpacer;

	[SerializeField]
	private GameObject prefabLabelWithIcon;

	[SerializeField]
	private GameObject prefabContentLocked;

	[Header("Text Styles")]
	[SerializeField]
	private TextStyleSetting textStyleTitle;

	[SerializeField]
	private TextStyleSetting textStyleSubtitle;

	[SerializeField]
	private TextStyleSetting textStyleBody;

	private List<CodexEntry> searchResults = new List<CodexEntry>();

	private Coroutine scrollToTargetRoutine;

	public enum PlanCategory
	{
		Home,
		Tips,
		MyLog,
		Emails,
		Journals,
		ResearchNotes,
		Creatures,
		Plants,
		Food,
		Tech,
		Diseases,
		Roles,
		Buildings,
		Elements
	}
}
