using System;
using System.Collections.Generic;
using Klei.AI;

public class ChoreGroup : Resource
{
	public ChoreGroup(string id, string name, string attribute)
		: base(id, name)
	{
		this.attribute = Db.Get().Attributes.Get(attribute);
		this.description = Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + id.ToUpper() + ".DESC").String;
	}

	public void Add(ChoreType chore_type)
	{
		this.choreTypes.Add(chore_type);
	}

	public List<ChoreType> choreTypes = new List<ChoreType>();

	public Klei.AI.Attribute attribute;

	public string description;
}
