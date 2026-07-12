using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class ChoreType : Resource
{
	public Urge urge { get; private set; }

	public ChoreGroup[] groups { get; private set; }

	public int priority { get; private set; }

	public int interruptPriority { get; set; }

	public int explicitPriority { get; private set; }

	private string ResolveStringCallback(string str, object data)
	{
		return ((Chore)data).ResolveString(str);
	}

	public ChoreType(string id, ResourceSet parent, string[] chore_groups, string urge, string name, string status_message, string tooltip, IEnumerable<Tag> interrupt_exclusion, int implicit_priority, int explicit_priority)
		: base(id, parent, name)
	{
		this.statusItem = new StatusItem(id, status_message, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);
		this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveStringCallback);
		this.tags.Add(TagManager.Create(id));
		this.interruptExclusion = new HashSet<Tag>(interrupt_exclusion);
		Db.Get().DuplicantStatusItems.Add(this.statusItem);
		List<ChoreGroup> list = new List<ChoreGroup>();
		for (int i = 0; i < chore_groups.Length; i++)
		{
			ChoreGroup choreGroup = Db.Get().ChoreGroups.TryGet(chore_groups[i]);
			if (choreGroup != null)
			{
				if (!choreGroup.choreTypes.Contains(this))
				{
					choreGroup.choreTypes.Add(this);
				}
				list.Add(choreGroup);
			}
		}
		this.groups = list.ToArray();
		if (!string.IsNullOrEmpty(urge))
		{
			this.urge = Db.Get().Urges.Get(urge);
		}
		this.priority = implicit_priority;
		this.explicitPriority = explicit_priority;
	}

	public StatusItem statusItem;

	public HashSet<Tag> tags = new HashSet<Tag>();

	public HashSet<Tag> interruptExclusion;

	public string reportName;
}
