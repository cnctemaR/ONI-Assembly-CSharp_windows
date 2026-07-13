using System;
using Database;

public interface IBlueprintInfo : IHasDlcRestrictions
{
	string id { get; set; }

	string name { get; set; }

	string desc { get; set; }

	PermitRarity rarity { get; set; }

	string animFile { get; set; }
}
