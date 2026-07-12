using System;
using STRINGS;

namespace Database
{
	public class ArtableStage : PermitResource
	{
		public ArtableStage(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, ArtableStatusItem status_item, string prefabId, string symbolName = "")
			: base(id, name, PermitCategory.Artwork, rarity)
		{
			this.id = id;
			this.description = desc;
			this.animFile = animFile;
			this.anim = anim;
			this.symbolName = symbolName;
			this.decor = decor_value;
			this.cheerOnComplete = cheer_on_complete;
			this.statusItem = status_item;
			this.prefabId = prefabId;
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.name = this.Name;
			permitPresentationInfo.description = this.description;
			permitPresentationInfo.category = this.PermitCategory;
			permitPresentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(this.animFile), "ui", false, "");
			permitPresentationInfo.SetFacadeForText(UI.KLEI_INVENTORY_SCREEN.ARTABLE_ITEM_FACADE_FOR.Replace("{ConfigProperName}", Assets.GetPrefab(this.prefabId).GetProperName()).Replace("{ArtableQuality}", this.statusItem.GetName(null)));
			permitPresentationInfo.SetRarityDetailsFor(this.Rarity);
			permitPresentationInfo.ownedCount = PermitItems.GetOwnedCount(this);
			return permitPresentationInfo;
		}

		public string id;

		public string description;

		public string anim;

		public string animFile;

		public string prefabId;

		public string symbolName;

		public int decor;

		public bool cheerOnComplete;

		public ArtableStatusItem statusItem;
	}
}
