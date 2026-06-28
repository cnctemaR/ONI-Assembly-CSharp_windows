using System;
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
	}

	private void SetupPrefabs()
	{
		this.contentContainerPool = new UIGameObjectPool(this.prefabContentContainer);
		this.contentContainerPool.disabledElementParent = this.widgetPool;
		for (int i = 0; i < 4; i++)
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
			else if (input == keyValuePair.Value.name.ToLower())
			{
				this.searchResults.Add(keyValuePair.Value);
			}
			else if (input.Contains(keyValuePair.Value.name.ToLower()))
			{
				this.searchResults.Add(keyValuePair.Value);
			}
			else if (keyValuePair.Value.name.ToLower().Contains(input))
			{
				this.searchResults.Add(keyValuePair.Value);
			}
		}
		this.FilterEntries(input != string.Empty);
		return this.searchResults;
	}

	private void FilterEntries(bool allowOpenCategories = true)
	{
		foreach (KeyValuePair<CodexEntry, GameObject> keyValuePair in this.entryButtons)
		{
			keyValuePair.Value.SetActive(this.searchResults.Contains(keyValuePair.Key));
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
		for (int i = 0; i < 4; i++)
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
		if (CodexCache.entries.ContainsKey(entryKVP.Value.category))
		{
			categoryHeader.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").text = CodexCache.entries[entryKVP.Value.category].name;
		}
		else
		{
			categoryHeader.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").text = Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES." + entryKVP.Value.category.ToUpper());
		}
		this.categoryHeaders.Add(categoryHeader);
		categoryContent.SetActive(false);
		MultiToggle reference = categoryHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle");
		reference.onClick = delegate
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
		dictionary["Root"].transform.parent.SetAsFirstSibling();
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
		if (!CodexCache.entries.ContainsKey(id) || CodexCache.entries[id].disabled)
		{
			id = "PAGENOTFOUND";
		}
		this.activeText.Clear();
		int num = 0;
		while (this.contentContainers.transform.childCount > 0)
		{
			GameObject gameObject = this.contentContainers.transform.GetChild(0).gameObject;
			int num2 = 0;
			while (gameObject.transform.childCount > 0)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
				CodexWidget.ContentType type = CodexCache.entries[this.activeEntryID].contentContainers[num].content[num2].type;
				this.ContentUIPools[type].ClearElement(gameObject2);
				num2++;
			}
			this.contentContainerPool.ClearElement(this.contentContainers.transform.GetChild(0).gameObject);
			num++;
		}
		this.activeEntryID = id;
		if (CodexCache.entries[id].contentContainers == null)
		{
			CodexCache.entries[id].contentContainers = new List<ContentContainer>();
		}
		foreach (ContentContainer contentContainer in CodexCache.entries[id].contentContainers)
		{
			GameObject gameObject3 = this.contentContainerPool.GetFreeElement(this.contentContainers.gameObject, true).gameObject;
			this.ConfigureContentContainer(contentContainer, gameObject3);
			if (contentContainer.content != null)
			{
				foreach (CodexWidget codexWidget in contentContainer.content)
				{
					GameObject gameObject4 = this.ContentUIPools[codexWidget.type].GetFreeElement(gameObject3, true).gameObject;
					this.ConfigureContentWidget(codexWidget, gameObject4);
				}
			}
		}
		string text = string.Empty;
		string text2 = id;
		int num3 = 0;
		while (text2 != CodexCache.FormatLinkID("HOME") && num3 < 6)
		{
			num3++;
			if (text2 != null)
			{
				if (text2 != id)
				{
					text = text.Insert(0, CodexCache.entries[text2].name + " > ");
				}
				else
				{
					text = text.Insert(0, CodexCache.entries[text2].name);
				}
				text2 = CodexCache.entries[text2].parentId;
			}
			else
			{
				text2 = CodexCache.entries[CodexCache.FormatLinkID("HOME")].id;
				text = text.Insert(0, CodexCache.entries[text2].name + " > ");
			}
		}
		this.currentLocationText.text = text;
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
			this.backButton.text = string.Empty;
		}
	}

	private void ConfigureContentContainer(ContentContainer container, GameObject containerGameObject)
	{
		HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup = containerGameObject.GetComponent<HorizontalOrVerticalLayoutGroup>();
		if (horizontalOrVerticalLayoutGroup != null)
		{
			global::UnityEngine.Object.DestroyImmediate(horizontalOrVerticalLayoutGroup);
		}
		if (container.contentLayout == ContentContainer.ContentLayout.Horizontal)
		{
			horizontalOrVerticalLayoutGroup = containerGameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalOrVerticalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
		}
		else if (container.contentLayout == ContentContainer.ContentLayout.Vertical)
		{
			horizontalOrVerticalLayoutGroup = containerGameObject.AddComponent<VerticalLayoutGroup>();
		}
		HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup2 = horizontalOrVerticalLayoutGroup;
		bool flag = false;
		horizontalOrVerticalLayoutGroup.childForceExpandWidth = flag;
		horizontalOrVerticalLayoutGroup2.childForceExpandHeight = flag;
		horizontalOrVerticalLayoutGroup.spacing = 8f;
	}

	private void ConfigureContentWidget(CodexWidget content, GameObject contentGameObject)
	{
		switch (content.type)
		{
		case CodexWidget.ContentType.Text:
		{
			LocText component = contentGameObject.GetComponent<LocText>();
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
			this.activeText.Add(component);
			break;
		}
		case CodexWidget.ContentType.Image:
			if (content.properties.ContainsKey("spriteName"))
			{
				string text;
				content.properties.TryGetValue("spriteName", out text);
				contentGameObject.GetComponent<Image>().sprite = Assets.GetSprite(text);
			}
			else if (content.properties.ContainsKey("batchedAnimPrefabSourceID"))
			{
				contentGameObject.GetComponent<Image>().sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(content.properties["batchedAnimPrefabSourceID"]).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
			}
			else
			{
				contentGameObject.GetComponent<Image>().sprite = (Sprite)content.objectProperties["sprite"];
			}
			this.ConfigurePreferredLayout(content, contentGameObject);
			break;
		case CodexWidget.ContentType.DividerLine:
			contentGameObject.GetComponent<LayoutElement>().minWidth = this.displayPane.rectTransform().sizeDelta.x - 64f;
			break;
		case CodexWidget.ContentType.Spacer:
			this.ConfigurePreferredLayout(content, contentGameObject);
			break;
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

	private List<LocText> activeText = new List<LocText>();

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

	[Header("Text Styles")]
	[SerializeField]
	private TextStyleSetting textStyleTitle;

	[SerializeField]
	private TextStyleSetting textStyleSubtitle;

	[SerializeField]
	private TextStyleSetting textStyleBody;

	private List<CodexEntry> searchResults = new List<CodexEntry>();

	public enum PlanCategory
	{
		Home,
		Tech,
		Creatures,
		Plants,
		Food,
		Diseases,
		Roles,
		Buildings,
		Elements,
		Systems
	}
}
