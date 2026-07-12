using System;

namespace Database
{
	public abstract class PermitResource : Resource
	{
		public string PermitId
		{
			get
			{
				return this.Id;
			}
		}

		public PermitResource(string id, string Name, string Desc, PermitCategory permitCategory, PermitRarity rarity)
			: base(id, Name)
		{
			DebugUtil.DevAssert(Name != null, "Name must be provided.", null);
			DebugUtil.DevAssert(Desc != null, "Description must be provided.", null);
			this.Description = Desc;
			this.Category = permitCategory;
			this.Rarity = rarity;
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
	}
}
