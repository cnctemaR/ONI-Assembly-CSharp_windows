using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TreeFilterableSideScreen : SideScreenContent
{
	public static TreeFilterableSideScreen Instance
	{
		get
		{
			return TreeFilterableSideScreen.instance;
		}
	}

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
		this.allCheckBox.onClick += this.AllCheckBoxClicked;
		this.onlyAllowTransportItemsImg = this.onlyAllowTransportItemsCheckBox.gameObject.GetComponentInChildrenOnly<KImage>();
		this.onlyAllowTransportItemsCheckBox.onClick += this.OnlyAllowTransportItemsClicked;
		TreeFilterableSideScreen.instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.allCheckBox.transform.parent.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
		this.onlyAllowTransportItemsCheckBox.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
	}

	private void SetAllCheckBoxVisualState(bool state)
	{
		this.allCheckBox.isOn = state;
		this.allCheckBoxImg.enabled = state;
	}

	private void OnlyAllowTransportItemsClicked()
	{
		this.storage.SetOnlyFetchMarkedItems(!this.storage.GetOnlyFetchMarkedItems());
	}

	private void AllCheckBoxClicked()
	{
		this.SetAllCheckBoxVisualState(!this.allCheckBox.isOn);
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			keyValuePair.Value.SetCheckBoxState(this.allCheckBox.isOn, true);
			if (this.allCheckBox.isOn)
			{
				this.targetFilterable.AddTagToFilter(keyValuePair.Key);
			}
			else
			{
				this.targetFilterable.RemoveTagFromFilter(keyValuePair.Key);
			}
		}
	}

	public void ElementSelectionChanged()
	{
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> keyValuePair in this.tagRowMap)
		{
			if (keyValuePair.Value.IsSelected)
			{
				this.SetAllCheckBoxVisualState(true);
				return;
			}
		}
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
		if (this.allCheckBox.isOn)
		{
			this.ElementSelectionChanged();
		}
	}

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
		this.tagRowMap.Add(rowTag, freeElement);
		List<Tag> discoveredResourcesFromTag = WorldInventory.Instance.GetDiscoveredResourcesFromTag(rowTag);
		if (discoveredResourcesFromTag.Count == 0)
		{
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(rowTag);
			if (pickupables != null)
			{
				foreach (Pickupable pickupable in pickupables)
				{
					Tag prefabTag = pickupable.GetComponent<KPrefabID>().PrefabTag;
					if (!discoveredResourcesFromTag.Contains(prefabTag))
					{
						discoveredResourcesFromTag.Add(prefabTag);
					}
				}
			}
		}
		Dictionary<Tag, bool> dictionary = new Dictionary<Tag, bool>();
		foreach (Tag tag in discoveredResourcesFromTag)
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
			foreach (Tag tag in this.storage.storageFilters)
			{
				TreeFilterableSideScreenRow treeFilterableSideScreenRow = this.AddRow(tag);
				if (!WorldInventory.Instance.IsDiscovered(tag))
				{
					treeFilterableSideScreenRow.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			Output.LogError(new object[] { "If you're filtering, your storage filter should have the filters set on it" });
		}
		this.ElementSelectionChanged();
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

	private KImage allCheckBoxImg;

	[SerializeField]
	private KToggle onlyAllowTransportItemsCheckBox;

	private KImage onlyAllowTransportItemsImg;

	[SerializeField]
	private TreeFilterableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	private UIPool<TreeFilterableSideScreenRow> rowPool;

	[SerializeField]
	private TreeFilterableSideScreenElement elementPrefab;

	public UIPool<TreeFilterableSideScreenElement> elementPool;

	private TreeFilterable targetFilterable;

	private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();

	private static TreeFilterableSideScreen instance;

	private Storage storage;
}
