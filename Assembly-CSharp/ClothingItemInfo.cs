using System;
using Database;

public class ClothingItemInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public ClothingItemInfo(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
	{
		Option<ClothingOutfitUtility.OutfitType> outfitTypeFor = PermitCategories.GetOutfitTypeFor(category);
		if (outfitTypeFor.IsNone())
		{
			throw new Exception(string.Format("Expected permit category {0} on ClothingItemResource \"{1}\" to have an {2} but none found.", category, id, "OutfitType"));
		}
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.outfitType = outfitTypeFor.Unwrap();
		this.category = category;
		this.rarity = rarity;
		this.animFile = animFile;
		this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public ClothingOutfitUtility.OutfitType outfitType;

	public PermitCategory category;
}
