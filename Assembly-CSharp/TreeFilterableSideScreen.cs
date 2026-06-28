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
			switch (this.GetAllCheckboxState())
			{
			case TreeFilterableSideScreenRow.State.Off:
			case TreeFilterableSideScreenRow.State.Mixed:
				this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.On);
				break;
			case TreeFilterableSideScreenRow.State.On:
				this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.Off);
				break;
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
		global::Debug.LogWarning("TreeFilterableSideScreen shouldn't hit this", null);
		return TreeFilterableSideScreenRow.State.On;
	}

	private void SetAllCheckboxState(TreeFilterableSideScreenRow.State newState)
	{
		switch (newState)
		{
		case TreeFilterableSideScreenRow.State.Off:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
			{
				keyValuePair.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
			}
			break;
		case TreeFilterableSideScreenRow.State.On:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair2 in this.tagRowMap)
			{
				keyValuePair2.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
			}
			break;
		}
		this.visualDirty = true;
	}

	public bool GetElementTagAcceptedState(Tag t)
	{
		return this.targetFilterable.ContainsTag(t);
	}

	public override void SetTarget(GameObject target)
	{
		this.target = target;
		if (target == null)
		{
			global::Debug.LogError("The target object provided was null", null);
			return;
		}
		this.targetFilterable = target.GetComponent<TreeFilterable>();
		if (this.targetFilterable == null)
		{
			global::Debug.LogError("The target provided does not have a Tree Filterable component", null);
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
		Storage storage = this.storage;
		storage.onPriorityChanged = (global::System.Action)Delegate.Combine(storage.onPriorityChanged, new global::System.Action(this.OnPriorityChanged));
		this.OnPriorityChanged();
		this.CreateCategories();
	}

	private void OnPriorityChanged()
	{
		this.onlyAllowTransportItemsCheckBox.isOn = this.storage.GetOnlyFetchMarkedItems();
		this.onlyAllowTransportItemsImg.enabled = this.storage.GetOnlyFetchMarkedItems();
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

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
		freeElement.Parent = this;
		this.tagRowMap.Add(rowTag, freeElement);
		List<Tag> list = new List<Tag>(WorldInventory.Instance.GetDiscoveredResourcesFromTag(rowTag));
		list.Sort((Tag a, Tag b) => a.ProperName().CompareTo(b.ProperName()));
		Dictionary<Tag, bool> dictionary = new Dictionary<Tag, bool>();
		foreach (Tag tag in list)
		{
			dictionary.Add(tag, this.targetFilterable.ContainsTag(tag) || this.targetFilterable.ContainsTag(rowTag));
		}
		freeElement.SetElement(rowTag, this.targetFilterable.ContainsTag(rowTag), dictionary);
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
			foreach (Tag tag in this.storage.storageFilters)
			{
				bool flag2 = flag || WorldInventory.Instance.IsDiscovered(tag);
				if (flag2)
				{
					this.AddRow(tag);
				}
			}
			this.visualDirty = true;
		}
		else
		{
			Output.LogError(new object[] { "If you're filtering, your storage filter should have the filters set on it" });
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.storage != null)
		{
			Storage storage = this.storage;
			storage.onPriorityChanged = (global::System.Action)Delegate.Remove(storage.onPriorityChanged, new global::System.Action(this.OnPriorityChanged));
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
}
