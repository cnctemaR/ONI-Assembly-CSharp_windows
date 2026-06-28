using System;
using System.Collections.Generic;

public class ChoreType : Resource
{
	public ChoreType(string id, ResourceSet parent, string[] chore_groups, string urge, string name, string status_message, Tag[] interrupt_exclusion, int priority)
		: base(id, parent, name)
	{
		this.statusItem = new StatusItem(id, status_message, status_message, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 14334);
		this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveStringCallback);
		this.tags.Add(TagManager.Create(id, null));
		this.interruptExclusion = new List<Tag>(interrupt_exclusion);
		Db.Get().DuplicantStatusItems.Add(this.statusItem);
		this.groups = new ChoreGroup[chore_groups.Length];
		for (int i = 0; i < this.groups.Length; i++)
		{
			this.groups[i] = Db.Get().ChoreGroups.Get(chore_groups[i]);
		}
		if (!string.IsNullOrEmpty(urge))
		{
			this.urge = Db.Get().Urges.Get(urge);
		}
		this.priority = priority;
	}

	public Urge urge { get; private set; }

	public ChoreGroup[] groups { get; private set; }

	public int priority { get; private set; }

	public int interruptPriority { get; set; }

	private string ResolveStringCallback(string str, object data)
	{
		Chore chore = (Chore)data;
		return chore.ResolveString(str);
	}

	public StatusItem statusItem;

	public List<Tag> tags = new List<Tag>();

	public List<Tag> interruptExclusion;
}
