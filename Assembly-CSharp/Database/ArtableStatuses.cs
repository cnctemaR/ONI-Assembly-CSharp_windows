using System;

namespace Database
{
	public class ArtableStatuses : ResourceSet<ArtableStatusItem>
	{
		public ArtableStatuses(ResourceSet parent)
			: base("ArtableStatuses", parent)
		{
			this.Ready = this.Add("AwaitingArting", ArtableStatuses.ArtableStatusType.AwaitingArting);
			this.Ugly = this.Add("LookingUgly", ArtableStatuses.ArtableStatusType.LookingUgly);
			this.Okay = this.Add("LookingOkay", ArtableStatuses.ArtableStatusType.LookingOkay);
			this.Great = this.Add("LookingGreat", ArtableStatuses.ArtableStatusType.LookingGreat);
		}

		public ArtableStatusItem Add(string id, ArtableStatuses.ArtableStatusType statusType)
		{
			ArtableStatusItem artableStatusItem = new ArtableStatusItem(id, statusType);
			this.resources.Add(artableStatusItem);
			return artableStatusItem;
		}

		public ArtableStatusItem Ready;

		public ArtableStatusItem Ugly;

		public ArtableStatusItem Okay;

		public ArtableStatusItem Great;

		public enum ArtableStatusType
		{
			AwaitingArting,
			LookingUgly,
			LookingOkay,
			LookingGreat
		}
	}
}
