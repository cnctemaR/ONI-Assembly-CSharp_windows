using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoresPanel : TargetScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		KPrefabID component = target.GetComponent<KPrefabID>();
		return component != null && component.HasTag(GameTags.HasChores) && !component.HasTag(GameTags.Minion);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreGroup = Util.KInstantiateUI<HierarchyReferences>(this.choreGroupPrefab, base.gameObject, false);
		this.choreGroup.gameObject.SetActive(true);
	}

	private void Update()
	{
		this.Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		this.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		this.RefreshDetails();
	}

	private void RefreshDetails()
	{
		foreach (Chore chore in GlobalChoreProvider.Instance.chores)
		{
			if (!chore.isNull && chore.gameObject == this.selectedTarget)
			{
				this.AddChoreEntry(chore);
			}
		}
		for (int i = this.activeDupeEntries; i < this.dupeEntries.Count; i++)
		{
			this.dupeEntries[i].gameObject.SetActive(false);
		}
		this.activeDupeEntries = 0;
		for (int j = this.activeChoreEntries; j < this.choreEntries.Count; j++)
		{
			this.choreEntries[j].gameObject.SetActive(false);
		}
		this.activeChoreEntries = 0;
	}

	private void AddChoreEntry(Chore chore)
	{
		HierarchyReferences choreEntry = this.GetChoreEntry(GameUtil.GetChoreName(chore, null), chore.choreType, this.choreGroup.GetReference<RectTransform>("EntriesContainer"));
		FetchChore fetchChore = chore as FetchChore;
		ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
		foreach (Component component in Components.LiveMinionIdentities.Items)
		{
			pooledList.Clear();
			ChoreConsumer component2 = component.GetComponent<ChoreConsumer>();
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			ChoreConsumer.PreconditionSnapshot lastPreconditionSnapshot = component2.GetLastPreconditionSnapshot();
			if (lastPreconditionSnapshot.doFailedContextsNeedSorting)
			{
				lastPreconditionSnapshot.failedContexts.Sort();
				lastPreconditionSnapshot.doFailedContextsNeedSorting = false;
			}
			pooledList.AddRange(lastPreconditionSnapshot.failedContexts);
			pooledList.AddRange(lastPreconditionSnapshot.succeededContexts);
			int num = -1;
			int num2 = 0;
			for (int i = pooledList.Count - 1; i >= 0; i--)
			{
				if (!(pooledList[i].chore.driver != null) || !(pooledList[i].chore.driver != component2.choreDriver))
				{
					bool flag = pooledList[i].IsPotentialSuccess();
					if (flag)
					{
						num2++;
					}
					FetchAreaChore fetchAreaChore = pooledList[i].chore as FetchAreaChore;
					if (pooledList[i].chore == chore || (fetchChore != null && fetchAreaChore != null && fetchAreaChore.smi.SameDestination(fetchChore)))
					{
						num = (flag ? num2 : int.MaxValue);
						context = pooledList[i];
						break;
					}
				}
			}
			if (num >= 0)
			{
				this.DupeEntryDatas.Add(new BuildingChoresPanel.DupeEntryData
				{
					consumer = component2,
					context = context,
					personalPriority = component2.GetPersonalPriority(chore.choreType),
					rank = num
				});
			}
		}
		pooledList.Recycle();
		this.DupeEntryDatas.Sort();
		foreach (BuildingChoresPanel.DupeEntryData dupeEntryData in this.DupeEntryDatas)
		{
			this.GetDupeEntry(dupeEntryData, choreEntry.GetReference<RectTransform>("DupeContainer"));
		}
		this.DupeEntryDatas.Clear();
	}

	private HierarchyReferences GetChoreEntry(string label, ChoreType choreType, RectTransform parent)
	{
		HierarchyReferences hierarchyReferences;
		if (this.activeChoreEntries >= this.choreEntries.Count)
		{
			hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.chorePrefab, parent.gameObject, false);
			this.choreEntries.Add(hierarchyReferences);
		}
		else
		{
			hierarchyReferences = this.choreEntries[this.activeChoreEntries];
			hierarchyReferences.transform.SetParent(parent);
			hierarchyReferences.transform.SetAsLastSibling();
		}
		this.activeChoreEntries++;
		hierarchyReferences.GetReference<LocText>("ChoreLabel").text = label;
		hierarchyReferences.GetReference<LocText>("ChoreSubLabel").text = GameUtil.ChoreGroupsForChoreType(choreType);
		Image reference = hierarchyReferences.GetReference<Image>("Icon");
		if (choreType.groups.Length != 0)
		{
			Sprite sprite = Assets.GetSprite(choreType.groups[0].sprite);
			reference.sprite = sprite;
			reference.gameObject.SetActive(true);
			reference.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, choreType.groups[0].Name);
		}
		else
		{
			reference.gameObject.SetActive(false);
		}
		Image reference2 = hierarchyReferences.GetReference<Image>("Icon2");
		if (choreType.groups.Length > 1)
		{
			Sprite sprite2 = Assets.GetSprite(choreType.groups[1].sprite);
			reference2.sprite = sprite2;
			reference2.gameObject.SetActive(true);
			reference2.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, choreType.groups[1].Name);
		}
		else
		{
			reference2.gameObject.SetActive(false);
		}
		hierarchyReferences.gameObject.SetActive(true);
		return hierarchyReferences;
	}

	private BuildingChoresPanelDupeRow GetDupeEntry(BuildingChoresPanel.DupeEntryData data, RectTransform parent)
	{
		BuildingChoresPanelDupeRow buildingChoresPanelDupeRow;
		if (this.activeDupeEntries >= this.dupeEntries.Count)
		{
			buildingChoresPanelDupeRow = Util.KInstantiateUI<BuildingChoresPanelDupeRow>(this.dupePrefab.gameObject, parent.gameObject, false);
			this.dupeEntries.Add(buildingChoresPanelDupeRow);
		}
		else
		{
			buildingChoresPanelDupeRow = this.dupeEntries[this.activeDupeEntries];
			buildingChoresPanelDupeRow.transform.SetParent(parent);
			buildingChoresPanelDupeRow.transform.SetAsLastSibling();
		}
		this.activeDupeEntries++;
		buildingChoresPanelDupeRow.Init(data);
		buildingChoresPanelDupeRow.gameObject.SetActive(true);
		return buildingChoresPanelDupeRow;
	}

	public GameObject choreGroupPrefab;

	public GameObject chorePrefab;

	public BuildingChoresPanelDupeRow dupePrefab;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	private HierarchyReferences choreGroup;

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

	private int activeChoreEntries;

	private List<BuildingChoresPanelDupeRow> dupeEntries = new List<BuildingChoresPanelDupeRow>();

	private int activeDupeEntries;

	private List<BuildingChoresPanel.DupeEntryData> DupeEntryDatas = new List<BuildingChoresPanel.DupeEntryData>();

	public class DupeEntryData : IComparable<BuildingChoresPanel.DupeEntryData>
	{
		public int CompareTo(BuildingChoresPanel.DupeEntryData other)
		{
			if (this.personalPriority != other.personalPriority)
			{
				return other.personalPriority.CompareTo(this.personalPriority);
			}
			if (this.rank != other.rank)
			{
				return this.rank.CompareTo(other.rank);
			}
			if (this.consumer.GetProperName() != other.consumer.GetProperName())
			{
				return this.consumer.GetProperName().CompareTo(other.consumer.GetProperName());
			}
			return this.consumer.GetInstanceID().CompareTo(other.consumer.GetInstanceID());
		}

		public ChoreConsumer consumer;

		public Chore.Precondition.Context context;

		public int personalPriority;

		public int rank;
	}
}
