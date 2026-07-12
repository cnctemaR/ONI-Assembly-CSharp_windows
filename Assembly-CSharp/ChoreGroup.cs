using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;

[DebuggerDisplay("{IdHash}")]
public class ChoreGroup : Resource
{
	public int DefaultPersonalPriority
	{
		get
		{
			return this.defaultPersonalPriority;
		}
	}

	public ChoreGroup(string id, string name, Klei.AI.Attribute attribute, string sprite, int default_personal_priority, bool user_prioritizable = true)
		: base(id, name)
	{
		this.attribute = attribute;
		this.description = Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + id.ToUpper() + ".DESC").String;
		this.sprite = sprite;
		this.defaultPersonalPriority = default_personal_priority;
		this.userPrioritizable = user_prioritizable;
	}

	public List<ChoreType> choreTypes = new List<ChoreType>();

	public Klei.AI.Attribute attribute;

	public string description;

	public string sprite;

	private int defaultPersonalPriority;

	public bool userPrioritizable;
}
