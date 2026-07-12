using System;

namespace Database
{
	public abstract class PermitResource : Resource
	{
		public PermitResource(string id, string Name, string Desc, PermitCategory permitCategory, PermitRarity rarity, string[] DLCIds)
			: base(id, Name)
		{
			DebugUtil.DevAssert(Name != null, "Name must be provided for permit with id \"" + id + "\" of type " + base.GetType().Name, null);
			DebugUtil.DevAssert(Desc != null, "Description must be provided for permit with id \"" + id + "\" of type " + base.GetType().Name, null);
			this.Description = Desc;
			this.Category = permitCategory;
			this.Rarity = rarity;
			this.DlcIds = DLCIds;
		}

		public abstract PermitPresentationInfo GetPermitPresentationInfo();

		public bool IsOwnable()
		{
			return this.Rarity != PermitRarity.Universal;
		}

		public bool IsUnlocked()
		{
			return !this.IsOwnable() || PermitItems.IsPermitUnlocked(this);
		}

		public string Description;

		public PermitCategory Category;

		public PermitRarity Rarity;

		public string[] DlcIds;
	}
}
