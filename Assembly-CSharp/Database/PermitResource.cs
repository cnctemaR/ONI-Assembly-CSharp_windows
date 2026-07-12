using System;

namespace Database
{
	public abstract class PermitResource : Resource
	{
		public PermitResource(string id, PermitCategory permitCategory, PermitRarity rarity)
			: this(id, id, permitCategory, rarity)
		{
		}

		public PermitResource(string id, string Name, PermitCategory permitCategory, PermitRarity rarity)
			: base(id, Name)
		{
			this.PermitId = id;
			this.PermitCategory = permitCategory;
			this.Rarity = rarity;
		}

		public abstract PermitPresentationInfo GetPermitPresentationInfo();

		public bool IsOwnable()
		{
			return PermitItems.IsPermitOwnable(this.PermitId);
		}

		public bool IsUnlocked()
		{
			return PermitItems.IsPermitUnlocked(this.PermitId);
		}

		public string PermitId;

		public PermitCategory PermitCategory;

		public PermitRarity Rarity;
	}
}
