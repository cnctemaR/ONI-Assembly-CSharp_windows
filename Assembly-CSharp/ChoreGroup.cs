using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;

[DebuggerDisplay("{IdHash}")]
public class ChoreGroup : Resource
{
	public ChoreGroup(string id, string name, string attribute, int default_personal_priority)
		: base(id, name)
	{
		this.attribute = Db.Get().Attributes.Get(attribute);
		this.description = Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + id.ToUpper() + ".DESC").String;
		this.defaultPersonalPriority = default_personal_priority;
	}

	public int DefaultPersonalPriority
	{
		get
		{
			return this.defaultPersonalPriority;
		}
	}

	public List<ChoreType> choreTypes = new List<ChoreType>();

	public Klei.AI.Attribute attribute;

	public string description;

	private int defaultPersonalPriority;
}
