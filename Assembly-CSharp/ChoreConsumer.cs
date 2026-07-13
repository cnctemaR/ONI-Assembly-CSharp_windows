using System;
using System.Collections.Generic;
using System.Diagnostics;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ChoreConsumer")]
public class ChoreConsumer : KMonoBehaviour, IPersonalPriorityManager
{
	public ChoreConsumer.PreconditionSnapshot GetLastPreconditionSnapshot()
	{
		return this.preconditionSnapshot;
	}

	public List<Chore.Precondition.Context> GetSuceededPreconditionContexts()
	{
		return this.lastSuccessfulPreconditionSnapshot.succeededContexts;
	}

	public List<Chore.Precondition.Context> GetFailedPreconditionContexts()
	{
		return this.lastSuccessfulPreconditionSnapshot.failedContexts;
	}

	public ChoreConsumer.PreconditionSnapshot GetLastSuccessfulPreconditionSnapshot()
	{
		return this.lastSuccessfulPreconditionSnapshot;
	}

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
			int personalPriority = this.GetPersonalPriority(choreGroup);
			this.UpdateChoreTypePriorities(choreGroup, personalPriority);
			this.SetPermittedByUser(choreGroup, personalPriority != 0);
		}
		this.consumerState = new ChoreConsumerState(this);
	}

	protected override void OnForcedCleanUp()
	{
		if (this.consumerState != null)
		{
			this.consumerState.navigator = null;
		}
		this.navigator = null;
		base.OnForcedCleanUp();
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
				return;
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
				return;
			}
		}
		else if (!this.traitDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			this.traitDisabledChoreGroups.Add(chore_group.IdHash);
			this.choreRulesChanged.Signal();
		}
	}

	private bool ChooseChore(ref Chore.Precondition.Context out_context, List<Chore.Precondition.Context> succeeded_contexts)
	{
		if (succeeded_contexts.Count == 0)
		{
			return false;
		}
		Chore currentChore = this.choreDriver.GetCurrentChore();
		if (currentChore == null)
		{
			for (int i = succeeded_contexts.Count - 1; i >= 0; i--)
			{
				Chore.Precondition.Context context = succeeded_contexts[i];
				if (context.IsSuccess())
				{
					out_context = context;
					return true;
				}
			}
		}
		else
		{
			int interruptPriority = Db.Get().ChoreTypes.TopPriority.interruptPriority;
			int num = ((currentChore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority) ? interruptPriority : currentChore.choreType.interruptPriority);
			for (int j = succeeded_contexts.Count - 1; j >= 0; j--)
			{
				Chore.Precondition.Context context2 = succeeded_contexts[j];
				if (context2.IsSuccess() && ((context2.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority) ? interruptPriority : context2.interruptPriority) > num && !currentChore.choreType.interruptExclusion.Overlaps(context2.chore.choreType.tags))
				{
					out_context = context2;
					return true;
				}
			}
		}
		return false;
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		this.preconditionSnapshot.Clear();
		this.consumerState.Refresh();
		if (this.consumerState.hasSolidTransferArm)
		{
			global::Debug.Assert(this.stationaryReach > 0);
			CellOffset offset = Grid.GetOffset(Grid.PosToCell(this));
			Extents extents = new Extents(offset.x, offset.y, this.stationaryReach);
			GameScenePartitioner.Instance.VisitEntries<ChoreConsumer>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.fetchChoreLayer, ChoreConsumer.FindNextChoreEvaluateEntryHelper, this);
		}
		else
		{
			for (int i = 0; i < this.providers.Count; i++)
			{
				this.providers[i].CollectChores(this.consumerState, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts);
			}
		}
		this.preconditionSnapshot.succeededContexts.Sort();
		List<Chore.Precondition.Context> succeededContexts = this.preconditionSnapshot.succeededContexts;
		bool flag = this.ChooseChore(ref out_context, succeededContexts);
		if (flag)
		{
			this.preconditionSnapshot.CopyTo(this.lastSuccessfulPreconditionSnapshot);
		}
		return flag;
	}

	public void AddProvider(ChoreProvider provider)
	{
		DebugUtil.Assert(provider != null);
		this.providers.Add(provider);
	}

	public void RemoveProvider(ChoreProvider provider)
	{
		this.providers.Remove(provider);
	}

	public void AddUrge(Urge urge)
	{
		DebugUtil.Assert(urge != null);
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
		bool flag = false;
		bool flag2 = true;
		for (int i = 0; i < chore_type.groups.Length; i++)
		{
			ChoreGroup choreGroup = chore_type.groups[i];
			if (!this.IsPermittedByTraits(choreGroup))
			{
				flag2 = false;
			}
			if (this.IsPermittedByUser(choreGroup))
			{
				flag = true;
			}
		}
		return flag && flag2;
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

	public bool GetNavigationCost(int cell, out int cost)
	{
		if (this.navigator)
		{
			cost = this.navigator.GetNavigationCost(cell);
			if (cost != -1)
			{
				return true;
			}
		}
		else if (this.consumerState.hasSolidTransferArm && this.consumerState.solidTransferArm.IsCellReachable(cell))
		{
			cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
			return true;
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
			string text = preconditionInstance.condition.description;
			if (string.IsNullOrEmpty(text))
			{
				text = preconditionInstance.condition.id;
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
			if (context.chore.showAvailabilityInHoverText && !context.chore.target.isNull && !(context.chore.target.gameObject != hover_obj.gameObject))
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
		foreach (Chore.Precondition.Context context2 in this.preconditionSnapshot.failedContexts)
		{
			if (context2.chore.showAvailabilityInHoverText && !context2.chore.target.isNull && !(context2.chore.target.gameObject != hover_obj.gameObject))
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

	public int GetPersonalPriority(ChoreGroup group)
	{
		int num = 3;
		ChoreConsumer.PriorityInfo priorityInfo;
		if (this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
		{
			num = priorityInfo.priority;
		}
		return Mathf.Clamp(num, 0, 5);
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
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
			priority = value
		};
		this.UpdateChoreTypePriorities(group, value);
		this.SetPermittedByUser(group, value != 0);
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return (int)this.GetAttributes().GetValue(group.attribute.Id);
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
					using (List<ChoreType>.Enumerator enumerator3 = choreGroup.choreTypes.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.IdHash == choreType.IdHash)
							{
								int personalPriority = this.GetPersonalPriority(choreGroup);
								num = Mathf.Max(num, personalPriority);
							}
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
		DebugUtil.Assert(!this.behaviourPreconditions.ContainsKey(tag));
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
		if (component != null && component.IsChoreGroupDisabled(chore_group))
		{
			flag = true;
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

	public const int PRIORITY_DISABLED = 0;

	public const int PRIORITY_VERYLOW = 1;

	public const int PRIORITY_LOW = 2;

	public const int PRIORITY_FLAT = 3;

	public const int PRIORITY_HIGH = 4;

	public const int PRIORITY_VERYHIGH = 5;

	[MyCmpAdd]
	private ChoreProvider choreProvider;

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpGet]
	public Navigator navigator;

	[MyCmpAdd]
	private User user;

	public bool prioritizeBrainIfNoChore;

	public global::System.Action choreRulesChanged;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	public ChoreTable choreTable;

	private ChoreTable.Instance choreTableInstance;

	public ChoreConsumerState consumerState;

	private Dictionary<Tag, ChoreConsumer.BehaviourPrecondition> behaviourPreconditions = new Dictionary<Tag, ChoreConsumer.BehaviourPrecondition>();

	private ChoreConsumer.PreconditionSnapshot preconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();

	private ChoreConsumer.PreconditionSnapshot lastSuccessfulPreconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();

	[Serialize]
	private Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	private Dictionary<HashedString, int> choreTypePriorities = new Dictionary<HashedString, int>();

	private List<HashedString> traitDisabledChoreGroups = new List<HashedString>();

	private List<HashedString> userDisabledChoreGroups = new List<HashedString>();

	private int stationaryReach = -1;

	private static Func<object, ChoreConsumer, Util.IterationInstruction> FindNextChoreEvaluateEntryHelper = delegate(object obj, ChoreConsumer consumer)
	{
		FetchChore fetchChore = obj as FetchChore;
		if (fetchChore == null)
		{
			DebugUtil.Assert(false, "FindNextChore found an entry that wasn't a FetchChore");
			return Util.IterationInstruction.Continue;
		}
		if (fetchChore.target == null)
		{
			DebugUtil.Assert(false, "FindNextChore found an entry with a null target");
			return Util.IterationInstruction.Continue;
		}
		if (fetchChore.isNull)
		{
			global::Debug.LogWarning("FindNextChore found an entry that isNull");
			return Util.IterationInstruction.Continue;
		}
		int num = Grid.PosToCell(fetchChore.gameObject);
		if (consumer.consumerState.solidTransferArm.IsCellReachable(num))
		{
			fetchChore.CollectChoresFromGlobalChoreProvider(consumer.consumerState, consumer.preconditionSnapshot.succeededContexts, consumer.preconditionSnapshot.failedContexts, false);
		}
		return Util.IterationInstruction.Continue;
	};

	private struct BehaviourPrecondition
	{
		public Func<object, bool> cb;

		public object arg;
	}

	public class PreconditionSnapshot
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
	}
}
