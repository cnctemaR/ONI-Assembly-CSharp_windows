using System;

public class ChoreType : Resource
{
	public ChoreType(string id, ResourceSet parent, string[] chore_groups, string urge, string name, string status_message, int priority)
		: base(id, parent, name)
	{
		this.statusItem = new StatusItem(id, status_message, string.Empty, status_message, false, StatusItem.IconType.Info, NotificationType.Neutral, SimViewMode.None, SimViewMode.None);
		this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveStringCallback);
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
}
