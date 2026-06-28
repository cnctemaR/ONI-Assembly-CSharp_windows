using System;
using System.Collections.Generic;
using KSerialization;

public class ChoreConsumer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ChoreGroupManager.instance != null)
		{
			foreach (Tag tag in ChoreGroupManager.instance.DefaultForbiddenTagsList)
			{
				bool flag = false;
				foreach (HashedString hashedString in this.forbiddenChoreGroups)
				{
					if (hashedString.HashValue == tag.Name.GetHashCode())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.forbiddenChoreGroups.Add(new HashedString(tag.Name));
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
		this.contexts.Clear();
		for (int i = 0; i < this.providers.Count; i++)
		{
			ChoreProvider choreProvider = this.providers[i];
			choreProvider.CollectChores(this, this.contexts);
		}
		this.contexts.Sort();
		bool flag = false;
		if (this.contexts.Count > 0)
		{
			Chore currentChore = this.choreDriver.GetCurrentChore();
			for (int j = this.contexts.Count - 1; j >= 0; j--)
			{
				Chore.Precondition.Context context = this.contexts[j];
				if (context.IsSuccess())
				{
					if (currentChore == null || context.interruptPriority > currentChore.choreType.interruptPriority)
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

	public bool IsPermittedOrEnabled(Chore chore)
	{
		bool flag;
		if (chore.choreType.groups.Length == 0)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < chore.choreType.groups.Length; i++)
			{
				ChoreGroup choreGroup = chore.choreType.groups[i];
				if (this.IsPermitted(choreGroup) && this.IsEnabled(choreGroup))
				{
					return true;
				}
			}
			flag = false;
		}
		return flag;
	}

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpReq]
	public Navigator navigator;

	[MyCmpReq]
	public MinionResume resume;

	[MyCmpAdd]
	private User user;

	public global::System.Action choreRulesChanged;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	private List<Chore.Precondition.Context> contexts = new List<Chore.Precondition.Context>();

	[Serialize]
	private List<HashedString> forbiddenChoreGroups = new List<HashedString>();

	private List<HashedString> disabledChoreGroups = new List<HashedString>();

	private LoggerFSS log = new LoggerFSS("ChoreConsumer");
}
