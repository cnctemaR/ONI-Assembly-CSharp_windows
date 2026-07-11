using System;
using Database;

public class EntityModifierSet : ModifierSet
{
	public override void Initialize()
	{
		base.Initialize();
		this.DuplicantStatusItems = new DuplicantStatusItems(this.Root);
		this.ChoreGroups = new ChoreGroups(this.Root);
		base.LoadTraits();
	}

	public DuplicantStatusItems DuplicantStatusItems;

	public ChoreGroups ChoreGroups;
}
