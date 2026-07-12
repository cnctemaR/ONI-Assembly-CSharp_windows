using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexScreen : KScreen
{
	public string activeEntryID
	{
		get
		{
			return this._activeEntryID;
		}
		private set
		{
			this._activeEntryID = value;
		}
	}

	protected override void OnActivate()
	{
		base.ConsumeMouseScroll = true;
		base.OnActivate();
		this.closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		this.clearSearchButton.onClick += delegate
		{
			this.searchInputField.text = "";
		};
		if (string.IsNullOrEmpty(this.activeEntryID))
		{
			this.ChangeArticle("HOME", false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		}
		this.searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			this.FilterSearch(value);
		});
		KInputTextField kinputTextField = this.searchInputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
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
		return 50f;
	}

	private void CodexScreenInit()
	{
		this.textStyles[CodexTextStyle.Title] = this.textStyleTitle;
		this.textStyles[CodexTextStyle.Subtitle] = this.textStyleSubtitle;
		this.textStyles[CodexTextStyle.Body] = this.textStyleBody;
		this.textStyles[CodexTextStyle.BodyWhite] = this.textStyleBodyWhite;
		this.SetupPrefabs();
		this.PopulatePools();
		this.CategorizeEntries();
		this.FilterSearch("");
		this.backButtonButton.onClick += this.HistoryStepBack;
		this.backButtonButton.soundPlayer.AcceptClickCondition = () => this.currentHistoryIdx > 0;
		this.fwdButtonButton.onClick += this.HistoryStepForward;
		this.fwdButtonButton.soundPlayer.AcceptClickCondition = () => this.currentHistoryIdx < this.history.Count - 1;
		Game.Instance.Subscribe(1594320620, delegate(object val)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			this.FilterSearch(this.searchInputField.text);
			if (!string.IsNullOrEmpty(this.activeEntryID))
			{
				this.ChangeArticle(this.activeEntryID, false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			}
		});
	}

	private void SetupPrefabs()
	{
		this.contentContainerPool = new UIGameObjectPool(this.prefabContentContainer);
		this.contentContainerPool.disabledElementParent = this.widgetPool;
		this.ContentPrefabs[typeof(CodexText)] = this.prefabTextWidget;
		this.ContentPrefabs[typeof(CodexTextWithTooltip)] = this.prefabTextWithTooltipWidget;
		this.ContentPrefabs[typeof(CodexImage)] = this.prefabImageWidget;
		this.ContentPrefabs[typeof(CodexDividerLine)] = this.prefabDividerLineWidget;
		this.ContentPrefabs[typeof(CodexSpacer)] = this.prefabSpacer;
		this.ContentPrefabs[typeof(CodexLabelWithIcon)] = this.prefabLabelWithIcon;
		this.ContentPrefabs[typeof(CodexLabelWithLargeIcon)] = this.prefabLabelWithLargeIcon;
		this.ContentPrefabs[typeof(CodexContentLockedIndicator)] = this.prefabContentLocked;
		this.ContentPrefabs[typeof(CodexLargeSpacer)] = this.prefabLargeSpacer;
		this.ContentPrefabs[typeof(CodexVideo)] = this.prefabVideoWidget;
		this.ContentPrefabs[typeof(CodexIndentedLabelWithIcon)] = this.prefabIndentedLabelWithIcon;
		this.ContentPrefabs[typeof(CodexRecipePanel)] = this.prefabRecipePanel;
		this.ContentPrefabs[typeof(CodexConversionPanel)] = this.prefabConversionPanel;
		this.ContentPrefabs[typeof(CodexCollapsibleHeader)] = this.prefabCollapsibleHeader;
	}

	private List<CodexEntry> FilterSearch(string input)
	{
		this.searchResults.Clear();
		this.subEntrySearchResults.Clear();
		input = input.ToLower();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			bool flag = false;
			string[] dlcIds = keyValuePair.Value.GetDlcIds();
			for (int i = 0; i < dlcIds.Length; i++)
			{
				if (DlcManager.IsContentActive(dlcIds[i]))
				{
					flag = true;
					break;
				}
			}
			string[] forbiddenDLCs = keyValuePair.Value.GetForbiddenDLCs();
			for (int j = 0; j < forbiddenDLCs.Length; j++)
			{
				if (DlcManager.IsContentActive(forbiddenDLCs[j]))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (input == "")
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
			}
		}
		foreach (KeyValuePair<string, SubEntry> keyValuePair2 in CodexCache.subEntries)
		{
			if (input == keyValuePair2.Value.name.ToLower() || input.Contains(keyValuePair2.Value.name.ToLower()) || keyValuePair2.Value.name.ToLower().Contains(input))
			{
				this.subEntrySearchResults.Add(keyValuePair2.Value);
			}
		}
		this.FilterEntries(input != "");
		return this.searchResults;
	}

	private bool HasUnlockedCategoryEntries(string entryID)
	{
		foreach (ContentContainer contentContainer in CodexCache.entries[entryID].contentContainers)
		{
			if (string.IsNullOrEmpty(contentContainer.lockID) || Game.Instance.unlocks.IsUnlocked(contentContainer.lockID))
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
		foreach (KeyValuePair<SubEntry, GameObject> keyValuePair2 in this.subEntryButtons)
		{
			keyValuePair2.Value.SetActive(this.subEntrySearchResults.Contains(keyValuePair2.Key));
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
		header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").ChangeState(open ? 1 : 0);
		header.GetComponent<HierarchyReferences>().GetReference("Content").gameObject.SetActive(open);
	}

	private void PopulatePools()
	{
		foreach (KeyValuePair<Type, GameObject> keyValuePair in this.ContentPrefabs)
		{
			UIGameObjectPool uigameObjectPool = new UIGameObjectPool(keyValuePair.Value);
			uigameObjectPool.disabledElementParent = this.widgetPool;
			this.ContentUIPools[keyValuePair.Key] = uigameObjectPool;
		}
	}

	private GameObject NewCategoryHeader(KeyValuePair<string, CodexEntry> entryKVP, Dictionary<string, GameObject> categories)
	{
		if (entryKVP.Value.category == "")
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
		categoryHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").onClick = delegate
		{
			this.ToggleCategoryOpen(categoryHeader, !categoryContent.activeSelf);
		};
		return categoryHeader;
	}

	private void CategorizeEntries()
	{
		GameObject gameObject = this.navigatorContent.gameObject;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		List<global::Tuple<string, CodexEntry>> list = new List<global::Tuple<string, CodexEntry>>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in CodexCache.entries)
		{
			if (string.IsNullOrEmpty(keyValuePair.Value.sortString))
			{
				keyValuePair.Value.sortString = UI.StripLinkFormatting(Strings.Get(keyValuePair.Value.title));
			}
			list.Add(new global::Tuple<string, CodexEntry>(keyValuePair.Key, keyValuePair.Value));
		}
		list.Sort((global::Tuple<string, CodexEntry> a, global::Tuple<string, CodexEntry> b) => string.Compare(a.second.sortString, b.second.sortString));
		for (int i = 0; i < list.Count; i++)
		{
			global::Tuple<string, CodexEntry> tuple = list[i];
			string text = tuple.second.category;
			if (text == "" || text == "Root")
			{
				text = "Root";
			}
			if (!dictionary.ContainsKey(text))
			{
				this.NewCategoryHeader(new KeyValuePair<string, CodexEntry>(tuple.first, tuple.second), dictionary);
			}
			GameObject gameObject2 = Util.KInstantiateUI(this.prefabNavigatorEntry, dictionary[text], true);
			string id = tuple.second.id;
			gameObject2.GetComponent<KButton>().onClick += delegate
			{
				this.ChangeArticle(id, false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
			if (string.IsNullOrEmpty(tuple.second.name))
			{
				tuple.second.name = Strings.Get(tuple.second.title);
			}
			gameObject2.GetComponentInChildren<LocText>().text = tuple.second.name;
			this.entryButtons.Add(tuple.second, gameObject2);
			foreach (SubEntry subEntry in tuple.second.subEntries)
			{
				GameObject gameObject3 = Util.KInstantiateUI(this.prefabNavigatorEntry, dictionary[text], true);
				string subEntryId = subEntry.id;
				gameObject3.GetComponent<KButton>().onClick += delegate
				{
					this.ChangeArticle(subEntryId, false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
				};
				if (string.IsNullOrEmpty(subEntry.name))
				{
					subEntry.name = Strings.Get(subEntry.title);
				}
				gameObject3.GetComponentInChildren<LocText>().text = subEntry.name;
				this.subEntryButtons.Add(subEntry, gameObject3);
				CodexCache.subEntries.Add(subEntry.id, subEntry);
			}
		}
		foreach (KeyValuePair<string, CodexEntry> keyValuePair2 in CodexCache.entries)
		{
			if (CodexCache.entries.ContainsKey(keyValuePair2.Value.category) && CodexCache.entries.ContainsKey(CodexCache.entries[keyValuePair2.Value.category].category))
			{
				keyValuePair2.Value.searchOnly = true;
			}
		}
		List<KeyValuePair<string, GameObject>> list2 = new List<KeyValuePair<string, GameObject>>();
		foreach (KeyValuePair<string, GameObject> keyValuePair3 in dictionary)
		{
			list2.Add(keyValuePair3);
		}
		list2.Sort((KeyValuePair<string, GameObject> a, KeyValuePair<string, GameObject> b) => string.Compare(a.Value.name, b.Value.name));
		for (int j = 0; j < list2.Count; j++)
		{
			list2[j].Value.transform.parent.SetSiblingIndex(j);
		}
		CodexScreen.SetupCategory(dictionary, "PLANTS");
		CodexScreen.SetupCategory(dictionary, "CREATURES");
		CodexScreen.SetupCategory(dictionary, "NOTICES");
		CodexScreen.SetupCategory(dictionary, "RESEARCHNOTES");
		CodexScreen.SetupCategory(dictionary, "JOURNALS");
		CodexScreen.SetupCategory(dictionary, "EMAILS");
		CodexScreen.SetupCategory(dictionary, "INVESTIGATIONS");
		CodexScreen.SetupCategory(dictionary, "MYLOG");
		CodexScreen.SetupCategory(dictionary, "LESSONS");
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

	public void ChangeArticle(string id, bool playClickSound = false, Vector3 targetPosition = default(Vector3), CodexScreen.HistoryDirection historyMovement = CodexScreen.HistoryDirection.NewArticle)
	{
		global::Debug.Assert(id != null);
		if (playClickSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		if (this.contentContainerPool == null)
		{
			this.CodexScreenInit();
		}
		string text = "";
		SubEntry subEntry = null;
		if (!CodexCache.entries.ContainsKey(id))
		{
			subEntry = CodexCache.FindSubEntry(id);
			if (subEntry != null && !subEntry.disabled)
			{
				id = subEntry.parentEntryID.ToUpper();
				text = UI.StripLinkFormatting(subEntry.name);
			}
			else
			{
				id = "PAGENOTFOUND";
			}
		}
		if (CodexCache.entries[id].disabled)
		{
			id = "PAGENOTFOUND";
		}
		if (string.IsNullOrEmpty(text))
		{
			text = UI.StripLinkFormatting(CodexCache.entries[id].name);
		}
		ICodexWidget codexWidget = null;
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
		int num = 0;
		string text2 = "";
		while (this.contentContainers.transform.childCount > 0)
		{
			while (!string.IsNullOrEmpty(text2) && CodexCache.entries[this.activeEntryID].contentContainers[num].lockID == text2)
			{
				num++;
			}
			GameObject gameObject = this.contentContainers.transform.GetChild(0).gameObject;
			int num2 = 0;
			while (gameObject.transform.childCount > 0)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
				Type type;
				if (gameObject2.name == "PrefabContentLocked")
				{
					text2 = CodexCache.entries[this.activeEntryID].contentContainers[num].lockID;
					type = typeof(CodexContentLockedIndicator);
				}
				else
				{
					type = CodexCache.entries[this.activeEntryID].contentContainers[num].content[num2].GetType();
				}
				this.ContentUIPools[type].ClearElement(gameObject2);
				num2++;
			}
			this.contentContainerPool.ClearElement(this.contentContainers.transform.GetChild(0).gameObject);
			num++;
		}
		bool flag = CodexCache.entries[id] is CategoryEntry;
		this.activeEntryID = id;
		if (CodexCache.entries[id].contentContainers == null)
		{
			CodexCache.entries[id].CreateContentContainerCollection();
		}
		bool flag2 = false;
		string text3 = "";
		for (int i = 0; i < CodexCache.entries[id].contentContainers.Count; i++)
		{
			ContentContainer contentContainer2 = CodexCache.entries[id].contentContainers[i];
			if (!string.IsNullOrEmpty(contentContainer2.lockID) && !Game.Instance.unlocks.IsUnlocked(contentContainer2.lockID))
			{
				if (text3 != contentContainer2.lockID)
				{
					GameObject gameObject3 = this.contentContainerPool.GetFreeElement(this.contentContainers.gameObject, true).gameObject;
					this.ConfigureContentContainer(contentContainer2, gameObject3, flag && flag2);
					text3 = contentContainer2.lockID;
					GameObject gameObject4 = this.ContentUIPools[typeof(CodexContentLockedIndicator)].GetFreeElement(gameObject3, true).gameObject;
				}
			}
			else
			{
				GameObject gameObject3 = this.contentContainerPool.GetFreeElement(this.contentContainers.gameObject, true).gameObject;
				this.ConfigureContentContainer(contentContainer2, gameObject3, flag && flag2);
				flag2 = !flag2;
				if (contentContainer2.content != null)
				{
					foreach (ICodexWidget codexWidget2 in contentContainer2.content)
					{
						GameObject gameObject5 = this.ContentUIPools[codexWidget2.GetType()].GetFreeElement(gameObject3, true).gameObject;
						codexWidget2.Configure(gameObject5, this.displayPane, this.textStyles);
						if (codexWidget2 == codexWidget)
						{
							rectTransform = gameObject5.rectTransform();
						}
					}
				}
			}
		}
		string text4 = "";
		string text5 = id;
		int num3 = 0;
		while (text5 != CodexCache.FormatLinkID("HOME") && num3 < 10)
		{
			num3++;
			if (text5 != null)
			{
				if (text5 != id)
				{
					text4 = text4.Insert(0, CodexCache.entries[text5].name + " > ");
				}
				else
				{
					text4 = text4.Insert(0, CodexCache.entries[text5].name);
				}
				text5 = CodexCache.entries[text5].parentId;
			}
			else
			{
				text5 = CodexCache.entries[CodexCache.FormatLinkID("HOME")].id;
				text4 = text4.Insert(0, CodexCache.entries[text5].name + " > ");
			}
		}
		this.currentLocationText.text = ((text4 == "") ? ("<b>" + UI.StripLinkFormatting(CodexCache.entries["HOME"].name) + "</b>") : text4);
		if (this.history.Count == 0)
		{
			this.history.Add(new CodexScreen.HistoryEntry(id, Vector3.zero, text));
			this.currentHistoryIdx = 0;
		}
		else if (historyMovement == CodexScreen.HistoryDirection.Back)
		{
			this.history[this.currentHistoryIdx].position = this.displayPane.transform.localPosition;
			this.currentHistoryIdx--;
		}
		else if (historyMovement == CodexScreen.HistoryDirection.Forward)
		{
			this.history[this.currentHistoryIdx].position = this.displayPane.transform.localPosition;
			this.currentHistoryIdx++;
		}
		else if (historyMovement == CodexScreen.HistoryDirection.NewArticle || historyMovement == CodexScreen.HistoryDirection.Up)
		{
			if (this.currentHistoryIdx == this.history.Count - 1)
			{
				this.history.Add(new CodexScreen.HistoryEntry(this.activeEntryID, Vector3.zero, text));
				this.history[this.currentHistoryIdx].position = this.displayPane.transform.localPosition;
				this.currentHistoryIdx++;
			}
			else
			{
				for (int j = this.history.Count - 1; j > this.currentHistoryIdx; j--)
				{
					this.history.RemoveAt(j);
				}
				this.history.Add(new CodexScreen.HistoryEntry(this.activeEntryID, Vector3.zero, text));
				this.history[this.history.Count - 2].position = this.displayPane.transform.localPosition;
				this.currentHistoryIdx++;
			}
		}
		if (this.currentHistoryIdx > 0)
		{
			this.backButtonButton.GetComponent<Image>().color = Color.black;
			this.backButton.text = UI.FormatAsLink(string.Format(UI.CODEX.BACK_BUTTON, UI.StripLinkFormatting(CodexCache.entries[this.history[this.history.Count - 2].id].name)), CodexCache.entries[this.history[this.history.Count - 2].id].id);
			this.backButtonButton.GetComponent<ToolTip>().toolTip = string.Format(UI.CODEX.BACK_BUTTON_TOOLTIP, this.history[this.currentHistoryIdx - 1].name);
		}
		else
		{
			this.backButtonButton.GetComponent<Image>().color = Color.grey;
			this.backButton.text = UI.StripLinkFormatting(GameUtil.ColourizeString(Color.grey, string.Format(UI.CODEX.BACK_BUTTON, CodexCache.entries["HOME"].name)));
			this.backButtonButton.GetComponent<ToolTip>().toolTip = UI.CODEX.BACK_BUTTON_NO_HISTORY_TOOLTIP;
		}
		if (this.currentHistoryIdx < this.history.Count - 1)
		{
			this.fwdButtonButton.GetComponent<Image>().color = Color.black;
			this.fwdButtonButton.GetComponent<ToolTip>().toolTip = string.Format(UI.CODEX.FORWARD_BUTTON_TOOLTIP, this.history[this.currentHistoryIdx + 1].name);
		}
		else
		{
			this.fwdButtonButton.GetComponent<Image>().color = Color.grey;
			this.fwdButtonButton.GetComponent<ToolTip>().toolTip = UI.CODEX.FORWARD_BUTTON_NO_HISTORY_TOOLTIP;
		}
		if (targetPosition != Vector3.zero)
		{
			if (this.scrollToTargetRoutine != null)
			{
				base.StopCoroutine(this.scrollToTargetRoutine);
			}
			this.scrollToTargetRoutine = base.StartCoroutine(this.ScrollToTarget(targetPosition));
			return;
		}
		if (rectTransform != null)
		{
			if (this.scrollToTargetRoutine != null)
			{
				base.StopCoroutine(this.scrollToTargetRoutine);
			}
			this.scrollToTargetRoutine = base.StartCoroutine(this.ScrollToTarget(rectTransform));
			return;
		}
		this.displayScrollRect.content.SetLocalPosition(Vector3.zero);
	}

	private void HistoryStepBack()
	{
		if (this.currentHistoryIdx == 0)
		{
			return;
		}
		this.ChangeArticle(this.history[this.currentHistoryIdx - 1].id, false, this.history[this.currentHistoryIdx - 1].position, CodexScreen.HistoryDirection.Back);
	}

	private void HistoryStepForward()
	{
		if (this.currentHistoryIdx == this.history.Count - 1)
		{
			return;
		}
		this.ChangeArticle(this.history[this.currentHistoryIdx + 1].id, false, this.history[this.currentHistoryIdx + 1].position, CodexScreen.HistoryDirection.Forward);
	}

	private void HistoryStepUp()
	{
		if (string.IsNullOrEmpty(CodexCache.entries[this.activeEntryID].parentId))
		{
			return;
		}
		this.ChangeArticle(CodexCache.entries[this.activeEntryID].parentId, false, default(Vector3), CodexScreen.HistoryDirection.Up);
	}

	private IEnumerator ScrollToTarget(RectTransform targetWidgetTransform)
	{
		yield return 0;
		this.displayScrollRect.content.SetLocalPosition(Vector3.down * (this.displayScrollRect.content.InverseTransformPoint(targetWidgetTransform.GetPosition()).y + 12f));
		yield break;
	}

	private IEnumerator ScrollToTarget(Vector3 position)
	{
		yield return 0;
		this.displayScrollRect.content.SetLocalPosition(position);
		yield break;
	}

	private void ConfigureContentContainer(ContentContainer container, GameObject containerGameObject, bool bgColor = false)
	{
		container.go = containerGameObject;
		LayoutGroup layoutGroup = containerGameObject.GetComponent<LayoutGroup>();
		if (layoutGroup != null)
		{
			global::UnityEngine.Object.DestroyImmediate(layoutGroup);
		}
		if (!Game.Instance.unlocks.IsUnlocked(container.lockID) && !string.IsNullOrEmpty(container.lockID))
		{
			layoutGroup = containerGameObject.AddComponent<VerticalLayoutGroup>();
			(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandHeight = ((layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
			(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
			return;
		}
		switch (container.contentLayout)
		{
		case ContentContainer.ContentLayout.Vertical:
			layoutGroup = containerGameObject.AddComponent<VerticalLayoutGroup>();
			(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandHeight = ((layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
			(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
			return;
		case ContentContainer.ContentLayout.Horizontal:
			layoutGroup = containerGameObject.AddComponent<HorizontalLayoutGroup>();
			layoutGroup.childAlignment = TextAnchor.MiddleLeft;
			(layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandHeight = ((layoutGroup as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
			(layoutGroup as HorizontalOrVerticalLayoutGroup).spacing = 8f;
			return;
		case ContentContainer.ContentLayout.Grid:
			layoutGroup = containerGameObject.AddComponent<GridLayoutGroup>();
			(layoutGroup as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			(layoutGroup as GridLayoutGroup).constraintCount = 4;
			(layoutGroup as GridLayoutGroup).cellSize = new Vector2(128f, 180f);
			(layoutGroup as GridLayoutGroup).spacing = new Vector2(6f, 6f);
			return;
		case ContentContainer.ContentLayout.GridTwoColumn:
			layoutGroup = containerGameObject.AddComponent<GridLayoutGroup>();
			(layoutGroup as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			(layoutGroup as GridLayoutGroup).constraintCount = 2;
			(layoutGroup as GridLayoutGroup).cellSize = new Vector2(264f, 32f);
			(layoutGroup as GridLayoutGroup).spacing = new Vector2(0f, 12f);
			return;
		case ContentContainer.ContentLayout.GridTwoColumnTall:
			layoutGroup = containerGameObject.AddComponent<GridLayoutGroup>();
			(layoutGroup as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			(layoutGroup as GridLayoutGroup).constraintCount = 2;
			(layoutGroup as GridLayoutGroup).cellSize = new Vector2(264f, 64f);
			(layoutGroup as GridLayoutGroup).spacing = new Vector2(0f, 12f);
			return;
		default:
			return;
		}
	}

	private string _activeEntryID;

	private Dictionary<Type, UIGameObjectPool> ContentUIPools = new Dictionary<Type, UIGameObjectPool>();

	private Dictionary<Type, GameObject> ContentPrefabs = new Dictionary<Type, GameObject>();

	private List<GameObject> categoryHeaders = new List<GameObject>();

	private Dictionary<CodexEntry, GameObject> entryButtons = new Dictionary<CodexEntry, GameObject>();

	private Dictionary<SubEntry, GameObject> subEntryButtons = new Dictionary<SubEntry, GameObject>();

	private UIGameObjectPool contentContainerPool;

	[SerializeField]
	private KScrollRect displayScrollRect;

	[SerializeField]
	private RectTransform scrollContentPane;

	private bool editingSearch;

	private List<CodexScreen.HistoryEntry> history = new List<CodexScreen.HistoryEntry>();

	private int currentHistoryIdx;

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
	private KInputTextField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private LocText backButton;

	[SerializeField]
	private KButton backButtonButton;

	[SerializeField]
	private KButton fwdButtonButton;

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
	private GameObject prefabTextWithTooltipWidget;

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
	private GameObject prefabLabelWithLargeIcon;

	[SerializeField]
	private GameObject prefabContentLocked;

	[SerializeField]
	private GameObject prefabVideoWidget;

	[SerializeField]
	private GameObject prefabIndentedLabelWithIcon;

	[SerializeField]
	private GameObject prefabRecipePanel;

	[SerializeField]
	private GameObject prefabConversionPanel;

	[SerializeField]
	private GameObject prefabCollapsibleHeader;

	[Header("Text Styles")]
	[SerializeField]
	private TextStyleSetting textStyleTitle;

	[SerializeField]
	private TextStyleSetting textStyleSubtitle;

	[SerializeField]
	private TextStyleSetting textStyleBody;

	[SerializeField]
	private TextStyleSetting textStyleBodyWhite;

	private Dictionary<CodexTextStyle, TextStyleSetting> textStyles = new Dictionary<CodexTextStyle, TextStyleSetting>();

	private List<CodexEntry> searchResults = new List<CodexEntry>();

	private List<SubEntry> subEntrySearchResults = new List<SubEntry>();

	private Coroutine scrollToTargetRoutine;

	public enum PlanCategory
	{
		Home,
		Tips,
		MyLog,
		Investigations,
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

	public enum HistoryDirection
	{
		Back,
		Forward,
		Up,
		NewArticle
	}

	public class HistoryEntry
	{
		public HistoryEntry(string entry, Vector3 pos, string articleName)
		{
			this.id = entry;
			this.position = pos;
			this.name = articleName;
		}

		public string id;

		public Vector3 position;

		public string name;
	}
}
