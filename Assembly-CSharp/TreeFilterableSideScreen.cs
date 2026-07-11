using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TreeFilterableSideScreen : SideScreenContent
{
	public bool IsStorage
	{
		get
		{
			return this.storage != null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rowPool = new UIPool<TreeFilterableSideScreenRow>(this.rowPrefab);
		this.elementPool = new UIPool<TreeFilterableSideScreenElement>(this.elementPrefab);
		MultiToggle multiToggle = this.allCheckBox;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			TreeFilterableSideScreenRow.State allCheckboxState = this.GetAllCheckboxState();
			if (allCheckboxState > TreeFilterableSideScreenRow.State.Mixed)
			{
				if (allCheckboxState == TreeFilterableSideScreenRow.State.On)
				{
					this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.Off);
					return;
				}
			}
			else
			{
				this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.On);
			}
		}));
		this.onlyAllowTransportItemsImg = this.onlyAllowTransportItemsCheckBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.onlyAllowTransportItemsCheckBox.onClick += this.OnlyAllowTransportItemsClicked;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.allCheckBox.transform.parent.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
		this.onlyAllowTransportItemsCheckBox.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
	}

	private void UpdateAllCheckBoxVisualState()
	{
		switch (this.GetAllCheckboxState())
		{
		case TreeFilterableSideScreenRow.State.Off:
			this.allCheckBox.ChangeState(0);
			break;
		case TreeFilterableSideScreenRow.State.Mixed:
			this.allCheckBox.ChangeState(1);
			break;
		case TreeFilterableSideScreenRow.State.On:
			this.allCheckBox.ChangeState(2);
			break;
		}
		this.visualDirty = false;
	}

	public void Update()
	{
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			if (keyValuePair.Value.visualDirty)
			{
				keyValuePair.Value.UpdateCheckBoxVisualState();
				this.visualDirty = true;
			}
		}
		if (this.visualDirty)
		{
			this.UpdateAllCheckBoxVisualState();
		}
	}

	private void OnlyAllowTransportItemsClicked()
	{
		this.storage.SetOnlyFetchMarkedItems(!this.storage.GetOnlyFetchMarkedItems());
	}

	private TreeFilterableSideScreenRow.State GetAllCheckboxState()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			switch (keyValuePair.Value.GetState())
			{
			case TreeFilterableSideScreenRow.State.Off:
				flag2 = true;
				break;
			case TreeFilterableSideScreenRow.State.Mixed:
				flag3 = true;
				break;
			case TreeFilterableSideScreenRow.State.On:
				flag = true;
				break;
			}
		}
		if (flag3)
		{
			return TreeFilterableSideScreenRow.State.Mixed;
		}
		if (flag && !flag2)
		{
			return TreeFilterableSideScreenRow.State.On;
		}
		if (!flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Off;
		}
		if (flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Mixed;
		}
		return TreeFilterableSideScreenRow.State.Off;
	}

	private void SetAllCheckboxState(TreeFilterableSideScreenRow.State newState)
	{
		switch (newState)
		{
		case TreeFilterableSideScreenRow.State.Off:
		{
			using (Dictionary<Tag, TreeFilterableSideScreenRow>.Enumerator enumerator = this.tagRowMap.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair = enumerator.Current;
					keyValuePair.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
				}
				goto IL_008C;
			}
			break;
		}
		case TreeFilterableSideScreenRow.State.Mixed:
			goto IL_008C;
		case TreeFilterableSideScreenRow.State.On:
			break;
		default:
			goto IL_008C;
		}
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair2 in this.tagRowMap)
		{
			keyValuePair2.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
		}
		IL_008C:
		this.visualDirty = true;
	}

	public bool GetElementTagAcceptedState(Tag t)
	{
		return this.targetFilterable.ContainsTag(t);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<TreeFilterable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.target = target;
		if (target == null)
		{
			global::Debug.LogError("The target object provided was null");
			return;
		}
		this.targetFilterable = target.GetComponent<TreeFilterable>();
		if (this.targetFilterable == null)
		{
			global::Debug.LogError("The target provided does not have a Tree Filterable component");
			return;
		}
		if (!this.targetFilterable.showUserMenu)
		{
			DetailsScreen.Instance.DeactivateSideContent();
			return;
		}
		if (this.IsStorage && !this.storage.showInUI)
		{
			DetailsScreen.Instance.DeactivateSideContent();
			return;
		}
		this.storage = this.targetFilterable.GetComponent<Storage>();
		this.storage.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		this.OnOnlyFetchMarkedItemsSettingChanged(null);
		this.CreateCategories();
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		if (this.storage.allowSettingOnlyFetchMarkedItems)
		{
			this.onlyallowTransportItemsRow.SetActive(true);
			this.onlyAllowTransportItemsCheckBox.isOn = this.storage.GetOnlyFetchMarkedItems();
			this.onlyAllowTransportItemsImg.enabled = this.storage.GetOnlyFetchMarkedItems();
			return;
		}
		this.onlyallowTransportItemsRow.SetActive(false);
	}

	public bool IsTagAllowed(Tag tag)
	{
		return this.targetFilterable.AcceptedTags.Contains(tag);
	}

	public void AddTag(Tag tag)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		this.targetFilterable.AddTagToFilter(tag);
	}

	public void RemoveTag(Tag tag)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		this.targetFilterable.RemoveTagFromFilter(tag);
	}

	private List<TreeFilterableSideScreen.TagOrderInfo> GetTagsSortedAlphabetically(ICollection<Tag> tags)
	{
		List<TreeFilterableSideScreen.TagOrderInfo> list = new List<TreeFilterableSideScreen.TagOrderInfo>();
		foreach (Tag tag in tags)
		{
			list.Add(new TreeFilterableSideScreen.TagOrderInfo
			{
				tag = tag,
				strippedName = UI.StripLinkFormatting(tag.ProperName())
			});
		}
		list.Sort((TreeFilterableSideScreen.TagOrderInfo a, TreeFilterableSideScreen.TagOrderInfo b) => a.strippedName.CompareTo(b.strippedName));
		return list;
	}

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
		freeElement.Parent = this;
		this.tagRowMap.Add(rowTag, freeElement);
		Dictionary<Tag, bool> dictionary = new Dictionary<Tag, bool>();
		foreach (TreeFilterableSideScreen.TagOrderInfo tagOrderInfo in this.GetTagsSortedAlphabetically(WorldInventory.Instance.GetDiscoveredResourcesFromTag(rowTag)))
		{
			dictionary.Add(tagOrderInfo.tag, this.targetFilterable.ContainsTag(tagOrderInfo.tag) || this.targetFilterable.ContainsTag(rowTag));
		}
		freeElement.SetElement(rowTag, this.targetFilterable.ContainsTag(rowTag), dictionary);
		freeElement.transform.SetAsLastSibling();
		return freeElement;
	}

	public float GetAmountInStorage(Tag tag)
	{
		if (!this.IsStorage)
		{
			return 0f;
		}
		return this.storage.GetMassAvailable(tag);
	}

	private void CreateCategories()
	{
		if (this.storage.storageFilters != null && this.storage.storageFilters.Count >= 1)
		{
			bool flag = this.target.GetComponent<CreatureDeliveryPoint>() != null;
			foreach (TreeFilterableSideScreen.TagOrderInfo tagOrderInfo in this.GetTagsSortedAlphabetically(this.storage.storageFilters))
			{
				Tag tag = tagOrderInfo.tag;
				if (flag || WorldInventory.Instance.IsDiscovered(tag))
				{
					this.AddRow(tag);
				}
			}
			this.visualDirty = true;
			return;
		}
		global::Debug.LogError("If you're filtering, your storage filter should have the filters set on it");
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.storage != null)
		{
			this.storage.Unsubscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		}
		this.rowPool.ClearAll();
		this.elementPool.ClearAll();
		this.tagRowMap.Clear();
	}

	[SerializeField]
	private MultiToggle allCheckBox;

	[SerializeField]
	private KToggle onlyAllowTransportItemsCheckBox;

	[SerializeField]
	private GameObject onlyallowTransportItemsRow;

	[SerializeField]
	private TreeFilterableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private TreeFilterableSideScreenElement elementPrefab;

	private GameObject target;

	private bool visualDirty;

	private KImage onlyAllowTransportItemsImg;

	public UIPool<TreeFilterableSideScreenElement> elementPool;

	private UIPool<TreeFilterableSideScreenRow> rowPool;

	private TreeFilterable targetFilterable;

	private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();

	private Storage storage;

	private struct TagOrderInfo
	{
		public Tag tag;

		public string strippedName;
	}
}
