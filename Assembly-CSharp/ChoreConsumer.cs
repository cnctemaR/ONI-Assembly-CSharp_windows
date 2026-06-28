using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;

public class ChoreConsumer : KMonoBehaviour
{
	public bool IsStationary
	{
		get
		{
			return this.stationaryReach >= 0;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ChoreGroupManager.instance != null)
		{
			foreach (KeyValuePair<Tag, int> keyValuePair in ChoreGroupManager.instance.DefaultChorePermission)
			{
				bool flag = false;
				foreach (HashedString hashedString in this.forbiddenChoreGroups)
				{
					if (hashedString.HashValue == keyValuePair.Key.GetHashCode())
					{
						flag = true;
						break;
					}
				}
				if (!flag && keyValuePair.Value == 0)
				{
					this.forbiddenChoreGroups.Add(new HashedString(keyValuePair.Key.GetHashCode()));
				}
			}
		}
	}

	public bool IsPermitted(ChoreGroup chore_group)
	{
		return chore_group == null || !this.forbiddenChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetPermitted(ChoreGroup chore_group, bool is_allowed)
	{
		if (is_allowed)
		{
			if (this.forbiddenChoreGroups.Remove(chore_group.IdHash))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.forbiddenChoreGroups.Contains(chore_group.IdHash))
		{
			this.forbiddenChoreGroups.Add(chore_group.IdHash);
			this.choreRulesChanged.Signal();
		}
	}

	public bool IsEnabled(ChoreGroup chore_group)
	{
		return chore_group == null || !this.disabledChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetEnabled(ChoreGroup chore_group, bool is_enabled)
	{
		if (is_enabled)
		{
			if (this.disabledChoreGroups.Remove(chore_group.IdHash))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.disabledChoreGroups.Contains(chore_group.IdHash))
		{
			this.disabledChoreGroups.Add(chore_group.IdHash);
			this.choreRulesChanged.Signal();
		}
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		this.preconditionSnapshot.Clear();
		if (this.IsStationary)
		{
			SolidTransferArm component = base.GetComponent<SolidTransferArm>();
			CellOffset offset = Grid.GetOffset(Grid.PosToCell(component));
			int pickupRange = component.pickupRange;
			List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(offset.x - pickupRange, offset.y - pickupRange, pickupRange * 2 + 1, pickupRange * 2 + 1, GameScenePartitioner.Instance.fetchChoreLayer, list);
			foreach (ScenePartitionerEntry scenePartitionerEntry in list)
			{
				Chore chore = scenePartitionerEntry.obj as Chore;
				chore.CollectChores(this, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts, false);
			}
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
		}
		else
		{
			for (int i = 0; i < this.providers.Count; i++)
			{
				ChoreProvider choreProvider = this.providers[i];
				choreProvider.CollectChores(this, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts);
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
		DebugUtil.Assert(provider != null, "Assert!");
		this.providers.Add(provider);
	}

	public void RemoveProvider(ChoreProvider provider)
	{
		this.providers.Remove(provider);
	}

	public void AddUrge(Urge urge)
	{
		this.Log("(AddUrge)", urge.ToString());
		DebugUtil.Assert(urge != null, "Assert!");
		this.urges.Add(urge);
		base.Trigger(-736698276, urge);
	}

	public void RemoveUrge(Urge urge)
	{
		this.Log("(RemoveUrge)", urge.ToString());
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

	public void Log(string evt, string param)
	{
	}

	public bool DoesPrefer(Chore chore)
	{
		return chore.isPreferredChoreRegardlessOfTags || (this.resume != null && (this.resume.IsFavouredChore(chore) || this.resume.IsPreferredChore(chore)));
	}

	public bool IsPermittedOrEnabled(ChoreType chore_type, Chore chore)
	{
		if (chore_type.groups.Length == 0)
		{
			return true;
		}
		if (this.DoesPrefer(chore))
		{
			return true;
		}
		for (int i = 0; i < chore_type.groups.Length; i++)
		{
			ChoreGroup choreGroup = chore_type.groups[i];
			if (this.IsEnabled(choreGroup) && (this.IsPermitted(choreGroup) || (this.resume != null && this.resume.IsChoreGroupInCurrentRoleGroup(choreGroup))))
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

	public bool GetNavigationCost(IApproachable approchable, out int cost)
	{
		if (this.navigator)
		{
			cost = this.navigator.GetNavigationCost(approchable);
			if (cost != PathProber.InvalidCost)
			{
				return true;
			}
		}
		else if (this.stationaryReach > 0)
		{
			cost = Grid.GetCellRange(this.NaturalBuildingCell(), approchable.GetCell());
			return this.stationaryReach >= cost;
		}
		cost = 0;
		return false;
	}

	public bool CanReach(IApproachable approchable)
	{
		if (this.navigator)
		{
			return this.navigator.CanReach(approchable);
		}
		if (this.stationaryReach > 0)
		{
			int cellRange = Grid.GetCellRange(this.NaturalBuildingCell(), approchable.GetCell());
			return this.stationaryReach >= cellRange;
		}
		return false;
	}

	public bool IsWithinReach(IApproachable approachable)
	{
		if (this.navigator)
		{
			return !(this == null) && !(base.gameObject == null) && Grid.IsCellOffsetOf(Grid.PosToCell(this), approachable.GetCell(), approachable.GetOffsets());
		}
		int num;
		return this.stationaryReach > 0 && this.GetNavigationCost(approachable, out num) && this.stationaryReach >= num;
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

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpGet]
	public Navigator navigator;

	[MyCmpGet]
	public MinionResume resume;

	[MyCmpAdd]
	private User user;

	public global::System.Action choreRulesChanged;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	private ChoreConsumer.PreconditionSnapshot preconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();

	[Serialize]
	private List<HashedString> forbiddenChoreGroups = new List<HashedString>();

	private List<HashedString> disabledChoreGroups = new List<HashedString>();

	public HashSet<Tag> preferredChoreTags = new HashSet<Tag>();

	private int stationaryReach = -1;

	private LoggerFSS log = new LoggerFSS("ChoreConsumer");

	private class PreconditionSnapshot
	{
		public void CopyTo(ChoreConsumer.PreconditionSnapshot snapshot)
		{
			snapshot.succeededContexts.AddRange(this.succeededContexts);
			snapshot.failedContexts.AddRange(this.failedContexts);
		}

		public void Clear()
		{
			this.succeededContexts.Clear();
			this.failedContexts.Clear();
		}

		public List<Chore.Precondition.Context> succeededContexts = new List<Chore.Precondition.Context>();

		public List<Chore.Precondition.Context> failedContexts = new List<Chore.Precondition.Context>();
	}
}
