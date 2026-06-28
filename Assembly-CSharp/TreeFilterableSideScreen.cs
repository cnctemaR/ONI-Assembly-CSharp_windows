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
		this.allCheckBoxImg = this.allCheckBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.allCheckBox.onClick += delegate
		{
			this.AllCheckBoxChanged(this.allCheckBox.isOn);
		};
		this.onlyAllowTransportItemsImg = this.onlyAllowTransportItemsCheckBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.onlyAllowTransportItemsCheckBox.onClick += this.OnlyAllowTransportItemsClicked;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.allCheckBox.transform.parent.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
		this.onlyAllowTransportItemsCheckBox.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
	}

	private void SetAllCheckBoxVisualState(bool state)
	{
		this.allCheckBoxImg.enabled = state;
	}

	private void OnlyAllowTransportItemsClicked()
	{
		this.storage.SetOnlyFetchMarkedItems(!this.storage.GetOnlyFetchMarkedItems());
	}

	private void AllCheckBoxChanged(bool is_on)
	{
		this.allCheckBoxImg.enabled = is_on;
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			keyValuePair.Value.SetCheckBoxState(is_on, true);
			if (is_on)
			{
				this.targetFilterable.AddTagToFilter(keyValuePair.Key);
			}
			else
			{
				this.targetFilterable.RemoveTagFromFilter(keyValuePair.Key);
			}
		}
	}

	public bool GetElementTagAcceptedState(Tag t)
	{
		return this.targetFilterable.ContainsTag(t);
	}

	public void ElementSelectionChanged()
	{
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			if (keyValuePair.Value.IsNotOff)
			{
				this.allCheckBox.isOn = true;
				this.SetAllCheckBoxVisualState(true);
				return;
			}
		}
		this.allCheckBox.isOn = false;
		this.SetAllCheckBoxVisualState(false);
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("The target object provided was null");
			return;
		}
		this.targetFilterable = target.GetComponent<TreeFilterable>();
		if (this.targetFilterable == null)
		{
			Debug.LogError("The target provided does not have a Tree Filterable component");
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

	public void AddTag(Tag tag)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		this.targetFilterable.AddTagToFilter(tag);
	}

	public void AddTags(List<Tag> tagsToAdd)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		foreach (Tag tag in tagsToAdd)
		{
			this.targetFilterable.AddTagToFilter(tag);
		}
		if (!this.allCheckBox.isOn)
		{
			this.ElementSelectionChanged();
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		this.targetFilterable.RemoveTagFromFilter(tag);
	}

	public void RemoveTags(List<Tag> removalTags)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		foreach (Tag tag in removalTags)
		{
			this.targetFilterable.RemoveTagFromFilter(tag);
		}
		this.ElementSelectionChanged();
	}

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
		freeElement.Parent = this;
		this.tagRowMap.Add(rowTag, freeElement);
		List<Tag> list = new List<Tag>(WorldInventory.Instance.GetDiscoveredResourcesFromTag(rowTag));
		list.Sort();
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
			bool flag = false;
			foreach (Tag tag in this.storage.storageFilters)
			{
				bool flag2 = WorldInventory.Instance.IsDiscovered(tag);
				if (flag2)
				{
					TreeFilterableSideScreenRow treeFilterableSideScreenRow = this.AddRow(tag);
					flag = flag || this.targetFilterable.ContainsTag(tag) || treeFilterableSideScreenRow.IsNotOff;
				}
			}
			this.allCheckBox.isOn = flag;
			this.SetAllCheckBoxVisualState(flag);
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
	private KToggle allCheckBox;

	[SerializeField]
	private KToggle onlyAllowTransportItemsCheckBox;

	[SerializeField]
	private TreeFilterableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private TreeFilterableSideScreenElement elementPrefab;

	private KImage allCheckBoxImg;

	private KImage onlyAllowTransportItemsImg;

	public UIPool<TreeFilterableSideScreenElement> elementPool;

	private UIPool<TreeFilterableSideScreenRow> rowPool;

	private TreeFilterable targetFilterable;

	private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();

	private Storage storage;
}
