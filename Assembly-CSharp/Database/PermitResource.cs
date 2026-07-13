using System;

namespace Database
{
	public abstract class PermitResource : Resource, IHasDlcRestrictions
	{
		public PermitResource(string id, string Name, string Desc, PermitCategory permitCategory, PermitRarity rarity, string[] requiredDlcIds, string[] forbiddenDlcIds)
			: base(id, Name)
		{
			DebugUtil.DevAssert(Name != null, "Name must be provided for permit with id \"" + id + "\" of type " + base.GetType().Name, null);
			DebugUtil.DevAssert(Desc != null, "Description must be provided for permit with id \"" + id + "\" of type " + base.GetType().Name, null);
			this.Description = Desc;
			this.Category = permitCategory;
			this.Rarity = rarity;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
		}

		public abstract PermitPresentationInfo GetPermitPresentationInfo();

		public bool IsOwnableOnServer()
		{
			return this.Rarity != PermitRarity.Universal && this.Rarity != PermitRarity.UniversalLocked;
		}

		public bool IsUnlocked()
		{
			return this.Rarity == PermitRarity.Universal || PermitItems.IsPermitUnlocked(this);
		}

		public string GetDlcIdFrom()
		{
			return DlcManager.GetMostSignificantDlc(this);
		}

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public string Description;

		public PermitCategory Category;

		public PermitRarity Rarity;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;
	}
}
