using System;
using System.Collections.Generic;
using KSerialization;

public class ChoreConsumer : KMonoBehaviour
{
	public bool IsPermitted(ChoreGroup chore_group)
	{
		return chore_group == null || !this.forbiddenChoreGroups.Contains(chore_group.Id);
	}

	public void SetPermitted(ChoreGroup chore_group, bool is_allowed)
	{
		if (is_allowed)
		{
			if (this.forbiddenChoreGroups.Remove(chore_group.Id))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.forbiddenChoreGroups.Contains(chore_group.Id))
		{
			this.forbiddenChoreGroups.Add(chore_group.Id);
			this.choreRulesChanged.Signal();
		}
	}

	public bool IsEnabled(ChoreGroup chore_group)
	{
		return chore_group == null || !this.disabledChoreGroups.Contains(chore_group.Id);
	}

	public void SetEnabled(ChoreGroup chore_group, bool is_enabled)
	{
		if (is_enabled)
		{
			if (this.disabledChoreGroups.Remove(chore_group.Id))
			{
				this.choreRulesChanged.Signal();
			}
		}
		else if (!this.disabledChoreGroups.Contains(chore_group.Id))
		{
			this.disabledChoreGroups.Add(chore_group.Id);
			this.choreRulesChanged.Signal();
		}
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		this.contexts.Clear();
		for (int i = 0; i < this.providers.Count; i++)
		{
			ChoreProvider choreProvider = this.providers[i];
			choreProvider.CollectChores(this, this.contexts);
		}
		this.contexts.Sort();
		if (this.contexts.Count > 0)
		{
			Chore currentChore = base.GetComponent<ChoreDriver>().GetCurrentChore();
			Chore.Precondition.Context context = this.contexts[this.contexts.Count - 1];
			for (int j = this.contexts.Count - 1; j >= 0; j--)
			{
				if (context.IsSuccess() && (currentChore == null || context.interruptPriority > currentChore.choreType.interruptPriority))
				{
					context.chore.PrepareChore(ref context);
					out_context = context;
					return true;
				}
			}
		}
		return false;
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
		this.Trigger(-736698276, urge);
	}

	public void RemoveUrge(Urge urge)
	{
		this.Log("(RemoveUrge)", urge.ToString());
		this.urges.Remove(urge);
		this.Trigger(231622047, urge);
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

	public bool IsPermittedOrEnabled(Chore chore)
	{
		if (chore.choreType.groups.Length == 0)
		{
			return true;
		}
		foreach (ChoreGroup choreGroup in chore.choreType.groups)
		{
			if (this.IsPermitted(choreGroup) && this.IsEnabled(choreGroup))
			{
				return true;
			}
		}
		return false;
	}

	[MyCmpAdd]
	private ChoreDriver choreDriver;

	[MyCmpAdd]
	private User user;

	public global::System.Action choreRulesChanged;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	private List<Chore.Precondition.Context> contexts = new List<Chore.Precondition.Context>();

	[Serialize]
	private List<string> forbiddenChoreGroups = new List<string>();

	private List<string> disabledChoreGroups = new List<string>();

	private LoggerFSS log = new LoggerFSS("ChoreConsumer");
}
