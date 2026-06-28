using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class ChoreType : Resource
{
	public ChoreType(string id, ResourceSet parent, string[] chore_groups, string urge, string name, string status_message, string tooltip, Tag[] interrupt_exclusion, int implicit_priority, int explicit_priority)
		: base(id, parent, name)
	{
		this.statusItem = new StatusItem(id, status_message, tooltip, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486);
		this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveStringCallback);
		this.tags.Add(TagManager.Create(id, null));
		this.interruptExclusion = new List<Tag>(interrupt_exclusion);
		Db.Get().DuplicantStatusItems.Add(this.statusItem);
		this.groups = new ChoreGroup[chore_groups.Length];
		for (int i = 0; i < this.groups.Length; i++)
		{
			ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(chore_groups[i]);
			if (!choreGroup.choreTypes.Contains(this))
			{
				choreGroup.choreTypes.Add(this);
			}
			this.groups[i] = choreGroup;
		}
		if (!string.IsNullOrEmpty(urge))
		{
			this.urge = Db.Get().Urges.Get(urge);
		}
		this.priority = implicit_priority;
		this.explicitPriority = explicit_priority;
	}

	public Urge urge { get; private set; }

	public ChoreGroup[] groups { get; private set; }

	public int priority { get; private set; }

	public int interruptPriority { get; set; }

	public int explicitPriority { get; private set; }

	private string ResolveStringCallback(string str, object data)
	{
		Chore chore = (Chore)data;
		return chore.ResolveString(str);
	}

	public StatusItem statusItem;

	public List<Tag> tags = new List<Tag>();

	public List<Tag> interruptExclusion;
}
