using System;
using STRINGS;

namespace Database
{
	public class ClothingItemResource : PermitResource
	{
		public string animFilename { get; private set; }

		public KAnimFile AnimFile { get; private set; }

		public ClothingOutfitUtility.OutfitType outfitType { get; private set; }

		[Obsolete("Please use constructor with dlcIds parameter")]
		public ClothingItemResource(string id, string name, string desc, ClothingOutfitUtility.OutfitType outfitType, PermitCategory category, PermitRarity rarity, string animFile)
			: this(id, name, desc, outfitType, category, rarity, animFile, DlcManager.AVAILABLE_ALL_VERSIONS)
		{
		}

		public ClothingItemResource(string id, string name, string desc, ClothingOutfitUtility.OutfitType outfitType, PermitCategory category, PermitRarity rarity, string animFile, string[] dlcIds)
			: base(id, name, desc, category, rarity, dlcIds)
		{
			this.AnimFile = Assets.GetAnim(animFile);
			this.animFilename = animFile;
			this.outfitType = outfitType;
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			if (this.AnimFile == null)
			{
				Debug.LogError("Clothing kanim is missing from bundle: " + this.animFilename);
			}
			permitPresentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(this.AnimFile, "ui", false, "");
			permitPresentationInfo.SetFacadeForText(UI.KLEI_INVENTORY_SCREEN.CLOTHING_ITEM_FACADE_FOR);
			return permitPresentationInfo;
		}
	}
}
