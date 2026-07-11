using System;
using System.Collections.Generic;
using System.Diagnostics;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ChoreConsumer : KMonoBehaviour, IPersonalPriorityManager
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ChoreGroupManager.instance != null)
		{
			foreach (KeyValuePair<Tag, int> keyValuePair in ChoreGroupManager.instance.DefaultChorePermission)
			{
				bool flag = false;
				foreach (HashedString hashedString in this.userDisabledChoreGroups)
				{
					if (hashedString.HashValue == keyValuePair.Key.GetHashCode())
					{
						flag = true;
						break;
					}
				}
				if (!flag && keyValuePair.Value == 0)
				{
					this.userDisabledChoreGroups.Add(new HashedString(keyValuePair.Key.GetHashCode()));
				}
			}
		}
		this.providers.Add(this.choreProvider);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (this.choreTable != null)
		{
			this.choreTableInstance = new ChoreTable.Instance(this.choreTable, component);
		}
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			bool flag;
			int personalPriority = this.GetPersonalPriority(choreGroup, out flag);
			this.UpdateChoreTypePriorities(choreGroup, personalPriority);
			this.SetPermittedByUser(choreGroup, personalPriority != 0);
		}
		this.consumerState = new ChoreConsumerState(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.choreTableInstance != null)
		{
			this.choreTableInstance.OnCleanUp(base.GetComponent<KPrefabID>());
			this.choreTableInstance = null;
		}
	}

	public bool IsPermittedByUser(ChoreGroup chore_group)
	{
		return chore_group == null || !this.userDisabledChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetPermittedByUser(ChoreGroup chore_group, bool is_allowed)
	{
		if (is_allowed)
		{
			if (this.userDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.userDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			this.userDisabledChoreGroups.Add(chore_group.IdHash);
			this.choreRulesChanged.Signal();
		}
	}

	public bool IsPermittedByTraits(ChoreGroup chore_group)
	{
		return chore_group == null || !this.traitDisabledChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetPermittedByTraits(ChoreGroup chore_group, bool is_enabled)
	{
		if (is_enabled)
		{
			if (this.traitDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.traitDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			this.traitDisabledChoreGroups.Add(chore_group.IdHash);
			this.choreRulesChanged.Signal();
		}
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		if (this.debug)
		{
			int num = 0;
			num++;
		}
		this.preconditionSnapshot.Clear();
		this.consumerState.Refresh();
		if (this.consumerState.hasSolidTransferArm)
		{
			CellOffset offset = Grid.GetOffset(Grid.PosToCell(this));
			Extents extents = new Extents(offset.x, offset.y, this.stationaryReach);
			ListPool<ScenePartitionerEntry, ChoreConsumer>.PooledList pooledList = ListPool<ScenePartitionerEntry, ChoreConsumer>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.fetchChoreLayer, pooledList);
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
			{
				FetchChore fetchChore = scenePartitionerEntry.obj as FetchChore;
				int num2 = Grid.PosToCell(fetchChore.gameObject);
				if (this.consumerState.solidTransferArm.IsCellReachable(num2))
				{
					fetchChore.CollectChoresFromGlobalChoreProvider(this.consumerState, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts, false);
				}
			}
			pooledList.Recycle();
		}
		else
		{
			for (int i = 0; i < this.providers.Count; i++)
			{
				ChoreProvider choreProvider = this.providers[i];
				choreProvider.CollectChores(this.consumerState, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts);
			}
		}
		List<Chore.Precondition.Context> succeededContexts = this.preconditionSnapshot.succeededContexts;
		succeededContexts.Sort();
		bool flag = false;
		if (succeededContexts.Count > 0)
		{
			Chore currentChore = this.choreDriver.GetCurrentChore();
			for (int j = succeededContexts.Count - 1; j >= 0; j--)
			{
				Chore.Precondition.Context context = succeededContexts[j];
				if (context.IsSuccess() && (currentChore == null || context.interruptPriority > currentChore.choreType.interruptPriority))
				{
					bool flag2 = false;
					if (currentChore != null)
					{
						for (int k = 0; k < currentChore.choreType.interruptExclusion.Count; k++)
						{
							if (context.chore.choreType.tags.Contains(currentChore.choreType.interruptExclusion[k]))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						context.chore.PrepareChore(ref context);
						out_context = context;
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	public void AddProvider(ChoreProvider provider)
	{
		DebugUtil.Assert(provider != null, "Assert!", string.Empty, string.Empty);
		this.providers.Add(provider);
	}

	public void RemoveProvider(ChoreProvider provider)
	{
		this.providers.Remove(provider);
	}

	public void AddUrge(Urge urge)
	{
		DebugUtil.Assert(urge != null, "Assert!", string.Empty, string.Empty);
		this.urges.Add(urge);
		base.Trigger(-736698276, urge);
	}

	public void RemoveUrge(Urge urge)
	{
		this.urges.Remove(urge);
		base.Trigger(231622047, urge);
	}

	public bool HasUrge(Urge urge)
	{
		return this.urges.Contains(urge);
	}

	public List<Urge> GetUrges()
	{
		return this.urges;
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(string evt, string param)
	{
	}

	public bool IsPermittedOrEnabled(ChoreType chore_type, Chore chore)
	{
		if (chore_type.groups.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < chore_type.groups.Length; i++)
		{
			ChoreGroup choreGroup = chore_type.groups[i];
			if (this.IsPermittedByTraits(choreGroup) && this.IsPermittedByUser(choreGroup))
			{
				return true;
			}
		}
		return false;
	}

	public void SetReach(int reach)
	{
		this.stationaryReach = reach;
	}

	public bool GetNavigationCost(IApproachable approachable, out int cost)
	{
		if (this.navigator)
		{
			cost = this.navigator.GetNavigationCost(approachable);
			if (cost != -1)
			{
				return true;
			}
		}
		else if (this.consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			if (this.consumerState.solidTransferArm.IsCellReachable(cell))
			{
				cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
				return true;
			}
		}
		cost = 0;
		return false;
	}

	public bool CanReach(IApproachable approachable)
	{
		if (this.navigator)
		{
			return this.navigator.CanReach(approachable);
		}
		if (this.consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			return this.consumerState.solidTransferArm.IsCellReachable(cell);
		}
		return false;
	}

	public bool IsWithinReach(IApproachable approachable)
	{
		if (this.navigator)
		{
			return !(this == null) && !(base.gameObject == null) && Grid.IsCellOffsetOf(Grid.PosToCell(this), approachable.GetCell(), approachable.GetOffsets());
		}
		return this.consumerState.hasSolidTransferArm && this.consumerState.solidTransferArm.IsCellReachable(approachable.GetCell());
	}

	public void ShowHoverTextOnHoveredItem(Chore.Precondition.Context context, KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		if (context.chore.target.isNull || context.chore.target.gameObject != hover_obj.gameObject)
		{
			return;
		}
		drawer.NewLine(26);
		drawer.AddIndent(36);
		drawer.DrawText(context.chore.choreType.Name, hover_text_card.Styles_BodyText.Standard);
		if (!context.IsSuccess())
		{
			Chore.PreconditionInstance preconditionInstance = context.chore.GetPreconditions()[context.failedPreconditionId];
			string text = preconditionInstance.description;
			if (string.IsNullOrEmpty(text))
			{
				text = preconditionInstance.id;
			}
			if (context.chore.driver != null)
			{
				text = text.Replace("{Assignee}", context.chore.driver.GetProperName());
			}
			text = text.Replace("{Selected}", this.GetProperName());
			drawer.DrawText(" (" + text + ")", hover_text_card.Styles_BodyText.Standard);
		}
	}

	public void ShowHoverTextOnHoveredItem(KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		bool flag = false;
		foreach (Chore.Precondition.Context context in this.preconditionSnapshot.succeededContexts)
		{
			if (context.chore.showAvailabilityInHoverText)
			{
				if (!context.chore.target.isNull && !(context.chore.target.gameObject != hover_obj.gameObject))
				{
					if (!flag)
					{
						drawer.NewLine(26);
						drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
						flag = true;
					}
					this.ShowHoverTextOnHoveredItem(context, hover_obj, drawer, hover_text_card);
				}
			}
		}
		foreach (Chore.Precondition.Context context2 in this.preconditionSnapshot.failedContexts)
		{
			if (context2.chore.showAvailabilityInHoverText)
			{
				if (!context2.chore.target.isNull && !(context2.chore.target.gameObject != hover_obj.gameObject))
				{
					if (!flag)
					{
						drawer.NewLine(26);
						drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
						flag = true;
					}
					this.ShowHoverTextOnHoveredItem(context2, hover_obj, drawer, hover_text_card);
				}
			}
		}
	}

	public int GetPersonalPriority(ChoreType chore_type)
	{
		int num;
		if (!this.choreTypePriorities.TryGetValue(chore_type.IdHash, out num))
		{
			num = 3;
		}
		num = Mathf.Clamp(num, 0, 5);
		return num;
	}

	public int GetPersonalPriority(ChoreGroup group, out bool auto_assigned)
	{
		int num = 3;
		auto_assigned = false;
		ChoreConsumer.PriorityInfo priorityInfo;
		if (this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
		{
			auto_assigned = priorityInfo.wasAutoAssigned;
			num = priorityInfo.priority;
		}
		return Mathf.Clamp(num, 0, 5);
	}

	public int GetPriorityBeforeAutoAssignment(ChoreGroup group)
	{
		int num = 3;
		ChoreConsumer.PriorityInfo priorityInfo;
		if (this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
		{
			num = priorityInfo.priorityWhenAutoAssigned;
		}
		return Mathf.Clamp(num, 0, 5);
	}

	public void SetPersonalPriority(ChoreGroup group, int value, bool auto_assigned)
	{
		if (group.choreTypes == null)
		{
			return;
		}
		value = Mathf.Clamp(value, 0, 5);
		ChoreConsumer.PriorityInfo priorityInfo;
		if (!this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
		{
			priorityInfo.priority = 3;
		}
		this.choreGroupPriorities[group.IdHash] = new ChoreConsumer.PriorityInfo
		{
			priority = value,
			wasAutoAssigned = auto_assigned,
			priorityWhenAutoAssigned = ((!auto_assigned) ? (-1) : priorityInfo.priority)
		};
		this.UpdateChoreTypePriorities(group, value);
		this.SetPermittedByUser(group, value != 0);
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		Klei.AI.Attributes attributes = this.GetAttributes();
		float value = attributes.GetValue(group.attribute.Id);
		return (int)value;
	}

	public bool CanRoleManageChoreGroup(ChoreGroup group)
	{
		bool flag = false;
		MinionResume component = base.GetComponent<MinionResume>();
		if (component != null)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(component.CurrentRole);
			RoleGroup roleGroup;
			if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out roleGroup) && group.Id == roleGroup.choreGroupID)
			{
				flag = true;
			}
		}
		return flag;
	}

	private void UpdateChoreTypePriorities(ChoreGroup group, int value)
	{
		ChoreGroups choreGroups = Db.Get().ChoreGroups;
		foreach (ChoreType choreType in group.choreTypes)
		{
			int num = 0;
			foreach (ChoreGroup choreGroup in choreGroups.resources)
			{
				if (choreGroup.choreTypes != null)
				{
					foreach (ChoreType choreType2 in choreGroup.choreTypes)
					{
						if (choreType2.IdHash == choreType.IdHash)
						{
							bool flag;
							int personalPriority = this.GetPersonalPriority(choreGroup, out flag);
							num = Mathf.Max(num, personalPriority);
						}
					}
				}
			}
			this.choreTypePriorities[choreType.IdHash] = num;
		}
	}

	public void ResetPersonalPriorities()
	{
	}

	public bool RunBehaviourPrecondition(Tag tag)
	{
		ChoreConsumer.BehaviourPrecondition behaviourPrecondition = default(ChoreConsumer.BehaviourPrecondition);
		return this.behaviourPreconditions.TryGetValue(tag, out behaviourPrecondition) && behaviourPrecondition.cb(behaviourPrecondition.arg);
	}

	public void AddBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		DebugUtil.Assert(!this.behaviourPreconditions.ContainsKey(tag), "Assert!", string.Empty, string.Empty);
		this.behaviourPreconditions[tag] = new ChoreConsumer.BehaviourPrecondition
		{
			cb = precondition,
			arg = arg
		};
	}

	public void RemoveBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		this.behaviourPreconditions.Remove(tag);
	}

	public bool IsChoreEqualOrAboveCurrentChorePriority<StateMachineType>()
	{
		Chore currentChore = this.choreDriver.GetCurrentChore();
		return currentChore == null || currentChore.choreType.priority <= this.choreTable.GetChorePriority<StateMachineType>(this);
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		bool flag = false;
		Traits component = base.gameObject.GetComponent<Traits>();
		foreach (Trait trait in component.TraitList)
		{
			if (trait.disabledChoreGroups != null)
			{
				foreach (ChoreGroup choreGroup in trait.disabledChoreGroups)
				{
					if (choreGroup.IdHash == chore_group.IdHash)
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	public Dictionary<HashedString, ChoreConsumer.PriorityInfo> GetChoreGroupPriorities()
	{
		return this.choreGroupPriorities;
	}

	public void SetChoreGroupPriorities(Dictionary<HashedString, ChoreConsumer.PriorityInfo> priorities)
	{
		this.choreGroupPriorities = priorities;
	}

	public const int DEFAULT_PERSONAL_CHORE_PRIORITY = 3;

	public const int MIN_PERSONAL_PRIORITY = 0;

	public const int MAX_PERSONAL_PRIORITY = 5;

	[MyCmpAdd]
	public ChoreProvider choreProvider;

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpGet]
	public Navigator navigator;

	[MyCmpGet]
	public MinionResume resume;

	[MyCmpAdd]
	private User user;

	public global::System.Action choreRulesChanged;

	public bool debug;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	public ChoreTable choreTable;

	private ChoreTable.Instance choreTableInstance;

	public ChoreConsumerState consumerState;

	private Dictionary<Tag, ChoreConsumer.BehaviourPrecondition> behaviourPreconditions = new Dictionary<Tag, ChoreConsumer.BehaviourPrecondition>();

	private ChoreConsumer.PreconditionSnapshot preconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();

	[Serialize]
	private Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	private Dictionary<HashedString, int> choreTypePriorities = new Dictionary<HashedString, int>();

	private List<HashedString> traitDisabledChoreGroups = new List<HashedString>();

	private List<HashedString> userDisabledChoreGroups = new List<HashedString>();

	public HashSet<Tag> preferredChoreTags = new HashSet<Tag>();

	private int stationaryReach = -1;

	private struct BehaviourPrecondition
	{
		public Func<object, bool> cb;

		public object arg;
	}

	private class PreconditionSnapshot
	{
		public void CopyTo(ChoreConsumer.PreconditionSnapshot snapshot)
		{
			snapshot.Clear();
			snapshot.succeededContexts.AddRange(this.succeededContexts);
			snapshot.failedContexts.AddRange(this.failedContexts);
			snapshot.doFailedContextsNeedSorting = true;
		}

		public void Clear()
		{
			this.succeededContexts.Clear();
			this.failedContexts.Clear();
			this.doFailedContextsNeedSorting = true;
		}

		public List<Chore.Precondition.Context> succeededContexts = new List<Chore.Precondition.Context>();

		public List<Chore.Precondition.Context> failedContexts = new List<Chore.Precondition.Context>();

		public bool doFailedContextsNeedSorting = true;
	}

	public struct PriorityInfo
	{
		public int priority;

		public bool wasAutoAssigned;

		public int priorityWhenAutoAssigned;
	}
}
