using System;

namespace Database
{
	public class ArtableStatuses : ResourceSet<ArtableStatusItem>
	{
		public ArtableStatuses(ResourceSet parent)
			: base("ArtableStatuses", parent)
		{
			this.AwaitingArting = this.Add("AwaitingArting", ArtableStatuses.ArtableStatusType.AwaitingArting);
			this.LookingUgly = this.Add("LookingUgly", ArtableStatuses.ArtableStatusType.LookingUgly);
			this.LookingOkay = this.Add("LookingOkay", ArtableStatuses.ArtableStatusType.LookingOkay);
			this.LookingGreat = this.Add("LookingGreat", ArtableStatuses.ArtableStatusType.LookingGreat);
		}

		public ArtableStatusItem Add(string id, ArtableStatuses.ArtableStatusType statusType)
		{
			ArtableStatusItem artableStatusItem = new ArtableStatusItem(id, statusType);
			this.resources.Add(artableStatusItem);
			return artableStatusItem;
		}

		public ArtableStatusItem AwaitingArting;

		public ArtableStatusItem LookingUgly;

		public ArtableStatusItem LookingOkay;

		public ArtableStatusItem LookingGreat;

		public enum ArtableStatusType
		{
			AwaitingArting,
			LookingUgly,
			LookingOkay,
			LookingGreat
		}
	}
}
